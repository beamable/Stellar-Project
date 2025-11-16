"use client"

import type { MutableRefObject, Dispatch, SetStateAction } from "react"
import type { Ball, Laser, Particle, Tower } from "@/components/Game/types"
import * as CONST from "@/components/Game/constants"
import type { ConfirmedCollisionResult } from "@/components/Game/physics"
import { detectBallTowerCollision } from "@/components/Game/physics"
import { DEBUG_COLLISION_MODE } from "@/components/Game/debug"
import {
  createParticles,
  createLaserParticles,
  createLaserDestructionParticles,
  createWinParticles,
  createLoseParticles,
} from "@/components/Game/particles"
import * as Audio from "@/components/Game/audio"

type StepPhysicsOptions = {
  ballsRef: MutableRefObject<Ball[]>
  lasersRef: MutableRefObject<Laser[]>
  towersRef: MutableRefObject<Tower[]>
  particlesRef: MutableRefObject<Particle[]>
  collisionCooldownRef: MutableRefObject<Set<string>>
  audioContextRef: MutableRefObject<AudioContext | null>
  ballsLeft: number
  gameState: "playing" | "won" | "gameOver"
  setScore: Dispatch<SetStateAction<number>>
  setGameState: Dispatch<SetStateAction<"playing" | "won" | "gameOver">>
  resetBall: () => void
  setRemainingTowers: Dispatch<SetStateAction<number>>
  remainingTowersRef: MutableRefObject<number>
}

const COLLISION_EXIT_EPSILON = 0.5
const SWEEP_PENETRATION_EPSILON = 0.5
const MIN_NORMAL_BOUNCE_SPEED = 0.5
const toStereoPan = (x: number) => Math.max(-1, Math.min(1, (x / CONST.CANVAS_WIDTH) * 2 - 1))

function resolveBallTowerCollision(ball: Ball, collision: ConfirmedCollisionResult) {
  if (collision.wasSwept) {
    ball.x = collision.impactPoint.x - collision.normal.x * SWEEP_PENETRATION_EPSILON
    ball.y = collision.impactPoint.y - collision.normal.y * SWEEP_PENETRATION_EPSILON
  } else {
    const separation = Math.max(collision.penetration + COLLISION_EXIT_EPSILON, COLLISION_EXIT_EPSILON)
    ball.x += collision.normal.x * separation
    ball.y += collision.normal.y * separation
  }

  const normalVelocity = ball.vx * collision.normal.x + ball.vy * collision.normal.y
  const tangentVx = ball.vx - collision.normal.x * normalVelocity
  const tangentVy = ball.vy - collision.normal.y * normalVelocity

  let resolvedNormalVelocity = normalVelocity
  if (normalVelocity < 0) {
    const clamped = Math.min(normalVelocity, -MIN_NORMAL_BOUNCE_SPEED)
    resolvedNormalVelocity = -clamped * CONST.BOUNCE_DAMPING
  }

  const resolvedNormalVx = collision.normal.x * resolvedNormalVelocity
  const resolvedNormalVy = collision.normal.y * resolvedNormalVelocity
  const resolvedTangentVx = tangentVx * CONST.FRICTION
  const resolvedTangentVy = tangentVy * CONST.FRICTION

  ball.vx = resolvedNormalVx + resolvedTangentVx
  ball.vy = resolvedNormalVy + resolvedTangentVy

  const push = CONST.COLLISION_PUSH_FORCE * 0.3
  ball.vx += collision.normal.x * push
  ball.vy += collision.normal.y * push
}

