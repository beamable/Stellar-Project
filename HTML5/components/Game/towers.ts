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

export type GenerateTowerOptions = {
  rng?: () => number
  minTowers?: number
  maxTowers?: number
  specialBlockRatio?: number
  movingTargetChance?: number
  movingAmplitude?: number
  movingSpeed?: number
}

/**
 * Generates a random set of towers with special blocks.
 * Allows campaign stages to override tower density + special ratios.
 */
export function generateTowers(options: GenerateTowerOptions = {}): { towers: Tower[]; towerCount: number } {
  const rng = options.rng ?? Math.random
  const towers: Tower[] = []

  const minTowers = options.minTowers ?? MIN_TOWERS
  const maxTowers = options.maxTowers ?? MAX_TOWERS
  const newTowerCount = Math.floor(rng() * (maxTowers - minTowers + 1)) + minTowers
  const specialBlockPercentage =
    typeof options.specialBlockRatio === "number"
      ? options.specialBlockRatio
      : newTowerCount > TOWER_THRESHOLD_FOR_HIGH_SPECIAL
        ? SPECIAL_BLOCK_PERCENTAGE_HIGH
        : SPECIAL_BLOCK_PERCENTAGE_LOW
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
      baseX: x,
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

  if (options.movingTargetChance && options.movingTargetChance > 0) {
    towers.forEach((tower) => {
      if (rng() <= options.movingTargetChance!) {
        const amplitude = options.movingAmplitude ?? 30
        const speed = options.movingSpeed ?? 0.002
        tower.motion = {
          originX: tower.baseX ?? tower.x,
          amplitude: amplitude * (0.6 + rng() * 0.8),
          speed: speed * (0.75 + rng() * 0.5),
          phase: rng() * Math.PI * 2,
        }
      }
    })
  }

  // Shuffle towers for random placement
  for (let i = towers.length - 1; i > 0; i--) {
    const j = Math.floor(rng() * (i + 1))
    ;[towers[i], towers[j]] = [towers[j], towers[i]]
  }

  return { towers, towerCount: newTowerCount }
}
