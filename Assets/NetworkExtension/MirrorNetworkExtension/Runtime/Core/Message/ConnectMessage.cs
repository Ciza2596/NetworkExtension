using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorNetworkExtension
{
    public struct ConnectMessage : NetworkMessage
    {
        public readonly uint PlayerId;
        public readonly int PlayerCount;

        [Preserve]
        public ConnectMessage(uint playerId, int playerCount)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
        }
    }
}