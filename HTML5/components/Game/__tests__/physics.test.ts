import { describe, expect, it } from "vitest"
import { detectBallTowerCollision } from "../physics"

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

const createBall = (overrides: Partial<Parameters<typeof detectBallTowerCollision>[0]> = {}) => ({
  x: 0,
  y: 0,
  lastX: 0,
  lastY: 0,
  vx: 0,
  vy: 0,
  radius: 10,
  active: true,
  type: "normal" as const,
  id: 1,
  ...overrides,
})

describe("detectBallTowerCollision", () => {
  it("returns true when the ball overlaps the tower", () => {
    const ball = createBall({ x: 110, y: 120 })
    const result = detectBallTowerCollision(ball, tower)
    expect(result.collided).toBe(true)
    if (!result.collided) {
      throw new Error("Expected a collision for overlapping ball")
    }
    expect(result.wasSwept).toBe(false)
  })

  it("returns false when the ball is outside tower bounds", () => {
    const ball = createBall({ x: 40, y: 40 })
    const result = detectBallTowerCollision(ball, tower)
    expect(result.collided).toBe(false)
  })

  it("accounts for the ball radius along all axes", () => {
    const ball = createBall({ x: tower.x - 5, y: tower.y + tower.height / 2 })
    const result = detectBallTowerCollision(ball, tower)
    expect(result.collided).toBe(true)
  })

  it("detects swept collisions when the ball moves quickly through a tower", () => {
    const ball = createBall({
      lastX: tower.x - 50,
      x: tower.x + tower.width + 50,
      y: tower.y + tower.height / 2,
      lastY: tower.y + tower.height / 2,
    })

    const result = detectBallTowerCollision(ball, tower)
    expect(result.collided).toBe(true)
    if (!result.collided) {
      throw new Error("Expected swept collision to register")
    }
    expect(result.wasSwept).toBe(true)
    expect(result.timeOfImpact).toBeGreaterThanOrEqual(0)
    expect(result.timeOfImpact).toBeLessThanOrEqual(1)
  })
})
