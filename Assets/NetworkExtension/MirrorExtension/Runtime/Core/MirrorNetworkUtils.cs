using System;
using System.Linq;
using Mirror;

namespace CizaMirrorExtension
{
    public static class MirrorNetworkUtils
    {
        public static void SendMessageToAll<T>(T message) where T : struct, NetworkMessage =>
            SendMessageToAll(Array.Empty<int>(), message);

        public static void SendMessageToAll<T>(int[] exceptPlayerIndexList, T message) where T : struct, NetworkMessage
        {
            foreach (var connection in NetworkServer.connections.Values)
                if (!exceptPlayerIndexList.Contains(connection.connectionId))
                    connection.Send(message);
        }
    }
}