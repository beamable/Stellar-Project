"use client"

import { useEffect } from "react"

export function useReadyStateCleanup({
  readyForGame,
  onReset,
}: {
  readyForGame: boolean
  onReset: () => void
}) {
  useEffect(() => {
    if (!readyForGame) {
      onReset()
    }
  }, [readyForGame, onReset])
}

export default useReadyStateCleanup
