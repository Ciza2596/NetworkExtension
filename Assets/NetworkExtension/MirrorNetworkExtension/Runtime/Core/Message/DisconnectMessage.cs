using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorNetworkExtension
{
    public struct DisconnectMessage : NetworkMessage
    {
        public readonly uint PlayerId;
        public readonly int PlayerCount;
        public readonly bool IsHost;

        [Preserve]
        public DisconnectMessage(uint playerId, int playerCount, bool isHost)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
            IsHost = isHost;
        }
    }
}