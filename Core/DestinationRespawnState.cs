using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using SpaceEngineers.Game.GUI;
using SpaceEngineers.Game.World;
using VRage.Game;

namespace SeamlessClient.Core
{
    internal static class DestinationRespawnState
    {
        public static void ResetUiBeforeSwitch()
        {
            MyScreenManager.CloseScreenNow(typeof(MyGuiScreenMedicals));
            MyScreenManager.RemoveScreenByType(typeof(MyGuiScreenMedicals));
        }

        public static void Apply(MyObjectBuilder_Checkpoint checkpoint)
        {
            var component = MySession.Static?.GetComponent<MySpaceRespawnComponent>();
            if (component == null)
                throw new System.InvalidOperationException("Destination respawn component is unavailable.");

            component.InitFromCheckpoint(checkpoint);
            MySession.Static.Players.RespawnComponent = component;
        }

        public static void BindToPlayers()
        {
            var component = MySession.Static?.GetComponent<MySpaceRespawnComponent>();
            if (component == null)
                throw new System.InvalidOperationException("Destination respawn component is unavailable.");

            MySession.Static.Players.RespawnComponent = component;
        }
    }
}
