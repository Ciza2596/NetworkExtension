using UnityEngine;

namespace CizaMirrorNetworkExtension
{
    public interface IMirrorNetworkHandlerConfig
    {
        string RootName { get; }
        bool IsDontDestroyOnLoad { get; }

        GameObject NetworkManagerPrefab { get; }
        GameObject NetworkPlayerPrefab { get; }

        string DefaultPlayerName { get; }
        int DefaultFps { get; }
        string DefaultNetworkAddress { get; }
        int DefaultMaxPlayerCount { get; }
    }
}