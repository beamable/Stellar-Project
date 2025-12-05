"use client"

import { useCallback, useEffect, useState } from "react"
import { getMasterVolume, setMasterVolume } from "@/components/Game/audio"

const VOLUME_STORAGE_KEY = "tower-destroyer-volume"

export type AudioSettingsState = {
  volume: number
  showAudioSettings: boolean
  onVolumeChange: (next: number) => void
  onToggleAudioSettings: () => void
  onCloseAudioSettings: () => void
}

export function useAudioSettings(): AudioSettingsState {
  const resolveInitialVolume = () => {
    if (typeof window === "undefined") return getMasterVolume()
    const stored = window.localStorage.getItem(VOLUME_STORAGE_KEY)
    const parsed = stored !== null ? Number.parseFloat(stored) : Number.NaN
    return Number.isFinite(parsed) ? Math.min(1, Math.max(0, parsed)) : getMasterVolume()
  }

  const [volume, setVolume] = useState<number>(() => resolveInitialVolume())
  const [showAudioSettings, setShowAudioSettings] = useState(false)

  useEffect(() => {
    setMasterVolume(volume)
  }, [volume])

  const handleVolumeChange = useCallback((next: number) => {
    const clamped = Math.min(1, Math.max(0, next))
    setVolume(clamped)
    if (typeof window !== "undefined") {
      try {
        window.localStorage.setItem(VOLUME_STORAGE_KEY, clamped.toString())
      } catch {
        // Ignore write failures
      }
    }
  }, [])

  const handleToggleAudioSettings = useCallback(() => {
    setShowAudioSettings((prev) => !prev)
  }, [])

  const handleCloseAudioSettings = useCallback(() => {
    setShowAudioSettings(false)
  }, [])

  return {
    volume,
    showAudioSettings,
    onVolumeChange: handleVolumeChange,
    onToggleAudioSettings: handleToggleAudioSettings,
    onCloseAudioSettings: handleCloseAudioSettings,
  }
}

export default useAudioSettings
