using System;
using Mirror;
using UnityEngine;

namespace CizaMirrorExtension
{
    public class MirrorNetworkManagerAdapter : NetworkManager, INetworkManager
    {
        public event Action OnStartServerEvent;
        public event Action OnStartClientEvent;
        public event Action OnStartHostEvent;

        public event Action OnStopServerEvent;
        public event Action OnStopClientEvent;
        public event Action OnStopHostEvent;

        public event Action<int> OnConnect;
        public event Action<int> OnDisconnect;


        public int PlayerCount => numPlayers;
        public NetworkManagerMode Mode => mode;

        public bool TryGetPlayer(int playerIndex, out NetworkConnectionToClient networkConnectionToClient) =>
            NetworkServer.connections.TryGetValue(playerIndex, out networkConnectionToClient);

        public void SetIsDontDestroyOnLoad(bool isDontDestroyOnLoad) =>
            dontDestroyOnLoad = isDontDestroyOnLoad;

        public void SetPlayerPrefab(GameObject playerPrefab) =>
            this.playerPrefab = playerPrefab;

        public void SetFps(int fps)
        {
            sendRate = fps;
            Update();
        }

        public void SetNetworkAddress(string networkAddress) =>
            this.networkAddress = networkAddress;

        public void SetMaxPlayerCount(int maxPlayerCount) =>
            maxConnections = maxPlayerCount;

        public void RegisterHandler<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage =>
            NetworkServer.RegisterHandler(handler, requireAuthentication);

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnStartServerEvent?.Invoke();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnStartClientEvent?.Invoke();
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            OnStartHostEvent?.Invoke();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            OnConnect?.Invoke(conn.connectionId);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            OnStopServerEvent?.Invoke();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnStopClientEvent?.Invoke();
        }

        public override void OnStopHost()
        {
            base.OnStopHost();
            OnStopHostEvent?.Invoke();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            OnDisconnect?.Invoke(conn.connectionId);
        }
    }
}