using UnityEngine;

namespace CizaMirrorExtension
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

        [Space]
        [SerializeField]
        private int _defaultFps = 60;

        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject NetworkManagerPrefab => _networkManagerPrefab;
        
        public int DefaultFps => _defaultFps;
    }
}