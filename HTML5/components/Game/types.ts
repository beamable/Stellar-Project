/**
 * Type definitions for the Tower Destroyer game
 */

export type BallType = "normal" | "multishot" | "fire" | "laser"

export interface Ball {
  x: number
  y: number
  lastX: number
  lastY: number
  vx: number
  vy: number
  radius: number
  active: boolean
  type: BallType
  id: number
  fireDestroyCount?: number
  shotTime?: number
}

export interface Laser {
  x: number
  y: number
  angle: number
  length: number
  active: boolean
  hitCount: number
  maxHits: number
  ballId: number
}

export interface Tower {
  x: number
  y: number
  width: number
  height: number
  destroyed: boolean
  color: string
  isSpecial: boolean
  hits: number
  maxHits: number
  baseX?: number
  motion?: TowerMotion
}

export interface Particle {
  x: number
  y: number
  vx: number
  vy: number
  life: number
  maxLife: number
  color: string
  type: "normal" | "fire" | "laser" | "multishot" | "win" | "lose"
  size: number
}

export interface BallTypeConfig {
  type: BallType
  name: string
  icon: string
  description: string
  color: string
  baseSpeedMultiplier: number
}

export interface TowerMotion {
  originX: number
  amplitude: number
  speed: number
  phase: number
}

export interface WindZone {
  id: string
  xStart: number
  xEnd: number
  yStart: number
  yEnd: number
  force: number
  pulseMs?: number
  phase?: number
}
