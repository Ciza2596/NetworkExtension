using UnityEngine;

namespace CizaMirrorNetworkExtension
{
    public interface IMirrorNetworkHandlerConfig
    {
        string RootName { get; }
        bool IsDontDestroyOnLoad { get; }

        GameObject NetworkManagerPrefab { get; }
        GameObject NetworkPlayerPrefab { get; }

        int DefaultFps { get; }

        string DefaultNetworkAddress { get; }

        int DefaultMaxPlayerCount { get; }
    }
}