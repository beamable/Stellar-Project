"use client"

import type React from "react"
import { useState, useEffect, useRef, useCallback } from "react"
import { Button } from "@/components/ui/button"
import getBeam from "@/lib/beam"
import { Card } from "@/components/ui/card"

// Import types
import type { Ball, Laser, Tower, Particle, BallType } from "./types"

// Import constants
import * as CONST from "./constants"

// Import utilities
import { generateTowers } from "./towers"
import { checkCollision } from "./physics"
import { BALL_TYPES } from "./ballTypes"

// Import particle functions
import {
  createParticles,
  createLaserParticles,
  createLaserDestructionParticles,
  createWinParticles,
  createLoseParticles,
} from "./particles"

// Import audio functions
import * as Audio from "./audio"

export default function TowerDestroyer() {
  const DEBUG = false
  const dlog = (...args: any[]) => {
    if (DEBUG) console.log(...args)
  }
  // ============================================================================
  // REFS
  // ============================================================================

  const canvasRef = useRef<HTMLCanvasElement>(null)
  const animationRef = useRef<number>()
  const audioContextRef = useRef<AudioContext | null>(null)
  const chargingOscillatorRef = useRef<OscillatorNode | null>(null)
  const chargingGainRef = useRef<GainNode | null>(null)

  // Game state refs
  const ballsRef = useRef<Ball[]>([])
  const lasersRef = useRef<Laser[]>([])
  const ballIdCounterRef = useRef(0)
  const towersRef = useRef<Tower[]>([])
  const particlesRef = useRef<Particle[]>([])
  const collisionCooldownRef = useRef<Set<string>>(new Set())
  const stellarLoggedOnceRef = useRef<boolean>(false)

  // ============================================================================
  // STATE
  // ============================================================================

  const [gameState, setGameState] = useState<"playing" | "won" | "gameOver">("playing")
  const [score, setScore] = useState(0)
  const [ballsLeft, setBallsLeft] = useState(CONST.BALLS_FOR_LOW_TOWER_COUNT)
  const [power, setPower] = useState(0)
  const [isCharging, setIsCharging] = useState(false)
  const [mousePos, setMousePos] = useState({ x: 0, y: 0 })
  const [towerCount, setTowerCount] = useState(0)
  const [hasShot, setHasShot] = useState(false)
  const [selectedBallType, setSelectedBallType] = useState<BallType>("normal")
  const [playerId, setPlayerId] = useState<string | null>(null)
  const [beamReady, setBeamReady] = useState(false)
  const [alias, setAlias] = useState<string | null>(null)
  const [aliasInput, setAliasInput] = useState('')
  const [aliasModalOpen, setAliasModalOpen] = useState(false)
  const [aliasSaving, setAliasSaving] = useState(false)
  const [aliasError, setAliasError] = useState<string | null>(null)
  const readyForGame = beamReady && !!(alias && alias.length > 0)
  const [showResetConfirm, setShowResetConfirm] = useState(false)
  const [showPlayerInfo, setShowPlayerInfo] = useState(false)
  const [stellarExternalId, setStellarExternalId] = useState<string | null>(null)

  // ============================================================================
  // INITIALIZATION
  // ============================================================================

  const initializeTowers = useCallback(() => {
    const { towers, towerCount: count } = generateTowers()
    towersRef.current = towers
    setTowerCount(count)
    const initialBallCount =
      count > CONST.TOWER_THRESHOLD_FOR_HIGH_SPECIAL
        ? CONST.BALLS_FOR_HIGH_TOWER_COUNT
        : CONST.BALLS_FOR_LOW_TOWER_COUNT
    setBallsLeft(initialBallCount)
  }, [])

  useEffect(() => {
    initializeTowers()
  }, [initializeTowers])

  // Initialize Beamable and capture the player id for logging/UI
  useEffect(() => {
    let mounted = true
    getBeam()
      .then((beam: any) => {
        const id = beam?.player?.id ?? null
        if (mounted) {
          setPlayerId(id)
          setBeamReady(true)
          console.log("[Beam] Initialized. Player ID:", id)
        }
      })
      .catch((err: unknown) => {
        console.error("[Beam] Initialization failed:", err?.message || err)
      })
    return () => {
      mounted = false
    }
  }, [])

  // ============================================================================
  // After Beam is ready, fetch Alias stat and decide whether to prompt
  // After Beam is ready, fetch Alias stat and decide whether to prompt
  useEffect(() => {
    if (!beamReady) return
    let mounted = true
    ;(async () => {
      try {
        const beam: any = await getBeam()
        let a = ''
        try {
          const statsPrivate = await beam.stats.get({ domainType: 'client', accessType: 'private', stats: ['Alias'] })
          a = (statsPrivate && (statsPrivate as any).Alias) || ''
        } catch {}
        if (!a) {
          try {
            const statsPublic = await beam.stats.get({ domainType: 'client', accessType: 'public', stats: ['Alias'] })
            a = (statsPublic && (statsPublic as any).Alias) || ''
          } catch {}
        }
        if (!mounted) return
        if (a && a.length > 0) {
          setAlias(a)
          setAliasModalOpen(false)
          setShowPlayerInfo(true)
        } else {
          setAlias(null)
          setAliasModalOpen(true)
        }
      } catch (e) {
        if (!mounted) return
        setAlias(null)
        setAliasModalOpen(true)
      }
    })()
    return () => { mounted = false }
  }, [beamReady])

  // Log Stellar ID for returning players (filter by provider)
  useEffect(() => {
    if (!beamReady) return
    if (!alias || alias.length === 0) return
    if (stellarLoggedOnceRef.current) return
    ;(async () => {
      try {
        const beam: any = await getBeam()
        const acct = await beam.account.current()
        const providerService: string = beam?.stellarFederationClient?.serviceName || "StellarFederation"
        const providerNamespace: string = beam?.stellarFederationClient?.federationIds?.StellarIdentity || "StellarIdentity"
        const ext = (acct?.external || []).find((e: any) => e.providerService === providerService && e.providerNamespace === providerNamespace)
        const stellarId = ext?.userId
        if (stellarId) {
          console.log("[Stellar] Returning player Stellar ID:", stellarId)
          setStellarExternalId(stellarId)
          stellarLoggedOnceRef.current = true
        }
      } catch {}
    })()
  }, [beamReady, alias])

  // BALL MANAGEMENT
  // ============================================================================

  const resetBall = useCallback(() => {
    dlog("[v0] Resetting ball for next shot")
    const newBall = {
      x: 100,
      y: 500,
      vx: 0,
      vy: 0,
      radius: 12,
      active: false,
      type: selectedBallType,
      id: ballIdCounterRef.current++,
    }
    ballsRef.current = [newBall]
    lasersRef.current = []
    dlog(`[v0] Ball reset complete - ball exists: ${ballsRef.current.length > 0}, type: ${newBall.type}`)
  }, [selectedBallType])

  const shootBall = useCallback(
    (targetX: number, targetY: number, power: number) => {
      dlog(`[v0] Shooting ball - type: ${selectedBallType}, power: ${power}`)

      if (ballsRef.current.length === 0) {
        dlog("[v0] No balls in array!")
        return
      }

      const currentBall = ballsRef.current[0]
      if (!currentBall) {
        dlog("[v0] No ball found to shoot!")
        return
      }

      const dx = targetX - currentBall.x
      const dy = targetY - currentBall.y
      const distance = Math.sqrt(dx * dx + dy * dy)

      if (distance < 1e-6) {
        dlog("[v0] Shot ignored due to minimal distance")
        return
      }

      Audio.playShootSound(audioContextRef, selectedBallType)

      const force = Math.min(power / CONST.MAX_POWER, 1) * CONST.SHOT_FORCE_MULTIPLIER
      const baseVx = (dx / distance) * force
      const baseVy = (dy / distance) * force

      if (selectedBallType === "multishot") {
        dlog("[v0] Creating multishot balls")
        ballsRef.current = []

        CONST.MULTISHOT_ANGLES.forEach((angleOffset) => {
          const cos = Math.cos(angleOffset)
          const sin = Math.sin(angleOffset)
          const rotatedVx = baseVx * cos - baseVy * sin
          const rotatedVy = baseVx * sin + baseVy * cos

          ballsRef.current.push({
            x: currentBall.x,
            y: currentBall.y,
            vx: rotatedVx,
            vy: rotatedVy,
            radius: CONST.MULTISHOT_BALL_RADIUS,
            active: true,
            type: "multishot",
            id: ballIdCounterRef.current++,
          })
        })
      } else if (selectedBallType === "laser") {
        dlog("[v0] Creating laser ball")
        currentBall.vx = baseVx
        currentBall.vy = baseVy
        currentBall.active = true
        currentBall.type = "laser"
        currentBall.shotTime = Date.now()
      } else {
        dlog(`[v0] Creating ${selectedBallType} ball`)
        currentBall.vx = baseVx
        currentBall.vy = baseVy
        currentBall.active = true
        currentBall.type = selectedBallType
        if (selectedBallType === "fire") {
          currentBall.fireDestroyCount = 0
        }
      }

      setBallsLeft((prev) => prev - 1)
      setHasShot(true)
      dlog(`[v0] Ball shot! Active balls: ${ballsRef.current.filter((b) => b.active).length}`)
    },
    [selectedBallType],
  )

  // ============================================================================
  // GAME LOOP
  // ============================================================================

  const gameLoop = useCallback(() => {
    const canvas = canvasRef.current
    if (!canvas) return

    const ctxMaybe = canvas.getContext("2d")
    if (!ctxMaybe) { animationRef.current = requestAnimationFrame(gameLoop); return }
    const ctx = ctxMaybe
    // Draw background gradient
    const gradient = ctx.createLinearGradient(0, 0, 0, CONST.CANVAS_HEIGHT)
    gradient.addColorStop(0, "#87CEEB")
    gradient.addColorStop(1, "#F0E68C")
    ctx.fillStyle = gradient
    ctx.fillRect(0, 0, CONST.CANVAS_WIDTH, CONST.CANVAS_HEIGHT)

    // Draw ground
    ctx.fillStyle = "#8B4513"
    ctx.fillRect(0, CONST.GROUND_Y, CONST.CANVAS_WIDTH, CONST.CANVAS_HEIGHT - CONST.GROUND_Y)

    // Draw grass
    ctx.fillStyle = "#228B22"
    for (let i = 0; i < CONST.CANVAS_WIDTH; i += 5) {
      const height = 3 + Math.sin(i * 0.1) * 2
      ctx.fillRect(i, CONST.GROUND_Y - height, 3, height)
    }

    // Ball physics and collision
    ballsRef.current.forEach((ball) => {
      if (!ball.active) return

      ball.x += ball.vx
      ball.y += ball.vy
      ball.vy += CONST.GRAVITY

      // Ground collision
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

      // Wall collision
      if (ball.x - ball.radius < 0 || ball.x + ball.radius > CONST.CANVAS_WIDTH) {
        ball.vx *= -CONST.BOUNCE_DAMPING
        ball.x = ball.x - ball.radius < 0 ? ball.radius : CONST.CANVAS_WIDTH - ball.radius
      }

      // Laser creation for laser balls
      if (
        ball.type === "laser" &&
        ball.y < CONST.LASER_CREATION_HEIGHT &&
        ball.shotTime &&
        Date.now() - ball.shotTime >= CONST.LASER_CREATION_DELAY_MS &&
        lasersRef.current.filter((l) => l.ballId === ball.id).length === 0
      ) {
        dlog("[v0] Creating lasers for laser ball")
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

      // Tower collision detection
      towersRef.current.forEach((tower, towerIndex) => {
        if (tower.destroyed || !checkCollision(ball, tower)) return

        const collisionKey = `${ball.id}-${towerIndex}`
        if (collisionCooldownRef.current.has(collisionKey)) return

        collisionCooldownRef.current.add(collisionKey)

        // Fire ball logic
        if (ball.type === "fire") {
          if (ball.fireDestroyCount !== undefined && ball.fireDestroyCount >= CONST.FIRE_BALL_DESTROY_THRESHOLD) {
            // convert fire ball to normal after threshold
            ball.type = "normal"
            delete (ball as any).fireDestroyCount
            // Fire ball converted to normal - bounce off tower
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
              createParticles(
                particlesRef,
                tower.x + tower.width / 2,
                tower.y + tower.height / 2,
                "normal",
                tower.color,
              )
              const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
              setScore((prev) => prev + points)
            }
            return
          } else {
            // Fire ball destroys tower instantly
            tower.destroyed = true
            Audio.playTowerBreakSound(audioContextRef)
            createParticles(particlesRef, tower.x + tower.width / 2, tower.y + tower.height / 2, "fire", tower.color)
            const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
            setScore((prev) => prev + points)
            ball.fireDestroyCount = (ball.fireDestroyCount || 0) + 1
            return
          }
        }

        // Normal collision logic
        tower.hits++

        // Special block bounce on first hit
        if (tower.isSpecial && tower.hits === 1) {
          const towerCenterX = tower.x + tower.width / 2
          const towerCenterY = tower.y + tower.height / 2
          const relativeX = ball.x - towerCenterX
          const relativeY = ball.y - towerCenterY

          let normalX = 0
          let normalY = 0

          if (Math.abs(relativeX) > Math.abs(relativeY)) {
            normalX = relativeX > 0 ? 1 : -1
          } else {
            normalY = relativeY > 0 ? 1 : -1
          }

          const dotProduct = ball.vx * normalX + ball.vy * normalY
          const reflectedVx = ball.vx - 2 * dotProduct * normalX
          const reflectedVy = ball.vy - 2 * dotProduct * normalY

          ball.vx = reflectedVx * CONST.SPECIAL_BLOCK_BOUNCE_STRENGTH
          ball.vy = reflectedVy * CONST.SPECIAL_BLOCK_BOUNCE_STRENGTH

          ball.vx += (Math.random() - 0.5) * 2
          ball.vy += (Math.random() - 0.5) * 2
        }

        // Check if tower is destroyed
        if (tower.hits >= tower.maxHits) {
          tower.destroyed = true
          Audio.playTowerBreakSound(audioContextRef)
          createParticles(particlesRef, tower.x + tower.width / 2, tower.y + tower.height / 2, ball.type, tower.color)
          const points = tower.isSpecial ? CONST.POINTS_SPECIAL_TOWER : CONST.POINTS_NORMAL_TOWER
          setScore((prev) => prev + points)
        } else if (tower.isSpecial) {
          // Special block first hit
          tower.color = "#8B4513"
          createParticles(particlesRef, tower.x + tower.width / 2, tower.y + tower.height / 2, ball.type, tower.color)
          setScore((prev) => prev + CONST.POINTS_SPECIAL_TOWER_FIRST_HIT)
        }

        // Push ball away from tower (except special blocks on first hit)
        if (!tower.isSpecial || tower.hits === 2) {
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

    // Laser physics and collision
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

    // Ball reset logic
    const activeBalls = ballsRef.current.filter((ball) => ball.active)
    const allBallsSettled = activeBalls.every((ball) => {
      const isOnGround = ball.y + ball.radius >= CONST.GROUND_Y - 1
      const isMovingSlowly =
        Math.abs(ball.vx) < CONST.VELOCITY_THRESHOLD && Math.abs(ball.vy) < CONST.VELOCITY_THRESHOLD
      return isOnGround && isMovingSlowly
    })

    const allBallsOffScreen = activeBalls.every(
      (ball) => ball.x < -50 || ball.x > CONST.CANVAS_WIDTH + 50 || ball.y > CONST.CANVAS_HEIGHT + 50,
  );

    if ((allBallsSettled || allBallsOffScreen) && activeBalls.length > 0) {
      dlog("[v0] All balls settled, resetting for next shot")
      collisionCooldownRef.current.clear()
      resetBall()
    }

    // Particle update and rendering
    particlesRef.current = particlesRef.current.filter((particle) => {
      particle.x += particle.vx
      particle.y += particle.vy
      particle.vy += CONST.PARTICLE_GRAVITY
      particle.life--
      return particle.life > 0
    })

    particlesRef.current.forEach((particle) => {
      const alpha = particle.life / particle.maxLife
      const r = Number.parseInt(particle.color.slice(1, 3), 16)
      const g = Number.parseInt(particle.color.slice(3, 5), 16)
      const b = Number.parseInt(particle.color.slice(5, 7), 16)

      ctx.fillStyle = `rgba(${r}, ${g}, ${b}, ${alpha})`
      ctx.beginPath()
      ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2)
      ctx.fill()

      if (particle.type === "fire" || particle.type === "laser" || particle.type === "win") {
        ctx.fillStyle = `rgba(${r}, ${g}, ${b}, ${alpha * 0.3})`
        ctx.beginPath()
        ctx.arc(particle.x, particle.y, particle.size * 1.5, 0, Math.PI * 2)
        ctx.fill()
      }
    })

    // Tower rendering
    towersRef.current.forEach((tower) => {
      if (tower.destroyed) return

      ctx.fillStyle = tower.color
      ctx.fillRect(tower.x, tower.y, tower.width, tower.height)

      ctx.fillStyle = "rgba(0,0,0,0.2)"
      ctx.fillRect(tower.x + 2, tower.y + 2, tower.width, tower.height)

      ctx.fillStyle = "rgba(255,255,255,0.3)"
      ctx.fillRect(tower.x, tower.y, tower.width, 3)

      if (tower.isSpecial && tower.hits === 0) {
        ctx.fillStyle = "rgba(255, 215, 0, 0.8)"
        ctx.beginPath()
        const centerX = tower.x + tower.width / 2
        const centerY = tower.y + 10
        const size = 4
        ctx.moveTo(centerX, centerY - size)
        ctx.lineTo(centerX + size, centerY + size)
        ctx.lineTo(centerX - size, centerY + size)
        ctx.closePath()
        ctx.fill()
      }
    })

    // Ball rendering
    ballsRef.current.forEach((ball) => {
      let ballColor = "#8B4513"

      if (ball.type === "fire" && ball.active) {
        ballColor = "#FF4500"

        ctx.fillStyle = "rgba(255, 69, 0, 0.6)"
        ctx.beginPath()
        ctx.arc(ball.x, ball.y, ball.radius + 5, 0, Math.PI * 2)
        ctx.fill()

        ctx.fillStyle = "rgba(255, 140, 0, 0.4)"
        ctx.beginPath()
        ctx.arc(ball.x, ball.y, ball.radius + 8, 0, Math.PI * 2)
        ctx.fill()
      } else if (ball.type === "laser") {
        ballColor = "#8A2BE2"
      } else if (ball.type === "multishot") {
        ballColor = "#FF6B35"
      }

      const r = Number.parseInt(ballColor.slice(1, 3), 16)
      const g = Number.parseInt(ballColor.slice(3, 5), 16)
      const b = Number.parseInt(ballColor.slice(5, 7), 16)
      ctx.fillStyle = `rgba(${r}, ${g}, ${b}, 1)`
      ctx.beginPath()
      ctx.arc(ball.x, ball.y, ball.radius, 0, Math.PI * 2)
      ctx.fill()

      ctx.fillStyle = "rgba(255,255,255,0.4)"
      ctx.beginPath()
      ctx.arc(ball.x - 3, ball.y - 3, ball.radius * 0.6, 0, Math.PI * 2)
      ctx.fill()
    })

    // Laser rendering
    lasersRef.current.forEach((laser) => {
      if (!laser.active) return

      ctx.strokeStyle = "rgba(138, 43, 226, 0.8)"
      ctx.lineWidth = 3
      ctx.beginPath()
      ctx.moveTo(laser.x, laser.y)
      ctx.lineTo(laser.x + Math.cos(laser.angle) * laser.length, laser.y + Math.sin(laser.angle) * laser.length)
      ctx.stroke()

      ctx.strokeStyle = "rgba(138, 43, 226, 0.3)"
      ctx.lineWidth = 6
      ctx.stroke()
    })

    // Increase power while charging
    if (isCharging) {
      setPower((prev) => Math.min(prev + CONST.POWER_INCREMENT, CONST.MAX_POWER))
    }

    // Aim line rendering
    if (isCharging && ballsRef.current.length > 0 && !ballsRef.current[0].active) {
      const ball = ballsRef.current[0]
      const dx = mousePos.x - ball.x
      const dy = mousePos.y - ball.y
      const distance = Math.sqrt(dx * dx + dy * dy)
      if (distance < 1e-6) {
        // avoid NaN when drawing aim line
        animationRef.current = requestAnimationFrame(gameLoop)
        return
      }
      const maxDistance = 150
      const lineLength = Math.min(distance, maxDistance)

      ctx.strokeStyle = `rgba(139, 69, 19, ${0.3 + (power / CONST.MAX_POWER) * 0.4})`
      ctx.lineWidth = 2 + (power / CONST.MAX_POWER) * 3
      ctx.setLineDash([5, 5])
      ctx.beginPath()
      ctx.moveTo(ball.x, ball.y)
      ctx.lineTo(ball.x + (dx / distance) * lineLength, ball.y + (dy / distance) * lineLength)
      ctx.stroke()
      ctx.setLineDash([])
    }

    // Game state checks
    const remainingTowers = towersRef.current.filter((tower) => !tower.destroyed).length

    if (remainingTowers === 0 && gameState === "playing") {
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
      remainingTowers > 0 &&
      gameState === "playing"
    ) {
      createLoseParticles(particlesRef)
      Audio.playLoseSound(audioContextRef)
      setGameState("gameOver")
    }

    animationRef.current = requestAnimationFrame(gameLoop)
  }, [ballsLeft, gameState, isCharging, mousePos, power, resetBall])

  useEffect(() => {
    animationRef.current = requestAnimationFrame(gameLoop)
    return () => {
      if (animationRef.current) {
        cancelAnimationFrame(animationRef.current)
      }
    }
  }, [gameLoop])

  // ============================================================================
  // EVENT HANDLERS
  // ============================================================================
 
  const handlePointerDown = (e: React.PointerEvent<HTMLCanvasElement>) => {
    if (!readyForGame) return
    if (
      (ballsRef.current.length > 0 && ballsRef.current.some((ball) => ball.active)) ||
      gameState !== "playing" ||
      ballsLeft === 0
    ) {
      return
    }
 
    setIsCharging(true)
    setPower(0)
    Audio.playChargingSound(audioContextRef, chargingOscillatorRef, chargingGainRef)
 
    const rect = canvasRef.current?.getBoundingClientRect()
    if (rect) {
      setMousePos({
        x: e.clientX - rect.left,
        y: e.clientY - rect.top,
      })
    }
  }
 
  const handlePointerMove = (e: React.PointerEvent<HTMLCanvasElement>) => {
    if (!readyForGame) return
    const rect = canvasRef.current?.getBoundingClientRect()
    if (rect) {
      setMousePos({
        x: e.clientX - rect.left,
        y: e.clientY - rect.top,
      })
    }
 
    // Power increases in the game loop while charging
  }
 
  const handlePointerUp = (e: React.PointerEvent<HTMLCanvasElement>) => {
    if (!readyForGame) return
    Audio.stopChargingSound(chargingOscillatorRef, chargingGainRef)
 
    if (!isCharging || (ballsRef.current.length > 0 && ballsRef.current.some((ball) => ball.active))) {
      return
    }
 
    const rect = canvasRef.current?.getBoundingClientRect()
    if (rect) {
      const targetX = e.clientX - rect.left
      const targetY = e.clientY - rect.top
      shootBall(targetX, targetY, power)
    }
 
    setIsCharging(false)
    setPower(0)
  }
  const resetGame = () => {
    dlog("[v0] Resetting game")
    Audio.playRestartSound(audioContextRef)

    setGameState("playing")
    setScore(0)
    setPower(0)
    setIsCharging(false)
    setHasShot(false)
    setSelectedBallType("normal")
    resetBall()
    particlesRef.current = []
    collisionCooldownRef.current.clear()

    initializeTowers()
  }

  const selectedBallInfo = BALL_TYPES.find((ball) => ball.type === selectedBallType)
  async function handleResetPlayer() {
    setShowResetConfirm(true)
  }

  async function confirmResetPlayer() {
    try {
      const beam: any = await getBeam().catch(() => null)
      await beam?.tokenStorage?.clear?.()
      await beam?.tokenStorage?.dispose?.()
    } catch {}
    try { window.sessionStorage?.removeItem('BEAM_TAB_INSTANCE_TAG') } catch {}
    try {
      const url = new URL(window.location.href)
      url.searchParams.set('beam_new', '1')
      window.location.href = url.toString()
      return
    } catch {}
    window.location.reload()
  }


  // ============================================================================
  // UI RENDERING

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4 bg-background">
      <Card className="p-6 bg-card border-2 border-primary/20 shadow-2xl">
        <div className="text-center mb-4">
          <h1 className="text-4xl font-bold text-primary mb-2 font-mono">Tower Destroyer</h1>
          <div className="flex items-center justify-between gap-4 text-lg font-semibold">
            <div className="flex justify-center gap-8 grow">
            <span className="text-accent">Score: {score}</span>
            <span className="text-secondary flex items-center gap-1">
              {selectedBallInfo && <span className="text-base">{selectedBallInfo.icon}</span>}
              Balls: {ballsLeft}
            </span>
            <span className="text-muted-foreground">
              Towers: {towersRef.current.filter((t) => !t.destroyed).length}/{towerCount}
            </span>
            {alias ? (
              <span className="text-muted-foreground">Alias: {alias}</span>
            ) : (
              playerId && <span className="text-muted-foreground">Player: {playerId}</span>
            )}
            {isCharging && <span className="text-destructive">Power: {power}%</span>}
          </div>
            <Button onClick={handleResetPlayer} variant="destructive" size="sm" className="text-xs transition-transform duration-150 hover:scale-105 hover:shadow-lg">
              Reset Player
            </Button>
          </div>
          {hasShot && gameState === "playing" && (
            <div className="mt-2 flex gap-2">
              <Button onClick={resetGame} variant="outline" size="sm" className="text-xs bg-transparent">
                Restart Game
              </Button>
            </div>
          )}
        </div>

        <div className="relative">
          <canvas
            ref={canvasRef}
            width={CONST.CANVAS_WIDTH}
            height={CONST.CANVAS_HEIGHT}
            className={`${!readyForGame ? 'pointer-events-none' : ''} border-4 border-primary/30 rounded-lg cursor-crosshair bg-gradient-to-b from-blue-200 to-yellow-200`}
            onPointerDown={handlePointerDown}
            onPointerMove={handlePointerMove}
            onPointerUp={handlePointerUp}
          />

          {!beamReady && (
            <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
              <div className="bg-card p-4 rounded-lg border-2 border-primary/30 text-center">
                <p className="text-lg font-semibold text-primary">Beam is initializing...</p>
              </div>
            </div>
          )}

          {!hasShot && gameState === "playing" && readyForGame && !showPlayerInfo && (
            <div
              className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center"
              onClick={(e) => {
                if (e.target === e.currentTarget) {
                  Audio.playStartSound(audioContextRef)
                  resetBall()
                  setHasShot(true)
                }
              }}
            >
              <div
                className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-lg"
                onClick={(e) => {
                  e.stopPropagation()
                }}
              >
                <h2 className="text-2xl font-bold text-primary mb-4">Choose Your Ball Type</h2>

                <div className="grid grid-cols-2 gap-3 mb-6">
                  {BALL_TYPES.map((ballType) => (
                    <button
                      key={ballType.type}
                      onClick={() => {
                        Audio.playSelectSound(audioContextRef)
                        setSelectedBallType(ballType.type)
                      }}
                      className={`p-3 rounded-lg border-2 transition-all hover:scale-105 ${
                        selectedBallType === ballType.type
                          ? "border-primary bg-primary/10 shadow-lg"
                          : "border-muted hover:border-primary/50"
                      }`}
                    >
                      <div className="text-2xl mb-1">{ballType.icon}</div>
                      <div className="text-sm font-semibold">{ballType.name}</div>
                    </button>
                  ))}

                </div>

                {selectedBallInfo && (
                  <div className="mb-6 p-3 bg-muted/50 rounded-lg">
                    <div className="flex items-center justify-center gap-2 mb-2">
                      <span className="text-xl">{selectedBallInfo.icon}</span>
                      <span className="font-semibold text-primary">{selectedBallInfo.name}</span>
                    </div>
                    <p className="text-sm text-muted-foreground">{selectedBallInfo.description}</p>
                  </div>
                )}

                <div className="space-y-2 text-sm mb-4">
                  <p className="text-muted-foreground">Click and hold to aim, release to shoot!</p>
                  <p className="text-muted-foreground">Destroy all towers to win.</p>
                  <p className="text-accent">Tip: Special blocks (darker) need 2 hits and give double points!</p>
                </div>

                <p className="text-primary font-semibold">Click anywhere outside this window to start playing!</p>
              </div>
            </div>
          )}
          {beamReady && readyForGame && showPlayerInfo && (
            <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
              <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-lg w-full">
                <h2 className="text-2xl font-bold text-primary mb-4">Player Info</h2>
                <div className="space-y-3 text-left">
                  <div className="flex items-center justify-between gap-2">
                    <div>
                      <div className="text-sm text-muted-foreground">GamerTag ID</div>
                      <div className="font-mono break-all">{playerId || '-'}</div>
                    </div>
                    <Button size="sm" variant="outline" onClick={async () => { try { await navigator.clipboard.writeText(playerId || '') } catch {} }}>Copy</Button>
                  </div>
                  <div className="flex items-center justify-between gap-2">
                    <div>
                      <div className="text-sm text-muted-foreground">Alias</div>
                      <div className="font-semibold">{alias || '-'}</div>
                    </div>
                  </div>
                  <div className="flex items-center justify-between gap-2">
                    <div>
                      <div className="text-sm text-muted-foreground">Stellar Custodial ID</div>
                      <div className="font-mono break-all">{stellarExternalId || '-'}</div>
                    </div>
                    <Button size="sm" variant="outline" onClick={async () => { try { await navigator.clipboard.writeText(stellarExternalId || '') } catch {} }}>Copy</Button>
                  </div>
                </div>
                <div className="flex items-center justify-center gap-3 mt-5">
                  <Button onClick={handleResetPlayer} variant="destructive" size="sm">Reset Player</Button>
                  <Button className="bg-primary hover:bg-primary/90" size="sm" onClick={() => setShowPlayerInfo(false)}>Play Game</Button>
                </div>
              </div>
            </div>
          )}
          {beamReady && (!alias || alias.length === 0 || aliasModalOpen) && (
            <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
              <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-md w-full">
                <h2 className="text-2xl font-bold text-primary mb-4">Set Your Alias</h2>
                <p className="text-sm text-muted-foreground mb-3">Alphabet letters only, minimum 3 characters.</p>
                <input
                  type="text"
                  value={aliasInput}
                  onChange={(e) => {
                    const v = e.target.value
                    const filtered = v.replace(/[^A-Za-z]/g, "")
                    setAliasInput(filtered)
                  }}
                  className="w-full p-2 border rounded mb-3 bg-background text-foreground border-primary/30"
                  placeholder="Enter alias"
                />
                {aliasError && <p className="text-destructive text-sm mb-2">{aliasError}</p>}
                <div className="flex gap-2 justify-center">
                  <Button
                    onClick={async () => {
                      setAliasError(null)
                      const valid = /^[A-Za-z]{3,}$/.test(aliasInput)
                      if (!valid) {
                        setAliasError('Alias must be letters only, at least 3 characters.')
                        return
                      }
                      setAliasSaving(true)
                      try {
                        const beam: any = await getBeam()
                        // 1) Save alias
                        await beam.stats.set({ domainType: 'client', accessType: 'private', stats: { Alias: aliasInput } })

                        // 2) Attach external identity (custodial Stellar wallet)
                        try {
                          const providerService: string = beam?.stellarFederationClient?.serviceName || "StellarFederation"
                          const providerNamespace: string = beam?.stellarFederationClient?.federationIds?.StellarIdentity || "StellarIdentity"

                          await beam.account.addExternalIdentity({
                            externalToken: "",
                            providerService,
                            providerNamespace,
                            // challengeHandler intentionally omitted (null)
                          })

                          // Log the Stellar ID (external identity userId) if available
                          try {
                            const acct = await beam.account.current()
                            const ext = (acct?.external || []).find((e: any) => e.providerService === providerService && e.providerNamespace === providerNamespace)
                            if (ext?.userId) {
                              console.log("[Stellar] Custodial wallet attached. Stellar ID:", ext.userId)
                              setStellarExternalId(ext.userId)
                              } else {
                              console.log("[Stellar] Custodial wallet attached (no external userId found).")
                              }
                          } catch {}
                        } catch (authErr: any) {
                          console.error("[Stellar] Failed to attach custodial wallet:", authErr?.message || authErr)
                          setAliasError('We could not attach your wallet. Please restart the game and try again.')
                          return
                        }

                        // 3) Success → close alias modal and start game
                        setAlias(aliasInput)
                        setAliasModalOpen(false)
                        setShowPlayerInfo(true)
                      } catch (e: any) {
                        setAliasError(e?.message || 'Failed to save alias. Try again.')
                      } finally {
                        setAliasSaving(false)
                      }
                    }}
                    disabled={aliasSaving || !/^[A-Za-z]{3,}$/.test(aliasInput)}
                    className="bg-primary hover:bg-primary/90"
                  >
                    {aliasSaving ? 'Saving...' : 'Save Alias'}
                  </Button>
                </div>
              </div>
            </div>
          )}
          {showResetConfirm && (
            <div className="absolute inset-0 bg-black/60 z-50 rounded-lg flex items-center justify-center">
              <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center max-w-md w-full">
                <h2 className="text-2xl font-bold text-primary mb-2">Reset Player?</h2>
                <p className="text-sm text-muted-foreground mb-4">This will create a new guest player for this tab.</p>
                <div className="flex items-center justify-center gap-3">
                  <Button onClick={() => setShowResetConfirm(false)} variant="outline" size="sm" className="text-xs">
                    Cancel
                  </Button>
                  <Button onClick={confirmResetPlayer} variant="destructive" size="sm" className="text-xs transition-transform duration-150 hover:scale-105 hover:shadow-lg">
                    Yes, Reset
                  </Button>
                </div>
              </div>
            </div>
          )}

          {gameState === "won" && (
            <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
              <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center">
                <p className="text-3xl font-bold text-primary mb-2">dYZ% Victory! dYZ%</p>
                <p className="text-accent mb-2">All towers destroyed! Final Score: {score}</p>
                {ballsLeft > 0 && (
                  <p className="text-sm text-primary mb-4">
                    Bonus: {ballsLeft} balls remaining = {(1 + ballsLeft * CONST.VICTORY_BONUS_MULTIPLIER).toFixed(1)}x
                    multiplier!
                  </p>
                )}
                <Button onClick={resetGame} className="bg-primary hover:bg-primary/90">
                  Play Again
                </Button>
              </div>
            </div>
          )}

          {gameState === "gameOver" && (
            <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
              <div className="bg-card p-6 rounded-lg border-2 border-primary/30 text-center">
                <p className="text-3xl font-bold text-destructive mb-2">Game Over</p>
                <p className="text-muted-foreground mb-4">No balls left! Final Score: {score}</p>
                <Button onClick={resetGame} className="bg-primary hover:bg-primary/90">
                  Play Again
                </Button>
              </div>
            </div>
          )}
        </div>
      </Card>
    </div>
  );

}
