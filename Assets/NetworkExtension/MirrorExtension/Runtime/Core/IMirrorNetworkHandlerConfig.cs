using UnityEngine;

namespace CizaMirrorExtension
{
    public interface IMirrorNetworkHandlerConfig
    {
        string RootName { get; }
        bool IsDontDestroyOnLoad { get; }

        GameObject NetworkManagerPrefab { get; }
    }
}