using SeamlessClient.Messages;
using SeamlessClient.Components;
using System;
using VRage.GameServices;
using Sandbox.Engine.Multiplayer;
using Sandbox.Engine.Networking;

namespace SeamlessClient.Core
{
    internal enum TransferState
    {
        Idle,
        Dispatching,
        Seamless,
        Normal,
        Failed
    }

    internal sealed class TransferCoordinator
    {
        public static TransferCoordinator Instance { get; } = new TransferCoordinator();

        public TransferState State { get; private set; } = TransferState.Idle;
        public DateTime StartedUtc { get; private set; }

        private TransferCoordinator() { }

        public void Start(TransferData transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException(nameof(transfer));
            if (transfer.TargetServerId == 0)
                throw new InvalidOperationException("Nexus supplied target server ID 0.");
            if (string.IsNullOrWhiteSpace(transfer.IpAddress))
                throw new InvalidOperationException("Nexus supplied an empty target address.");
            if (transfer.WorldRequest == null || transfer.WorldRequest.WorldData == null)
                throw new InvalidOperationException("Nexus supplied no destination world data.");
            if (State != TransferState.Idle && State != TransferState.Failed)
                throw new InvalidOperationException($"A transfer is already active ({State}).");

            State = TransferState.Dispatching;
            StartedUtc = DateTime.UtcNow;

            var server = new MyGameServerItem
            {
                ConnectionString = transfer.IpAddress,
                SteamID = transfer.TargetServerId,
                Name = transfer.ServerName,
                AppID = MyGameService.AppId,
                ServerVersion = MyMultiplayer.Static?.AppVersion ?? 0,
                HadSuccessfulResponse = true
            };

            if (server.ServerVersion == 0)
                throw new InvalidOperationException("Unable to determine the current Space Engineers app version.");

            var world = transfer.WorldRequest.DeserializeWorldData();
            TransferDecision decision = TransferPolicy.Decide(transfer, world);
            Seamless.TryShow($"Transfer policy: {decision.Route}; {decision.Reason}");

            try
            {
                if (decision.Route == TransferRoute.NormalLobby)
                {
                    State = TransferState.Normal;
                    NormalTransferEngine.Start(server, transfer);
                    return;
                }

                State = TransferState.Seamless;
                SectorTransferEngine.Instance.StartBackendSwitch(server, world);
            }
            catch
            {
                State = TransferState.Failed;
                throw;
            }
        }

        public void Reset()
        {
            State = TransferState.Idle;
        }

        public void Complete()
        {
            State = TransferState.Idle;
        }

        public void Fail(string reason)
        {
            Seamless.TryShow($"Transfer failed: {reason}");
            State = TransferState.Failed;
        }
    }
}
