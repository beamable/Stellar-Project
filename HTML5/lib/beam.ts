import { Beam, BeamEnvironment, clientServices, type BeamEnvironmentName } from "beamable-sdk"
import { StellarFederationClient } from "@/beamable/clients/StellarFederationClient"

export type BeamResolvedConfig = { cid: string; pid: string; environment: BeamEnvironmentName }

let beamPromise: Promise<any> | null = null
let cachedBeamConfig: BeamResolvedConfig | null = null
let currentBeamInstance: any | null = null

function normalizeEnvName(env?: string | null): BeamEnvironmentName {
  const normalized = (env || "prod").trim().toLowerCase() as BeamEnvironmentName
  return (normalized.length > 0 ? normalized : "prod") as BeamEnvironmentName
}

function stripTrailingSlash(url: string) {
  return url.replace(/\/+$/, "")
}

function ensureEnvironment(host: string | undefined, fallbackEnv: BeamEnvironmentName): BeamEnvironmentName {
  if (!host) return fallbackEnv
  const normalizedHost = stripTrailingSlash(host.trim())
  if (!normalizedHost) return fallbackEnv

  const existing = Object.entries(BeamEnvironment.list()).find(
    ([, cfg]) => stripTrailingSlash(cfg.apiUrl) === normalizedHost,
  )
  if (existing) {
    return existing[0] as BeamEnvironmentName
  }

  let baseConfig = BeamEnvironment.get("prod")
  try {
    baseConfig = BeamEnvironment.get(fallbackEnv)
  } catch {
    // ignore, use prod fallback
  }

  const slug = normalizedHost
    .replace(/^https?:\/\//, "")
    .replace(/[^a-zA-Z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase()
  const customEnv = (slug ? `${fallbackEnv}-${slug}` : `${fallbackEnv}-custom`) as BeamEnvironmentName
  const config = { ...baseConfig, apiUrl: normalizedHost }
  BeamEnvironment.register(customEnv, config)
  return customEnv
}

export async function resolveBeamConfig(): Promise<BeamResolvedConfig> {
  if (cachedBeamConfig) {
    return cachedBeamConfig
  }

  const envName = normalizeEnvName(process.env.NEXT_PUBLIC_BEAM_ENV || process.env.BEAM_ENV)

  const remember = (cfg: { cid: string; pid: string; env?: BeamEnvironmentName; host?: string }) => {
    const environment = ensureEnvironment(cfg.host, cfg.env ?? envName)
    const final: BeamResolvedConfig = { cid: cfg.cid, pid: cfg.pid, environment }
    cachedBeamConfig = final
    return final
  }

  const cidEnv = (process.env.NEXT_PUBLIC_BEAM_CID || "").trim()
  const pidEnv = (process.env.NEXT_PUBLIC_BEAM_PID || "").trim()
  const hostEnv = (process.env.NEXT_PUBLIC_BEAM_HOST || "").trim() || (process.env.BEAM_HOST || "").trim()
  if (cidEnv && pidEnv) {
    return remember({ cid: cidEnv, pid: pidEnv, env: envName, host: hostEnv })
  }

  if (typeof window !== "undefined") {
    const w = window as any
    const fromWindow = w.__BEAM__ as (Partial<BeamResolvedConfig> & { host?: string }) | undefined
    if (fromWindow?.cid && fromWindow?.pid) {
      return remember({
        cid: fromWindow.cid,
        pid: fromWindow.pid,
        env: fromWindow.environment,
        host: fromWindow.host,
      })
    }

    // Read from API route -> uses .beamable/connection-configuration.json at runtime
    try {
      const res = await fetch("/api/beam-config", { cache: "no-store" })
      if (res.ok) {
        const data = (await res.json()) as Partial<BeamResolvedConfig> & { host?: string }
        if (data.cid && data.pid) {
          return remember({ cid: data.cid, pid: data.pid, env: data.environment, host: data.host })
        }
      }
    } catch {}
  }

  throw new Error(
    `Missing Beam config. Preferred source is .beamable/connection-configuration.json (via /api/beam-config). Alternatively set NEXT_PUBLIC_BEAM_CID/NEXT_PUBLIC_BEAM_PID or provide window.__BEAM__ = { cid, pid }`,
  )
}

function newInstanceTag() {
  try {
    // @ts-ignore
    const g = typeof crypto !== 'undefined' && crypto?.randomUUID?.()
    if (g) return g
  } catch {}
  return Math.random().toString(36).slice(2)
}

function getOrCreateTabInstanceTag(): string {
  if (typeof window === 'undefined') return newInstanceTag()
  try {
    const key = 'BEAM_TAB_INSTANCE_TAG'
    const params = new URLSearchParams(window.location.search)
    if (params.has('beam_new') || params.has('newPlayer')) {
      try { window.sessionStorage.removeItem(key) } catch {}
    }
    const existing = window.sessionStorage.getItem(key)
    if (existing && existing.length > 0) return existing
    const fresh = newInstanceTag()
    window.sessionStorage.setItem(key, fresh)
    return fresh
  } catch {
    return newInstanceTag()
  }
}

async function bootBeamOnce(cfg: BeamResolvedConfig, tag?: string) {
  const instanceTag = tag || getOrCreateTabInstanceTag()
  const beam = await Beam.init({
    cid: cfg.cid,
    pid: cfg.pid,
    environment: cfg.environment,
    instanceTag,
  })
  clientServices(beam)
  ;(beam as any).use?.(StellarFederationClient)
  // Ensure we have a token
  try {
    const tokenData = await (beam as any)?.tokenStorage?.getTokenData?.()
    const hasAccess = !!(tokenData && tokenData.accessToken)
    if (!hasAccess) {
      const tokenResponse = await beam.auth.loginAsGuest()
      if ((beam as any)?.refresh) {
        await (beam as any).refresh(tokenResponse)
      }
    }
  } catch (e) {
    console.warn('[Beam] Auth bootstrap warning:', (e as any)?.message || e)
  }
  currentBeamInstance = beam
  return beam
}

export function getBeam() {
  if (!beamPromise) {
    beamPromise = (async () => {
      const cfg = await resolveBeamConfig()
      try {
        return await bootBeamOnce(cfg)
      } catch (e) {
        console.warn('[Beam] First init failed, retrying with fresh instanceTag:', (e as any)?.message || e)
        // Retry once with a new instanceTag
        return await bootBeamOnce(cfg)
      }
    })()
  }
  return beamPromise
}

export function resetBeam() {
  try {
    currentBeamInstance?.tokenStorage?.dispose?.()
  } catch {}
  currentBeamInstance = null
  beamPromise = null
}

export default getBeam
