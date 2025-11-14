import { describe, expect, it } from "vitest"
import { generateTowers } from "../towers"
import {
  MIN_TOWERS,
  MAX_TOWERS,
  TOWER_THRESHOLD_FOR_HIGH_SPECIAL,
  SPECIAL_BLOCK_PERCENTAGE_HIGH,
  SPECIAL_BLOCK_PERCENTAGE_LOW,
} from "../constants"

const createDeterministicRng = (seed = 1) => {
  let value = seed
  return () => {
    const next = Math.sin(value++) * 10000
    return next - Math.floor(next)
  }
}

const createScriptedRng = (headValues: number[], tailSeed = 1) => {
  const tail = createDeterministicRng(tailSeed)
  let index = 0
  return () => {
    if (index < headValues.length) {
      return headValues[index++]
    }
    return tail()
  }
}

describe("generateTowers", () => {
  it("returns towers within configured bounds", () => {
    const rng = createDeterministicRng(2)
    const { towers, towerCount } = generateTowers(rng)

    expect(towerCount).toBeGreaterThanOrEqual(MIN_TOWERS)
    expect(towerCount).toBeLessThanOrEqual(MAX_TOWERS)
    expect(towers).toHaveLength(towerCount)
    expect(towers.every((tower) => tower.height > 0 && tower.width > 0)).toBe(true)
  })

  it("uses low special block percentage below the threshold", () => {
    // Force tower count to MIN_TOWERS by returning 0 on the first RNG call
    const rng = createScriptedRng([0])
    const { towers, towerCount } = generateTowers(rng)
    expect(towerCount).toBeLessThanOrEqual(TOWER_THRESHOLD_FOR_HIGH_SPECIAL)

    const expectedSpecialBlocks = Math.floor(towerCount * SPECIAL_BLOCK_PERCENTAGE_LOW)
    const actualSpecialBlocks = towers.filter((tower) => tower.isSpecial).length

    expect(actualSpecialBlocks).toBe(expectedSpecialBlocks)
    expect(towers.filter((tower) => tower.isSpecial).every((tower) => tower.maxHits === 2)).toBe(true)
  })

  it("uses high special block percentage above the threshold", () => {
    // Force tower count near the maximum by returning 0.99 on the first RNG call
    const rng = createScriptedRng([0.99])
    const { towers, towerCount } = generateTowers(rng)
    expect(towerCount).toBeGreaterThan(TOWER_THRESHOLD_FOR_HIGH_SPECIAL)

    const expectedSpecialBlocks = Math.floor(towerCount * SPECIAL_BLOCK_PERCENTAGE_HIGH)
    const actualSpecialBlocks = towers.filter((tower) => tower.isSpecial).length

    expect(actualSpecialBlocks).toBe(expectedSpecialBlocks)
  })
})
