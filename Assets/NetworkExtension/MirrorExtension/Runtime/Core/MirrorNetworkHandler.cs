using System;
using Mirror;

namespace CizaMirrorExtension
{
    public class MirrorNetworkHandler
    {
        private readonly IMirrorNetworkHandlerConfig _mirrorNetworkHandlerConfig;

        private NetworkManager _networkManager;


        public bool IsInitialized => _networkManager != null;

        public MirrorNetworkHandler(IMirrorNetworkHandlerConfig mirrorNetworkHandlerConfig) =>
            _mirrorNetworkHandlerConfig = mirrorNetworkHandlerConfig;


        public void Initialize()
        {
            if (IsInitialized)
                return;
        }

        public void Release()
        {
            if (!IsInitialized)
                return;
        }

        public void SetFps(int fps)
        {
            if (!IsInitialized)
                return;

            _networkManager.sendRate = fps;
            _networkManager.Update();
        }


        public void StartServer()
        {
            if (!IsInitialized)
                return;
        }

        public void StartClient(Uri uri)
        {
            if (!IsInitialized)
                return;
        }

        public void StartHost()
        {
            if (!IsInitialized)
                return;
        }

        public void StopServer()
        {
            if (!IsInitialized)
                return;
        }

        public void StopClient()
        {
            if (!IsInitialized)
                return;
        }

        public void StopHost()
        {
            if (!IsInitialized)
                return;

            StopClient();
            StopServer();
        }
    }
}