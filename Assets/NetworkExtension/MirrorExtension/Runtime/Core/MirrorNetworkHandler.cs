using System;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaMirrorExtension
{
    public class MirrorNetworkHandler
    {
        private readonly IMirrorNetworkHandlerConfig _mirrorNetworkHandlerConfig;

        private Transform _root;
        private NetworkManager _networkManager;


        public bool IsInitialized => _networkManager != null;

        public int Fps { get; private set; }

        public NetworkManagerMode Mode => IsInitialized ? _networkManager.mode : NetworkManagerMode.Offline;

        public MirrorNetworkHandler(IMirrorNetworkHandlerConfig mirrorNetworkHandlerConfig) =>
            _mirrorNetworkHandlerConfig = mirrorNetworkHandlerConfig;


        public void Initialize()
        {
            if (IsInitialized)
                return;

            var rootGameObject = new GameObject(_mirrorNetworkHandlerConfig.RootName);
            if (_mirrorNetworkHandlerConfig.IsDontDestroyOnLoad)
                Object.DontDestroyOnLoad(rootGameObject);
            _root = rootGameObject.transform;
            _networkManager = Object.Instantiate(_mirrorNetworkHandlerConfig.NetworkManagerPrefab, _root).GetComponent<NetworkManager>();
            _networkManager.StopHost();
            SetFps(_mirrorNetworkHandlerConfig.DefaultFps);
        }

        public void Release()
        {
            if (!IsInitialized)
                return;

            _networkManager.StopHost();
            _networkManager = null;
            var root = _root;
            _root = null;
            Object.DestroyImmediate(root.gameObject);
        }

        public void SetFps(int fps)
        {
            if (!IsInitialized)
                return;

            Fps = fps;
            _networkManager.sendRate = Fps;
            _networkManager.Update();
        }

        public void StartServer()
        {
            if (!IsInitialized)
                return;

            _networkManager.StartServer();
        }

        public void StartClient(Uri uri)
        {
            if (!IsInitialized)
                return;

            _networkManager.StartClient(uri);
        }

        public void StartHost()
        {
            if (!IsInitialized)
                return;

            _networkManager.StartHost();
        }

        public void StopServer()
        {
            if (!IsInitialized)
                return;

            _networkManager.StopServer();
        }

        public void StopClient()
        {
            if (!IsInitialized)
                return;

            _networkManager.StopClient();
        }

        public void StopHost()
        {
            if (!IsInitialized)
                return;

            _networkManager.StopHost();
        }
    }
}