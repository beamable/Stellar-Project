"use client"

import { useEffect } from "react"

export function usePlayerInfoInventory({
  showPlayerInfo,
  onInventoryRefresh,
}: {
  showPlayerInfo: boolean
  onInventoryRefresh: () => void
}) {
  useEffect(() => {
    if (showPlayerInfo) {
      onInventoryRefresh()
    }
  }, [showPlayerInfo, onInventoryRefresh])
}

export default usePlayerInfoInventory
