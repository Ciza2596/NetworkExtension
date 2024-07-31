using System;
using Mirror;
using UnityEngine;

namespace CizaMirrorExtension
{
    public interface INetworkManager
    {
        event Action OnStartServerEvent;
        event Action OnStartClientEvent;
        event Action OnStartHostEvent;
        // PlayerIndex
        event Action<int> OnConnect;

        event Action OnStopServerEvent;
        event Action OnStopClientEvent;
        event Action OnStopHostEvent;
        // PlayerIndex
        event Action<int> OnDisconnect;

        int PlayerCount { get; }

        NetworkManagerMode Mode { get; }

        bool TryGetPlayer(int playerIndex, out NetworkConnectionToClient networkConnectionToClient);

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

        void RegisterHandler<T>(Action<NetworkConnectionToClient, T> handler, bool requireAuthentication = true) where T : struct, NetworkMessage;
    }
}