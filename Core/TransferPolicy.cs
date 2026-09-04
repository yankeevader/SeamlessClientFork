using SeamlessClient.Messages;
using SeamlessClient.OnlinePlayersWindow;
using Sandbox.Game.World;
using System.Collections.Generic;
using System.Linq;
using VRage.Game;
using System;

namespace SeamlessClient.Core
{
    internal enum TransferRoute
    {
        SeamlessSector,
        NormalLobby
    }

    internal sealed class TransferDecision
    {
        public TransferRoute Route { get; }
        public string Reason { get; }

        public TransferDecision(TransferRoute route, string reason)
        {
            Route = route;
            Reason = reason;
        }
    }

    internal static class TransferPolicy
    {
        public static TransferDecision Decide(TransferData transfer, MyObjectBuilder_World targetWorld)
        {
            string sourceName = PlayersWindowComponent.CurrentServerName;
            if (string.IsNullOrWhiteSpace(sourceName))
                sourceName = MySession.Static?.Name;
            string targetName = transfer?.ServerName;

            if (targetWorld?.Checkpoint == null || MySession.Static == null)
                return new TransferDecision(TransferRoute.NormalLobby,
                    "world compatibility data is unavailable; using a normal load");

            if (!HaveSameMods(MySession.Static.Mods, targetWorld.Checkpoint.Mods))
                return new TransferDecision(TransferRoute.NormalLobby,
                    $"mod cohort differs for {Describe(sourceName)} -> {Describe(targetName)}");

            return new TransferDecision(TransferRoute.SeamlessSector,
                $"compatible world transfer {Describe(sourceName)} -> {Describe(targetName)}");
        }

        private static bool HaveSameMods(
            IEnumerable<MyObjectBuilder_Checkpoint.ModItem> source,
            IEnumerable<MyObjectBuilder_Checkpoint.ModItem> target)
        {
            string[] sourceKeys = (source ?? Enumerable.Empty<MyObjectBuilder_Checkpoint.ModItem>())
                .Select(ModKey).OrderBy(x => x, StringComparer.Ordinal).ToArray();
            string[] targetKeys = (target ?? Enumerable.Empty<MyObjectBuilder_Checkpoint.ModItem>())
                .Select(ModKey).OrderBy(x => x, StringComparer.Ordinal).ToArray();

            return sourceKeys.SequenceEqual(targetKeys, StringComparer.Ordinal);
        }

        private static string ModKey(MyObjectBuilder_Checkpoint.ModItem mod)
        {
            return $"{mod.PublishedServiceName ?? string.Empty}:{mod.PublishedFileId}:{mod.Name ?? string.Empty}";
        }

        private static string Describe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "unknown" : $"'{value}'";
        }
    }
}
