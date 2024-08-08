using System;
using System.Globalization;
using System.Linq;
using Mirror;

namespace CizaMirrorNetworkExtension
{
    public static class MirrorNetworkUtils
    {
        public static void SendMessageToClient<TMessage>(string playerId, TMessage message) where TMessage : struct, NetworkMessage
        {
            if (playerId == MirrorNetworkHandler.OfflinePlayerId)
                return;

            if (!UInt32.TryParse(playerId, NumberStyles.None, CultureInfo.InvariantCulture, out var netId))
                return;

            foreach (var connection in NetworkServer.connections.Values)
                if (connection.identity.netId == playerId.ToUint())
                {
                    connection.Send(message);
                    return;
                }
        }

        public static void SendMessageToAllClient<TMessage>(TMessage message, string[] exceptPlayerIdList) where TMessage : struct, NetworkMessage
        {
            var uints = exceptPlayerIdList.ToUint();

            foreach (var connection in NetworkServer.connections.Values)
                if (exceptPlayerIdList == null || !uints.Contains(connection.identity.netId))
                    connection.Send(message);
        }
    }
}