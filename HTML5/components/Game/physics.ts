/**
 * Physics and collision detection utilities for Tower Destroyer
 */

import type { Ball, Tower } from "./types"

/**
 * Checks if a ball is colliding with a tower using AABB collision detection
 */
export function checkCollision(ball: Ball, tower: Tower): boolean {
  return (
    ball.x + ball.radius > tower.x &&
    ball.x - ball.radius < tower.x + tower.width &&
    ball.y + ball.radius > tower.y &&
    ball.y - ball.radius < tower.y + tower.height
  )
}
