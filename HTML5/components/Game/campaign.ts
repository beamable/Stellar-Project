/**
 * Campaign data and metadata for Tower Destroyer.
 * Defines the chapter, stage flow, boss encounter, and mechanic unlock text.
 */

import type { WindZone } from "./types"

export type MechanicTag = "tower_density" | "shielded_blocks" | "moving_targets" | "wind_gusts" | "boss_phase"

export type MechanicDefinition = {
  id: MechanicTag
  title: string
  description: string
}

export const MECHANICS: Record<MechanicTag, MechanicDefinition> = {
  tower_density: {
    id: "tower_density",
    title: "Tower Clusters",
    description: "Heavier layouts force precise shots. Expect tight groupings and fewer breather lanes.",
  },
  shielded_blocks: {
    id: "shielded_blocks",
    title: "Shielded Blocks",
    description: "Reinforced towers that require multiple hits. Line up consecutive shots to crack the shell.",
  },
  moving_targets: {
    id: "moving_targets",
    title: "Moving Targets",
    description: "Oscillating platforms that sweep the lane. Track their rhythm before releasing the ball.",
  },
  wind_gusts: {
    id: "wind_gusts",
    title: "Wind Gusts",
    description: "Directional push zones that subtly bend trajectories. Watch particle cues to time your shot.",
  },
  boss_phase: {
    id: "boss_phase",
    title: "Boss Phases",
    description: "Multi-stage encounter with unique shields and reinforcements. Expect the full mechanic stack.",
  },
}

export type TowerProfile = {
  minTowers: number
  maxTowers: number
  specialBlockRatio: number
  ballCount: number
  movingTargetChance?: number
  movingAmplitude?: number
  movingSpeed?: number
  windZones?: WindZone[]
  bossPhases?: number
}

export type CampaignStage = {
  id: string
  chapterId: string
  order: number
  name: string
  summary: string
  objective: string
  type: "standard" | "boss"
  mechanics: MechanicTag[]
  towerProfile: TowerProfile
}

const CHAPTER_ID = "chapter-one"

