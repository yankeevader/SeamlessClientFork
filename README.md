# SKP Seamless Client 3.1.12 alpha 3

This is a Nexus V3 client-plugin rewrite for mixed server clusters. It retains the proven in-session multiplayer handoff for compatible sector worlds and deliberately uses Space Engineers' normal join pipeline whenever a transfer touches the Lobby.

## Routing rules

- Source and destination have identical mod cohorts: seamless handoff, regardless of server name.
- Source and destination mod cohorts differ: normal loading transition.
- Missing world/mod compatibility data: normal loading transition (fail safe).
- A second transfer is rejected while one is active.
- A rejected seamless join, lost destination host, or P2P failure falls back to a normal join of the intended destination.

The current server identity comes from Nexus' `OnlinePlayers` message and is used for logging only. The word `Lobby` does not force a route. A Lobby built inside the compatible sector cohort can therefore switch seamlessly; a separate Lobby with a different mod set uses the normal loader.

## Respawn ownership

Respawn state belongs to the destination world. Before a seamless handoff the old medical screen is closed and removed. During destination checkpoint application, `MySpaceRespawnComponent` is initialized from the destination checkpoint and rebound to the player collection. Respawn pods are then supplied by the destination's replicated entities.

The previous plugin's global medical-screen patch was removed. It incorrectly renamed every suit-spawn row to `Nexus Lobby` and could not represent different respawn setups per world.

This means an already-living character follows Nexus' normal identity/body transfer. If the player subsequently dies, the respawn UI and available pods are those belonging to the destination world.

## Nexus V3 compatibility

- Network channel remains `2936`.
- Existing protobuf field numbers and wire models are unchanged.
- Assembly version is `3.1.12.0`, following the upstream `3.1.9` source and the packaged `3.1.7` client.
- Normal-load server items inherit the active multiplayer app version and game AppID; leaving the version at zero causes Keen's `Server version: 0` rejection.
- Nexus/TransferSafety remains authoritative for TransferID, journal, destination verification, source cleanup, identity and grid transfer. This client does not invent or acknowledge a parallel transfer protocol.

## Build

The project targets .NET Framework 4.8. By default it expects:

- game assemblies in `../GameBinaries/Bin64`
- Harmony at `../input/earth/0Harmony.dll`

Override MSBuild properties `GameBinPath` and `HarmonyPath` if your layout differs. Build `SeamlessClient.sln` in Release mode. Only `SeamlessClient.dll` is the plugin artifact; game and Harmony dependencies already supplied by Space Engineers/Nexus should not be bundled into a client release.

## Deployment and testing

This is an alpha build. Test on a staging clone before production:

1. Connect to a sector and cross to another sector while alive and controlling a character/grid.
2. Verify identity, body association, controlled entity, toolbar and shared GPS.
3. Die after arrival and confirm only the destination world's respawn pods/options appear.
4. Transfer sector -> Lobby and Lobby -> sector; both must show the normal loading transition and load the destination mod set.
5. Interrupt or reject a destination connection and confirm the normal-join fallback activates without source cleanup occurring ahead of TransferSafety's VERIFIED state.

Do not install this DLL on dedicated servers as a server plugin. It implements `VRage.Plugins.IPlugin` and runs in the Space Engineers client. Distribution through your Nexus-provided client plugin/mod-list mechanism still requires whatever loader Nexus uses to place and load client plugin DLLs; an ordinary workshop mod cannot load arbitrary client plugin assemblies by itself.
