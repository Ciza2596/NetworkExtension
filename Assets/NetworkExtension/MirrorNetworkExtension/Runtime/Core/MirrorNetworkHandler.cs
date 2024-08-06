using System;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaMirrorNetworkExtension
{
    public class MirrorNetworkHandler
    {
        public static readonly uint DisconnectPlayerId = UInt32.MaxValue;
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
        public event Action<uint> OnConnect;

        // PlayerId
        public event Action<uint> OnDisconnect;

        public bool IsInitialized => _networkManager != null;

        public uint PlayerId
        {
            get
            {
                if (Mode.CheckIsOffline() || NetworkClient.connection == null || NetworkClient.connection.identity == null)
                    return DisconnectPlayerId;

                return NetworkClient.connection.identity.netId;
            }
        }

        public int Fps { get; private set; }

        public string NetworkAddress { get; private set; }

        public int MaxPlayerCount { get; private set; }

        public int PlayerCount => IsInitialized && !Mode.CheckIsOffline() ? _playerCount : 0;

        public string UserName { get; private set; }

        public NetworkManagerMode Mode => IsInitialized ? _networkManager.Mode : NetworkManagerMode.Offline;

        public bool IsStoppingClient { get; private set; }


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
            SetUserName(_mirrorNetworkHandlerConfig.DefaultUserName);
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

        public void SetUserName(string userName)
        {
            if (Mode.CheckIsHost() || Mode.CheckIsClientOnly())
                return;

            UserName = userName;
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

            SendDisconnectMessage(false);
        }

        public void StopHost()
        {
            if (!IsInitialized || IsStoppingClient)
                return;

            SendDisconnectMessage(true);
        }

        public void SendMessageToServer<TMessage>(TMessage message) where TMessage : struct, NetworkMessage =>
            _networkManager.SendMessageToServer(message);

        public void SendMessageToAllClient<TMessage>(TMessage message) where TMessage : struct, NetworkMessage =>
            _networkManager.SendMessageToAllClient(message);

        public void RegisterHandlerOnServer<TMessage>(Action<NetworkConnectionToClient, TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage =>
            _networkManager.RegisterHandlerOnServer(handler, requireAuthentication);
        
        public void UnregisterHandlerOnServer<TMessage>(Action<NetworkConnectionToClient, TMessage> handler) where TMessage : struct, NetworkMessage =>
            _networkManager.UnregisterHandlerOnServer(handler);

        public void RegisterHandlerOnClient<TMessage>(Action<TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage =>
            _networkManager.RegisterHandlerOnClient(handler, requireAuthentication);
        
        public void UnregisterHandlerOnClient<TMessage>(Action<TMessage> handler) where TMessage : struct, NetworkMessage =>
            _networkManager.UnregisterHandlerOnClient(handler);


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
            SendMessageToServer(new ConnectMessage(NetworkClient.connection.identity.netId, _networkManager.PlayerCount));

        private void SendDisconnectMessage(bool isHost) =>
            SendMessageToServer(new DisconnectMessage(NetworkClient.connection.identity.netId, PlayerCount - 1, isHost && Mode.CheckIsHost()));


        private void OnReceiveConnectMessageOnServer(NetworkConnectionToClient networkConnectionToClient, ConnectMessage connectMessage) =>
            SendMessageToAllClient(connectMessage);


        private void OnReceiveDisconnectMessageOnServer(NetworkConnectionToClient networkConnectionToClient, DisconnectMessage disconnectMessage) =>
            SendMessageToAllClient(disconnectMessage);

        private void OnReceiveConnectMessageOnClient(ConnectMessage connectMessage)
        {
            _playerCount = connectMessage.PlayerCount;
            OnConnect?.Invoke(connectMessage.PlayerId);
        }

        private void OnReceiveDisconnectMessageOnClient(DisconnectMessage disconnectMessage)
        {
            _playerCount = disconnectMessage.PlayerCount;

            if (!IsStoppingClient && (NetworkClient.connection.identity.netId == disconnectMessage.PlayerId || disconnectMessage.IsHost))
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