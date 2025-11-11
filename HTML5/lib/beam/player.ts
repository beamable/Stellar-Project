import getBeam, { resolveBeamConfig, resetBeam } from "@/lib/beam"
import { getNotificationBootstrap, PlayerNotificationContexts, subscribeToContext, type NotificationHandler } from "@/lib/notifications"

type ProviderInfo = {
  providerService: string
  custodialNamespace: string
  externalNamespace: string
}

export type StellarIdentityInfo = {
  custodialId: string | null
  externalId: string | null
  providerService: string
  custodialNamespace: string
  externalNamespace: string
}

export type NotificationChannelDebugPayload = {
  channel: string
  body: any
}

export type ExternalAddressSubscription = {
  stop: () => void
}

export const EXTERNAL_AUTH_CONTEXT = PlayerNotificationContexts.ExternalAuthAddress

export async function initBeamPlayer() {
  const beam: any = await getBeam()
  return {
    playerId: beam?.player?.id ?? null,
  }
}

export async function fetchPlayerAlias(): Promise<string | null> {
  const beam: any = await getBeam()

  const readAlias = async (accessType: "private" | "public") => {
    try {
      const stats = await beam.stats.get({ domainType: "client", accessType, stats: ["Alias"] })
      const aliasCandidate = (stats && (stats as any).Alias) || ""
      return typeof aliasCandidate === "string" ? aliasCandidate : ""
    } catch {
      return ""
    }
  }

  return (await readAlias("private")) || (await readAlias("public")) || null
}

export async function fetchStellarIdentityInfo(): Promise<StellarIdentityInfo> {
  const beam: any = await getBeam()
  const { providerService, custodialNamespace, externalNamespace } = getProviderInfo(beam)
  try {
    const acct = await beam.account.current()
    const custodialEntry = (acct?.external || []).find(
      (entry: any) => entry.providerService === providerService && entry.providerNamespace === custodialNamespace,
    )
    const externalEntry = (acct?.external || []).find(
      (entry: any) => entry.providerService === providerService && entry.providerNamespace === externalNamespace,
    )
    return {
      custodialId: custodialEntry?.userId ?? null,
      externalId: externalEntry?.userId ?? null,
      providerService,
      custodialNamespace,
      externalNamespace,
    }
  } catch {
    return {
      custodialId: null,
      externalId: null,
      providerService,
      custodialNamespace,
      externalNamespace,
    }
  }
}

export async function saveAliasAndAttachWallet(alias: string): Promise<{ stellarId: string | null }> {
  const beam: any = await getBeam()
  await beam.stats.set({ domainType: "client", accessType: "private", stats: { Alias: alias } })

  try {
    const stellarId = await attachCustodialWallet(beam)
    return { stellarId }
  } catch (err) {
    const message = (err as any)?.message || "Failed to attach custodial wallet."
    throw new Error(message)
  }
}

export async function resetBeamSession() {
  try {
    const beam: any = await getBeam().catch(() => null)
    await beam?.tokenStorage?.clear?.()
    await beam?.tokenStorage?.dispose?.()
  } catch {}
  resetBeam()
}

export async function fetchNotificationDebugData(playerId: string | null): Promise<NotificationChannelDebugPayload[]> {
  const beam: any = await getBeam()
  const cid = beam?.cid
  const pid = beam?.pid
  if (!cid || !pid) {
    return []
  }
  const channelBase = `custom.${cid}.${pid}.external-auth-address`
  const channels = [channelBase, playerId ? `${channelBase}.${playerId}` : null].filter(Boolean) as string[]
  const results: NotificationChannelDebugPayload[] = []
  for (const channel of channels) {
    try {
      const res = await beam.requester.request({
        url: `/basic/notification/?channel=${encodeURIComponent(channel)}`,
        method: "GET",
        withAuth: true,
      })
      results.push({ channel, body: res.body })
    } catch (err) {
      results.push({ channel, body: { error: (err as any)?.message || err } })
    }
  }
  return results
}

export async function sendStellarTestNotification(message: string) {
  const beam: any = await getBeam()
  await beam?.stellarFederationClient?.sendTestNotification?.({ message })
}

export async function buildWalletConnectUrl(playerId: string | null): Promise<{ url: string }> {
  const beam: any = await getBeam()
  const config = await beam.stellarFederationClient.stellarConfiguration()
  const { cid, pid } = await deriveCidPid()
  const gamerTag = (playerId || "").toString()
  const url = `https://${config.walletConnectBridgeUrl}/?network=${encodeURIComponent(config.network)}&cid=${encodeURIComponent(
    cid,
  )}&pid=${encodeURIComponent(pid)}&gamerTag=${encodeURIComponent(gamerTag)}`
  return { url }
}

