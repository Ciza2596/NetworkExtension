using System;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaMirrorExtension
{
    public class MirrorNetworkHandler
    {
        public readonly float DelayStopHostTime = 0.05f;

        private readonly IMirrorNetworkHandlerConfig _mirrorNetworkHandlerConfig;

        private Transform _root;
        private INetworkManager _networkManager;

        private int _playerCount;

        private float _stoppingClientTime;
        private bool _isStoppingHost;

        public event Action OnServerRegisterHandler;
        public event Action OnStartServer;
        public event Action OnStopServer;


        public event Action OnClientRegisterHandler;
        public event Action OnStartClient;
        public event Action OnStopClient;

        // PlayerId
        public event Action<int> OnConnect;

        // PlayerId
        public event Action<int> OnDisconnect;

        public bool IsInitialized => _networkManager != null;

        public int Fps { get; private set; }

        public string NetworkAddress { get; private set; }

        public int MaxPlayerCount { get; private set; }

        public int PlayerCount => IsInitialized && !Mode.CheckIsOffline() ? _playerCount : 0;

        public NetworkManagerMode Mode => IsInitialized ? _networkManager.Mode : NetworkManagerMode.Offline;

        public bool IsStoppingClient { get; private set; }

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
            _networkManager.OnStopServerEvent += OnStopServerImp;

            _networkManager.OnStartClientEvent += OnStartClientImp;
            _networkManager.OnStopClientEvent += OnStopClientImp;

            _networkManager.OnServerAddPlayerEvent += OnServerAddPlayerEventImp;
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

        public void Tick(float deltaTime)
        {
            if (!IsInitialized)
                return;

            CheckStoppingClient(deltaTime);
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

            SendDisconnectMessage();
        }

        public void StopHost()
        {
            if (!IsInitialized || IsStoppingClient)
                return;

            SendDisconnectMessage();
        }

        public void RegisterHandlerOnServer<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage =>
            _networkManager.RegisterHandlerOnServer(handler, requireAuthentication);

        public void RegisterHandlerOnClient<T>(Action<T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage =>
            _networkManager.RegisterHandlerOnClient(handler, requireAuthentication);

        public void SendMessage<TMessage>(TMessage message) where TMessage : struct, NetworkMessage =>
            _networkManager.SendMessage(message);


        private void OnStartServerImp()
        {
            OnServerRegisterHandler?.Invoke();
            RegisterHandlerOnServer<ConnectMessage>(OnReceiveConnectMessageOnServer);
            RegisterHandlerOnServer<DisconnectMessage>(OnReceiveDisconnectMessageOnServer);
            OnStartServer?.Invoke();
        }


        private void OnStopServerImp() =>
            OnStopServer?.Invoke();

        private void OnStartClientImp()
        {
            OnClientRegisterHandler?.Invoke();
            RegisterHandlerOnClient<ConnectMessage>(OnReceiveConnectMessageOnClient);
            RegisterHandlerOnClient<DisconnectMessage>(OnReceiveDisconnectMessageOnClient);
            OnStartClient?.Invoke();
        }


        private void OnStopClientImp() =>
            OnStopClient?.Invoke();

        private void OnServerAddPlayerEventImp() =>
            SendConnectMessage();


        private void SendConnectMessage() =>
            SendMessage(new ConnectMessage(NetworkClient.connection.connectionId, _networkManager.PlayerCount));

        private void SendDisconnectMessage() =>
            SendMessage(new DisconnectMessage(NetworkClient.connection.connectionId, _networkManager.PlayerCount, Mode.CheckIsHost()));


        private void OnReceiveConnectMessageOnServer(NetworkConnectionToClient networkConnectionToClient, ConnectMessage connectMessage) =>
            MirrorNetworkUtils.SendMessageToAll(connectMessage);


        private void OnReceiveDisconnectMessageOnServer(NetworkConnectionToClient networkConnectionToClient, DisconnectMessage disconnectMessage) =>
            MirrorNetworkUtils.SendMessageToAll(disconnectMessage);

        private void OnReceiveConnectMessageOnClient(ConnectMessage connectMessage)
        {
            _playerCount = connectMessage.PlayerCount;
            OnConnect?.Invoke(connectMessage.PlayerId);
        }

        private void OnReceiveDisconnectMessageOnClient(DisconnectMessage disconnectMessage)
        {
            _playerCount = disconnectMessage.PlayerCount;

            if (!IsStoppingClient && (NetworkClient.connection.connectionId == disconnectMessage.PlayerId || disconnectMessage.IsHost))
            {
                OnDisconnect?.Invoke(disconnectMessage.PlayerId);
                EnableStoppingHost(disconnectMessage.IsHost);
            }
        }

        private void EnableStoppingHost(bool isStoppingHost)
        {
            _stoppingClientTime = DelayStopHostTime;
            IsStoppingClient = true;
            _isStoppingHost = isStoppingHost;
        }

        private void DisableStoppingHost()
        {
            _stoppingClientTime = 0;
            _isStoppingHost = false;
            IsStoppingClient = false;
        }

        private void CheckStoppingClient(float deltaTime)
        {
            if (!IsStoppingClient)
                return;

            _stoppingClientTime -= deltaTime;
            if (_stoppingClientTime < 0)
            {
                if (_isStoppingHost)
                    _networkManager.StopHost();
                else
                    _networkManager.StopClient();

                DisableStoppingHost();
            }
        }
    }
}