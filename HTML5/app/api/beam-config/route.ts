import { NextResponse } from "next/server"
import path from "path"
import { promises as fs } from "fs"

export const dynamic = "force-dynamic"
export const runtime = 'nodejs'

type BeamConfigFile = {
  cid?: string
  pid?: string
  host?: string
}

type BeamConfigResponse = {
  cid: string
  pid: string
  host?: string
  environment?: string
}

function normalizeEnvName(raw?: string | null): string | undefined {
  const trimmed = (raw || "").trim().toLowerCase()
  return trimmed.length > 0 ? trimmed : undefined
}

function normalizeHost(raw?: string | null): string | undefined {
  const trimmed = (raw || "").trim()
  return trimmed.length > 0 ? trimmed : undefined
}

export async function GET() {
  const envName = normalizeEnvName(process.env.NEXT_PUBLIC_BEAM_ENV || process.env.BEAM_ENV)

  // Primary source of truth: .beamable/connection-configuration.json
  try {
    const cfgPath = path.join(process.cwd(), ".beamable", "connection-configuration.json")
    const raw = await fs.readFile(cfgPath, "utf-8")
    const json = JSON.parse(raw) as BeamConfigFile
    const fileCid = (json.cid || "").trim()
    const filePid = (json.pid || "").trim()
    const host = normalizeHost(json.host)
    if (fileCid && filePid) {
      const payload: BeamConfigResponse = { cid: fileCid, pid: filePid }
      if (host) payload.host = host
      if (envName) payload.environment = envName
      return NextResponse.json(payload)
    }
  } catch {}

  // Fallback to environment variables if file is unavailable
  const cid = process.env.NEXT_PUBLIC_BEAM_CID || process.env.BEAM_CID || ""
  const pid = process.env.NEXT_PUBLIC_BEAM_PID || process.env.BEAM_PID || ""
  const host = normalizeHost(process.env.NEXT_PUBLIC_BEAM_HOST || process.env.BEAM_HOST)

  if (cid && pid) {
    const payload: BeamConfigResponse = { cid: cid.trim(), pid: pid.trim() }
    if (host) payload.host = host
    if (envName) payload.environment = envName
    return NextResponse.json(payload)
  }

  return NextResponse.json(
    {
      error:
        "Missing cid/pid. Ensure .beamable/connection-configuration.json exists or set NEXT_PUBLIC_BEAM_* / BEAM_* in .env.local",
    },
    { status: 404 },
  )
}
