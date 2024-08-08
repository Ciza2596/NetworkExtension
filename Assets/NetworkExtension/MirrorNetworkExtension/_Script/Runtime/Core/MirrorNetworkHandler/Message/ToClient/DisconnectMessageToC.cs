using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorNetworkExtension
{
    public struct DisconnectMessageToC : NetworkMessage
    {
        public readonly string PlayerId;
        
        public readonly int PlayerCount;
        public readonly bool IsHost;

        [Preserve]
        public DisconnectMessageToC(string playerId, int playerCount, bool isHost)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
            IsHost = isHost;
        }
    }
}