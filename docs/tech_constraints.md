# Technical Constraints & Guidelines

## Engine & Setup
*   **Engine**: Unity 2021.3.45f2 LTS.
*   **Language**: C#.
*   **Architecture**: MonoBehaviour-based only. **NO** ECS, DOTS, or Jobs.
*   **Scope**: Prototype-first, optimize later.

## Movement & Physics
*   **Movement**: Grid-based, time-tick movement (Snake style).
*   **Physics**: **NO** Rigidbody-based movement.
*   **Collision**: Simple trigger colliders only.

## Performance
*   **Pooling**: Standard object pooling for enemies and projectiles.
*   **Optimization**: Reasonable pooling, do not over-engineer.
