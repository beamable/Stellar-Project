import type React from "react"
/**
 * Particle system for Tower Destroyer
 * Creates and manages visual effects for collisions and game events
 */

import type { Particle, BallType } from "./types"

const DEBUG = false
const dlog = (...args: any[]) => {
  if (DEBUG) console.log(...args)
}
import {
  PARTICLE_COUNT_NORMAL,
  PARTICLE_COUNT_MULTISHOT,
  PARTICLE_COUNT_FIRE,
  PARTICLE_COUNT_LASER,
  PARTICLE_COUNT_LASER_DESTRUCTION,
  PARTICLE_COUNT_WIN,
  PARTICLE_COUNT_LOSE,
  CANVAS_WIDTH,
  CANVAS_HEIGHT,
  FIRE_COLORS,
  LASER_COLORS,
  MULTISHOT_COLORS,
  WIN_COLORS,
  LOSE_COLORS,
} from "./constants"

/**
 * Creates particles at a specific location with ball-type-specific effects
 */
export function createParticles(
  particlesRef: React.MutableRefObject<Particle[]>,
  x: number,
  y: number,
  ballType: BallType,
  towerColor: string,
): void {
  const particleCount =
    ballType === "fire"
      ? PARTICLE_COUNT_FIRE
      : ballType === "multishot"
        ? PARTICLE_COUNT_MULTISHOT
        : PARTICLE_COUNT_NORMAL

  dlog(`[v0] Creating ${particleCount} particles at (${x}, ${y}) for ${ballType} ball`)

  for (let i = 0; i < particleCount; i++) {
    let color = towerColor
    let size = 3
    let speed = 8

    // Customize particle appearance based on ball type
    if (ballType === "fire") {
      color = FIRE_COLORS[Math.floor(Math.random() * FIRE_COLORS.length)]
      size = 4 + Math.random() * 3
      speed = 10
    } else if (ballType === "laser") {
      color = LASER_COLORS[Math.floor(Math.random() * LASER_COLORS.length)]
      size = 2 + Math.random() * 2
      speed = 12
    } else if (ballType === "multishot") {
      color = MULTISHOT_COLORS[Math.floor(Math.random() * MULTISHOT_COLORS.length)]
      size = 3 + Math.random() * 2
      speed = 9
    } else {
      size = 3 + Math.random() * 2
      speed = 8
    }

    particlesRef.current.push({
      x,
      y,
      vx: (Math.random() - 0.5) * speed,
      vy: (Math.random() - 0.5) * speed - 2,
      life: 60,
      maxLife: 60,
      color,
      type: ballType,
      size,
    })
  }

  dlog(`[v0] Total particles in array: ${particlesRef.current.length}`)
}

/**
 * Creates laser-specific particles when a laser hits a tower
 */
export function createLaserParticles(particlesRef: React.MutableRefObject<Particle[]>, x: number, y: number): void {
  dlog(`[v0] Creating ${PARTICLE_COUNT_LASER} laser particles at (${x}, ${y})`)

  for (let i = 0; i < PARTICLE_COUNT_LASER; i++) {
    particlesRef.current.push({
      x,
      y,
      vx: (Math.random() - 0.5) * 10,
      vy: (Math.random() - 0.5) * 10 - 1,
      life: 50,
      maxLife: 50,
      color: LASER_COLORS[Math.floor(Math.random() * LASER_COLORS.length)],
      type: "laser",
      size: 2 + Math.random() * 3,
    })
  }
}

/**
 * Creates an electric burst effect when a laser is destroyed after hitting max towers
 */
export function createLaserDestructionParticles(
  particlesRef: React.MutableRefObject<Particle[]>,
  x: number,
  y: number,
): void {
  dlog(`[v0] Creating laser destruction particles at (${x}, ${y})`)

  for (let i = 0; i < PARTICLE_COUNT_LASER_DESTRUCTION; i++) {
    const laserColors = [...LASER_COLORS, "#FF00FF"]
    particlesRef.current.push({
      x,
      y,
      vx: (Math.random() - 0.5) * 15,
      vy: (Math.random() - 0.5) * 15,
      life: 40,
      maxLife: 40,
      color: laserColors[Math.floor(Math.random() * laserColors.length)],
      type: "laser",
      size: 3 + Math.random() * 4,
    })
  }
}

/**
 * Creates celebratory particles when the player wins
 */
export function createWinParticles(particlesRef: React.MutableRefObject<Particle[]>): void {
  dlog("[v0] Creating win particles!")

  for (let i = 0; i < PARTICLE_COUNT_WIN; i++) {
    particlesRef.current.push({
      x: CANVAS_WIDTH / 2,
      y: CANVAS_HEIGHT / 2,
      vx: (Math.random() - 0.5) * 20,
      vy: (Math.random() - 0.5) * 20 - 5,
      life: 120,
      maxLife: 120,
      color: WIN_COLORS[Math.floor(Math.random() * WIN_COLORS.length)],
      type: "win",
      size: 4 + Math.random() * 4,
    })
  }
}

/**
 * Creates sad falling particles when the player loses
 */
export function createLoseParticles(particlesRef: React.MutableRefObject<Particle[]>): void {
  dlog("[v0] Creating lose particles!")

  for (let i = 0; i < PARTICLE_COUNT_LOSE; i++) {
    particlesRef.current.push({
      x: CANVAS_WIDTH / 2,
      y: CANVAS_HEIGHT / 2,
      vx: (Math.random() - 0.5) * 10,
      vy: (Math.random() - 0.5) * 10 - 3,
      life: 100,
      maxLife: 100,
      color: LOSE_COLORS[Math.floor(Math.random() * LOSE_COLORS.length)],
      type: "lose",
      size: 3 + Math.random() * 3,
    })
  }
}
