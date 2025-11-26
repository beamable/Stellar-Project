"use client"

import { useCallback, useEffect, useMemo, useRef, useState } from "react"
import type React from "react"

import type { Ball, BallType, BallTypeConfig, Laser, Particle, Tower, WindZone } from "@/components/Game/types"
import * as CONST from "@/components/Game/constants"
import { generateTowers } from "@/components/Game/towers"
import * as Audio from "@/components/Game/audio"
import stepPhysics from "@/components/Game/engine/stepPhysics"
import { DEFAULT_BALL_TYPE_MAP } from "@/components/Game/ballTypes"
import { createDebugTowers, DEBUG_COLLISION_MODE } from "@/components/Game/debug"
import type { TowerProfile } from "@/components/Game/campaign"

type UseTowerGameOptions = {
  readyForGame: boolean
  towerProfile: TowerProfile
  stageId: string
  ballTypeMap?: Record<BallType, BallTypeConfig>
}

export type UseTowerGameResult = {
  canvasRef: React.RefObject<HTMLCanvasElement>
  selectedBallType: BallType
  selectBallType: (type: BallType) => void
  gameState: "playing" | "won" | "gameOver"
  score: number
  ballsLeft: number
  towerCount: number
  remainingTowers: number
  powerSnapshot: number
  isCharging: boolean
  hasShot: boolean
  handlePointerDown: (event: React.PointerEvent<HTMLCanvasElement>) => void
  handlePointerMove: (event: React.PointerEvent<HTMLCanvasElement>) => void
  handlePointerUp: (event: React.PointerEvent<HTMLCanvasElement>) => void
  resetGame: () => void
  startFirstShot: () => void
  debugForceWin: () => void
}

const PHYSICS_TIMESTEP = 1000 / 60
const MAX_PHYSICS_ACCUMULATION = PHYSICS_TIMESTEP * 5
const DEBUG = false
const dlog = (...args: any[]) => {
  if (DEBUG) console.log(...args)
}

const SPECIAL_OUTLINE_COLOR = "#FFD700"
const SPECIAL_CRACK_COLOR = "rgba(255, 215, 0, 0.75)"
const SPECIAL_CRACK_COUNT = 3

type BackgroundTheme = {
  id: string
  skyTop: string
  skyBottom: string
  ground: string
  grass: string
}

const BACKGROUND_THEMES: BackgroundTheme[] = [
  { id: "sunrise", skyTop: "#87CEEB", skyBottom: "#F0E68C", ground: "#8B4513", grass: "#228B22" },
  { id: "sunset", skyTop: "#FF7E5F", skyBottom: "#FCD283", ground: "#6B2C1A", grass: "#D16A5C" },
  { id: "twilight", skyTop: "#0F2027", skyBottom: "#203A43", ground: "#1F130A", grass: "#0D4B2F" },
  { id: "desert", skyTop: "#F8C978", skyBottom: "#F5E4B7", ground: "#B2732B", grass: "#C99E53" },
  { id: "aurora", skyTop: "#1A2980", skyBottom: "#26D0CE", ground: "#2D1E2F", grass: "#1CAF8F" },
]

const getRandomBackgroundTheme = () => {
  const index = Math.floor(Math.random() * BACKGROUND_THEMES.length)
  return BACKGROUND_THEMES[index] ?? BACKGROUND_THEMES[0]
}

const TWO_PI = Math.PI * 2

const resolveWindForce = (zone: WindZone, timeMs: number) => {
  if (!zone.pulseMs || zone.pulseMs <= 0) {
    return zone.force
  }
  const oscillation = Math.sin(((timeMs + (zone.phase ?? 0)) / zone.pulseMs) * TWO_PI)
  return oscillation > 0 ? zone.force : 0
}

const ballInsideZone = (zone: WindZone, x: number, y: number) =>
  x >= zone.xStart && x <= zone.xEnd && y >= zone.yStart && y <= zone.yEnd

const pseudoRandomFromTower = (tower: Tower, salt: number) => {
  const seed = Math.sin(tower.x * 12.9898 + tower.y * 78.233 + salt) * 43758.5453
  return seed - Math.floor(seed)
}

