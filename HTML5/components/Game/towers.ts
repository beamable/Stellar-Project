/**
 * Tower generation and management for Tower Destroyer
 */

import type { Tower } from "./types"
const DEBUG = false
const dlog = (...args: any[]) => {
  if (DEBUG) console.log(...args)
}
import {
  MIN_TOWERS,
  MAX_TOWERS,
  SPECIAL_BLOCK_PERCENTAGE_LOW,
  SPECIAL_BLOCK_PERCENTAGE_HIGH,
  TOWER_THRESHOLD_FOR_HIGH_SPECIAL,
  GROUND_Y,
  TOWER_COLORS,
  SPECIAL_TOWER_COLORS,
} from "./constants"

/**
 * Generates a random set of towers with special blocks
 * Tower count ranges from MIN_TOWERS to MAX_TOWERS
 * Special blocks require 2 hits and give double points
 */
export function generateTowers(rng: () => number = Math.random): { towers: Tower[]; towerCount: number } {
  const towers: Tower[] = []

  const newTowerCount = Math.floor(rng() * (MAX_TOWERS - MIN_TOWERS + 1)) + MIN_TOWERS
  const specialBlockPercentage =
    newTowerCount > TOWER_THRESHOLD_FOR_HIGH_SPECIAL ? SPECIAL_BLOCK_PERCENTAGE_HIGH : SPECIAL_BLOCK_PERCENTAGE_LOW
  const specialBlockCount = Math.floor(newTowerCount * specialBlockPercentage)

  dlog(
    `[v0] Generating ${newTowerCount} towers with ${specialBlockCount} special blocks (${specialBlockPercentage * 100}%)`,
  )

  // Create towers with random positions and heights
  for (let i = 0; i < newTowerCount; i++) {
    const x = 400 + (i % 12) * 65 + rng() * 50
    const height = 20 + rng() * 120
    const y = GROUND_Y - height

    towers.push({
      x,
      y,
      width: 25 + rng() * 25,
      height,
      destroyed: false,
      color: TOWER_COLORS[Math.floor(rng() * TOWER_COLORS.length)],
      isSpecial: false,
      hits: 0,
      maxHits: 1,
    })
  }

  // Randomly select towers to be special blocks
  const specialIndices = new Set<number>()
  while (specialIndices.size < specialBlockCount) {
    const randomIndex = Math.floor(rng() * newTowerCount)
    specialIndices.add(randomIndex)
  }

  specialIndices.forEach((index) => {
    towers[index].isSpecial = true
    towers[index].maxHits = 2
    towers[index].color = SPECIAL_TOWER_COLORS[Math.floor(rng() * SPECIAL_TOWER_COLORS.length)]
  })

  // Shuffle towers for random placement
  for (let i = towers.length - 1; i > 0; i--) {
    const j = Math.floor(rng() * (i + 1))
    ;[towers[i], towers[j]] = [towers[j], towers[i]]
  }

  return { towers, towerCount: newTowerCount }
}
