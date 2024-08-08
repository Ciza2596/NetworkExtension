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

        public static void SendMessageToAllClient<TMessage>(TMessage message, uint[] exceptPlayerIdList) where TMessage : struct, NetworkMessage
        {
            foreach (var connection in NetworkServer.connections.Values)
                if (exceptPlayerIdList == null || !exceptPlayerIdList.Contains(connection.identity.netId))
                    connection.Send(message);
        }
    }
}