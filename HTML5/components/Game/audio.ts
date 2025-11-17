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

let masterGainNode: GainNode | null = null
let masterVolume = 1

const clampVolume = (value: number) => Math.min(1, Math.max(0, value))
const clampPan = (value: number) => Math.min(1, Math.max(-1, value))

const withVariation = (value: number, variation = 0.08) =>
  value * (1 + (Math.random() * 2 - 1) * variation)

type AudioDestination = { node: AudioNode; dispose: () => void }

const ensureMasterGain = (audioContext: AudioContext): GainNode => {
  if (masterGainNode && masterGainNode.context !== audioContext) {
    try {
      masterGainNode.disconnect()
    } catch {
      // Already disconnected or context disposed
    }
    masterGainNode = null
  }

  if (!masterGainNode) {
    masterGainNode = audioContext.createGain()
    masterGainNode.gain.setValueAtTime(masterVolume, audioContext.currentTime)
    masterGainNode.connect(audioContext.destination)
  }

  return masterGainNode
}

export function setMasterVolume(volume: number): void {
  masterVolume = clampVolume(volume)
  if (masterGainNode) {
    masterGainNode.gain.setValueAtTime(masterVolume, masterGainNode.context.currentTime)
  }
}

export function getMasterVolume(): number {
  return masterVolume
}

const createPannedDestination = (audioContext: AudioContext, pan?: number): AudioDestination => {
  const master = ensureMasterGain(audioContext)
  if (typeof audioContext.createStereoPanner !== "function" || typeof pan !== "number") {
    return { node: master, dispose: () => {} }
  }
  const panner = audioContext.createStereoPanner()
  panner.pan.setValueAtTime(clampPan(pan), audioContext.currentTime)
  panner.connect(master)
  return {
    node: panner,
    dispose: () => {
      try {
        panner.disconnect()
      } catch {
        /* no-op */
      }
    },
  }
}

const createNoiseBurst = (
  audioContext: AudioContext,
  destination: AudioNode,
  duration = 0.15,
  gainValue = 0.2,
) => {
  const sampleCount = Math.floor(audioContext.sampleRate * duration)
  const buffer = audioContext.createBuffer(1, sampleCount, audioContext.sampleRate)
  const data = buffer.getChannelData(0)
  for (let i = 0; i < sampleCount; i++) {
    data[i] = Math.random() * 2 - 1
  }
  const noiseSource = audioContext.createBufferSource()
  noiseSource.buffer = buffer

  const gainNode = audioContext.createGain()
  gainNode.gain.setValueAtTime(gainValue, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.001, audioContext.currentTime + duration)

  noiseSource.connect(gainNode)
  gainNode.connect(destination)
  noiseSource.start()
  noiseSource.stop(audioContext.currentTime + duration)
  noiseSource.addEventListener("ended", () => {
    try {
      gainNode.disconnect()
    } catch {
      /* no-op */
    }
  })
}

let activeTowerBreakVoices = 0
const MAX_TOWER_BREAK_VOICES = 4
let lastWinSoundTime = 0
let lastLoseSoundTime = 0
const RESULT_SOUND_INTERVAL = 0.5

/**
 * Initializes the Web Audio API context
 * Must be called after user interaction due to browser autoplay policies
 */
export function initAudioContext(audioContextRef: React.MutableRefObject<AudioContext | null>): AudioContext | null {
  if (!audioContextRef.current) {
    try {
      audioContextRef.current = new (window.AudioContext || (window as any).webkitAudioContext)()
      dlog("[v0] Audio context initialized")
    } catch (error) {
      console.error("[v0] Failed to initialize audio context:", error)
    }
  }

  const audioContext = audioContextRef.current
  if (!audioContext) {
    return null
  }

  if (audioContext.state === "suspended") {
    audioContext
      .resume()
      .then(() => dlog("[v0] Audio context resumed"))
      .catch((error) => console.warn("[v0] Failed to resume audio context", error))
  }

  ensureMasterGain(audioContext)
  return audioContext
}

/**
 * Plays a continuous charging sound that increases in pitch and volume
 */
