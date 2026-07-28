# EasyRPG
## 🏗️ Architecture & Technical Highlights

This project evolved from a console-based C# RPG into a decoupled, Event-Driven Unity UI game.

### 🌟 Key Design Patterns & Systems

* **State Pattern (Menu & UI Flow):** UI screens are managed dynamically via an `IMenuState` interface mapped through a `Dictionary<GameState, IMenuState>` inside the `GameManager`, eliminating massive conditional blocks.
* **Event-Driven Architecture (Observer Pattern):** Loose coupling between UI and Core logic is achieved through static event groups (`HeroEvents`, `CombatEvents`). UI components like `RightSectionManager` update instantly upon state changes without direct reference to data models.
* **Pure C# Core (POCOs):** Game entities (`Hero`, `Item`, `Entity`) are written in pure C# completely detached from Unity’s engine/MonoBehaviour dependencies to ensure clean separation of concerns.
* **Dependency Inversion:** Data persistence leverages an `ISaveService` interface for modularity and easy swap between save managers (e.g., JSON via `DataManager`).

---

## 📁 Project Structure

* `Core/` — Pure C# game models, logic & math calculations.
* `Events/` — `EventManager` handling decoupled system communication.
* `States/` — Concrete `IMenuState` implementations for screen flows.
* `Managers/` — Domain managers (`CombatManager`, `InventoryManager`, `DataManager`).
* `UI/` — View layer and display components listening to system events.
