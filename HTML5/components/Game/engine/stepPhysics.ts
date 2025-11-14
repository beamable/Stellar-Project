"use client"

import type { MutableRefObject, Dispatch, SetStateAction } from "react"
import type { Ball, Laser, Particle, Tower } from "@/components/Game/types"
import * as CONST from "@/components/Game/constants"
import { checkCollision } from "@/components/Game/physics"
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
    if (!ball.active) return

    ball.x += ball.vx
    ball.y += ball.vy
    ball.vy += CONST.GRAVITY

    if (ball.y + ball.radius > CONST.GROUND_Y) {
      ball.y = CONST.GROUND_Y - ball.radius
      ball.vy *= -CONST.BOUNCE_DAMPING
      ball.vx *= CONST.FRICTION

      if (Math.abs(ball.vy) > CONST.BOUNCE_VELOCITY_THRESHOLD) {
        Audio.playGroundBounceSound(audioContextRef)
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
      if (tower.destroyed || !checkCollision(ball, tower)) return

      const collisionKey = `${ball.id}-${towerIndex}`
      if (collisionCooldownRef.current.has(collisionKey)) return

      collisionCooldownRef.current.add(collisionKey)

      if (ball.type === "fire") {
        if (ball.fireDestroyCount !== undefined && ball.fireDestroyCount >= CONST.FIRE_BALL_DESTROY_THRESHOLD) {
          ball.type = "normal"
          delete (ball as any).fireDestroyCount
          const centerX = tower.x + tower.width / 2
          const centerY = tower.y + tower.height / 2
          const dx = ball.x - centerX
          const dy = ball.y - centerY
          const distance = Math.sqrt(dx * dx + dy * dy)

          if (distance > 0) {
            ball.vx += (dx / distance) * CONST.COLLISION_PUSH_FORCE
            ball.vy += (dy / distance) * CONST.COLLISION_PUSH_FORCE
          }

          tower.hits++

          if (tower.hits >= tower.maxHits) {
            tower.destroyed = true
            Audio.playTowerBreakSound(audioContextRef)
            createParticles(particlesRef, tower.x + tower.width / 2, tower.y + tower.height / 2, "normal", tower.color)
            const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
            setScore((prev) => prev + points)
          }
          return
        }

        ball.fireDestroyCount = (ball.fireDestroyCount ?? 0) + 1
        tower.destroyed = true
        Audio.playTowerBreakSound(audioContextRef)
        createParticles(particlesRef, tower.x + tower.width / 2, tower.y + tower.height / 2, "fire", tower.color)
        const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
        setScore((prev) => prev + points)
        return
      }

      tower.hits++

      if (tower.isSpecial) {
        const firstHitPoints = tower.hits === 1 ? CONST.POINTS_SPECIAL_TOWER_FIRST_HIT : 0
        if (firstHitPoints > 0) {
          setScore((prev) => prev + firstHitPoints)
        }
      }

      if (tower.hits >= tower.maxHits) {
        tower.destroyed = true
        Audio.playTowerBreakSound(audioContextRef)
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
        const centerX = tower.x + tower.width / 2
        const centerY = tower.y + tower.height / 2
        const dx = ball.x - centerX
        const dy = ball.y - centerY
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

    laser.x += Math.cos(laser.angle) * CONST.LASER_SPEED
    laser.y += Math.sin(laser.angle) * CONST.LASER_SPEED

    towersRef.current.forEach((tower) => {
      if (tower.destroyed || laser.hitCount >= laser.maxHits) return

      if (
        laser.x >= tower.x &&
        laser.x <= tower.x + tower.width &&
        laser.y >= tower.y &&
        laser.y <= tower.y + tower.height
      ) {
        tower.destroyed = true
        Audio.playTowerBreakSound(audioContextRef)
        createLaserParticles(particlesRef, tower.x + tower.width / 2, tower.y + tower.height / 2)
        const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
        setScore((prev) => prev + points)
        laser.hitCount++

        if (laser.hitCount >= laser.maxHits) {
          createLaserDestructionParticles(particlesRef, laser.x, laser.y)
          laser.active = false
        }
      }
    })

    if (laser.x < 0 || laser.x > CONST.CANVAS_WIDTH || laser.y < 0 || laser.y > CONST.CANVAS_HEIGHT) {
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
