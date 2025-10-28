/**
 * Ball type configurations for Tower Destroyer
 */

import type { BallTypeConfig } from "./types"

export const BALL_TYPES: BallTypeConfig[] = [
  {
    type: "normal",
    name: "Normal Ball",
    icon: "⚪",
    description: "Standard ball with normal physics and collision",
    color: "#8B4513",
  },
  {
    type: "multishot",
    name: "Multishot",
    icon: "🔱",
    description: "Splits into 3 balls with different angles",
    color: "#FF6B35",
  },
  {
    type: "fire",
    name: "Fire Ball",
    icon: "🔥",
    description: "Passes through towers for 5 hits, then becomes normal",
    color: "#FF4500",
  },
  {
    type: "laser",
    name: "Laser Ball",
    icon: "⚡",
    description: "Shoots 2 lasers that can destroy 3 towers each",
    color: "#8A2BE2",
  },
]

