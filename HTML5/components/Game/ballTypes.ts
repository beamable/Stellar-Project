/**
 * Ball type configurations for Tower Destroyer.
 * These serve as defaults when server content is unavailable.
 */

import type { BallType, BallTypeConfig } from "./types"

export const BALL_TYPES: BallTypeConfig[] = [
  {
    type: "normal",
    name: "Normal Ball",
    icon: "O",
    description: "Standard ball with balanced physics and collision",
    color: "#8B4513",
    baseSpeedMultiplier: 1.25,
  },
  {
    type: "multishot",
    name: "Multishot",
    icon: "*",
    description: "Splits into 3 balls with different angles",
    color: "#FF6B35",
    baseSpeedMultiplier: 1.15,
  },
  {
    type: "fire",
    name: "Fire Ball",
    icon: "F",
    description: "Passes through towers for 5 hits, then becomes normal",
    color: "#FF4500",
    baseSpeedMultiplier: 1.15,
  },
  {
    type: "laser",
    name: "Laser Ball",
    icon: "L",
    description: "Shoots 2 lasers that can destroy 3 towers each",
    color: "#8A2BE2",
    baseSpeedMultiplier: 0.85,
  },
]

export const BALL_TYPE_MAP: Record<BallType, BallTypeConfig> = BALL_TYPES.reduce(
  (acc, config) => {
    acc[config.type] = config
    return acc
  },
  {} as Record<BallType, BallTypeConfig>,
)

export const DEFAULT_BALL_TYPE_MAP = BALL_TYPE_MAP
