/**
 * Game constants for Tower Destroyer
 * All magic numbers are defined here for easy tuning
 */

// Canvas dimensions
export const CANVAS_WIDTH = 1200 // total width of the play area
export const CANVAS_HEIGHT = 400 // total height of the play area
export const GROUND_Y = CANVAS_HEIGHT - 50 // y-position where the ground sits

// Physics constants
export const GRAVITY = 0.3 // per-frame downward acceleration
export const BOUNCE_DAMPING = 0.7 // velocity retained after a bounce
export const FRICTION = 0.9 // horizontal slowdown each frame
export const VELOCITY_THRESHOLD = 0.5 // cutoff for considering a ball "settled"
export const BOUNCE_VELOCITY_THRESHOLD = 2 // minimum impact to trigger bounce SFX

// Ball shooting constants
export const MAX_POWER = 100 // cap on charge meter
export const POWER_INCREMENT = 2 // charge gain per timestep
export const SHOT_FORCE_MULTIPLIER = 15 // converts power to actual velocity

// Tower generation constants
export const MIN_TOWERS = 20 // smallest tower count per round
export const MAX_TOWERS = 60 // largest tower count per round
export const SPECIAL_BLOCK_PERCENTAGE_LOW = 0.1 // special block ratio for small fields
export const SPECIAL_BLOCK_PERCENTAGE_HIGH = 0.2 // special block ratio for large fields
export const TOWER_THRESHOLD_FOR_HIGH_SPECIAL = 35 // tower count that flips to high special ratio
export const BALLS_FOR_LOW_TOWER_COUNT = 10 // shots the player gets on smaller layouts
export const BALLS_FOR_HIGH_TOWER_COUNT = 20 // shots when tower count is high

// Laser constants
export const LASER_CREATION_HEIGHT = 480 // y-position above which laser balls spawn lasers
export const LASER_CREATION_DELAY_MS = 100 // delay after shooting before lasers appear
export const LASER_COUNT = 2 // number of lasers per activation
export const LASER_MIN_LENGTH = 120 // shortest laser beam length
export const LASER_MAX_LENGTH = 150 // longest laser beam length
export const LASER_SPEED = 4 // travel speed of laser projectiles
export const LASER_MAX_HITS = 3 // towers a single laser can destroy
export const LASER_COLLISION_OFFSET = 0.85 // fraction along the beam used for collision probing

// Multishot constants
export const MULTISHOT_ANGLES = [-0.3, 0, 0.3] // angular offsets for the three shots
export const MULTISHOT_BALL_RADIUS = 10 // radius override for multishot balls

// Fire ball constants
export const FIRE_BALL_DESTROY_THRESHOLD = 5 // number of towers a fireball pierces before reverting

// Particle constants
export const PARTICLE_GRAVITY = 0.2 // gravity applied to particles
export const PARTICLE_COUNT_NORMAL = 8 // debris spawned on normal tower destruction
export const PARTICLE_COUNT_MULTISHOT = 12 // particles for multishot FX
export const PARTICLE_COUNT_FIRE = 15 // fireball impact particles
export const PARTICLE_COUNT_LASER = 10 // particles spawned by laser impact
export const PARTICLE_COUNT_LASER_DESTRUCTION = 20 // particles when a laser dissipates
export const PARTICLE_COUNT_WIN = 100 // celebratory particles on victory
export const PARTICLE_COUNT_LOSE = 50 // particles on defeat

// Collision constants
export const COLLISION_PUSH_FORCE = 3 // impulse added to balls after tower hits
export const SPECIAL_BLOCK_BOUNCE_STRENGTH = 0.8 // extra bounce from special tower blocks

// Scoring constants
export const POINTS_NORMAL_TOWER = 100 // base points per normal tower
export const POINTS_SPECIAL_TOWER = 200 // total points for clearing a special tower
export const POINTS_SPECIAL_TOWER_FIRST_HIT = 50 // bonus for the first hit on specials
export const VICTORY_BONUS_MULTIPLIER = 0.1 // multiplier applied to leftover score after a win

// Tower colors
export const TOWER_COLORS = ["#8B4513", "#A0522D", "#CD853F", "#DEB887", "#F4A460"] // palette for normal towers
export const SPECIAL_TOWER_COLORS = ["#654321", "#5D4037", "#3E2723"] // palette for special towers

// Particle colors
export const FIRE_COLORS = ["#FF4500", "#FF6347", "#FFA500", "#FFD700", "#FF8C00"] // palette for fire FX
export const LASER_COLORS = ["#8A2BE2", "#9370DB", "#BA55D3", "#00FFFF", "#4169E1"] // palette for laser FX
export const MULTISHOT_COLORS = ["#FF6B35", "#FF8C42", "#FFA07A", "#FF7F50"] // palette for multishot FX
export const WIN_COLORS = ["#FFD700", "#FFA500", "#FF69B4", "#00FF00", "#00FFFF", "#FF1493"] // palette on victory
export const LOSE_COLORS = ["#808080", "#696969", "#A9A9A9", "#778899"] // palette on defeat
