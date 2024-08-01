using System;
using Mirror;
using UnityEngine;

namespace CizaMirrorExtension
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

        bool TryGetPlayerWhenServer(int playerId, out NetworkIdentity networkIdentity);

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

        void RegisterHandlerOnServer<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage;
        void RegisterHandlerOnClient<T>(Action<T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage;

        void SendMessage<TMessage>(TMessage message) where TMessage : struct, NetworkMessage;
    }
}