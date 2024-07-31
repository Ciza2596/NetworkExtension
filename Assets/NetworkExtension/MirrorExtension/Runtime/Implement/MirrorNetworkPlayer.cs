using System;
using Mirror;

namespace CizaMirrorExtension.Implement
{
    public class MirrorNetworkPlayer : NetworkBehaviour, IMirrorNetworkPlayer
    {
        public event Action<int> OnStartClientEvent;

        public int PlayerIndex { get; private set; }

        public void SetPlayerIndex(int playerIndex) =>
            PlayerIndex = playerIndex;

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnStartClientEvent?.Invoke(PlayerIndex);
        }
    }
}