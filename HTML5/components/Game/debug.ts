import { GROUND_Y } from "./constants"
import type { Tower } from "./types"

const rawDebugFlag =
  typeof process !== "undefined" && process.env.NEXT_PUBLIC_TOWER_DEBUG
    ? process.env.NEXT_PUBLIC_TOWER_DEBUG.toLowerCase()
    : ""

export const DEBUG_COLLISION_MODE = rawDebugFlag === "true" || rawDebugFlag === "1"

export function createDebugTowers(): { towers: Tower[]; towerCount: number } {
  const baseX = 420
  const spacing = 34
  const towers: Tower[] = Array.from({ length: 12 }, (_, index) => {
    const width = 12
    const height = 50 + (index % 4) * 20
    const isSpecial = index % 3 === 0
    return {
      x: baseX + index * spacing,
      y: GROUND_Y - height,
      width,
      height,
      destroyed: false,
      color: isSpecial ? "#8B0000" : "#4682B4",
      isSpecial,
      hits: 0,
      maxHits: isSpecial ? 2 : 1,
    }
  })

  return { towers, towerCount: towers.length }
}
