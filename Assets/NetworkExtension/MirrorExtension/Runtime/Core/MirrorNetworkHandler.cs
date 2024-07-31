using System;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaMirrorExtension
{
    public class MirrorNetworkHandler
    {
        public readonly float DelayStopHostTime = 0.1f;

        private readonly IMirrorNetworkHandlerConfig _mirrorNetworkHandlerConfig;

        private Transform _root;
        private INetworkManager _networkManager;

        private int _playerCount;

        private float _time;

        public event Action OnServerRegisterHandler;
        public event Action OnStartServer;

        public event Action OnStopServer;

        // PlayerIndex
        public event Action<int> OnPlayerRegisterHandler;

        // PlayerIndex
        public event Action<int> OnConnect;

        // PlayerIndex
        public event Action<int> OnDisconnect;

        public bool IsInitialized => _networkManager != null;

        public int Fps { get; private set; }

        public string NetworkAddress { get; private set; }

        public int MaxPlayerCount { get; private set; }

        public int PlayerCount => IsInitialized && !Mode.CheckIsOffline() ? _playerCount : 0;

        public NetworkManagerMode Mode => IsInitialized ? _networkManager.Mode : NetworkManagerMode.Offline;

        public bool IsStoppingHost { get; private set; }

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
            _networkManager.OnAddPlayer += OnAddPlayerImp;
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

            if (IsStoppingHost)
            {
                _time -= deltaTime;
                if (_time < 0)
                {
                    IsStoppingHost = false;
                    _networkManager.StopHost();
                }
            }
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
            if (!IsInitialized || IsStoppingHost)
                return;

            _time = DelayStopHostTime;
            IsStoppingHost = true;
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

        private void OnAddPlayerImp(int playerIndex)
        {
            if (!_networkManager.TryGetPlayer(playerIndex, out var networkConnectionToClient) || !networkConnectionToClient.identity.TryGetComponent<IMirrorNetworkPlayer>(out var mirrorNetworkPlayer))
                return;

            mirrorNetworkPlayer.SetPlayerIndex(playerIndex);
            mirrorNetworkPlayer.OnStartClientEvent += OnPlayerStartClientEventImp;
        }

        private void SendConnectMessage() =>
            SendMessage(new ConnectMessage(NetworkClient.connection.connectionId, _networkManager.PlayerCount));

        private void SendDisconnectMessage() =>
            SendMessage(new DisconnectMessage(NetworkClient.connection.connectionId, _networkManager.PlayerCount, Mode.CheckIsHost()));

        private void OnPlayerStartClientEventImp(int playerIndex)
        {
            OnPlayerRegisterHandler?.Invoke(playerIndex);

            RegisterHandlerOnClient<ConnectMessage>(OnReceiveConnectMessageOnClient);
            RegisterHandlerOnClient<DisconnectMessage>(OnReceiveDisconnectMessageOnClient);
            SendConnectMessage();
        }

        private void OnReceiveConnectMessageOnServer(NetworkConnectionToClient networkConnectionToClient, ConnectMessage connectMessage) =>
            MirrorNetworkUtils.SendMessageToAll(connectMessage);


        private void OnReceiveDisconnectMessageOnServer(NetworkConnectionToClient networkConnectionToClient, DisconnectMessage disconnectMessage) =>
            MirrorNetworkUtils.SendMessageToAll(new[] { networkConnectionToClient.connectionId }, disconnectMessage);

        private void OnReceiveConnectMessageOnClient(ConnectMessage connectMessage)
        {
            _playerCount = connectMessage.PlayerCount;
            OnConnect?.Invoke(connectMessage.PlayerIndex);
        }

        private void OnReceiveDisconnectMessageOnClient(DisconnectMessage disconnectMessage)
        {
            _playerCount = disconnectMessage.PlayerCount;

            if (NetworkClient.connection.connectionId == disconnectMessage.PlayerIndex || disconnectMessage.IsHost)
            {
                OnDisconnect?.Invoke(disconnectMessage.PlayerIndex);
                _networkManager.StopClient();
            }
        }
    }
}