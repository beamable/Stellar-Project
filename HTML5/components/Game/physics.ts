/**
 * Physics and collision detection utilities for Tower Destroyer
 */

import type { Ball, Tower } from "./types"

const EPSILON = 1e-6

type Vec2 = { x: number; y: number }

export type CollisionResult =
  | { collided: false }
  | {
      collided: true
      wasSwept: boolean
      normal: Vec2
      penetration: number
      impactPoint: Vec2
      contactPoint: Vec2
      timeOfImpact: number
    }

export type ConfirmedCollisionResult = Extract<CollisionResult, { collided: true }>

const clamp = (value: number, min: number, max: number) => Math.max(min, Math.min(max, value))

const magnitude = (x: number, y: number) => Math.sqrt(x * x + y * y)

const normalize = (x: number, y: number): Vec2 => {
  const length = magnitude(x, y)
  if (length < EPSILON) {
    return { x: 0, y: 0 }
  }
  return { x: x / length, y: y / length }
}

const getInteriorNormal = (ball: Ball, tower: Tower): Vec2 => {
  const leftDistance = ball.x - tower.x
  const rightDistance = tower.x + tower.width - ball.x
  const topDistance = ball.y - tower.y
  const bottomDistance = tower.y + tower.height - ball.y

  const minDistance = Math.min(leftDistance, rightDistance, topDistance, bottomDistance)

  if (minDistance === leftDistance) return { x: -1, y: 0 }
  if (minDistance === rightDistance) return { x: 1, y: 0 }
  if (minDistance === topDistance) return { x: 0, y: -1 }
  return { x: 0, y: 1 }
}

const staticCircleRectCollision = (ball: Ball, tower: Tower): CollisionResult | null => {
  const closestX = clamp(ball.x, tower.x, tower.x + tower.width)
  const closestY = clamp(ball.y, tower.y, tower.y + tower.height)

  const deltaX = ball.x - closestX
  const deltaY = ball.y - closestY
  const distanceSquared = deltaX * deltaX + deltaY * deltaY
  const radiusSquared = ball.radius * ball.radius

  if (distanceSquared > radiusSquared) {
    return null
  }

  const distance = Math.sqrt(distanceSquared)
  const penetration = ball.radius - distance

  const normal =
    distance > EPSILON
      ? normalize(deltaX, deltaY)
      : getInteriorNormal(ball, tower)

  const impactPoint = { x: ball.x, y: ball.y }
  const contactPoint =
    distance > EPSILON
      ? { x: closestX, y: closestY }
      : {
          x: clamp(ball.x, tower.x, tower.x + tower.width),
          y: clamp(ball.y, tower.y, tower.y + tower.height),
        }

  return {
    collided: true,
    wasSwept: false,
    normal,
    penetration: Math.max(penetration, 0),
    impactPoint,
    contactPoint,
    timeOfImpact: 1,
  }
}

type AxisIntersection = {
  entry: number
  exit: number
  normalSign: number
}

const computeAxisIntersection = (start: number, velocity: number, min: number, max: number): AxisIntersection | null => {
  if (Math.abs(velocity) < EPSILON) {
    if (start < min || start > max) {
      return null
    }
    return { entry: -Infinity, exit: Infinity, normalSign: 0 }
  }

  const invVelocity = 1 / velocity
  const t1 = (min - start) * invVelocity
  const t2 = (max - start) * invVelocity

  return {
    entry: Math.min(t1, t2),
    exit: Math.max(t1, t2),
    normalSign: t1 < t2 ? -1 : 1,
  }
}

const sweptCircleRectCollision = (ball: Ball, tower: Tower): CollisionResult | null => {
  const velocityX = ball.x - ball.lastX
  const velocityY = ball.y - ball.lastY

  if (Math.abs(velocityX) < EPSILON && Math.abs(velocityY) < EPSILON) {
    return null
  }

  const expandedMinX = tower.x - ball.radius
  const expandedMaxX = tower.x + tower.width + ball.radius
  const expandedMinY = tower.y - ball.radius
  const expandedMaxY = tower.y + tower.height + ball.radius

  const xIntersection = computeAxisIntersection(ball.lastX, velocityX, expandedMinX, expandedMaxX)
  const yIntersection = computeAxisIntersection(ball.lastY, velocityY, expandedMinY, expandedMaxY)

  if (!xIntersection || !yIntersection) {
    return null
  }

  let entryTime = Math.max(xIntersection.entry, yIntersection.entry)
  const exitTime = Math.min(xIntersection.exit, yIntersection.exit)

  if (entryTime > exitTime || exitTime < 0 || entryTime > 1) {
    return null
  }

  entryTime = Math.max(entryTime, 0)

  let normal: Vec2 = { x: 0, y: 0 }
  if (xIntersection.entry > yIntersection.entry) {
    normal = { x: xIntersection.normalSign, y: 0 }
  } else {
    normal = { x: 0, y: yIntersection.normalSign }
  }

  const impactPoint = {
    x: ball.lastX + velocityX * entryTime,
    y: ball.lastY + velocityY * entryTime,
  }

  const contactPoint = {
    x: impactPoint.x - normal.x * ball.radius,
    y: impactPoint.y - normal.y * ball.radius,
  }

  return {
    collided: true,
    wasSwept: true,
    normal,
    penetration: 0,
    impactPoint,
    contactPoint,
    timeOfImpact: entryTime,
  }
}

export function detectBallTowerCollision(ball: Ball, tower: Tower): CollisionResult {
  const staticResult = staticCircleRectCollision(ball, tower)
  if (staticResult) {
    return staticResult
  }

  const sweptResult = sweptCircleRectCollision(ball, tower)
  if (sweptResult) {
    return sweptResult
  }

  return { collided: false }
}
