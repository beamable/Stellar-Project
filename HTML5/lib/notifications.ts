import type { HttpRequester } from 'beamable-sdk'
import getBeam from '@/lib/beam'

export type NotificationHandler = (data: any) => void

const NOTIFY_GET_PATH = '/basic/notification/'
const NOTIFY_POST_CHANNEL_PATH = '/basic/notification/channel'

export const PlayerNotificationContexts = {
  ExternalAuthAddress: 'external-auth-address',
  ExternalAuthSignature: 'external-auth-signature',
} as const

type Subscription = {
  context: string
  timerId: number | null
  stopped: boolean
}

async function ensureChannel(requester: HttpRequester, context: string) {
  try {
    await requester.request({
      url: NOTIFY_POST_CHANNEL_PATH,
      method: 'POST',
      withAuth: true,
      body: { channel: context },
    })
  } catch (e) {
    // Best-effort: if channel already exists or endpoint differs, ignore
    // Consumers will still poll GET for notifications below.
    // eslint-disable-next-line no-console
    console.debug('[Notifications] ensureChannel warning:', (e as any)?.message || e)
  }
}

async function getOnce(requester: HttpRequester, context: string) {
  const params = new URLSearchParams({ context })
  const url = `${NOTIFY_GET_PATH}?${params.toString()}`
  const res = await requester.request({ url, method: 'GET', withAuth: true })
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

  await ensureChannel(requester, context)

  const sub: Subscription = { context, timerId: null, stopped: false }

  const tick = async () => {
    if (sub.stopped) return
    try {
      const payload = await getOnce(requester, context)
      if (payload) {
        handler(payload)
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

