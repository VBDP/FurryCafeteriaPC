# Position Sync example

## Description
A simple PlayerObject prefab which saves each player's position and rotation, then restores them when they rejoin the world.

## Inspector Parameters
* `string` **Synced Position key** - The key name for the position you want to use from PlayerData.
* `string` **Synced Rotation key** - The key name for the rotation you want to use from PlayerData.

---
## How to Use This Example
1. Add the `PositionSync` prefab to your scene.
2. Change the `Synced Position key` and `Synced Rotation key` to the PlayerData variable key you want to use for position syncing.
3. Run Build & Test!