using UnityEngine;

namespace CizaMirrorExtension
{
    [CreateAssetMenu(fileName = "MirrorNetworkHandlerConfig", menuName = "Ciza/Network/MirrorNetworkHandlerConfig", order = 0)]
    public class MirrorNetworkHandlerConfig : ScriptableObject, IMirrorNetworkHandlerConfig
    {
        [SerializeField]
        private string _rootName = "[Network]";

        [SerializeField]
        private bool _isDontDestroyOnLoad;

        [Space]
        [SerializeField]
        private GameObject _networkManagerPrefab;

        public string RootName => _rootName;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        public GameObject NetworkManagerPrefab => _networkManagerPrefab;
    }
}