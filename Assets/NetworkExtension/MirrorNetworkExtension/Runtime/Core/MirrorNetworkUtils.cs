using System;
using System.Linq;
using Mirror;

namespace CizaMirrorNetworkExtension
{
    public static class MirrorNetworkUtils
    {
        public static void SendMessageToClient<TMessage>(uint playerId, TMessage message) where TMessage : struct, NetworkMessage
        {
            foreach (var connection in NetworkServer.connections.Values)
                if (connection.identity.netId == playerId)
                {
                    connection.Send(message);
                    return;
                }
        }

        public static void SendMessageToAllClient<T>(T message) where T : struct, NetworkMessage =>
            SendMessageToAllClient(Array.Empty<uint>(), message);

        public static void SendMessageToAllClient<T>(uint[] exceptPlayerIdList, T message) where T : struct, NetworkMessage
        {
            foreach (var connection in NetworkServer.connections.Values)
                if (!exceptPlayerIdList.Contains(connection.identity.netId))
                    connection.Send(message);
        }
    }
}