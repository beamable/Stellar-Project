import { BeamServer, type HttpRequester } from 'beamable-sdk'
import getBeam, { resolveBeamConfig } from '@/lib/beam'

export type NotificationHandler = (data: any) => void

const NOTIFY_GET_PATH = '/basic/notification/'
// Browser clients lack notification-write; avoid POST registration entirely.
// const NOTIFY_POST_CUSTOM_PATH = '/basic/notification/custom'

export const PlayerNotificationContexts = {
  ExternalAuthAddress: 'external-auth-address',
  ExternalAuthSignature: 'external-auth-signature',
} as const

type Subscription = {
  context: string
  timerId: number | null
  stopped: boolean
  customPrefix?: string
  pubnub?: any
  channelNames?: string[]
  serverEventStops?: Map<string, () => void>
}

type ServerEventSubscription = {
  handlers: Set<NotificationHandler>
  forwarder: (payload: any) => void
}

let beamServerPromise: Promise<any | null> | null = null
const serverEventSubscriptions = new Map<string, ServerEventSubscription>()

async function ensureBeamServerInstance(beam: any) {
  if (typeof window === 'undefined') return null
  if (!beamServerPromise) {
    const initPromise = (async () => {
      try {
        const cfg = await resolveBeamConfig().catch(() => null)
        const cid = (beam?.cid && String(beam.cid)) || cfg?.cid
        const pid = (beam?.pid && String(beam.pid)) || cfg?.pid
        const envRaw =
          cfg?.environment ||
          (process.env.NEXT_PUBLIC_BEAM_ENV || process.env.BEAM_ENV || 'prod')
        const environment = (envRaw || 'prod').toLowerCase()
        const tokenStorage = beam?.tokenStorage
        if (!cid || !pid || !tokenStorage) {
          console.warn('[Notifications] BeamServer init missing cid/pid/tokenStorage')
          return null
        }
        const instanceTag = (() => {
          try {
            return `${tokenStorage?.prefix ?? 'beam-web'}-server`
          } catch {
            return undefined
          }
        })()
        const server = await BeamServer.init({
          cid,
          pid,
          environment: environment as any,
          tokenStorage,
          instanceTag,
          serverEvents: {
            enabled: true,
          },
        })
        console.log('[Notifications] BeamServer server-events enabled')
        return server
      } catch (err) {
        console.warn('[Notifications] BeamServer init failed:', (err as any)?.message || err)
        return null
      }
    })()
    beamServerPromise = initPromise
      .then((server) => {
        if (!server) {
          beamServerPromise = null
        }
        return server
      })
      .catch((err) => {
        beamServerPromise = null
        throw err
      })
  }
  return beamServerPromise
}

async function setupServerEventSubscription(
  context: string,
  handler: NotificationHandler,
  beam: any
): Promise<(() => void) | null> {
  if (typeof window === 'undefined') return null
  try {
    const server = await ensureBeamServerInstance(beam)
    if (!server) return null
    const eventType = context
    const key = eventType.toLowerCase()
    let existing = serverEventSubscriptions.get(key)
    if (!existing) {
      const forwarder = (payload: any) => {
        const active = serverEventSubscriptions.get(key)
        if (!active) return
        try {
          console.log('[Notifications] BeamServer event received', { context: eventType, payload })
        } catch {}
        for (const cb of active.handlers) {
          try {
            cb(payload)
          } catch (err) {
            console.error('[Notifications] BeamServer handler error:', (err as any)?.message || err)
          }
        }
      }
      existing = {
        handlers: new Set<NotificationHandler>(),
        forwarder,
      }
      serverEventSubscriptions.set(key, existing)
      try {
        server.on(eventType as any, forwarder)
        console.log('[Notifications] BeamServer subscribed to context:', eventType)
      } catch (err) {
        serverEventSubscriptions.delete(key)
        console.warn('[Notifications] BeamServer subscription failed:', (err as any)?.message || err)
        return null
      }
    }
    existing.handlers.add(handler)
    return () => {
      const active = serverEventSubscriptions.get(key)
      if (!active) return
      active.handlers.delete(handler)
      if (active.handlers.size === 0) {
        serverEventSubscriptions.delete(key)
        const promiseRef = beamServerPromise
        if (promiseRef && typeof promiseRef.then === 'function') {
          promiseRef
            .then((srv) => {
              try {
                srv?.off?.(eventType as any, active.forwarder)
                console.log('[Notifications] BeamServer unsubscribed from context:', eventType)
              } catch {}
            })
            .catch(() => {})
        }
      }
    }
  } catch (err) {
    console.warn('[Notifications] BeamServer setup error:', (err as any)?.message || err)
    return null
  }
}

