using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorExtension
{
    public struct ConnectMessage : NetworkMessage
    {
        public readonly int PlayerId;
        public readonly int PlayerCount;

        [Preserve]
        public ConnectMessage(int playerId, int playerCount)
        {
            PlayerId = playerId;
            PlayerCount = playerCount;
        }
    }
}