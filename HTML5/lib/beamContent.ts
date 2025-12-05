import type { Beam, ContentBase } from "beamable-sdk"
import getBeam from "@/lib/beam"
import { debugLog } from "@/lib/debugLog"

/**
 * Beam content helpers.
 * ContentService usage docs: C:\Beamable\Repos\Downloads\BeamWebSdk-htmlPages\ContentService - Web SDK.html
 */
export type BallContent = ContentBase<{
  name?: string
  description?: string
  image?: string
  customProperties?: Record<string, string>
}> & { contentId?: string }

const BALL_CONTENT_TYPE = "ball"

let cachedBalls: BallContent[] | null = null
let inFlight: Promise<BallContent[]> | null = null

const getContentService = (beam: Beam) => {
  const service = (beam as any).content ?? (beam as any).clientServices?.content
  if (!service?.getByType) {
    throw new Error("Beam ContentService is unavailable. Ensure clientServices(beam) has been registered.")
  }
  return service as {
    getByType: (params: { type: string; manifestId?: string }) => Promise<unknown[]>
  }
}

export async function resolveBallContent(): Promise<BallContent[]> {
  if (cachedBalls) return cachedBalls
  if (inFlight) return inFlight

  inFlight = (async () => {
    debugLog("[BeamContent] Resolving ball content...")
    const beam = await getBeam()
    const contentService = getContentService(beam as Beam)
    const balls = (await contentService.getByType({
      type: BALL_CONTENT_TYPE,
    })) as BallContent[]
    balls.forEach((ball) => {
      if (!ball.contentId && ball.id) {
        ball.contentId = ball.id
      }
    })
    debugLog("[BeamContent] Retrieved ball content count:", balls.length)
    cachedBalls = balls
    return balls
  })()

  try {
    return await inFlight
  } finally {
    debugLog("[BeamContent] Resolve cycle complete")
    inFlight = null
  }
}

export function getKnownBalls(): BallContent[] {
  return cachedBalls ?? []
}

export function resetBallContentCache() {
  debugLog("[BeamContent] Resetting ball content cache")
  cachedBalls = null
  inFlight = null
}
