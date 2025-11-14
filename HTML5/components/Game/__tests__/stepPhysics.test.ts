"use client"

import { describe, expect, it, vi, afterEach } from "vitest"
import stepPhysics from "@/components/Game/engine/stepPhysics"
import * as CONST from "@/components/Game/constants"
import type { Ball, Laser, Particle, Tower } from "@/components/Game/types"
import * as ParticlesModule from "@/components/Game/particles"
import * as AudioModule from "@/components/Game/audio"

vi.mock("@/components/Game/audio", () => ({
  playGroundBounceSound: vi.fn(),
  playLaserShootSound: vi.fn(),
  playTowerBreakSound: vi.fn(),
  playWinSound: vi.fn(),
  playLoseSound: vi.fn(),
  playRestartSound: vi.fn(),
  playStartSound: vi.fn(),
  playChargingSound: vi.fn(),
  stopChargingSound: vi.fn(),
  playSelectSound: vi.fn(),
}))

vi.mock("@/components/Game/particles", () => ({
  createParticles: vi.fn(),
  createLaserParticles: vi.fn(),
  createLaserDestructionParticles: vi.fn(),
  createWinParticles: vi.fn(),
  createLoseParticles: vi.fn(),
}))

const makeRef = <T,>(value: T) => ({ current: value })

const createTower = (overrides: Partial<Tower> = {}): Tower => ({
  x: 500,
  y: CONST.GROUND_Y - 100,
  width: 40,
  height: 100,
  destroyed: false,
  color: "#FFFFFF",
  isSpecial: false,
  hits: 0,
  maxHits: 1,
  ...overrides,
})

const createBall = (overrides: Partial<Ball> = {}): Ball => ({
  x: 500,
  y: CONST.GROUND_Y - 110,
  vx: 0,
  vy: 0,
  radius: 12,
  active: true,
  type: "normal",
  id: 1,
  ...overrides,
})