export async function subscribeToExternalAddress(
  handler: NotificationHandler,
  options?: { intervalMs?: number },
): Promise<ExternalAddressSubscription> {
  const sub = await subscribeToContext(PlayerNotificationContexts.ExternalAuthAddress, handler, options)
  return {
    stop: () => {
      try {
        sub.stop()
      } catch {}
    },
  }
}

export async function attachExternalIdentityToken(
  token: string,
  challengeHandler?: (challenge: string) => string | Promise<string>,
) {
  const beam: any = await getBeam()
  const { providerService, externalNamespace } = getProviderInfo(beam)
  const normalizedHandler: (challenge: string) => string | Promise<string> =
    typeof challengeHandler === 'function'
      ? challengeHandler
      : (challenge: string) => {
          console.warn('[Stellar] challengeHandler missing; returning empty string for challenge:', challenge)
          return ''
        }
  try {
    console.log('[Stellar] attachExternalIdentityToken params:', {
      token,
      providerService,
      providerNamespace: externalNamespace,
      hasChallengeHandler: typeof challengeHandler === 'function',
    })
  } catch {}
  const attemptedAuthAttach = await tryAuthAttach(beam, {
    token,
    providerService,
    providerNamespace: externalNamespace,
    challengeHandler: normalizedHandler,
  })
  if (attemptedAuthAttach) {
    return attemptedAuthAttach
  }
  const params: {
    externalToken: string
    providerService: string
    providerNamespace: string
    challengeHandler?: (challenge: string) => string | Promise<string>
  } = {
    externalToken: token,
    providerService,
    providerNamespace: externalNamespace,
  }
  params.challengeHandler = normalizedHandler
  return beam.account.addExternalIdentity(params)
}

async function tryAuthAttach(
  beam: any,
  opts: {
    token: string
    providerService: string
    providerNamespace: string
    challengeHandler?: (challenge: string) => string | Promise<string>
  },
) {
  try {
    const auth = beam?.auth
    const attachFn =
      auth?.attachExternalIdentityToken ??
      auth?.attachExternalIdentity ??
      auth?.attachIdentity
    if (typeof attachFn !== 'function') {
      return null
    }
    console.log('[Stellar] Using auth attach flow.')
    if (typeof opts.challengeHandler === 'function') {
      return await attachFn.call(auth, opts.token, opts.providerService, opts.providerNamespace, opts.challengeHandler)
    }
    return await attachFn.call(auth, opts.token, opts.providerService, opts.providerNamespace)
  } catch (err) {
    console.warn('[Stellar] Auth attach flow failed, falling back to account service:', (err as any)?.message || err)
    return null
  }
}

export async function loginExternalIdentityToken(token: string) {
  const beam: any = await getBeam()
  const { providerService, externalNamespace } = getProviderInfo(beam)
  return beam.auth.loginWithExternalIdentity({
    externalToken: token,
    providerService,
    providerNamespace: externalNamespace,
  })
}

export function fetchNotificationBootstrapData() {
  return getNotificationBootstrap()
}

function getProviderInfo(beam: any): ProviderInfo {
  const providerService: string = beam?.stellarFederationClient?.serviceName || "StellarFederation"
  const federationIds = beam?.stellarFederationClient?.federationIds || {}
  return {
    providerService,
    custodialNamespace: federationIds?.StellarIdentity || "StellarIdentity",
    externalNamespace: federationIds?.StellarExternalIdentity || "StellarExternalIdentity",
  }
}

async function attachCustodialWallet(beam: any) {
  const { providerService, custodialNamespace } = getProviderInfo(beam)
  await beam.account.addExternalIdentity({
    externalToken: "",
    providerService,
    providerNamespace: custodialNamespace,
  })
  try {
    const acct = await beam.account.current()
    const match = (acct?.external || []).find(
      (entry: any) => entry.providerService === providerService && entry.providerNamespace === custodialNamespace,
    )
    return match?.userId ?? null
  } catch {
    return null
  }
}

async function deriveCidPid(): Promise<{ cid: string; pid: string }> {
  const fromBootstrap = await cidPidFromBootstrap()
  if (fromBootstrap) return fromBootstrap
  const cfg = await resolveBeamConfig()
  return { cid: cfg.cid, pid: cfg.pid }
}

async function cidPidFromBootstrap(): Promise<{ cid: string; pid: string } | null> {
  try {
    const bootstrap = await getNotificationBootstrap()
    const prefix = String(bootstrap?.customChannelPrefix || "")
    const parts = prefix.split(".")
    if (parts.length >= 3) {
      const cid = (parts[1] || "").trim()
      const pid = (parts[2] || "").trim()
      if (cid && pid) {
        return { cid, pid }
      }
    }
  } catch {}
  if (typeof window !== "undefined") {
    const win = window as any
    if (win.__BEAM__?.cid && win.__BEAM__?.pid) {
      return { cid: String(win.__BEAM__.cid).trim(), pid: String(win.__BEAM__.pid).trim() }
    }
  }
  return null
}