const drawSpecialTowerCracks = (ctx: CanvasRenderingContext2D, tower: Tower) => {
  ctx.save()
  ctx.strokeStyle = SPECIAL_CRACK_COLOR
  ctx.lineWidth = 1.5
  ctx.setLineDash([2, 3])

  for (let i = 0; i < SPECIAL_CRACK_COUNT; i++) {
    const offsetRatio = (i + 1) / (SPECIAL_CRACK_COUNT + 1)
    const startX = tower.x + offsetRatio * tower.width
    const randomStartY = pseudoRandomFromTower(tower, i * 13.37)
    const startY = tower.y + randomStartY * tower.height * 0.3
    const jaggedness = tower.width * 0.2
    const crackLengthRatio = 0.4 + pseudoRandomFromTower(tower, i * 23.77) * 0.4
    const segmentCount = 2

    ctx.beginPath()
    ctx.moveTo(startX, startY)
    for (let segment = 1; segment <= segmentCount; segment++) {
      const progress = (segment / segmentCount) * crackLengthRatio
      const offsetX = (pseudoRandomFromTower(tower, i * 41.31 + segment) - 0.5) * jaggedness
      const targetX = startX + offsetX
      const targetY = startY + tower.height * progress
      ctx.lineTo(targetX, targetY)
    }
    ctx.stroke()
  }

  ctx.restore()
}

