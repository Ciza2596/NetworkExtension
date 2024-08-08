using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorNetworkExtension
{
    public struct ConnectMessageToC : NetworkMessage
    {
        public readonly string PlayerId;
        
        public readonly int PlayerCount;

        [Preserve]
        public ConnectMessageToC(string playerId, int playerCount)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
        }
    }
}