using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorNetworkExtension
{
    public struct DisconnectMessageToS : NetworkMessage
    {
        public readonly string PlayerId;

        public readonly int PlayerCount;
        public readonly bool IsHost;

        [Preserve]
        public DisconnectMessageToS(string playerId, int playerCount, bool isHost)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
            IsHost = isHost;
        }
    }
}