export default function useTowerGame({
  readyForGame,
  towerProfile,
  stageId,
  ballTypeMap = DEFAULT_BALL_TYPE_MAP,
}: UseTowerGameOptions): UseTowerGameResult {
  const canvasRef = useRef<HTMLCanvasElement>(null)
  const animationRef = useRef<number>()
  const audioContextRef = useRef<AudioContext | null>(null)
  const chargingOscillatorRef = useRef<OscillatorNode | null>(null)
  const chargingGainRef = useRef<GainNode | null>(null)

  const ballsRef = useRef<Ball[]>([])
  const lasersRef = useRef<Laser[]>([])
  const ballIdCounterRef = useRef(0)
  const towersRef = useRef<Tower[]>([])
  const particlesRef = useRef<Particle[]>([])
  const collisionCooldownRef = useRef<Set<string>>(new Set())
  const mousePosRef = useRef({ x: 0, y: 0 })
  const remainingTowersRef = useRef(0)
  const lastPhysicsTimeRef = useRef(0)
  const physicsAccumulatorRef = useRef(0)
  const lastChargeUpdateRef = useRef(0)
  const backgroundThemeRef = useRef<BackgroundTheme>(getRandomBackgroundTheme())
  const bossPhaseRef = useRef(1)
  const totalBossPhases = towerProfile.bossPhases ?? 1
  const resolvedWindZones = useMemo(() => towerProfile.windZones ?? [], [towerProfile])

  const [gameState, setGameState] = useState<"playing" | "won" | "gameOver">("playing")
  const [score, setScore] = useState(0)
  const [ballsLeft, setBallsLeft] = useState(() => towerProfile.ballCount ?? CONST.BALLS_FOR_LOW_TOWER_COUNT)
  const powerRef = useRef(0)
  const [powerSnapshot, setPowerSnapshot] = useState(0)
  const [isCharging, setIsCharging] = useState(false)
  const [towerCount, setTowerCount] = useState(0)
  const [remainingTowers, setRemainingTowers] = useState(0)
  const [hasShot, setHasShot] = useState(false)
  const [selectedBallType, setSelectedBallType] = useState<BallType>("normal")
  const stageWindZones = resolvedWindZones

  const initializeTowers = useCallback(
    (options?: { preserveAmmo?: boolean }) => {
      const phaseBoost = 1 + (bossPhaseRef.current - 1) * 0.15
      const resolvedMin = Math.round(
        (Number.isFinite(towerProfile.minTowers) ? towerProfile.minTowers : CONST.MIN_TOWERS) * phaseBoost,
      )
      const resolvedMax = Math.round(
        (Number.isFinite(towerProfile.maxTowers) ? towerProfile.maxTowers : CONST.MAX_TOWERS) * phaseBoost,
      )
      const resolvedSpecialBase =
        typeof towerProfile.specialBlockRatio === "number"
          ? towerProfile.specialBlockRatio
          : resolvedMax > CONST.TOWER_THRESHOLD_FOR_HIGH_SPECIAL
            ? CONST.SPECIAL_BLOCK_PERCENTAGE_HIGH
            : CONST.SPECIAL_BLOCK_PERCENTAGE_LOW
      const resolvedSpecial = Math.min(0.6, resolvedSpecialBase * (1 + (bossPhaseRef.current - 1) * 0.1))

      const { towers, towerCount: count } = DEBUG_COLLISION_MODE
        ? createDebugTowers()
        : generateTowers({
            minTowers: resolvedMin,
            maxTowers: resolvedMax,
            specialBlockRatio: resolvedSpecial,
            movingTargetChance: towerProfile.movingTargetChance,
            movingAmplitude: towerProfile.movingAmplitude,
            movingSpeed: towerProfile.movingSpeed,
          })
      towersRef.current = towers.map((tower) => ({
        ...tower,
        baseX: typeof tower.baseX === "number" ? tower.baseX : tower.x,
      }))
      setTowerCount(count)
      remainingTowersRef.current = count
      setRemainingTowers(count)
      if (!options?.preserveAmmo) {
        const initialBallCount =
          towerProfile.ballCount ??
          (DEBUG_COLLISION_MODE || count <= CONST.TOWER_THRESHOLD_FOR_HIGH_SPECIAL
            ? CONST.BALLS_FOR_LOW_TOWER_COUNT
            : CONST.BALLS_FOR_HIGH_TOWER_COUNT)
        setBallsLeft(initialBallCount)
      }
    },
    [towerProfile],
  )

  const resetBall = useCallback(
    (typeOverride?: BallType) => {
      const effectiveType = typeOverride ?? selectedBallType
      dlog("[v0] Resetting ball for next shot")
      const startX = 100
      const startY = CONST.GROUND_Y - 10 - 12 // keep the 12px radius just above ground
      const newBall: Ball = {
        x: startX,
        y: startY,
        lastX: startX,
        lastY: startY,
        vx: 0,
        vy: 0,
        radius: 12,
        active: false,
        type: effectiveType,
        id: ballIdCounterRef.current++,
      }
      ballsRef.current = [newBall]
      lasersRef.current = []
      dlog(
        `[v0] Ball reset complete - ball exists: ${ballsRef.current.length > 0}, type: ${newBall.type}`,
      )
    },
    [selectedBallType],
  )

  const selectBallType = useCallback(
    (type: BallType) => {
      const target = ballTypeMap[type] ? type : "normal"
      setSelectedBallType((current) => (current === target ? current : target))
      Audio.playSelectSound(audioContextRef)
    },
    [ballTypeMap],
  )

  const applyTowerMotion = useCallback((timeMs: number) => {
    towersRef.current.forEach((tower) => {
      if (tower.destroyed) {
        return
      }
      if (tower.motion) {
        const offset = Math.sin(timeMs * tower.motion.speed + tower.motion.phase) * tower.motion.amplitude
        tower.x = (tower.motion.originX ?? tower.baseX ?? tower.x) + offset
      } else if (typeof tower.baseX === "number") {
        tower.x = tower.baseX
      }
    })
  }, [])

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

      const shootPan = toStereoPan(currentBall.x)
      Audio.playShootSound(audioContextRef, selectedBallType, shootPan)
      const typeConfig = ballTypeMap[selectedBallType] ?? DEFAULT_BALL_TYPE_MAP.normal
      const powerRatio = Math.min(power / CONST.MAX_POWER, 1)
      const force = powerRatio * CONST.SHOT_FORCE_MULTIPLIER * typeConfig.baseSpeedMultiplier
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
            lastX: currentBall.x,
            lastY: currentBall.y,
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
    [ballTypeMap, selectedBallType],
  )

  const handleAllTowersDestroyed = useCallback(() => {
    if (bossPhaseRef.current < totalBossPhases) {
      bossPhaseRef.current += 1
      initializeTowers({ preserveAmmo: true })
      resetBall()
      Audio.playStartSound(audioContextRef)
      return "phase"
    }
    return "win"
  }, [initializeTowers, totalBossPhases, resetBall])

  const gameLoop = useCallback(() => {
    const canvas = canvasRef.current
    if (!canvas) return

    const ctxMaybe = canvas.getContext("2d")
    if (!ctxMaybe) {
      animationRef.current = requestAnimationFrame(gameLoop)
      return
    }
    const ctx = ctxMaybe

    const now = performance.now()
    const previousChargeSample = lastChargeUpdateRef.current || now
    const canCharge = isCharging && ballsRef.current.length > 0 && !ballsRef.current[0].active

    if (canCharge) {
      const deltaChargeMs = now - previousChargeSample
      if (deltaChargeMs > 0) {
        const increment =
          (deltaChargeMs / PHYSICS_TIMESTEP) * CONST.POWER_INCREMENT
        powerRef.current = Math.min(CONST.MAX_POWER, powerRef.current + increment)
      }
    }

    lastChargeUpdateRef.current = now
    const previousPhysicsTime = lastPhysicsTimeRef.current || now
    const deltaPhysics = Math.max(0, now - previousPhysicsTime)
    lastPhysicsTimeRef.current = now

    physicsAccumulatorRef.current = Math.min(
      physicsAccumulatorRef.current + deltaPhysics,
      MAX_PHYSICS_ACCUMULATION,
    )

    applyTowerMotion(now)

    while (physicsAccumulatorRef.current >= PHYSICS_TIMESTEP) {
      stepPhysics({
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
        windZones: stageWindZones,
        windTimeMs: now,
        onAllTowersDestroyed: handleAllTowersDestroyed,
      })
      physicsAccumulatorRef.current -= PHYSICS_TIMESTEP
    }

    const backgroundTheme = backgroundThemeRef.current
    const gradient = ctx.createLinearGradient(0, 0, 0, CONST.CANVAS_HEIGHT)
    gradient.addColorStop(0, backgroundTheme.skyTop)
    gradient.addColorStop(1, backgroundTheme.skyBottom)
    ctx.fillStyle = gradient
    ctx.fillRect(0, 0, CONST.CANVAS_WIDTH, CONST.CANVAS_HEIGHT)

    ctx.fillStyle = backgroundTheme.ground
    ctx.fillRect(0, CONST.GROUND_Y, CONST.CANVAS_WIDTH, CONST.CANVAS_HEIGHT - CONST.GROUND_Y)

    ctx.fillStyle = backgroundTheme.grass
    for (let i = 0; i < CONST.CANVAS_WIDTH; i += 5) {
      const height = 3 + Math.sin(i * 0.1) * 2
      ctx.fillRect(i, CONST.GROUND_Y - height, 3, height)
    }

    if (stageWindZones.length > 0) {
      stageWindZones.forEach((zone) => {
        const force = resolveWindForce(zone, now)
        const alpha = force === 0 ? 0.07 : 0.18
        const color = force >= 0 ? `rgba(96, 165, 250, ${alpha})` : `rgba(129, 140, 248, ${alpha})`
        ctx.fillStyle = color
        ctx.fillRect(zone.xStart, zone.yStart, zone.xEnd - zone.xStart, zone.yEnd - zone.yStart)
      })
    }

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

    towersRef.current.forEach((tower) => {
      if (tower.destroyed) return

      ctx.fillStyle = tower.color
      ctx.fillRect(tower.x, tower.y, tower.width, tower.height)

      if (tower.isSpecial && tower.hits === 0) {
        ctx.strokeStyle = SPECIAL_OUTLINE_COLOR
        ctx.lineWidth = 2
        ctx.strokeRect(tower.x - 1, tower.y - 1, tower.width + 2, tower.height + 2)
      } else if (tower.isSpecial && tower.hits > 0) {
        drawSpecialTowerCracks(ctx, tower)
      }
    })

    ballsRef.current.forEach((ball) => {
      const ballColor = ballTypeMap[ball.type]?.color ?? DEFAULT_BALL_TYPE_MAP.normal.color
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

    if (isCharging && ballsRef.current.length > 0 && !ballsRef.current[0].active) {
      const ball = ballsRef.current[0]
      const dx = mousePosRef.current.x - ball.x
      const dy = mousePosRef.current.y - ball.y
      const distance = Math.sqrt(dx * dx + dy * dy)
      if (distance >= 1e-6) {
        const maxDistance = 150
        const lineLength = Math.min(distance, maxDistance)

        const powerRatio = powerRef.current / CONST.MAX_POWER
        ctx.strokeStyle = `rgba(139, 69, 19, ${0.3 + powerRatio * 0.4})`
        ctx.lineWidth = 2 + powerRatio * 3
        ctx.setLineDash([5, 5])
        ctx.beginPath()
        ctx.moveTo(ball.x, ball.y)
        ctx.lineTo(ball.x + (dx / distance) * lineLength, ball.y + (dy / distance) * lineLength)
        ctx.stroke()
        ctx.setLineDash([])
      }
    }

    animationRef.current = requestAnimationFrame(gameLoop)
  }, [ballsLeft, gameState, isCharging, resetBall, stageWindZones, handleAllTowersDestroyed, applyTowerMotion, ballTypeMap])

  useEffect(() => {
    animationRef.current = requestAnimationFrame(gameLoop)
    return () => {
      if (animationRef.current) {
        cancelAnimationFrame(animationRef.current)
      }
    }
  }, [gameLoop])

  const resetGameInternal = useCallback(
    (options?: { silent?: boolean }) => {
      dlog("[v0] Resetting game")
      if (!options?.silent) {
        Audio.playRestartSound(audioContextRef)
      }

      setGameState("playing")
      setScore(0)
      powerRef.current = 0
      setPowerSnapshot(0)
      setIsCharging(false)
      setHasShot(false)
      setSelectedBallType("normal")
      resetBall("normal")
      particlesRef.current = []
      collisionCooldownRef.current.clear()
      backgroundThemeRef.current = getRandomBackgroundTheme()
      bossPhaseRef.current = 1

      initializeTowers()
    },
    [initializeTowers, resetBall],
  )

  const bootstrappedRef = useRef(false)
  const activeStageRef = useRef(stageId)
  useEffect(() => {
    if (!bootstrappedRef.current) {
      bootstrappedRef.current = true
      activeStageRef.current = stageId
      resetGameInternal({ silent: true })
      return
    }
    if (activeStageRef.current !== stageId) {
      activeStageRef.current = stageId
      resetGameInternal({ silent: true })
    }
  }, [stageId, resetGameInternal])

  useEffect(() => {
    if (typeof window === "undefined") return
    const id = window.setInterval(() => {
      setPowerSnapshot((prev) => {
        const next = Math.round(powerRef.current)
        return prev === next ? prev : next
      })
    }, 100)
    return () => {
      window.clearInterval(id)
    }
  }, [])

  const handlePointerDown = useCallback(
    (e: React.PointerEvent<HTMLCanvasElement>) => {
      if (!readyForGame) return
      if (
        (ballsRef.current.length > 0 && ballsRef.current.some((ball) => ball.active)) ||
        gameState !== "playing" ||
        ballsLeft === 0
      ) {
        return
      }

      setIsCharging(true)
      powerRef.current = 0
      setPowerSnapshot(0)
      Audio.playChargingSound(audioContextRef, chargingOscillatorRef, chargingGainRef)

      const rect = canvasRef.current?.getBoundingClientRect()
      if (rect) {
        mousePosRef.current = {
          x: e.clientX - rect.left,
          y: e.clientY - rect.top,
        }
      }
    },
    [ballsLeft, gameState, readyForGame],
  )

  const handlePointerMove = useCallback((e: React.PointerEvent<HTMLCanvasElement>) => {
    if (!readyForGame) return
    const rect = canvasRef.current?.getBoundingClientRect()
    if (rect) {
      mousePosRef.current = {
        x: e.clientX - rect.left,
        y: e.clientY - rect.top,
      }
    }
  }, [readyForGame])

  const handlePointerUp = useCallback(
    (e: React.PointerEvent<HTMLCanvasElement>) => {
      if (!readyForGame) return
      Audio.stopChargingSound(chargingOscillatorRef, chargingGainRef)

      if (!isCharging || (ballsRef.current.length > 0 && ballsRef.current.some((ball) => ball.active))) {
        return
      }

      const rect = canvasRef.current?.getBoundingClientRect()
      if (rect) {
        const targetX = e.clientX - rect.left
        const targetY = e.clientY - rect.top
        const currentPower = powerRef.current
        shootBall(targetX, targetY, currentPower)
      }

      setIsCharging(false)
      powerRef.current = 0
      setPowerSnapshot(0)
    },
    [readyForGame, shootBall, isCharging],
  )

  const resetGame = useCallback(() => {
    resetGameInternal()
  }, [resetGameInternal])

  const startFirstShot = useCallback(() => {
    Audio.playStartSound(audioContextRef)
    resetBall()
    setHasShot(true)
  }, [resetBall])

  const debugForceWin = useCallback(() => {
    towersRef.current.forEach((tower) => {
      tower.destroyed = true
    })
    remainingTowersRef.current = 0
    setRemainingTowers(0)
    const ballMultiplier = ballsLeft > 0 ? 1 + ballsLeft * CONST.VICTORY_BONUS_MULTIPLIER : 1
    if (ballMultiplier > 1) {
      setScore((prev) => {
        const bonusPoints = Math.floor(prev * (ballMultiplier - 1))
        return bonusPoints > 0 ? prev + bonusPoints : prev
      })
    }
    Audio.playWinSound(audioContextRef)
    setGameState("won")
  }, [ballsLeft])

  return {
    canvasRef,
    selectedBallType,
    selectBallType,
    gameState,
    score,
    ballsLeft,
    towerCount,
    remainingTowers,
    powerSnapshot,
    isCharging,
    hasShot,
    handlePointerDown,
    handlePointerMove,
    handlePointerUp,
    resetGame,
    startFirstShot,
    debugForceWin,
  }
}
const toStereoPan = (x: number) => Math.max(-1, Math.min(1, (x / CONST.CANVAS_WIDTH) * 2 - 1))
