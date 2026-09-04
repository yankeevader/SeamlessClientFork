# Alpha 1 staging matrix

Record the client log around each test. A pass requires both the visible behavior and the TransferSafety journal result.

| Case | Expected client route | Respawn expectation | TransferSafety expectation |
|---|---|---|---|
| Earth -> Open Space, alive | Seamless | Destination options after later death | Destination VERIFIED before source cleanup |
| Open Space -> Earth, alive | Seamless | Earth-only pods/options | Destination VERIFIED before source cleanup |
| Sector -> separate Lobby with different mods | Normal loading | Lobby-only pods/options | Normal Nexus transfer remains authoritative |
| Separate Lobby -> sector with different mods | Normal loading | Destination-world pods/options | Normal Nexus transfer remains authoritative |
| Sector -> integrated/flyable Lobby with same mods | Seamless | Integrated world's destination options | Destination VERIFIED before source cleanup |
| Sector -> sector, player already dead | Seamless only if Nexus sends a supported transfer | Destination respawn UI only | No premature source cleanup |
| Destination rejects join | Automatic normal-join fallback | Destination owns respawn after load | Failed seamless attempt must not authorize cleanup |
| P2P connection failure | Automatic normal-join fallback | Destination owns respawn after load | Failed seamless attempt must not authorize cleanup |
| Missing destination compatibility data | Normal loading (fail safe) | Destination owns respawn after load | No guessed seamless route |

Also verify character identity, body association, controlled grid/seat, toolbar, build colors, faction/economy visibility, GPS, chat, voxel/planet rendering and procedural asteroids on every seamless case.
