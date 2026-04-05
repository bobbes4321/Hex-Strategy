# Unity Strategy & Runtime Patterns Testbed

This repository serves as both a portfolio piece for a grid-based strategy game prototype and a technical playground for exploring various Unity runtime patterns and architectural challenges.

## Technical Highlights

### Service Locator & Decoupled Architecture
The project utilizes a custom `ServiceLocator` to manage core systems like the `MessagingManager`, `GameManager`, and `ChessBoard`. This provides a centralized point for dependency resolution without the overhead of heavy DI frameworks, while still allowing for easier unit testing and cross-scene persistence.

### Reflection-Based Messaging System
To maintain a decoupled architecture, I implemented a `MessagingManager` that allows objects to publish and subscribe to strongly-typed messages (`IMessage`).
- **Dynamic Handler Resolution**: Uses reflection to automatically find and invoke the correct `Handle<T>` method on registered objects.
- **Ease of Use**: Components simply implement `IMessageHandler<T>` and register themselves with the manager.

### Flexible Data Binding
A custom reflection-based data binding system (`ReflectedValue<T>`) allows for dynamic linking between Unity `Component` fields/properties, `ScriptableObjects`, and other data sources.
- **Cached Reflection**: Optimizes performance by caching `PropertyInfo` and `FieldInfo` after the first lookup.
- **Transformers**: Supports value transformation during binding (e.g., `FloatToColorAlphaTransformer`, `RoundFloatTransformer`).
- **Versatility**: Useful for rapid UI prototyping where data sources might shift between different types of objects.

### Strategy Game Architecture
The core of the project is a grid-based strategy system designed with a strict separation between data, logic, and visualization.

#### Grid and Board Management
The `ChessBoard` class acts as the central data structure, managing a collection of `Tile` structs and `IPiece` references.
- **Grid Abstraction**: Logic is handled via a coordinate-based `Tile` system (X, Y), decoupled from Unity's world space.
- **Visual Separation**: `TileView` handles the 3D representation and user input, communicating back to the logic layer via static events and the messaging system.
- **State Management**: The board maintains the authoritative state of piece positions, providing methods for move validation and piece capture.

#### Piece Behavior and Extensibility
Units are defined through the `IPiece` interface, which favors composition over deep inheritance.
- **Component-Based Pieces**: Pieces can host multiple `IPieceComponent` instances (e.g., `Health`), allowing for modular behavior definition.
- **Move Generation**: Each piece is responsible for generating its own valid `Move` set based on current board state, enabling unique movement patterns for different unit types.
- **Null Object Pattern**: Implementation of `NullPiece` eliminates null checks throughout the codebase, leading to cleaner and safer move logic.

#### Move Execution Pipeline
The movement system is split between selection logic and execution:
- **StrategyMoveManager**: Orchestrates the user interaction flow—selecting a piece, highlighting valid moves via the board, and processing the subsequent tile selection.
- **Command Pattern**: Moves are encapsulated as `Move` structs containing the necessary logic (`MoveAction`) to execute the change. This allows for easy extension to support attacks, special abilities, or move undo/redo functionality.
- **Event-Driven Updates**: Upon successful move execution, a `MoveMessage` is published, allowing disparate systems (like UI or VFX) to react without direct coupling to the move manager.

### Additional Research Areas
- **UI Flow Management**: Exploration of state-driven UI navigation using `FlowManager` and integration with DoozyUI.
- **Auto-Referencing**: Implementation of attribute-based component gathering (`[Get]`) to reduce `GetComponent` boilerplate in `Awake`.
- **Odin Inspector Integration**: Heavy use of Odin attributes (like `[Button]`, `[HorizontalGroup]`) to create powerful, developer-friendly tooling within the Unity Editor.

## Purpose
This project is an ongoing exploration of how to build robust, maintainable, and decoupled systems in Unity. It prioritizes clean code patterns and architectural flexibility over being a finished game product.
