using UnityEngine;

namespace CizaMirrorNetworkExtension.Implement
{
    [CreateAssetMenu(fileName = "MirrorNetworkHandlerConfig", menuName = "Ciza/Network/MirrorNetworkHandlerConfig", order = 0)]
    public class MirrorNetworkHandlerConfig : ScriptableObject, IMirrorNetworkHandlerConfig
    {
        [SerializeField]
        private string _rootName = "[Network]";

        [SerializeField]
        private bool _isDontDestroyOnLoad = true;

        [Space]
        [SerializeField]
        private GameObject _networkManagerPrefab;
        
        [SerializeField]
        private GameObject _networkPlayerPrefab;

        [Space]
        [SerializeField]
        private int _defaultFps = 60;

        [SerializeField]
        private string _defaultNetworkAddress = "localhost";

        [SerializeField]
        private int _defaultMaxPlayerCount = 4;

        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject NetworkManagerPrefab => _networkManagerPrefab;
        public GameObject NetworkPlayerPrefab => _networkPlayerPrefab;

        public int DefaultFps => _defaultFps;
        public string DefaultNetworkAddress => _defaultNetworkAddress;
        public int DefaultMaxPlayerCount => _defaultMaxPlayerCount;
    }
}