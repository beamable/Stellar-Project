import type React from "react"
/**
 * Audio system for Tower Destroyer
 * Uses Web Audio API to generate retro-style game sounds
 */

import type { BallType } from "./types"

const DEBUG = false
const dlog = (...args: any[]) => {
  if (DEBUG) console.log(...args)
}

/**
 * Initializes the Web Audio API context
 * Must be called after user interaction due to browser autoplay policies
 */
export function initAudioContext(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  if (!audioContextRef.current) {
    try {
      audioContextRef.current = new (window.AudioContext || (window as any).webkitAudioContext)()
      dlog("[v0] Audio context initialized")
    } catch (error) {
      console.error("[v0] Failed to initialize audio context:", error)
    }
  }
}

/**
 * Plays a continuous charging sound that increases in pitch and volume
 */
export function playChargingSound(
  audioContextRef: React.MutableRefObject<AudioContext | null>,
  chargingOscillatorRef: React.MutableRefObject<OscillatorNode | null>,
  chargingGainRef: React.MutableRefObject<GainNode | null>,
): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  stopChargingSound(chargingOscillatorRef, chargingGainRef)

  const audioContext = audioContextRef.current
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "sine"
  oscillator.frequency.setValueAtTime(200, audioContext.currentTime)
  oscillator.frequency.linearRampToValueAtTime(400, audioContext.currentTime + 1)

  gainNode.gain.setValueAtTime(0.1, audioContext.currentTime)
  gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 1)

  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)

  oscillator.start()

  chargingOscillatorRef.current = oscillator
  chargingGainRef.current = gainNode

  dlog("[v0] Charging sound started")
}

/**
 * Stops the charging sound effect
 */
export function stopChargingSound(
  chargingOscillatorRef: React.MutableRefObject<OscillatorNode | null>,
  chargingGainRef: React.MutableRefObject<GainNode | null>,
): void {
  if (chargingOscillatorRef.current) {
    try {
      chargingOscillatorRef.current.stop()
      chargingOscillatorRef.current.disconnect()
    } catch (e) {
      // Oscillator might already be stopped
    }
    chargingOscillatorRef.current = null
    chargingGainRef.current = null
    dlog("[v0] Charging sound stopped")
  }
}

/**
 * Plays a ball-type-specific shooting sound
 */
export function playShootSound(audioContextRef: React.MutableRefObject<AudioContext | null>, ballType: BallType): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  switch (ballType) {
    case "normal":
      oscillator.type = "sine"
      oscillator.frequency.setValueAtTime(300, audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(100, audioContext.currentTime + 0.1)
      gainNode.gain.setValueAtTime(0.3, audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.1)
      break

    case "multishot":
      oscillator.type = "square"
      oscillator.frequency.setValueAtTime(400, audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(200, audioContext.currentTime + 0.15)
      gainNode.gain.setValueAtTime(0.25, audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.15)
      break

    case "fire":
      oscillator.type = "sawtooth"
      oscillator.frequency.setValueAtTime(150, audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(50, audioContext.currentTime + 0.2)
      gainNode.gain.setValueAtTime(0.4, audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.2)
      break

    case "laser":
      oscillator.type = "sawtooth"
      oscillator.frequency.setValueAtTime(800, audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(200, audioContext.currentTime + 0.12)
      gainNode.gain.setValueAtTime(0.35, audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.12)
      break
  }

  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.3)

  dlog(`[v0] ${ballType} ball shoot sound played`)
}

export function playTowerBreakSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "square"
  oscillator.frequency.setValueAtTime(600, audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(100, audioContext.currentTime + 0.15)

  gainNode.gain.setValueAtTime(0.25, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.15)

  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.15)
}

export function playLaserShootSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "sawtooth"
  oscillator.frequency.setValueAtTime(400, audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(1200, audioContext.currentTime + 0.08)

  gainNode.gain.setValueAtTime(0.3, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.08)

  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.08)
}

export function playWinSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const notes = [523.25, 659.25, 783.99, 1046.5] // C5, E5, G5, C6

  notes.forEach((frequency, index) => {
    const oscillator = audioContext.createOscillator()
    const gainNode = audioContext.createGain()

    oscillator.type = "sine"
    oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime + index * 0.15)

    gainNode.gain.setValueAtTime(0, audioContext.currentTime + index * 0.15)
    gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + index * 0.15 + 0.05)
    gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + index * 0.15 + 0.3)

    oscillator.connect(gainNode)
    gainNode.connect(audioContext.destination)

    oscillator.start(audioContext.currentTime + index * 0.15)
    oscillator.stop(audioContext.currentTime + index * 0.15 + 0.3)
  })
}

export function playLoseSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const notes = [523.25, 493.88, 440, 392] // C5, B4, A4, G4

  notes.forEach((frequency, index) => {
    const oscillator = audioContext.createOscillator()
    const gainNode = audioContext.createGain()

    oscillator.type = "triangle"
    oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime + index * 0.2)

    gainNode.gain.setValueAtTime(0, audioContext.currentTime + index * 0.2)
    gainNode.gain.linearRampToValueAtTime(0.25, audioContext.currentTime + index * 0.2 + 0.05)
    gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + index * 0.2 + 0.4)

    oscillator.connect(gainNode)
    gainNode.connect(audioContext.destination)

    oscillator.start(audioContext.currentTime + index * 0.2)
    oscillator.stop(audioContext.currentTime + index * 0.2 + 0.4)
  })
}

export function playGroundBounceSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "sine"
  oscillator.frequency.setValueAtTime(80, audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(40, audioContext.currentTime + 0.1)

  gainNode.gain.setValueAtTime(0.2, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.1)

  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.1)
}

export function playRestartSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "square"
  oscillator.frequency.setValueAtTime(800, audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(400, audioContext.currentTime + 0.05)

  gainNode.gain.setValueAtTime(0.2, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.05)

  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.05)
}

export function playSelectSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "sine"
  oscillator.frequency.setValueAtTime(600, audioContext.currentTime)

  gainNode.gain.setValueAtTime(0.15, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.08)

  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.08)
}

export function playStartSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  initAudioContext(audioContextRef)
  if (!audioContextRef.current) return

  const audioContext = audioContextRef.current
  const notes = [523.25, 659.25] // C5, E5

  notes.forEach((frequency, index) => {
    const oscillator = audioContext.createOscillator()
    const gainNode = audioContext.createGain()

    oscillator.type = "sine"
    oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime + index * 0.08)

    gainNode.gain.setValueAtTime(0, audioContext.currentTime + index * 0.08)
    gainNode.gain.linearRampToValueAtTime(0.2, audioContext.currentTime + index * 0.08 + 0.02)
    gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + index * 0.08 + 0.15)

    oscillator.connect(gainNode)
    gainNode.connect(audioContext.destination)

    oscillator.start(audioContext.currentTime + index * 0.08)
    oscillator.stop(audioContext.currentTime + index * 0.08 + 0.15)
  })
}
