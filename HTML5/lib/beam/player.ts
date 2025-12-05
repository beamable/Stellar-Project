import type { Beam } from "beamable-sdk"
import getBeam, { resolveBeamConfig, resetBeam } from "@/lib/beam"
import { getNotificationBootstrap, PlayerNotificationContexts, subscribeToContext, type NotificationHandler } from "@/lib/notifications"

type StellarFederationClient = {
  attachCustodialWallet?: () => Promise<{ stellarId?: string; userId?: string } | void>
  stellarConfiguration: () => Promise<{
    walletConnectBridgeUrl?: string
    network?: string | number
  }>
  updateCurrency?: (payload: { currencyContentId: string; amount: number }) => Promise<unknown>
  sendTestNotification?: (payload: { message: string }) => Promise<unknown>
}

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

type ExternalChallengePayload = {
  challenge_token: string
  challenge_ttl?: number
  user_id?: string | null
}

type ExternalChallengeRequestResult = ExternalChallengePayload & {
  challengeResponse: ExternalChallengePayload
}

type Deferred<T> = {
  promise: Promise<T>
  resolve: (value: T | PromiseLike<T>) => void
  reject: (reason?: any) => void
  readonly settled: boolean
}

type PendingExternalIdentityAttach = {
  challengeDeferred: Deferred<ExternalChallengeRequestResult>
  signatureDeferred: Deferred<string>
  attachPromise: Promise<any>
  challengeToken?: string
}

let pendingExternalIdentityAttach: PendingExternalIdentityAttach | null = null

function createDeferred<T>(): Deferred<T> {
  let isSettled = false
  let resolveFn: (value: T | PromiseLike<T>) => void = () => {}
  let rejectFn: (reason?: any) => void = () => {}
  const promise = new Promise<T>((resolve, reject) => {
    resolveFn = (value) => {
      if (isSettled) return
      isSettled = true
      resolve(value)
    }
    rejectFn = (reason) => {
      if (isSettled) return
      isSettled = true
      reject(reason)
    }
  })
  return {
    promise,
    resolve: resolveFn,
    reject: rejectFn,
    get settled() {
      return isSettled
    },
  }
}

export const EXTERNAL_AUTH_CONTEXT = PlayerNotificationContexts.ExternalAuthAddress
export const EXTERNAL_SIGN_CONTEXT = PlayerNotificationContexts.ExternalAuthSignature

export async function initBeamPlayer() {
  const beam = (await getBeam()) as Beam & { stellarFederationClient?: StellarFederationClient }
  return {
    playerId: beam?.player?.id ?? null,
  }
}

export async function fetchPlayerAlias(): Promise<string | null> {
  const beam = (await getBeam()) as Beam

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
  const beam = (await getBeam()) as Beam
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
  const beam = (await getBeam()) as Beam & { stellarFederationClient?: StellarFederationClient }
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
    const beam = (await getBeam().catch(() => null)) as (Beam & { tokenStorage?: any }) | null
    await beam?.tokenStorage?.clear?.()
    await beam?.tokenStorage?.dispose?.()
  } catch {}
  resetBeam()
}

export async function fetchNotificationDebugData(playerId: string | null): Promise<NotificationChannelDebugPayload[]> {
  const beam = (await getBeam()) as Beam
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
  const beam = (await getBeam()) as Beam & { stellarFederationClient?: StellarFederationClient }
  await beam?.stellarFederationClient?.sendTestNotification?.({ message })
}

export async function buildWalletConnectUrl(playerId: string | null): Promise<{ url: string }> {
  const beam = (await getBeam()) as Beam & { stellarFederationClient: StellarFederationClient }
  if (!beam.stellarFederationClient) {
    throw new Error("[Stellar] stellarFederationClient unavailable on Beam instance.")
  }
  const config = await beam.stellarFederationClient.stellarConfiguration()
  const { cid, pid } = await deriveCidPid()
  const gamerTag = (playerId || "").toString()
  const rawBridge = String(config.walletConnectBridgeUrl || "").trim()
  if (!rawBridge) {
    throw new Error("[Stellar] Missing walletConnectBridgeUrl from stellar federation configuration.")
  }
  const hasProtocol = /^https?:\/\//i.test(rawBridge)
  const base = hasProtocol ? rawBridge : `https://${rawBridge}`
  const normalizedBase = base.endsWith("/") ? base : `${base}/`
  const url = new URL(normalizedBase)
  url.searchParams.set("network", String(config.network || ""))
  url.searchParams.set("cid", String(cid))
  url.searchParams.set("pid", String(pid))
  url.searchParams.set("gamerTag", gamerTag)
  return { url: url.toString() }
}

export async function subscribeToExternalContext(
  context: string,
  handler: NotificationHandler,
  options?: { intervalMs?: number },
): Promise<ExternalAddressSubscription> {
  const sub = await subscribeToContext(context, handler, options)
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
  return beam.account.addExternalIdentity({
    externalToken: token,
    providerService,
    providerNamespace: externalNamespace,
    challengeHandler,
  })
}

export async function requestExternalIdentityChallenge(token: string) {
  if (pendingExternalIdentityAttach) {
    throw new Error("[Stellar] An external identity challenge is already in progress.")
  }
  const beam: any = await getBeam()
  const { providerService, externalNamespace } = getProviderInfo(beam)
  const challengeDeferred = createDeferred<ExternalChallengeRequestResult>()
  const signatureDeferred = createDeferred<string>()
  const session: PendingExternalIdentityAttach = {
    challengeDeferred,
    signatureDeferred,
    attachPromise: Promise.resolve(null),
  }
  session.attachPromise = beam.account
    .addExternalIdentity({
      externalToken: token,
      providerService,
      providerNamespace: externalNamespace,
      challengeHandler: async (challenge: string) => {
        const payload: ExternalChallengePayload = {
          challenge_token: challenge,
        }
        session.challengeToken = payload.challenge_token
        challengeDeferred.resolve({
          ...payload,
          challengeResponse: payload,
        })
        return signatureDeferred.promise
      },
    })
    .then((result: any) => {
      if (!challengeDeferred.settled) {
        const err = new Error("[Stellar] Beam account.addExternalIdentity completed without issuing a challenge.")
        challengeDeferred.reject(err)
        throw err
      }
      return result
    })
    .catch((err: any) => {
      if (!challengeDeferred.settled) {
        challengeDeferred.reject(err)
      }
      if (!signatureDeferred.settled) {
        signatureDeferred.reject(err)
      }
      throw err
    })
    .finally(() => {
      if (pendingExternalIdentityAttach === session) {
        pendingExternalIdentityAttach = null
      }
    })
  pendingExternalIdentityAttach = session
  return challengeDeferred.promise
}

export async function completeExternalIdentityChallenge(challengeToken: string, signature: string) {
  const session = pendingExternalIdentityAttach
  if (!session) {
    throw new Error("[Stellar] No external identity challenge is currently pending.")
  }
  if (session.challengeToken && session.challengeToken !== challengeToken) {
    console.warn("[Stellar] Challenge token mismatch detected. Continuing with provided signature.")
  }
  if (session.signatureDeferred.settled) {
    console.warn("[Stellar] Signature has already been provided for the current challenge.")
    return session.attachPromise
  }
  session.signatureDeferred.resolve(signature)
  return session.attachPromise
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
