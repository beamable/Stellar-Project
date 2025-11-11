import type { HttpRequester } from 'beamable-sdk'
import getBeam from '@/lib/beam'

export type NotificationHandler = (data: any) => void

const NOTIFY_GET_PATH = '/basic/notification/'

export const PlayerNotificationContexts = {
  ExternalAuthAddress: 'external-auth-address',
  ExternalAuthSignature: 'external-auth-signature',
} as const

type Subscription = {
  context: string
  stopped: boolean
  clientEventStop?: () => void
}

async function setupBeamRealtimeSubscription(
  context: string,
  handler: NotificationHandler,
  beam: any
): Promise<(() => void) | null> {
  if (typeof window === 'undefined') return null
  const socket: WebSocket | undefined = (beam as any)?.ws?.rawSocket
  if (!socket) {
    console.warn('[Notifications] Beam websocket not ready; skipping realtime subscription')
    return null
  }
  const key = context.toLowerCase()

  const listener = (event: MessageEvent) => {
    let payload: any
    try {
      payload = JSON.parse((event as MessageEvent).data ?? '{}')
    } catch (err) {
      console.warn('[Notifications] Unable to parse websocket payload:', err)
      return
    }
    const ctxRaw = payload?.context
    if (!ctxRaw || String(ctxRaw).toLowerCase() !== key) {
      return
    }

    let inner: any = payload
    const messageFull = payload?.messageFull ?? payload?.message_full
    if (typeof messageFull === 'string') {
      try {
        inner = JSON.parse(messageFull)
      } catch {
        inner = { Value: messageFull }
      }
    }

    const normalized =
      inner && typeof inner === 'object'
        ? { ...inner, context: inner.context ?? ctxRaw }
        : { Value: inner, context: ctxRaw }

    try {
      console.log('[Notifications] Beam websocket message received', normalized)
    } catch {}
    try {
      handler(normalized)
    } catch (err) {
      console.error('[Notifications] Beam websocket handler error:', (err as any)?.message || err)
    }
  }

  socket.addEventListener('message', listener)
  console.log('[Notifications] Beam websocket subscribed to context:', context)

  return () => {
    try {
      socket.removeEventListener('message', listener)
      console.log('[Notifications] Beam websocket unsubscribed from context:', context)
    } catch (err) {
      console.warn('[Notifications] Failed to remove websocket listener:', (err as any)?.message || err)
    }
  }
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
  _options?: { intervalMs?: number }
) {
  const beam: any = await getBeam()
  const sub: Subscription = { context, stopped: false }
  const contextKey = context.toLowerCase()
  console.log('[Notifications] subscribe start:', { context })

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

  try {
    const stop = await setupBeamRealtimeSubscription(context, normalizeAndHandle, beam)
    if (stop) {
      sub.clientEventStop = stop
    }
  } catch (err) {
    console.warn('[Notifications] Beam realtime subscription error:', (err as any)?.message || err)
  }

  return {
    stop: () => {
      if (sub.stopped) return
      sub.stopped = true
      try {
        sub.clientEventStop?.()
        sub.clientEventStop = undefined
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
