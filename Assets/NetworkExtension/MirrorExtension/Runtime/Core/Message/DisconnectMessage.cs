using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorExtension
{
    public struct DisconnectMessage : NetworkMessage
    {
        public readonly int PlayerId;
        public readonly int PlayerCount;
        public readonly bool IsHost;

        [Preserve]
        public DisconnectMessage(int playerId, int playerCount, bool isHost)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
            IsHost = isHost;
        }
    }
}