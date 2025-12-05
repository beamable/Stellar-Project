"use client"

const DEBUG_ENABLED = process.env.NEXT_PUBLIC_DEBUG_LOGS === "true" || process.env.NODE_ENV !== "production"

export function debugLog(...args: unknown[]) {
  if (!DEBUG_ENABLED) return
  try {
    console.log(...args)
  } catch {}
}