export function playChargingSound(
  audioContextRef: React.MutableRefObject<AudioContext | null>,
  chargingOscillatorRef: React.MutableRefObject<OscillatorNode | null>,
  chargingGainRef: React.MutableRefObject<GainNode | null>,
): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return

  stopChargingSound(chargingOscillatorRef, chargingGainRef)

  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "sine"
  oscillator.frequency.setValueAtTime(200, audioContext.currentTime)
  oscillator.frequency.linearRampToValueAtTime(400, audioContext.currentTime + 1)

  gainNode.gain.setValueAtTime(0.1, audioContext.currentTime)
  gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 1)

  oscillator.connect(gainNode)
  gainNode.connect(ensureMasterGain(audioContext))

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
    } catch {
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
export function playShootSound(
  audioContextRef: React.MutableRefObject<AudioContext | null>,
  ballType: BallType,
  pan = 0,
): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()
  const destination = createPannedDestination(audioContext, pan)

  switch (ballType) {
    case "normal":
      oscillator.type = "sine"
      oscillator.frequency.setValueAtTime(withVariation(300, 0.05), audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(withVariation(100, 0.05), audioContext.currentTime + 0.1)
      gainNode.gain.setValueAtTime(withVariation(0.3, 0.1), audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.1)
      break

    case "multishot":
      oscillator.type = "square"
      oscillator.frequency.setValueAtTime(withVariation(400, 0.08), audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(withVariation(200, 0.08), audioContext.currentTime + 0.15)
      gainNode.gain.setValueAtTime(withVariation(0.25, 0.1), audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.15)
      break

    case "fire":
      oscillator.type = "sawtooth"
      oscillator.frequency.setValueAtTime(withVariation(150, 0.1), audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(withVariation(50, 0.1), audioContext.currentTime + 0.2)
      gainNode.gain.setValueAtTime(withVariation(0.4, 0.15), audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.2)
      break

    case "laser":
      oscillator.type = "sawtooth"
      oscillator.frequency.setValueAtTime(withVariation(800, 0.05), audioContext.currentTime)
      oscillator.frequency.exponentialRampToValueAtTime(withVariation(200, 0.05), audioContext.currentTime + 0.12)
      gainNode.gain.setValueAtTime(withVariation(0.35, 0.1), audioContext.currentTime)
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.12)
      break
  }

  oscillator.connect(gainNode)
  gainNode.connect(destination.node)

  oscillator.start()
  const stopTime = audioContext.currentTime + 0.3
  oscillator.stop(stopTime)
  oscillator.addEventListener("ended", () => {
    try {
      gainNode.disconnect()
    } catch {}
    destination.dispose()
  })

  dlog(`[v0] ${ballType} ball shoot sound played`)
}

export function playTowerBreakSound(
  audioContextRef: React.MutableRefObject<AudioContext | null>,
  pan = 0,
): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  if (activeTowerBreakVoices >= MAX_TOWER_BREAK_VOICES) {
    return
  }
  activeTowerBreakVoices++

  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()
  const destination = createPannedDestination(audioContext, pan)

  oscillator.type = "square"
  oscillator.frequency.setValueAtTime(withVariation(600, 0.06), audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(withVariation(100, 0.08), audioContext.currentTime + 0.15)

  gainNode.gain.setValueAtTime(withVariation(0.25, 0.1), audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.15)

  oscillator.connect(gainNode)
  gainNode.connect(destination.node)
  createNoiseBurst(audioContext, destination.node, 0.12, 0.12)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.15)
  oscillator.addEventListener("ended", () => {
    activeTowerBreakVoices = Math.max(0, activeTowerBreakVoices - 1)
    try {
      gainNode.disconnect()
    } catch {}
    destination.dispose()
  })
}

export function playLaserShootSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "sawtooth"
  oscillator.frequency.setValueAtTime(400, audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(1200, audioContext.currentTime + 0.08)

  gainNode.gain.setValueAtTime(0.3, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.08)

  oscillator.connect(gainNode)
  gainNode.connect(ensureMasterGain(audioContext))

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.08)
}

export function playWinSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  if (audioContext.currentTime - lastWinSoundTime < RESULT_SOUND_INTERVAL) {
    return
  }
  lastWinSoundTime = audioContext.currentTime
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
    gainNode.connect(ensureMasterGain(audioContext))

    oscillator.start(audioContext.currentTime + index * 0.15)
    oscillator.stop(audioContext.currentTime + index * 0.15 + 0.3)
  })
}

export function playLoseSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  if (audioContext.currentTime - lastLoseSoundTime < RESULT_SOUND_INTERVAL) {
    return
  }
  lastLoseSoundTime = audioContext.currentTime
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
    gainNode.connect(ensureMasterGain(audioContext))

    oscillator.start(audioContext.currentTime + index * 0.2)
    oscillator.stop(audioContext.currentTime + index * 0.2 + 0.4)
  })
}

export function playGroundBounceSound(
  audioContextRef: React.MutableRefObject<AudioContext | null>,
  options?: { pan?: number; intensity?: number },
): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()
  const intensity = options?.intensity ?? 0.5
  const destination = createPannedDestination(audioContext, options?.pan)

  oscillator.type = "sine"
  oscillator.frequency.setValueAtTime(withVariation(80, 0.1), audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(withVariation(40, 0.1), audioContext.currentTime + 0.1)

  const startGain = clampVolume(0.12 + intensity * 0.2)
  gainNode.gain.setValueAtTime(startGain, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.1)

  oscillator.connect(gainNode)
  gainNode.connect(destination.node)

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.1)
  oscillator.addEventListener("ended", () => {
    try {
      gainNode.disconnect()
    } catch {}
    destination.dispose()
  })
}

export function playRestartSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "square"
  oscillator.frequency.setValueAtTime(800, audioContext.currentTime)
  oscillator.frequency.exponentialRampToValueAtTime(400, audioContext.currentTime + 0.05)

  gainNode.gain.setValueAtTime(0.2, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.05)

  oscillator.connect(gainNode)
  gainNode.connect(ensureMasterGain(audioContext))

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.05)
}

export function playSelectSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()

  oscillator.type = "sine"
  oscillator.frequency.setValueAtTime(600, audioContext.currentTime)

  gainNode.gain.setValueAtTime(0.15, audioContext.currentTime)
  gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.08)

  oscillator.connect(gainNode)
  gainNode.connect(ensureMasterGain(audioContext))

  oscillator.start()
  oscillator.stop(audioContext.currentTime + 0.08)
}

export function playStartSound(audioContextRef: React.MutableRefObject<AudioContext | null>): void {
  const audioContext = initAudioContext(audioContextRef)
  if (!audioContext) return
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
    gainNode.connect(ensureMasterGain(audioContext))

    oscillator.start(audioContext.currentTime + index * 0.08)
    oscillator.stop(audioContext.currentTime + index * 0.08 + 0.15)
  })
}
