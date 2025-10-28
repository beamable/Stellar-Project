/**
 * Game constants for Tower Destroyer
 * All magic numbers are defined here for easy tuning
 */

// Canvas dimensions
export const CANVAS_WIDTH = 1200
export const CANVAS_HEIGHT = 600
export const GROUND_Y = CANVAS_HEIGHT - 50

// Physics constants
export const GRAVITY = 0.3
export const BOUNCE_DAMPING = 0.7
export const FRICTION = 0.9
export const VELOCITY_THRESHOLD = 0.5
export const BOUNCE_VELOCITY_THRESHOLD = 2

// Ball shooting constants
export const MAX_POWER = 100
export const POWER_INCREMENT = 2
export const SHOT_FORCE_MULTIPLIER = 15

// Tower generation constants
export const MIN_TOWERS = 20
export const MAX_TOWERS = 60
export const SPECIAL_BLOCK_PERCENTAGE_LOW = 0.1
export const SPECIAL_BLOCK_PERCENTAGE_HIGH = 0.2
export const TOWER_THRESHOLD_FOR_HIGH_SPECIAL = 35
export const BALLS_FOR_LOW_TOWER_COUNT = 10
export const BALLS_FOR_HIGH_TOWER_COUNT = 20

// Laser constants
export const LASER_CREATION_HEIGHT = 480
export const LASER_CREATION_DELAY_MS = 100
export const LASER_COUNT = 2
export const LASER_MIN_LENGTH = 120
export const LASER_MAX_LENGTH = 200
export const LASER_SPEED = 3.472875
export const LASER_MAX_HITS = 3

// Multishot constants
export const MULTISHOT_ANGLES = [-0.3, 0, 0.3]
export const MULTISHOT_BALL_RADIUS = 10

// Fire ball constants
export const FIRE_BALL_DESTROY_THRESHOLD = 5

// Particle constants
export const PARTICLE_GRAVITY = 0.2
export const PARTICLE_COUNT_NORMAL = 8
export const PARTICLE_COUNT_MULTISHOT = 12
export const PARTICLE_COUNT_FIRE = 15
export const PARTICLE_COUNT_LASER = 10
export const PARTICLE_COUNT_LASER_DESTRUCTION = 20
export const PARTICLE_COUNT_WIN = 100
export const PARTICLE_COUNT_LOSE = 50

// Collision constants
export const COLLISION_PUSH_FORCE = 3
export const SPECIAL_BLOCK_BOUNCE_STRENGTH = 0.8

// Scoring constants
export const POINTS_NORMAL_TOWER = 100
export const POINTS_SPECIAL_TOWER = 200
export const POINTS_SPECIAL_TOWER_FIRST_HIT = 50
export const VICTORY_BONUS_MULTIPLIER = 0.1

// Tower colors
export const TOWER_COLORS = ["#8B4513", "#A0522D", "#CD853F", "#DEB887", "#F4A460"]
export const SPECIAL_TOWER_COLORS = ["#654321", "#5D4037", "#3E2723"]

// Particle colors
export const FIRE_COLORS = ["#FF4500", "#FF6347", "#FFA500", "#FFD700", "#FF8C00"]
export const LASER_COLORS = ["#8A2BE2", "#9370DB", "#BA55D3", "#00FFFF", "#4169E1"]
export const MULTISHOT_COLORS = ["#FF6B35", "#FF8C42", "#FFA07A", "#FF7F50"]
export const WIN_COLORS = ["#FFD700", "#FFA500", "#FF69B4", "#00FF00", "#00FFFF", "#FF1493"]
export const LOSE_COLORS = ["#808080", "#696969", "#A9A9A9", "#778899"]
