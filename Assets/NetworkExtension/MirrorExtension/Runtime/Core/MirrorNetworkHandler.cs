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
        private INetworkManager _networkManager;

        public event Action OnStartServer;
        public event Action OnStartClient;
        public event Action OnStartHost;

        public event Action OnStopServer;
        public event Action OnStopClient;
        public event Action OnStopHost;

        // PlayerIndex
        public event Action<int> OnConnect;

        // PlayerIndex
        public event Action<int> OnDisconnect;

        public bool IsInitialized => _networkManager != null;

        public int Fps { get; private set; }

        public string NetworkAddress { get; private set; }

        public int MaxPlayerCount { get; private set; }

        public int PlayerCount => IsInitialized ? _networkManager.PlayerCount : 0;

        public NetworkManagerMode Mode => IsInitialized ? _networkManager.Mode : NetworkManagerMode.Offline;

        public bool TryGetPlayer(int playerIndex, out NetworkConnectionToClient networkConnectionToClient)
        {
            if (!IsInitialized)
            {
                networkConnectionToClient = null;
                return false;
            }

            return _networkManager.TryGetPlayer(playerIndex, out networkConnectionToClient);
        }


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
            _networkManager = Object.Instantiate(_mirrorNetworkHandlerConfig.NetworkManagerPrefab, _root).GetComponent<INetworkManager>();

            _networkManager.SetIsDontDestroyOnLoad(_mirrorNetworkHandlerConfig.IsDontDestroyOnLoad);
            _networkManager.SetPlayerPrefab(_mirrorNetworkHandlerConfig.NetworkPlayerPrefab);
            _networkManager.StopHost();
            SetFps(_mirrorNetworkHandlerConfig.DefaultFps);
            SetNetworkAddress(_mirrorNetworkHandlerConfig.DefaultNetworkAddress);
            SetMaxPlayerCount(_mirrorNetworkHandlerConfig.DefaultMaxPlayerCount);

            _networkManager.OnStartServerEvent += OnStartServerImp;
            _networkManager.OnStartClientEvent += OnStartClientImp;
            _networkManager.OnStartHostEvent += OnStartHostImp;
            _networkManager.OnConnect += OnConnectImp;

            _networkManager.OnStopServerEvent += OnStopServerImp;
            _networkManager.OnStopClientEvent += OnStopClientImp;
            _networkManager.OnStopHostEvent += OnStopHostImp;
            _networkManager.OnDisconnect += OnDisconnectImp;
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
            _networkManager.SetFps(Fps);
        }

        public void SetNetworkAddress(string networkAddress)
        {
            if (!IsInitialized)
                return;

            NetworkAddress = networkAddress;
            _networkManager.SetNetworkAddress(NetworkAddress);
        }

        public void SetMaxPlayerCount(int maxPlayerCount)
        {
            if (!IsInitialized)
                return;

            MaxPlayerCount = maxPlayerCount;
            _networkManager.SetMaxPlayerCount(MaxPlayerCount);
        }

        public void StartServer()
        {
            if (!IsInitialized)
                return;

            _networkManager.StartServer();
        }

        public void StartClient()
        {
            if (!IsInitialized)
                return;

            _networkManager.StartClient();
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

        public void RegisterHandler<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage =>
            _networkManager.RegisterHandler(handler, requireAuthentication);

        public void SendMessage<TMessage>(TMessage message) where TMessage : struct, NetworkMessage =>
            NetworkClient.Send(message);


        private void OnStartServerImp() =>
            OnStartServer?.Invoke();

        private void OnStartClientImp() =>
            OnStartClient?.Invoke();

        private void OnStartHostImp() =>
            OnStartHost?.Invoke();

        private void OnStopServerImp() =>
            OnStopServer?.Invoke();

        private void OnStopClientImp() =>
            OnStopClient?.Invoke();

        private void OnStopHostImp() =>
            OnStopHost?.Invoke();

        private void OnConnectImp(int playerIndex) =>
            OnConnect?.Invoke(playerIndex);

        private void OnDisconnectImp(int playerIndex) =>
            OnDisconnect?.Invoke(playerIndex);
    }
}