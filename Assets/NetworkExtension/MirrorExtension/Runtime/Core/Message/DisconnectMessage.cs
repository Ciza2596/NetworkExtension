using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorExtension
{
    public struct DisconnectMessage : NetworkMessage
    {
        public readonly int PlayerIndex;
        public readonly int PlayerCount;
        public readonly bool IsHost;

        [Preserve]
        public DisconnectMessage(int playerIndex, int playerCount, bool isHost)
        {
            PlayerIndex = playerIndex;
            PlayerCount = playerCount;
            IsHost = isHost;
        }
    }
}