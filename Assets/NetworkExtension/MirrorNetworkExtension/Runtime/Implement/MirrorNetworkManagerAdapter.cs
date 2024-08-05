using System;
using System.Linq;
using Mirror;
using UnityEngine;

namespace CizaMirrorNetworkExtension.Implement
{
    public class MirrorNetworkManagerAdapter : NetworkManager, INetworkManager
    {
        public event Action OnStartServerEvent;
        public event Action OnStopServerEvent;

        public event Action OnStartClientEvent;
        public event Action OnStopClientEvent;

        public event Action OnServerAddPlayerEvent;


        public int PlayerCount => numPlayers;
        public NetworkManagerMode Mode => mode;

        public bool TryGetPlayer(int playerId, out NetworkIdentity networkIdentity)
        {
            if (Mode.CheckIsOffline() || Mode.CheckIsClientOnly())
            {
                networkIdentity = null;
                return false;
            }

            var networkConnectionToClient = NetworkServer.connections.First(connection => connection.Value.identity != null && connection.Value.identity.netId == playerId).Value;
            networkIdentity = networkConnectionToClient.identity;
            return networkIdentity != null;
        }

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

        public void SendMessageToServer<TMessage>(TMessage message) where TMessage : struct, NetworkMessage =>
            NetworkClient.Send(message);

        public void SendMessageToAllClient<TMessage>(TMessage message) where TMessage : struct, NetworkMessage =>
            MirrorNetworkUtils.SendMessageToAllClient(message);

        public void RegisterHandlerOnServer<TMessage>(Action<NetworkConnectionToClient, TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage =>
            NetworkServer.RegisterHandler(handler, requireAuthentication);

        public void RegisterHandlerOnClient<TMessage>(Action<TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage =>
            NetworkClient.RegisterHandler(handler, requireAuthentication);

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

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnStartClientEvent?.Invoke();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnStopClientEvent?.Invoke();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            OnServerAddPlayerEvent?.Invoke();
        }
    }
}