import { describe, expect, it } from "vitest"
import { checkCollision } from "../physics"

const tower = {
  x: 100,
  y: 100,
  width: 50,
  height: 80,
  destroyed: false,
  color: "#000",
  isSpecial: false,
  hits: 0,
  maxHits: 1,
}

const createBall = (overrides: Partial<Parameters<typeof checkCollision>[0]> = {}) => ({
  x: 0,
  y: 0,
  vx: 0,
  vy: 0,
  radius: 10,
  active: true,
  type: "normal" as const,
  id: 1,
  ...overrides,
})

describe("checkCollision", () => {
  it("returns true when the ball overlaps the tower", () => {
    const ball = createBall({ x: 110, y: 120 })
    expect(checkCollision(ball, tower)).toBe(true)
  })

  it("returns false when the ball is outside tower bounds", () => {
    const ball = createBall({ x: 40, y: 40 })
    expect(checkCollision(ball, tower)).toBe(false)
  })

  it("accounts for the ball radius along all axes", () => {
    const ball = createBall({ x: tower.x - 5, y: tower.y + tower.height / 2 })
    expect(checkCollision(ball, tower)).toBe(true)
  })
})
