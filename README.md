# procedural-dungeons
A Mystery Dungeon inspired project made in Godot 4.2

Current features:

- Basic floor generation algorithm based on Binary Space Partioning
- Custom blob autotiling system (2 terrains, optionally supports tile variants)
- Movement & animation handled through composition
- Field of view implementation based on https://www.albertford.com/shadowcasting/
- Minimap that updates itself based on the player's field of view
- NPC pathfinding using the A* algorithm that follows the player when visible