describe("stepPhysics", () => {
  afterEach(() => {
    vi.clearAllMocks()
    vi.restoreAllMocks()
  })

  it("destroys towers on collision and updates score", () => {
    const ballsRef = makeRef<Ball[]>([createBall()])
    const lasersRef = makeRef<Laser[]>([])
    const towersRef = makeRef<Tower[]>([createTower()])
    const particlesRef = makeRef<Particle[]>([])
    const collisionCooldownRef = makeRef<Set<string>>(new Set())
    const remainingTowersRef = makeRef<number>(towersRef.current.length)
    const audioContextRef = makeRef<AudioContext | null>(null)

    let score = 0
    const setScore = vi.fn((updater: any) => {
      score = typeof updater === "function" ? updater(score) : updater
    })

    let gameState: "playing" | "won" | "gameOver" = "playing"
    const setGameState = vi.fn((value: any) => {
      gameState = typeof value === "function" ? value(gameState) : value
    })

    let remainingTowers = towersRef.current.length
    const setRemainingTowers = vi.fn((value: number | ((prev: number) => number)) => {
      remainingTowers = typeof value === "function" ? value(remainingTowers) : value
    })

    const resetBall = vi.fn()

    stepPhysics({
      ballsRef,
      lasersRef,
      towersRef,
      particlesRef,
      collisionCooldownRef,
      audioContextRef,
      ballsLeft: 0,
      gameState,
      setScore,
      setGameState,
      resetBall,
      setRemainingTowers,
      remainingTowersRef,
    })

    expect(towersRef.current[0].destroyed).toBe(true)
    expect(score).toBe(CONST.POINTS_NORMAL_TOWER)
    expect(remainingTowers).toBe(0)
  })

  it("resets the ball when all active balls are settled", () => {
    const ballsRef = makeRef<Ball[]>([
      createBall({
        y: CONST.GROUND_Y - 12,
        vy: 0,
        vx: 0,
      }),
    ])
    const lasersRef = makeRef<Laser[]>([])
    const towersRef = makeRef<Tower[]>([])
    const particlesRef = makeRef<Particle[]>([])
    const collisionCooldownRef = makeRef<Set<string>>(new Set(["1-0"]))
    const remainingTowersRef = makeRef<number>(0)
    const audioContextRef = makeRef<AudioContext | null>(null)

    let score = 0
    const setScore = vi.fn((updater: any) => {
      score = typeof updater === "function" ? updater(score) : updater
    })
    const setGameState = vi.fn()
    const setRemainingTowers = vi.fn()
    const resetBall = vi.fn()

    stepPhysics({
      ballsRef,
      lasersRef,
      towersRef,
      particlesRef,
      collisionCooldownRef,
      audioContextRef,
      ballsLeft: CONST.BALLS_FOR_LOW_TOWER_COUNT,
      gameState: "playing",
      setScore,
      setGameState,
      resetBall,
      setRemainingTowers,
      remainingTowersRef,
    })

    expect(resetBall).toHaveBeenCalledTimes(1)
    expect(collisionCooldownRef.current.size).toBe(0)
  })

  it("creates lasers for laser balls and targets towers", () => {
    const dateSpy = vi.spyOn(Date, "now").mockReturnValue(10_000)
    const randomSpy = vi.spyOn(Math, "random").mockReturnValue(0)

    const ballsRef = makeRef<Ball[]>([
      createBall({
        type: "laser",
        y: CONST.LASER_CREATION_HEIGHT - 20,
        shotTime: 10_000 - CONST.LASER_CREATION_DELAY_MS - 1,
      }),
    ])
    const lasersRef = makeRef<Laser[]>([])
    const towersRef = makeRef<Tower[]>([createTower(), createTower({ x: 650 })])
    const particlesRef = makeRef<Particle[]>([])
    const collisionCooldownRef = makeRef<Set<string>>(new Set())
    const remainingTowersRef = makeRef<number>(towersRef.current.length)
    const audioContextRef = makeRef<AudioContext | null>(null)

    const setScore = vi.fn()
    const setGameState = vi.fn()
    const setRemainingTowers = vi.fn()
    const resetBall = vi.fn()

    stepPhysics({
      ballsRef,
      lasersRef,
      towersRef,
      particlesRef,
      collisionCooldownRef,
      audioContextRef,
      ballsLeft: CONST.BALLS_FOR_LOW_TOWER_COUNT,
      gameState: "playing",
      setScore,
      setGameState,
      resetBall,
      setRemainingTowers,
      remainingTowersRef,
    })

    expect(lasersRef.current.length).toBe(CONST.LASER_COUNT)
    expect(AudioModule.playLaserShootSound).toHaveBeenCalled()

    dateSpy.mockRestore()
    randomSpy.mockRestore()
  })

  it("sets the game state to won when no towers remain", () => {
    const ballsRef = makeRef<Ball[]>([])
    const lasersRef = makeRef<Laser[]>([])
    const towersRef = makeRef<Tower[]>([])
    const particlesRef = makeRef<Particle[]>([])
    const collisionCooldownRef = makeRef<Set<string>>(new Set())
    const remainingTowersRef = makeRef<number>(0)
    const audioContextRef = makeRef<AudioContext | null>(null)

    let score = 100
    const setScore = vi.fn((updater: any) => {
      score = typeof updater === "function" ? updater(score) : updater
    })

    let gameState: "playing" | "won" | "gameOver" = "playing"
    const setGameState = vi.fn((value: any) => {
      gameState = typeof value === "function" ? value(gameState) : value
    })

    const setRemainingTowers = vi.fn()
    const resetBall = vi.fn()

    stepPhysics({
      ballsRef,
      lasersRef,
      towersRef,
      particlesRef,
      collisionCooldownRef,
      audioContextRef,
      ballsLeft: 3,
      gameState,
      setScore,
      setGameState,
      resetBall,
      setRemainingTowers,
      remainingTowersRef,
    })

    expect(setGameState).toHaveBeenCalledWith("won")
    expect(AudioModule.playWinSound).toHaveBeenCalled()
    expect(ParticlesModule.createWinParticles).toHaveBeenCalled()
    expect(score).toBeGreaterThan(100)
  })

  it("sets the game state to gameOver when out of balls and towers remain", () => {
    const ballsRef = makeRef<Ball[]>([
      createBall({
        active: false,
      }),
    ])
    const lasersRef = makeRef<Laser[]>([])
    const towersRef = makeRef<Tower[]>([createTower()])
    const particlesRef = makeRef<Particle[]>([])
    const collisionCooldownRef = makeRef<Set<string>>(new Set())
    const remainingTowersRef = makeRef<number>(1)
    const audioContextRef = makeRef<AudioContext | null>(null)

    const setScore = vi.fn()
    const setGameState = vi.fn()
    const setRemainingTowers = vi.fn()
    const resetBall = vi.fn()

    stepPhysics({
      ballsRef,
      lasersRef,
      towersRef,
      particlesRef,
      collisionCooldownRef,
      audioContextRef,
      ballsLeft: 0,
      gameState: "playing",
      setScore,
      setGameState,
      resetBall,
      setRemainingTowers,
      remainingTowersRef,
    })

    expect(setGameState).toHaveBeenCalledWith("gameOver")
    expect(AudioModule.playLoseSound).toHaveBeenCalled()
    expect(ParticlesModule.createLoseParticles).toHaveBeenCalled()
  })
})
