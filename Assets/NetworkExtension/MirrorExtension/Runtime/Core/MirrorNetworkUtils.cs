using System;
using System.Linq;
using Mirror;

namespace CizaMirrorExtension
{
    public static class MirrorNetworkUtils
    {
        public static void SendMessageToAll<T>(T message) where T : struct, NetworkMessage =>
            SendMessageToAll(Array.Empty<uint>(), message);

        public static void SendMessageToAll<T>(uint[] exceptPlayerIdList, T message) where T : struct, NetworkMessage
        {
            foreach (var connection in NetworkServer.connections.Values)
                if (!exceptPlayerIdList.Contains(connection.identity.netId))
                    connection.Send(message);
        }
    }
}