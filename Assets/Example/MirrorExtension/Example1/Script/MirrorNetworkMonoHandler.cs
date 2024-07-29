using UnityEngine;

namespace CizaMirrorExtension.Example
{
    public class MirrorNetworkMonoHandler : MonoBehaviour
    {
        [SerializeField]
        private MirrorNetworkHandlerConfig _mirrorNetworkHandlerConfig;

        public MirrorNetworkHandler MirrorNetworkHandler { get; private set; }

        public void OnEnable()
        {
            MirrorNetworkHandler = new MirrorNetworkHandler(_mirrorNetworkHandlerConfig);
            MirrorNetworkHandler.Initialize();
        }

        public void OnDisable()
        {
            MirrorNetworkHandler.Release();
            MirrorNetworkHandler = null;
        }
    }
}