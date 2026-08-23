# 🗡️ EasyRPG

## Project Summary

EasyRPG is a dynamic, UI-driven Tactical RPG that blends deep strategic management with engaging text-based storytelling and minigames. Build your hero, sell your loot, explore uncharted territories, and conquer challenging PvE encounters in a lightweight yet immersive world. `In Development since 2025 Q3`


## ✨ Key Features

* 📜 Event-Driven Adventure System: Explore dynamic locations with rich storytelling choices, interactive minigames, randomized item drops, and tactical PvE battles.

* 🛡️ Deep Class & Stat Customization: Shape your hero's destiny. Choose your class and strategically distribute stats to match your unique playstyle.

* ⚔️ Vast Arsenal & Crafting: Equipment upgrade system paired with a massive variety of weapons, armor, and enemies to defeat.

* ⚖️ Dynamic Markets: Buy & Sell various items to profit.


* 🎒 Optimized Inventory Management: Clean, responsive, and high-performance inventory architecture built for seamless item sorting and equipping.

* 🗺️ Unlocking World Progression: Discover and unlock diverse new regions as your power grows.

* 📊 Comprehensive Analytics & Stats: Track your progress with detailed player statistics and combat analytics.

* 🎯 Quest System: (In Active Development) Dynamic objectives and rewarding questlines coming soon!



## 🏗️ Architecture & Technical Highlights

This project evolved from a console-based C# RPG into a decoupled, Event-Driven Unity UI game.


### 🌟 Key Design Patterns & Systems

* **State Pattern (Menu & UI Flow):** UI screens are managed dynamically via an `IMenuState` interface mapped through a `Dictionary<GameState, IMenuState>` inside the `GameManager`, eliminating massive conditional blocks.
* **Event-Driven Architecture (Observer Pattern):** Loose coupling between UI and Core logic is achieved through static event groups (`HeroEvents`, `CombatEvents`). UI components like `RightSectionManager` update instantly upon state changes without direct reference to data models.
* **Pure C# Core (POCOs):** Game entities (`Hero`, `Item`, `Entity`) are written in pure C# completely detached from Unity’s engine/MonoBehaviour dependencies to ensure clean separation of concerns.
* **Dependency Inversion:** Data persistence leverages an `ISaveService` interface for modularity and easy swap between save managers (e.g., JSON via `DataManager`).

---

## 📁 Project Structure `Scripts/`

* `Core/` — Pure C# game models, logic & math calculations.
* `Events/` — `EventManager` handling decoupled system communication.
* `States/` — Concrete `IMenuState` implementations for screen flows.
* `Managers/` — Domain managers (`CombatManager`, `InventoryManager`, `DataManager`).
* `UI/` — View layer and display components listening to system events.

## 📽️ Gameplay GIFs
<img width="831" height="468" alt="ss" src="https://github.com/user-attachments/assets/0a7de1a2-0b81-4830-a764-cb1a100844b0" />

---

## ✨ Special thanks
* Special thanks to [@Meenic](https://github.com/Meenic) for design tips and more.