export default function stepPhysics({
  ballsRef,
  lasersRef,
  towersRef,
  particlesRef,
  collisionCooldownRef,
  audioContextRef,
  ballsLeft,
  gameState,
  setScore,
  setGameState,
  resetBall,
  setRemainingTowers,
  remainingTowersRef,
}: StepPhysicsOptions) {
  ballsRef.current.forEach((ball) => {
    ball.lastX = ball.x
    ball.lastY = ball.y
    if (!ball.active) return

    ball.x += ball.vx
    ball.y += ball.vy
    ball.vy += CONST.GRAVITY

    if (ball.y + ball.radius > CONST.GROUND_Y) {
      ball.y = CONST.GROUND_Y - ball.radius
      const impactVelocity = Math.abs(ball.vy)
      ball.vy *= -CONST.BOUNCE_DAMPING
      ball.vx *= CONST.FRICTION

      if (impactVelocity > CONST.BOUNCE_VELOCITY_THRESHOLD) {
        const pan = toStereoPan(ball.x)
        const intensity = Math.min(impactVelocity / 20, 1)
        Audio.playGroundBounceSound(audioContextRef, { pan, intensity })
      }

      if (Math.abs(ball.vy) < CONST.BOUNCE_VELOCITY_THRESHOLD && Math.abs(ball.vx) < 1) {
        ball.vy = 0
        ball.vx *= 0.8
      }
    }

    if (ball.x - ball.radius < 0 || ball.x + ball.radius > CONST.CANVAS_WIDTH) {
      ball.vx *= -CONST.BOUNCE_DAMPING
      ball.x = ball.x - ball.radius < 0 ? ball.radius : CONST.CANVAS_WIDTH - ball.radius
    }

    if (
      ball.type === "laser" &&
      ball.y < CONST.LASER_CREATION_HEIGHT &&
      ball.shotTime &&
      Date.now() - ball.shotTime >= CONST.LASER_CREATION_DELAY_MS &&
      lasersRef.current.filter((l) => l.ballId === ball.id).length === 0
    ) {
      Audio.playLaserShootSound(audioContextRef)

      const activeTowers = towersRef.current.filter((t) => !t.destroyed)
      const midIndex = Math.floor(activeTowers.length / 2)
      const targetableTowers = activeTowers.slice(midIndex)

      for (let i = 0; i < CONST.LASER_COUNT; i++) {
        if (targetableTowers.length > 0) {
          const randomTower = targetableTowers[Math.floor(Math.random() * targetableTowers.length)]
          const targetX = randomTower.x + randomTower.width / 2
          const targetY = randomTower.y + randomTower.height / 2

          const dx = targetX - ball.x
          const dy = targetY - ball.y
          const angle = Math.atan2(dy, dx)

          const randomLength =
            CONST.LASER_MIN_LENGTH + Math.random() * (CONST.LASER_MAX_LENGTH - CONST.LASER_MIN_LENGTH)

          lasersRef.current.push({
            x: ball.x,
            y: ball.y,
            angle: angle,
            length: randomLength,
            active: true,
            hitCount: 0,
            maxHits: CONST.LASER_MAX_HITS,
            ballId: ball.id,
          })
        }
      }
    }

    towersRef.current.forEach((tower, towerIndex) => {
      const collisionKey = `${ball.id}-${towerIndex}`
      const towerCenterX = tower.x + tower.width / 2
      const towerCenterY = tower.y + tower.height / 2
      const towerPan = toStereoPan(towerCenterX)

      if (tower.destroyed) {
        collisionCooldownRef.current.delete(collisionKey)
        return
      }

      const collision = detectBallTowerCollision(ball, tower)
      if (!collision.collided) {
        collisionCooldownRef.current.delete(collisionKey)
        return
      }

      if (DEBUG_COLLISION_MODE) {
        console.debug(
          `[TowerDebug] Ball ${ball.id} hit tower#${towerIndex} (special=${tower.isSpecial}) ` +
            `normal=(${collision.normal.x.toFixed(2)}, ${collision.normal.y.toFixed(2)}) ` +
            `swept=${collision.wasSwept}`,
        )
      }

      const fireDestroyCount = ball.fireDestroyCount ?? 0
      const hasPassThroughCharge =
        ball.type === "fire" && fireDestroyCount < CONST.FIRE_BALL_DESTROY_THRESHOLD

      if (!hasPassThroughCharge && collisionCooldownRef.current.has(collisionKey)) {
        return
      }

      if (hasPassThroughCharge) {
        ball.fireDestroyCount = fireDestroyCount + 1
        tower.destroyed = true
        Audio.playTowerBreakSound(audioContextRef, towerPan)
        createParticles(particlesRef, tower.x + tower.width / 2, tower.y + tower.height / 2, "fire", tower.color)
        const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
        setScore((prev) => prev + points)
        collisionCooldownRef.current.delete(collisionKey)
        return
      }

      collisionCooldownRef.current.add(collisionKey)

      if (ball.type === "fire") {
        ball.type = "normal"
        delete (ball as any).fireDestroyCount
      }

      resolveBallTowerCollision(ball, collision)

      tower.hits++

      if (tower.isSpecial) {
        const firstHitPoints = tower.hits === 1 ? CONST.POINTS_SPECIAL_TOWER_FIRST_HIT : 0
        if (firstHitPoints > 0) {
          setScore((prev) => prev + firstHitPoints)
        }
        if (tower.hits === 1) {
          createParticles(
            particlesRef,
            tower.x + tower.width / 2,
            tower.y + tower.height / 2,
            "multishot",
            tower.color,
          )
        }
      }

      if (tower.hits >= tower.maxHits) {
        tower.destroyed = true
        Audio.playTowerBreakSound(audioContextRef, towerPan)
        createParticles(
          particlesRef,
          tower.x + tower.width / 2,
          tower.y + tower.height / 2,
          tower.isSpecial ? "multishot" : "normal",
          tower.color,
        )
        const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
        setScore((prev) => prev + points)
      } else {
        const dx = ball.x - towerCenterX
        const dy = ball.y - towerCenterY
        const distance = Math.sqrt(dx * dx + dy * dy)

        if (distance > 0) {
          ball.vx += (dx / distance) * CONST.COLLISION_PUSH_FORCE
          ball.vy += (dy / distance) * CONST.COLLISION_PUSH_FORCE
        }
      }
    })
  })

  lasersRef.current.forEach((laser) => {
    if (!laser.active) return

    const dx = Math.cos(laser.angle) * CONST.LASER_SPEED
    const dy = Math.sin(laser.angle) * CONST.LASER_SPEED
    laser.x += dx
    laser.y += dy

    const probeX = laser.x + Math.cos(laser.angle) * laser.length * CONST.LASER_COLLISION_OFFSET
    const probeY = laser.y + Math.sin(laser.angle) * laser.length * CONST.LASER_COLLISION_OFFSET

    towersRef.current.forEach((tower) => {
      if (tower.destroyed || laser.hitCount >= laser.maxHits) return

      if (
        probeX >= tower.x &&
        probeX <= tower.x + tower.width &&
        probeY >= tower.y &&
        probeY <= tower.y + tower.height
      ) {
        tower.destroyed = true
        const laserTowerCenterX = tower.x + tower.width / 2
        Audio.playTowerBreakSound(audioContextRef, toStereoPan(laserTowerCenterX))
        createLaserParticles(particlesRef, laserTowerCenterX, tower.y + tower.height / 2)
        const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
        setScore((prev) => prev + points)
        laser.hitCount++

        if (laser.hitCount >= laser.maxHits) {
          createLaserDestructionParticles(particlesRef, probeX, probeY)
          laser.active = false
        }
      }
    })

    if (
      probeX < 0 ||
      probeX > CONST.CANVAS_WIDTH ||
      probeY < 0 ||
      probeY > CONST.CANVAS_HEIGHT
    ) {
      laser.active = false
    }
  })

  lasersRef.current = lasersRef.current.filter((laser) => laser.active)

  const activeBalls = ballsRef.current.filter((ball) => ball.active)
  const allBallsSettled = activeBalls.every((ball) => {
    const isOnGround = ball.y + ball.radius >= CONST.GROUND_Y - 1
    const isMovingSlowly =
      Math.abs(ball.vx) < CONST.VELOCITY_THRESHOLD && Math.abs(ball.vy) < CONST.VELOCITY_THRESHOLD
    return isOnGround && isMovingSlowly
  })

  const allBallsOffScreen = activeBalls.every(
    (ball) => ball.x < -50 || ball.x > CONST.CANVAS_WIDTH + 50 || ball.y > CONST.CANVAS_HEIGHT + 50,
  )

  if ((allBallsSettled || allBallsOffScreen) && activeBalls.length > 0) {
    collisionCooldownRef.current.clear()
    resetBall()
  }

  particlesRef.current = particlesRef.current.filter((particle) => {
    particle.x += particle.vx
    particle.y += particle.vy
    particle.vy += CONST.PARTICLE_GRAVITY
    particle.life--
    return particle.life > 0
  })

  const remaining = towersRef.current.reduce((count, tower) => (tower.destroyed ? count : count + 1), 0)
  if (remaining !== remainingTowersRef.current) {
    remainingTowersRef.current = remaining
    setRemainingTowers(remaining)
  }

  if (remaining === 0 && gameState === "playing") {
    const ballMultiplier = ballsLeft > 0 ? 1 + ballsLeft * CONST.VICTORY_BONUS_MULTIPLIER : 1
    setScore((prev) => {
      if (ballMultiplier <= 1) {
        return prev
      }
      const bonusPoints = Math.floor(prev * (ballMultiplier - 1))
      return bonusPoints > 0 ? prev + bonusPoints : prev
    })
    createWinParticles(particlesRef)
    Audio.playWinSound(audioContextRef)
    setGameState("won")
  } else if (
    ballsLeft === 0 &&
    ballsRef.current.every((ball) => !ball.active) &&
    remaining > 0 &&
    gameState === "playing"
  ) {
    createLoseParticles(particlesRef)
    Audio.playLoseSound(audioContextRef)
    setGameState("gameOver")
  }
}
