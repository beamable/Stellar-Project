import { Beam } from "beamable-sdk"

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

export function getBeam() {
  if (!beamPromise) {
    beamPromise = (async () => {
      const cfg = await resolveBeamConfig()
      return Beam.init({ cid: cfg.cid, pid: cfg.pid, environment: cfg.environment })
    })()
  }
  return beamPromise
}