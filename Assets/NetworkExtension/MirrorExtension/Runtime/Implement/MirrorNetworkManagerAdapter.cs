using System;
using Mirror;
using UnityEngine;

namespace CizaMirrorExtension.Implement
{
    public class MirrorNetworkManagerAdapter : NetworkManager, INetworkManager
    {
        public event Action OnStartServerEvent;
        public event Action OnStopServerEvent;

        public event Action<int> OnAddPlayer;


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

        public void RegisterHandlerOnServer<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage =>
            NetworkServer.RegisterHandler(handler, requireAuthentication);

        public void RegisterHandlerOnClient<T>(Action<T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage =>
            NetworkClient.RegisterHandler(handler, requireAuthentication);

        public void SendMessage<TMessage>(TMessage message) where TMessage : struct, NetworkMessage =>
            NetworkClient.Send(message);

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnStartServerEvent?.Invoke();
        }


        public override void OnStopServer()
        {
            base.OnStopServer();
            OnStopServerEvent?.Invoke();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            OnAddPlayer?.Invoke(conn.connectionId);
        }
    }
}