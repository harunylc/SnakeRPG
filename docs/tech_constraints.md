# Technical Constraints & Guidelines

## Engine & Setup
*   **Engine**: Unity 2021.3.45f2 LTS.
*   **Language**: C#.
*   **Architecture**: MonoBehaviour-based only. **NO** ECS, DOTS, or Jobs.
*   **Scope**: Prototype-first, optimize later.

## Movement & Physics
*   **Movement**: Continuous Vector-based movement (Snake moves forward, player steers).
*   **Physics**: **NO** Rigidbody-based movement. Direct transform modification.
*   **Collision**: Simple trigger colliders only.

## Performance
*   **Pooling**: Standard object pooling for enemies and projectiles.
*   **Optimization**: Reasonable pooling, do not over-engineer.

## Locked Systems
*   **Snake Movement**: Continuous Vector-based (SnakeHeadController).
*   **Body Following**: History buffer with fixed spacing (SnakeBodyManager).
*   **Status**: FINAL. Do not modify logic.
