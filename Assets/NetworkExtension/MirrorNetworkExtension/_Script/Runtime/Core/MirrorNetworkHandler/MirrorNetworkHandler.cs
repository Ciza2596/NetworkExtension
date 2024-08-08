using System;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CizaMirrorNetworkExtension
{
    public class MirrorNetworkHandler
    {
        public static readonly string OfflinePlayerId = "-1";
        public static readonly float DelayStopHostTime = 0.05f;

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
        public event Action<string> OnConnect;

        // PlayerId
        public event Action<string> OnDisconnect;

        public bool IsInitialized => _networkManager != null;

        public string PlayerId
        {
            get
            {
                if (Mode.CheckIsOffline() || NetworkClient.connection == null || NetworkClient.connection.identity == null)
                    return OfflinePlayerId;

                return NetworkClient.connection.identity.netId.ToString();
            }
        }

        public int Fps { get; private set; }

        public string NetworkAddress { get; private set; }

        public int MaxPlayerCount { get; private set; }

        public int PlayerCount => IsInitialized && !Mode.CheckIsOffline() ? _playerCount : 0;

        public string PlayerName { get; private set; }

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

            _networkManager.SetPlayerPrefab(_mirrorNetworkHandlerConfig.NetworkPlayerPrefab);
            _networkManager.StopHost();
            SetPlayerName(_mirrorNetworkHandlerConfig.DefaultPlayerName);
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

        public void SetPlayerName(string playerName) =>
            PlayerName = playerName;


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

        public void SendMessageToClient<TMessage>(string playerId, TMessage message) where TMessage : struct, NetworkMessage =>
            _networkManager.SendMessageToClient(playerId, message);

        public void SendMessageToAllClient<TMessage>(TMessage message, string[] exceptPlayerIdList = null) where TMessage : struct, NetworkMessage =>
            _networkManager.SendMessageToAllClient(message, exceptPlayerIdList);

        public void RegisterHandlerOnServer<TMessage>(Action<NetworkConnectionToClient, TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage =>
            _networkManager.RegisterHandlerOnServer(handler, requireAuthentication);

        public void UnregisterHandlerOnServer<TMessage>() where TMessage : struct, NetworkMessage =>
            _networkManager.UnregisterHandlerOnServer<TMessage>();

        public void RegisterHandlerOnClient<TMessage>(Action<TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage =>
            _networkManager.RegisterHandlerOnClient(handler, requireAuthentication);

        public void UnregisterHandlerOnClient<TMessage>() where TMessage : struct, NetworkMessage =>
            _networkManager.UnregisterHandlerOnClient<TMessage>();


        private void OnStartServerImp()
        {
            OnServerRegisterHandler?.Invoke();
            RegisterHandlerOnServer<ConnectMessageToS>(OnConnectMessageToS);
            RegisterHandlerOnServer<DisconnectMessageToS>(OnDisconnectMessageToS);
            OnStartServer?.Invoke();
        }


        private void OnStopServerImp()
        {
            UnregisterHandlerOnServer<ConnectMessageToS>();
            UnregisterHandlerOnServer<DisconnectMessageToS>();
            OnStopServer?.Invoke();
        }

        private void OnStartClientImp()
        {
            OnClientRegisterHandler?.Invoke();
            RegisterHandlerOnClient<ConnectMessageToC>(OnConnectMessageToC);
            RegisterHandlerOnClient<DisconnectMessageToC>(OnDisconnectMessageToC);
            OnStartClient?.Invoke();
        }


        private void OnStopClientImp()
        {
            UnregisterHandlerOnClient<ConnectMessageToC>();
            UnregisterHandlerOnClient<DisconnectMessageToC>();
            OnStopClient?.Invoke();
        }

        private void OnServerAddPlayerEventImp(string playerId) =>
            SendConnectMessage(playerId);


        private void SendConnectMessage(string playerId) =>
            SendMessageToServer(new ConnectMessageToS(playerId, _networkManager.PlayerCount));

        private void SendDisconnectMessage(bool isHost) =>
            SendMessageToServer(new DisconnectMessageToS(PlayerId, PlayerCount - 1, isHost && Mode.CheckIsHost()));


        private void OnConnectMessageToS(NetworkConnectionToClient networkConnectionToClient, ConnectMessageToS connectMessageToS) =>
            SendMessageToAllClient(new ConnectMessageToC(connectMessageToS.PlayerId, connectMessageToS.PlayerCount));


        private void OnDisconnectMessageToS(NetworkConnectionToClient networkConnectionToClient, DisconnectMessageToS disconnectMessageToS) =>
            SendMessageToAllClient(new DisconnectMessageToC(disconnectMessageToS.PlayerId, disconnectMessageToS.PlayerCount, disconnectMessageToS.IsHost));

        private void OnConnectMessageToC(ConnectMessageToC connectMessageToC)
        {
            _playerCount = connectMessageToC.PlayerCount;
            if (PlayerId == connectMessageToC.PlayerId)
                OnConnect?.Invoke(connectMessageToC.PlayerId);
        }

        private void OnDisconnectMessageToC(DisconnectMessageToC disconnectMessageToC)
        {
            _playerCount = disconnectMessageToC.PlayerCount;

            if (!IsStoppingClient && (PlayerId == disconnectMessageToC.PlayerId || disconnectMessageToC.IsHost))
            {
                OnDisconnect?.Invoke(disconnectMessageToC.PlayerId);
                EnableStoppingHost(disconnectMessageToC.IsHost);
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