using System;

namespace CizaMirrorExtension
{
    public interface IMirrorNetworkPlayer
    {
        event Action<int> OnStartClientEvent;

        int PlayerIndex { get; }

        void SetPlayerIndex(int playerIndex);
    }
}