export type BeamableConfig = {
  cid: string;
  pid: string;
  email?: string;
};

function getEnv(key: string, fallbackKey?: string): string | undefined {
  const v = process.env[key];
  if (v && v.length > 0) return v;
  if (fallbackKey) {
    const f = process.env[fallbackKey];
    if (f && f.length > 0) return f;
  }
  return undefined;
}

export function readBeamableConfig(): BeamableConfig {
  const cid =
    getEnv("NEXT_PUBLIC_BEAMABLE_CID", "BEAMABLE_CID") ?? "";
  const pid =
    getEnv("NEXT_PUBLIC_BEAMABLE_PID", "BEAMABLE_PID") ?? "";
  const email = getEnv("NEXT_PUBLIC_BEAMABLE_EMAIL", "BEAMABLE_EMAIL");

  if (!cid) {
    throw new Error(
      "Beamable config missing CID. Set NEXT_PUBLIC_BEAMABLE_CID or BEAMABLE_CID in environment."
    );
  }
  if (!pid) {
    throw new Error(
      "Beamable config missing PID. Set NEXT_PUBLIC_BEAMABLE_PID or BEAMABLE_PID in environment."
    );
  }

  return { cid, pid, email };
}

export const beamableConfig: BeamableConfig = readBeamableConfig();

