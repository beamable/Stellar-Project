import { describe, expect, it } from "vitest"
import { BALL_TYPES } from "../ballTypes"
import type { BallType } from "../types"

describe("BALL_TYPES", () => {
  it("matches the documented configuration", () => {
    expect(BALL_TYPES).toMatchSnapshot()
  })

  it("covers each BallType exactly once", () => {
    const typeSet = new Set(BALL_TYPES.map((ball) => ball.type))
    expect(typeSet.size).toBe(BALL_TYPES.length)
  })

  it("provides a description and color for each entry", () => {
    BALL_TYPES.forEach((ball) => {
      expect(typeof ball.description).toBe("string")
      expect(ball.description.length).toBeGreaterThan(0)
      expect(ball.color.startsWith("#")).toBe(true)
      expect(ball.baseSpeedMultiplier).toBeGreaterThan(0)
    })
  })

  it("only contains valid BallType discriminators", () => {
    const allowedTypes: BallType[] = ["normal", "multishot", "fire", "laser"]
    BALL_TYPES.forEach((ball) => {
      expect(allowedTypes).toContain(ball.type)
    })
  })
})