async function getOnce(requester: HttpRequester, query?: Record<string, string>) {
  const url = query && Object.keys(query).length > 0
    ? `${NOTIFY_GET_PATH}?${new URLSearchParams(query).toString()}`
    : NOTIFY_GET_PATH
  const res = await requester.request({ url, method: 'GET', withAuth: true })
  return res.body
}

export async function getNotificationBootstrap() {
  const beam: any = await getBeam()
  const requester: HttpRequester = beam.requester
  const res = await requester.request({ url: NOTIFY_GET_PATH, method: 'GET', withAuth: true })
  return res.body
}

export async function subscribeToContext(
  context: string,
  handler: NotificationHandler,
  options?: { intervalMs?: number }
) {
  const beam: any = await getBeam()
  const requester: HttpRequester = beam.requester
  const intervalMs = options?.intervalMs ?? 2500

  const sub: Subscription = { context, timerId: null, stopped: false, customPrefix: undefined }
  const contextKey = context.toLowerCase()
  console.log('[Notifications] subscribe start:', { context, intervalMs })

  const normalizeAndHandle = (msg: any) => {
    if (!msg) return
    let working = msg

    if (working && typeof working === 'object' && working.payload && typeof working.payload === 'object') {
      working = { ...working, ...working.payload }
    }

    const unwrap = (source: any) => {
      let result = source
      const mf = result?.messageFull ?? result?.message_full
      if (typeof mf === 'string') {
        try {
          const parsed = JSON.parse(mf)
          result = { ...(typeof result === 'object' ? result : {}), ...parsed }
        } catch {}
      } else if (result?.d && typeof result.d === 'object') {
        const inner = result.d
        const mf2 = inner.messageFull ?? inner.message_full
        if (typeof mf2 === 'string') {
          try {
            const parsed = JSON.parse(mf2)
            result = { ...result, ...inner, ...parsed }
          } catch {
            result = { ...result, ...inner }
          }
        } else {
          result = { ...result, ...inner }
        }
      }
      return result
    }

    working = unwrap(working)

    const ctxValue = working?.context ?? working?.Context
    if (typeof ctxValue === 'string' && ctxValue.length > 0) {
      const normalizedCtx = ctxValue.toLowerCase()
      if (normalizedCtx !== contextKey) {
        return
      }
      if (!working.context || working.context !== ctxValue) {
        working = { ...working, context: ctxValue }
      }
    }

    try {
      console.log('[Notifications] normalized message ready for handler', working)
    } catch {}

    handler(working)
  }

  const ensureServerEventHandlers = async (channels?: string[]) => {
    if (!channels || channels.length === 0) return
    const stops = sub.serverEventStops ?? new Map<string, () => void>()
    sub.serverEventStops = stops
    const normalized = Array.from(
      new Set(
        channels
          .map((ch) => (typeof ch === 'string' ? ch.trim() : ''))
          .filter((ch) => ch && !ch.includes('*'))
      )
    )
    for (const ch of normalized) {
      const key = ch.toLowerCase()
      if (stops.has(key)) continue
      try {
        const stop = await setupServerEventSubscription(ch, normalizeAndHandle, beam)
        if (stop) {
          stops.set(key, stop)
        }
      } catch (err) {
        console.warn('[Notifications] BeamServer channel subscription error:', ch, (err as any)?.message || err)
      }
    }
  }

  const tick = async () => {
    if (sub.stopped) return
    try {
      // First, acquire notification config to get custom channel prefix
      if (!sub.customPrefix) {
        const cfg = await getOnce(requester)
        console.log('[Notifications] base config payload:', cfg)
        if (cfg && typeof cfg === 'object') {
          const configCustomPrefix =
            typeof (cfg as any).customChannelPrefix === 'string'
              ? String((cfg as any).customChannelPrefix)
              : ''
          if (configCustomPrefix) {
            sub.customPrefix = configCustomPrefix
          }
          const playerForRealm = (cfg as any).playerForRealmChannel
          const playerChannel = (cfg as any).playerChannel
          const gameChannel = (cfg as any).gameNotificationChannel
          const extraPlayerChannels: string[] = Array.isArray((cfg as any).playerChannels)
            ? ((cfg as any).playerChannels as any[]).map(String)
            : []
          const beamCid = (beam?.cid && String(beam.cid).trim()) || ''
          const beamPid = (beam?.pid && String(beam.pid).trim()) || ''
          const beamGamerTag = (beam?.player?.id && String(beam.player.id)) || ''
          const prefixParts = (configCustomPrefix || '').split('.')
          const cfgCid = prefixParts.length >= 2 ? String(prefixParts[1] || '').trim() : ''
          const cfgPid = prefixParts.length >= 3 ? String(prefixParts[2] || '').trim() : ''
          const cid = beamCid || cfgCid
          if (!beamCid && cfgCid) {
            console.warn('[Notifications] Beam CID missing from beam instance; falling back to bootstrap cid', { cid: cfgCid })
          }
          const primaryPid = beamPid || cfgPid
          if (!beamPid && cfgPid) {
            console.warn('[Notifications] Beam PID missing from beam instance; falling back to bootstrap pid', { pid: cfgPid })
          }
          const gamerTag = beamGamerTag
          const preferredCustomPrefix =
            cid && primaryPid ? `custom.${cid}.${primaryPid}.` : configCustomPrefix
          const customPrefixCandidates = Array.from(
            new Set(
              [configCustomPrefix, preferredCustomPrefix]
                .filter((val): val is string => typeof val === 'string' && val.length > 0)
                .map((val) => (val.endsWith('.') ? val : `${val}.`))
            )
          )
          if (customPrefixCandidates.length > 0) {
            sub.customPrefix = customPrefixCandidates[customPrefixCandidates.length - 1]
          }

          // Subscribe to player-specific channels; the server posts per-player notifications there.
          const channels: string[] = []
          if (playerForRealm) channels.push(String(playerForRealm))
          if (playerChannel) channels.push(String(playerChannel))
          if (gameChannel) channels.push(String(gameChannel))
          channels.push(...extraPlayerChannels)
          const contextOnlyChannel = `custom.${context}`
          channels.push(contextOnlyChannel)
          // As a fallback, include a custom channel derived from the context
          for (const prefix of customPrefixCandidates.length ? customPrefixCandidates : [sub.customPrefix]) {
            if (!prefix) continue
            const normalizedPrefix = prefix.endsWith('.') ? prefix : `${prefix}.`
            const customBase = `${normalizedPrefix}${context}`
            channels.push(customBase)
            if (gamerTag) {
              channels.push(`${customBase}.${gamerTag}`)
              channels.push(`${normalizedPrefix}${gamerTag}.${context}`)
            }
          }
          sub.channelNames = Array.from(new Set(channels))
          console.log('[Notifications] using channels:', sub.channelNames)
          await ensureServerEventHandlers(sub.channelNames)

          // Prefer realtime subscription via PubNub when available
          try {
            // @ts-ignore
            const win: any = typeof window !== 'undefined' ? window : undefined
            const ensurePubNub = async () => {
              if (win?.PubNub) return win.PubNub
              await new Promise<void>((resolve, reject) => {
                const script = document.createElement('script')
                script.src = 'https://cdn.pubnub.com/sdk/javascript/pubnub.7.2.2.min.js'
                script.async = true
                script.onload = () => resolve()
                script.onerror = () => reject(new Error('Failed to load PubNub SDK'))
                document.head.appendChild(script)
              })
              return win.PubNub
            }

            const PubNub = await ensurePubNub()
            if (PubNub && !sub.stopped) {
              const subscribeKey = (cfg as any).subscribeKey
              const authKey = (cfg as any).authenticationKey
              const baseId = gamerTag || 'beam-web'
              const suffix = (() => {
                try { return (crypto as any)?.randomUUID?.() || Math.random().toString(36).slice(2) } catch { return Math.random().toString(36).slice(2) }
              })()
              const uuid = `${baseId}.${suffix}`
              const config: any = { subscribeKey, authKey, uuid }
              sub.pubnub = new PubNub(config)
              console.log('[Notifications] PubNub subscribe init', { channels: sub.channelNames, uuid })
              sub.pubnub.addListener({
                message: (event: any) => {
                  try {
                    console.log('[Notifications] PubNub message:', event)
                    const msg = event?.message ?? event
                    // If runtime wraps payload in messageFull (stringified JSON), unwrap it
                    let inner: any = undefined
                    const mf = (msg && (msg.messageFull || msg.message_full))
                    if (typeof mf === 'string') {
                      try { inner = JSON.parse(mf) } catch {}
                    }
                    const combined = (inner && typeof inner === 'object') ? { ...(typeof msg === 'object' ? msg : {}), ...inner } : msg
                    console.log('[Notifications] PubNub combined payload:', combined)
                    normalizeAndHandle(combined)
                  } catch (err) {
                    console.warn('[Notifications] handler error:', (err as any)?.message || err)
                  }
                },
                status: (st: any) => {
                  console.log('[Notifications] PubNub status:', st?.category || st)
                },
                presence: () => {},
              })
              // Also try wildcards for this realm (best-effort; ignored if not allowed)
              const wildcardCandidates: string[] = []
              try {
                const realmPrefix = String(sub.customPrefix) // like 'custom.<realm>.<pid>.'
                const realmBase = realmPrefix.endsWith('.') ? realmPrefix.slice(0, -1) : realmPrefix
                wildcardCandidates.push(`${realmBase}.*`) // custom.<realm>.<pid>.*
                const frParts = (cfg as any).playerForRealmChannel?.split('.') || []
                if (frParts.length >= 3) {
                  wildcardCandidates.push(`${frParts[0]}.${frParts[1]}.*`) // playerfr.<realm>.*
                }
                const pParts = (cfg as any).playerChannel?.split('.') || []
                if (pParts.length >= 1) {
                  wildcardCandidates.push(`${pParts[0]}.*`) // player.*
                }
              } catch {}

    // Include fallback project PIDs if configured (handles cross-project notifications)
              const fallbackPids: string[] = []
              try {
                const envFallback = (process.env.NEXT_PUBLIC_BEAM_FALLBACK_PIDS || '').split(',').map((s) => s.trim()).filter(Boolean)
                fallbackPids.push(...envFallback)
                fallbackPids.push('DE_1925146714814464', 'DE_1925146714814465')
              } catch {}
              const uniqFallback = Array.from(new Set(fallbackPids.filter((pid) => pid && pid !== primaryPid)))
              const fallbackChannels: string[] = []
              if (uniqFallback.length) {
                console.log('[Notifications] adding fallback pid channels:', uniqFallback)
              }
              for (const pid of uniqFallback) {
                if (!cid) continue
                const fallbackPrefix = `custom.${cid}.${pid}.`
                fallbackChannels.push(`${fallbackPrefix}${context}`)
                if (gamerTag) {
                  fallbackChannels.push(`${fallbackPrefix}${context}.${gamerTag}`)
                  fallbackChannels.push(`${fallbackPrefix}${gamerTag}.${context}`)
                }
                if (gamerTag) {
                  fallbackChannels.push(`playerfr.${cid}.${pid}.${gamerTag}`)
                }
                fallbackChannels.push(`gamen.${cid}.${pid}`)
              }

              const allSubs = Array.from(new Set([...(sub.channelNames || []), ...wildcardCandidates, ...fallbackChannels]))
              sub.channelNames = allSubs
              console.log('[Notifications] PubNub subscribe channels:', allSubs)
              sub.pubnub.subscribe({ channels: allSubs, withPresence: false })
              // Keep polling in parallel as a fallback; do not stop here
              await ensureServerEventHandlers(sub.channelNames)
            }
          } catch (e) {
            console.warn('[Notifications] PubNub subscribe failed, falling back to polling:', (e as any)?.message || e)
          }
        } else {
          // deliver raw payload to allow caller inspection
          handler(cfg)
        }
      } else {
        await ensureServerEventHandlers(sub.channelNames)
      }

      // Fallback polling (if PubNub not available)
      if (sub.customPrefix) {
        const channelsToPoll = (sub.channelNames && sub.channelNames.length)
          ? sub.channelNames
          : [`${sub.customPrefix}${context}`]
        for (const ch of channelsToPoll) {
          try {
            const query: Record<string, string> = { channel: ch }
            const payload = await getOnce(requester, query)
            console.log('[Notifications] poll payload:', { context, channel: ch, payload })
            if (!payload) continue

            const forward = (msg: any) => {
              const withChannel = (msg && typeof msg === 'object' && !('channel' in msg))
                ? { ...msg, channel: ch }
                : msg
              try {
                console.log('[Notifications] delivering polled message', { context, channel: ch, message: withChannel })
                console.log('[Notifications] delivering polled message raw', JSON.stringify(withChannel))
              } catch {}
              normalizeAndHandle(withChannel)
            }

            if (Array.isArray(payload)) {
              for (const item of payload) forward(item)
            } else if (payload && Array.isArray((payload as any).m)) {
              for (const item of (payload as any).m) {
                forward(item?.d ?? item)
              }
            } else {
              forward(payload)
            }
          } catch (pe) {
            console.warn('[Notifications] poll channel error:', ch, (pe as any)?.message || pe)
          }
        }
      }
    } catch (e) {
      // eslint-disable-next-line no-console
      console.warn('[Notifications] poll error:', (e as any)?.message || e)
    } finally {
      if (!sub.stopped) {
        sub.timerId = (setTimeout(tick, intervalMs) as unknown) as number
      }
    }
  }

  sub.timerId = (setTimeout(tick, 0) as unknown) as number

  return {
    stop: () => {
      sub.stopped = true
      if (sub.timerId) {
        clearTimeout(sub.timerId as unknown as number)
        sub.timerId = null
      }
      try {
        if (sub.pubnub && sub.channelNames && sub.channelNames.length) {
          console.log('[Notifications] PubNub unsubscribe', sub.channelNames)
          sub.pubnub.unsubscribe({ channels: sub.channelNames })
        }
      } catch {}
      try {
        if (sub.serverEventStops && sub.serverEventStops.size) {
          for (const stop of sub.serverEventStops.values()) {
            try {
              stop()
            } catch {}
          }
          sub.serverEventStops.clear()
        }
      } catch {}
    },
  }
}

export async function subscribeExternalAuthNotifications(
  onAddress: NotificationHandler,
  onSignature: NotificationHandler,
  options?: { intervalMs?: number }
) {
  const addressSub = await subscribeToContext(PlayerNotificationContexts.ExternalAuthAddress, onAddress, options)
  const signatureSub = await subscribeToContext(PlayerNotificationContexts.ExternalAuthSignature, onSignature, options)
  return {
    stop: () => {
      addressSub.stop()
      signatureSub.stop()
    },
  }
}
