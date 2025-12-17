"use client"

export const mintingDelayOffset = {
  minSeconds: 10,
  maxSeconds: 15,
} as const

function clamp(value: number, min: number, max: number) {
  return Math.min(max, Math.max(min, value))
}

export function getMintingDelayOffsetMs(): number {
  if (process.env.NODE_ENV === "test") return 0
  const minMs = mintingDelayOffset.minSeconds * 1000
  const maxMs = mintingDelayOffset.maxSeconds * 1000
  const normalizedMin = Math.min(minMs, maxMs)
  const normalizedMax = Math.max(minMs, maxMs)
  const raw = normalizedMin + Math.random() * (normalizedMax - normalizedMin)
  return Math.round(clamp(raw, normalizedMin, normalizedMax))
}

export async function waitForMintingDelayOffset(): Promise<void> {
  const ms = getMintingDelayOffsetMs()
  if (ms <= 0) return
  await new Promise<void>((resolve) => setTimeout(resolve, ms))
}

