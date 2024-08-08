using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorNetworkExtension
{
    public struct ConnectMessageToS : NetworkMessage
    {
        public readonly string PlayerId;

        public readonly int PlayerCount;

        [Preserve]
        public ConnectMessageToS(string playerId, int playerCount)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
        }
    }
}