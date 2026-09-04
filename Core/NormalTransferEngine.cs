using Sandbox;
using Sandbox.Game.Gui;
using SeamlessClient.Messages;
using System;
using VRage.GameServices;

namespace SeamlessClient.Core
{
    internal static class NormalTransferEngine
    {
        public static void Start(MyGameServerItem server, TransferData transfer)
        {
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            Seamless.TryShow($"Starting normal Lobby transition to {server.Name} ({server.ConnectionString})");

            MySandboxGame.Static.Invoke(() =>
            {
                // This is Keen's normal join pipeline. It owns unloading the current
                // session, displaying the loading screen, loading the destination's
                // mod set, and constructing fresh multiplayer/session state.
                MyJoinGameHelper.JoinGame(server);
                TransferCoordinator.Instance.Complete();
            }, "SKPSeamless.NormalLobbyTransfer");
        }
    }
}
