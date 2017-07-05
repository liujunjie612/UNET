using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Message_MasterServer
{
    public class ConnectionGameServerNotify : MessageBase
    {
        public string ipAdress;
        public int port;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(ipAdress);
            writer.Write(port);
        }
    }

    public class MasterServerRsp : MessageBase
    {
        public int type;

        public override void Deserialize(NetworkReader reader)
        {
            type = reader.ReadInt32();
        }
    }

    public class GameServerNotify : MessageBase
    {
        public int maxConnection;
        public int port;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(maxConnection);
            writer.Write(port);
        }
    }

    public class PlayerOfflineNotify : MessageBase
    {
        public int playerConnId;

        public override void Deserialize(NetworkReader reader)
        {
            playerConnId = reader.ReadInt32();
        }
    }

    public class GameServerOpenedNotify : MessageBase
    {
        public bool opened;

        public override void Deserialize(NetworkReader reader)
        {
            opened = reader.ReadBoolean();
        }
    }
}