export const CAMPAIGN_STAGES: CampaignStage[] = [
  {
    id: "stage-01",
    chapterId: CHAPTER_ID,
    order: 0,
    name: "Launch Bay",
    summary: "Baseline firing range that calibrates aim and pacing.",
    objective: "Clear the scattered outposts with 10 balls.",
    type: "standard",
    mechanics: [],
    towerProfile: {
      minTowers: 18,
      maxTowers: 22,
      specialBlockRatio: 0.05,
      ballCount: 10,
    },
  },
  {
    id: "stage-02",
    chapterId: CHAPTER_ID,
    order: 1,
    name: "Frontier Ridge",
    summary: "Adds second firing lane and mild tower clusters.",
    objective: "Manage 24+ towers without wasting ammo.",
    type: "standard",
    mechanics: ["tower_density"],
    towerProfile: {
      minTowers: 22,
      maxTowers: 28,
      specialBlockRatio: 0.08,
      ballCount: 10,
    },
  },
  {
    id: "stage-03",
    chapterId: CHAPTER_ID,
    order: 2,
    name: "Crumbling Canyon",
    summary: "Tighter cluster curve with cover walls.",
    objective: "Use bounce shots to break into compact stacks.",
    type: "standard",
    mechanics: ["tower_density"],
    towerProfile: {
      minTowers: 26,
      maxTowers: 32,
      specialBlockRatio: 0.12,
      ballCount: 11,
    },
  },
  {
    id: "stage-04",
    chapterId: CHAPTER_ID,
    order: 3,
    name: "Shield Array",
    summary: "Introduces reinforced towers that need double taps.",
    objective: "Break shield cores before reinforcements spawn.",
    type: "standard",
    mechanics: ["shielded_blocks"],
    towerProfile: {
      minTowers: 28,
      maxTowers: 34,
      specialBlockRatio: 0.18,
      ballCount: 12,
    },
  },
  {
    id: "stage-05",
    chapterId: CHAPTER_ID,
    order: 4,
    name: "Lockdown Plaza",
    summary: "Shielded blocks now guard key choke points.",
    objective: "Chain shots to punch through the locked lattice.",
    type: "standard",
    mechanics: ["shielded_blocks", "tower_density"],
    towerProfile: {
      minTowers: 32,
      maxTowers: 36,
      specialBlockRatio: 0.22,
      ballCount: 12,
    },
  },
  {
    id: "stage-06",
    chapterId: CHAPTER_ID,
    order: 5,
    name: "Hover Convoy",
    summary: "First wave of moving targets with gentle sway.",
    objective: "Track oscillating towers and land mid-flight hits.",
    type: "standard",
    mechanics: ["moving_targets"],
    towerProfile: {
      minTowers: 30,
      maxTowers: 38,
      specialBlockRatio: 0.2,
      ballCount: 13,
      movingTargetChance: 0.25,
      movingAmplitude: 35,
      movingSpeed: 0.002,
    },
  },
  {
    id: "stage-07",
    chapterId: CHAPTER_ID,
    order: 6,
    name: "Shielded Caravan",
    summary: "Mix of moving platforms and shield bricks.",
    objective: "Disable escorts while timing shots between passes.",
    type: "standard",
    mechanics: ["moving_targets", "shielded_blocks"],
    towerProfile: {
      minTowers: 34,
      maxTowers: 42,
      specialBlockRatio: 0.24,
      ballCount: 14,
      movingTargetChance: 0.4,
      movingAmplitude: 40,
      movingSpeed: 0.0022,
    },
  },
  {
    id: "stage-08",
    chapterId: CHAPTER_ID,
    order: 7,
    name: "Gust Corridor",
    summary: "Deterministic wind lanes nudge long shots.",
    objective: "Ride the gust columns to reach backline targets.",
    type: "standard",
    mechanics: ["wind_gusts"],
    towerProfile: {
      minTowers: 36,
      maxTowers: 44,
      specialBlockRatio: 0.2,
      ballCount: 14,
      movingTargetChance: 0.2,
      movingAmplitude: 32,
      movingSpeed: 0.0018,
      windZones: [
        { id: "stage8-gust-a", xStart: 520, xEnd: 660, yStart: 60, yEnd: 240, force: 0.18, pulseMs: 4200 },
        { id: "stage8-gust-b", xStart: 760, xEnd: 880, yStart: 120, yEnd: 320, force: -0.16, pulseMs: 5200, phase: Math.PI / 2 },
      ],
    },
  },
  {
    id: "stage-09",
    chapterId: CHAPTER_ID,
    order: 8,
    name: "Storm Citadel",
    summary: "Wind, shields, and motion layered together.",
    objective: "Adapt to rotating modifiers without losing tempo.",
    type: "standard",
    mechanics: ["wind_gusts", "moving_targets", "shielded_blocks"],
    towerProfile: {
      minTowers: 40,
      maxTowers: 48,
      specialBlockRatio: 0.26,
      ballCount: 15,
      movingTargetChance: 0.45,
      movingAmplitude: 42,
      movingSpeed: 0.0024,
      windZones: [
        { id: "stage9-gust-a", xStart: 500, xEnd: 640, yStart: 80, yEnd: 260, force: 0.2, pulseMs: 3600 },
        { id: "stage9-gust-b", xStart: 700, xEnd: 840, yStart: 100, yEnd: 260, force: -0.22, pulseMs: 4900, phase: Math.PI },
        { id: "stage9-gust-c", xStart: 880, xEnd: 1020, yStart: 140, yEnd: 320, force: 0.18, pulseMs: 4200, phase: Math.PI / 3 },
      ],
    },
  },
  {
    id: "stage-10",
    chapterId: CHAPTER_ID,
    order: 9,
    name: "Falling Sky",
    summary: "Final gauntlet before the boss strike.",
    objective: "Perfect runs rewarded with extra loop bonuses.",
    type: "standard",
    mechanics: ["tower_density", "wind_gusts", "moving_targets"],
    towerProfile: {
      minTowers: 44,
      maxTowers: 50,
      specialBlockRatio: 0.28,
      ballCount: 16,
      movingTargetChance: 0.5,
      movingAmplitude: 48,
      movingSpeed: 0.0026,
      windZones: [
        { id: "stage10-gust-a", xStart: 520, xEnd: 680, yStart: 60, yEnd: 220, force: 0.22, pulseMs: 3300 },
        { id: "stage10-gust-b", xStart: 760, xEnd: 900, yStart: 120, yEnd: 320, force: -0.24, pulseMs: 4700, phase: Math.PI / 4 },
        { id: "stage10-gust-c", xStart: 940, xEnd: 1080, yStart: 80, yEnd: 260, force: 0.2, pulseMs: 4100, phase: Math.PI / 1.5 },
      ],
    },
  },
  {
    id: "stage-11",
    chapterId: CHAPTER_ID,
    order: 10,
    name: "Obelisk Prime",
    summary: "Boss encounter with rotating shield plates and escorts.",
    objective: "Survive three boss phases to reset the loop.",
    type: "boss",
    mechanics: ["boss_phase", "shielded_blocks", "moving_targets", "wind_gusts"],
    towerProfile: {
      minTowers: 48,
      maxTowers: 60,
      specialBlockRatio: 0.32,
      ballCount: 18,
      movingTargetChance: 0.55,
      movingAmplitude: 52,
      movingSpeed: 0.0028,
      windZones: [
        { id: "boss-gust-a", xStart: 520, xEnd: 660, yStart: 60, yEnd: 300, force: 0.24, pulseMs: 3000 },
        { id: "boss-gust-b", xStart: 700, xEnd: 840, yStart: 100, yEnd: 320, force: -0.26, pulseMs: 4300, phase: Math.PI / 2 },
        { id: "boss-gust-c", xStart: 900, xEnd: 1040, yStart: 80, yEnd: 280, force: 0.22, pulseMs: 3600, phase: Math.PI },
      ],
      bossPhases: 3,
    },
  },
]

export const CAMPAIGN_STAGE_MAP = new Map(CAMPAIGN_STAGES.map((stage) => [stage.id, stage]))
export const DEFAULT_STAGE_ID = CAMPAIGN_STAGES[0]?.id ?? "stage-01"
