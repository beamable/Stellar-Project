import { Beam, StatsService } from "beamable-sdk"

type BeamConfig = { cid: string; pid: string; environment?: "prod" | "stg" | "dev" }

let beamPromise: Promise<any> | null = null

async function resolveBeamConfig(): Promise<BeamConfig> {
  const cidEnv = (process.env.NEXT_PUBLIC_BEAM_CID || "").trim()
  const pidEnv = (process.env.NEXT_PUBLIC_BEAM_PID || "").trim()
  const envName = ((process.env.NEXT_PUBLIC_BEAM_ENV || process.env.BEAM_ENV || "prod").trim().toLowerCase() || "prod") as
    | "prod"
    | "stg"
    | "dev"

  if (cidEnv && pidEnv) {
    return { cid: cidEnv, pid: pidEnv, environment: envName }
  }

  if (typeof window !== "undefined") {
    const w = window as any
    const fromWindow = w.__BEAM__ as Partial<BeamConfig> | undefined
    if (fromWindow?.cid && fromWindow?.pid) {
      return { cid: fromWindow.cid, pid: fromWindow.pid, environment: fromWindow.environment }
    }

    // Read from API route -> uses .beamable/connection-configuration.json at runtime
    try {
      const res = await fetch("/api/beam-config", { cache: "no-store" })
      if (res.ok) {
        const data = (await res.json()) as Partial<BeamConfig>
        if (data.cid && data.pid) {
          return { cid: data.cid, pid: data.pid, environment: (data as any).environment }
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

async function bootBeamOnce(cfg: BeamConfig, tag?: string) {
  const instanceTag = tag || getOrCreateTabInstanceTag()
  const beam = await Beam.init({
    cid: cfg.cid,
    pid: cfg.pid,
    environment: cfg.environment,
    instanceTag,
  })
  ;(beam as any).use?.(StatsService)
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

export default getBeam