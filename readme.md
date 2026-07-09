# Matatu Coin Rush — Complete Unity Game

## 🎮 Game Overview
Matatu Coin Rush is an endless runner game set in Nairobi, Kenya. Drive your matatu through the streets, collect coins, avoid obstacles, and learn amazing facts about Nairobi!

## 📋 Marking Points Covered

| # | Requirement | Implementation |
|---|-------------|----------------|
| 1 | Type of Game (Genre) | Endless Runner / Arcade |
| 2 | Rules of the Game | Score points, collect coins, avoid obstacles, use power-ups |
| 3 | Game Transition | Start/Game Over screens with smooth scene loading |
| 4 | Animation | Player movement, idle/run, jump, obstacle shake, pickup float |
| 5 | Game Physics | Lane movement, jumping, collision detection, speed ramping |
| 6 | Sound Effects/UI | Music, SFX for pickups/obstacles, Kenyan-themed UI |
| 7 | Particle Emitter | Trail, jump, coin collect, fact boost, obstacle hit effects |
| 8 | Game Scene | Nairobi-themed environment with buildings, trees, road |
| 9 | Game Interaction | Touch/keyboard controls, visual feedback, responsive gameplay |

## 🚀 Setup Instructions

### Step 1: Create Unity Project
1. Open Unity Hub → New Project → 3D Core
2. Name: `MatatuCoinRush`
3. Wait for project to load

### Step 2: Folder Structure
Create these folders:
Assets/
├── Scripts/
├── Models/
├── Materials/
├── Audio/
│ ├── Music/
│ └── SFX/
├── Prefabs/
├── Scenes/
└── Textures/


### Step 3: Import Scripts
1. Copy all `.cs` files to `Assets/Scripts/`
2. Wait for Unity to compile

### Step 4: Scene Setup
1. **Create Road**: GameObject → 3D Object → Plane
   - Scale: (20, 1, 100)
   - Position: (0, -0.5, 0)
   - Material: Dark gray (asphalt)

2. **Create Player**:
   - Import a matatu/vehicle model from Mixamo or Asset Store
   - OR use a simple cube with wheel cylinders
   - Position: (0, 0.5, -5)
   - Add `LaneRunnerPlayer` script
   - Add Box Collider (set as trigger)

3. **Add Lighting**:
   - Directional Light for sunlight
   - Warm, golden color (Nairobi sunset feel)

### Step 5: GameManager Setup
1. Create Empty → Name: `GameManager`
2. Add `GameManager` script
3. Add AudioSource component
4. Assign UI Text fields (use TextMeshPro)

### Step 6: UI Setup
Create Canvas with:
- Score display (top-left)
- Coin display (top-right)
- Best score (top-center)
- Boost timer (bottom-left)
- Start panel with "Start" button
- Game Over panel with "Restart" button
- Fact display (appears when collecting power-up)

### Step 7: Create Prefabs

**Coin Prefab**:
- Sphere or coin model
- Add `Pickup` script → Type: Coin
- Add Sphere Collider (isTrigger)
- Assign collect effect particle

**Fact Power-Up Prefab**:
- Star or book model
- Add `Pickup` script → Type: NairobiFact
- Add Sphere Collider (isTrigger)

**Obstacle Prefabs** (Nairobi-themed):
- Matatu (bus) - large obstacle
- Traffic cone - small obstacle
- Pothole - ground obstacle
- Add `Obstacle` script to all
- Add Box Collider (isTrigger)

### Step 8: RoadSpawner Setup
1. Create Empty → Name: `Spawner`
2. Add `RoadSpawner` script
3. Assign:
   - Coin Prefab
   - Fact Power-Up Prefab
   - Obstacle Prefabs list
   - Building/Tree prefabs for environment

### Step 9: Import Mixamo Character
1. Go to Mixamo.com
2. Download a character model (e.g., "Aiden" or "Remy")
3. Import to Unity
4. Set Animation Type to Humanoid
5. Create Animator Controller with:
   - Idle → Run transition
   - Run → Jump transition
   - Jump back to Run

### Step 10: Audio Setup
Import and assign:
- Background music (Afrobeat or Kenyan-inspired)
- Coin collect sound
- Power-up sound
- Obstacle hit sound
- Game over sound
- Boost activation sound

### Step 11: Particle Effects
Create particle systems for:
- Dust trail behind player
- Coin collection sparkle
- Fact power-up glow
- Obstacle explosion
- Speed boost effect

## 🎮 Controls

| Action | Keyboard | Mobile |
|--------|----------|--------|
| Move Left | Left Arrow / A | Swipe Left |
| Move Right | Right Arrow / D | Swipe Right |
| Jump | Space / Up Arrow / W | Swipe Up |

## 🏆 Game Features

- **Progressive Difficulty**: Speed increases over time
- **Coin Streaks**: Earn bonus points for consecutive coins
- **Nairobi Facts**: Collect power-ups to learn about Nairobi
- **Protection Mode**: Facts protect you from one obstacle
- **Score Multiplier**: Streaks multiply your score
- **Best Score**: Saved locally with PlayerPrefs

## 🎨 Visual Style

- Colorful matatu art style
- Warm Nairobi sunset lighting
- African-inspired UI fonts and colors
- Urban environment with buildings and trees

## 🔊 Audio Design

- Upbeat Afrobeat background music
- Satisfying coin collection sound
- Educational fact announcement sound
- Impact sounds for obstacles
- Boost activation sound effect

## 📱 Mobile Optimization

- Touch swipe controls
- Portrait mode recommended
- Responsive UI scaling
- Optimized particle systems
- Mobile-friendly font sizes

## 🐛 Troubleshooting

**Player not moving**: Check if GameManager is running
**Objects not spawning**: Verify prefabs are assigned
**No sound**: Check AudioSource and audio clips
**No animation**: Verify Animator Controller is assigned

 📝 Credits

- Game Design: [Your Name]
- Nairobi Facts: Research from various sources
- Sound Effects: [Free sound library credits]
- 3D Models: Mixamo, Unity Asset Store

