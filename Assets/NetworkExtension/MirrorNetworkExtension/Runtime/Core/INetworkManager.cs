using System;
using Mirror;
using UnityEngine;

namespace CizaMirrorNetworkExtension
{
    public interface INetworkManager
    {
        event Action OnStartServerEvent;

        event Action OnStopServerEvent;

        event Action OnStartClientEvent;

        event Action OnStopClientEvent;

        event Action OnServerAddPlayerEvent;

        int PlayerCount { get; }

        NetworkManagerMode Mode { get; }

        bool TryGetPlayer(int playerId, out NetworkIdentity networkIdentity);

        void SetIsDontDestroyOnLoad(bool isDontDestroyOnLoad);
        void SetPlayerPrefab(GameObject playerPrefab);

        void SetFps(int fps);
        void SetNetworkAddress(string networkAddress);
        void SetMaxPlayerCount(int maxPlayerCount);


        void StartServer();

        void StartClient();
        void StartClient(Uri uri);

        void StartHost();

        void StopServer();
        void StopClient();
        void StopHost();

        void SendMessageToServer<TMessage>(TMessage message) where TMessage : struct, NetworkMessage;
        void SendMessageToAllClient<TMessage>(TMessage message) where TMessage : struct, NetworkMessage;

        void RegisterHandlerOnServer<TMessage>(Action<NetworkConnectionToClient, TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage;
        void RegisterHandlerOnClient<TMessage>(Action<TMessage> handler, bool requireAuthentication = true) where TMessage : struct, NetworkMessage;
    }
}