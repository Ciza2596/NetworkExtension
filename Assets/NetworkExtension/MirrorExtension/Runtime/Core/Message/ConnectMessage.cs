using Mirror;
using UnityEngine.Scripting;

namespace CizaMirrorExtension
{
    public struct ConnectMessage : NetworkMessage
    {
        public readonly int PlayerIndex;
        public readonly int PlayerCount;

        [Preserve]
        public ConnectMessage(int playerIndex, int playerCount)
        {
            PlayerIndex = playerIndex;
            PlayerCount = playerCount;
        }
    }
}