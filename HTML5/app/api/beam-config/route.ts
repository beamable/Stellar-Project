import { NextResponse } from "next/server"

export const dynamic = "force-dynamic"

export async function GET() {
  const cid = process.env.NEXT_PUBLIC_BEAM_CID || process.env.BEAM_CID || ""
  const pid = process.env.NEXT_PUBLIC_BEAM_PID || process.env.BEAM_PID || ""

  if (!cid || !pid) {
    return NextResponse.json(
      { error: "Missing cid/pid. Set NEXT_PUBLIC_BEAM_* or BEAM_* in .env.local" },
      { status: 404 },
    )
  }

  return NextResponse.json({ cid: cid.trim(), pid: pid.trim() })
}

