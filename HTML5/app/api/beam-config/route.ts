import { NextResponse } from "next/server"
import path from "path"
import { promises as fs } from "fs"

export const dynamic = "force-dynamic"
export const runtime = 'nodejs'

export async function GET() {
  // Primary source of truth: .beamable/connection-configuration.json
  try {
    const cfgPath = path.join(process.cwd(), ".beamable", "connection-configuration.json")
    const raw = await fs.readFile(cfgPath, "utf-8")
    const json = JSON.parse(raw) as { cid?: string; pid?: string }
    const fileCid = (json.cid || "").trim()
    const filePid = (json.pid || "").trim()
    if (fileCid && filePid) {
      return NextResponse.json({ cid: fileCid, pid: filePid })
    }
  } catch {}

  // Fallback to environment variables if file is unavailable
  const cid = process.env.NEXT_PUBLIC_BEAM_CID || process.env.BEAM_CID || ""
  const pid = process.env.NEXT_PUBLIC_BEAM_PID || process.env.BEAM_PID || ""

  if (cid && pid) {
    return NextResponse.json({ cid: cid.trim(), pid: pid.trim() })
  }

  return NextResponse.json(
    {
      error:
        "Missing cid/pid. Ensure .beamable/connection-configuration.json exists or set NEXT_PUBLIC_BEAM_* / BEAM_* in .env.local",
    },
    { status: 404 },
  )
}