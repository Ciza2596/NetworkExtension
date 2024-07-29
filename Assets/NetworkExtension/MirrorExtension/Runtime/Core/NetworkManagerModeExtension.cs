using Mirror;

namespace CizaMirrorExtension
{
    public static class NetworkManagerModeExtension
    {
        public static bool CheckIsOffline(this NetworkManagerMode networkManagerMode) =>
            networkManagerMode == NetworkManagerMode.Offline;

        public static bool CheckIsServerOnly(this NetworkManagerMode networkManagerMode) =>
            networkManagerMode == NetworkManagerMode.ServerOnly;

        public static bool CheckIsClientOnly(this NetworkManagerMode networkManagerMode) =>
            networkManagerMode == NetworkManagerMode.ClientOnly;

        public static bool CheckIsHost(this NetworkManagerMode networkManagerMode) =>
            networkManagerMode == NetworkManagerMode.Host;
    }
}