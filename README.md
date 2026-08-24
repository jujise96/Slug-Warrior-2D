# Slug Warrior 2D ⚔️

**Slug Warrior** is a 2D action-platformer built with Unity. Inspired by the fast-paced arcade style of titles like *Metal Slug*, it adapts the core mechanics into a medieval setting focused on sword-based melee combat.

Developed by **Juan Jiménez Serrano** and **Luis Miguel Pérez Martín** for the *Game Programming* course.

---

## 🎮 Key Features

* **2D Arcade Gameplay:** Play as a warrior featuring movement, jumping, sliding, and melee attack mechanics.
* **Custom AI Systems:**
  * **Main Boss (Knight):** Uses a Finite State Machine (FSM) driven by triggers and boolean logic to switch between waiting, advancing, blocking, or attacking based on distance, player health, and damage dealt.
  * **Flying Enemies (Bats):** Dynamic pathfinding and obstacle avoidance implemented using the A* Pathfinding Project.
* **Scene Management & Persistence:**
  * Options for audio volume, post-processing brightness adjustment (`Auto-Exposure`), and player life selection (3, 5, or 7 lives).
  * Data persistence across scenes using `DontDestroyOnLoad` and `PlayerPrefs`.
* **Leaderboard & Records:** Best time tracking and score leaderboard scene.

---

## 🛠️ Tech Stack & Tools

* **Engine:** Unity
* **Language:** C#
* **Physics & Collisions:** `Rigidbody2D`, `Physics2D.OverlapCircleAll`
* **AI & Navigation:** A* Pathfinding Project, Unity Animator (FSM)
* **Visuals:** 2D Pixel Art Sprites, Post-Processing Stack

---

## 📂 Code Architecture

Modular script organization across core systems:

* **Player & AI:** `PlayerMovement.cs`, `Enemigo.cs`, `BatController.cs`
* **Systems & UI:** `BrilloManager.cs`, `OpcionesMenu.cs`, `RecordsScript.cs`, `TimerRecord.cs`
* **Camera & Environment:** `CamaraScript.cs`
