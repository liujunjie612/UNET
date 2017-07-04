using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Message_MasterServer
{
    public class Notify_T : MessageBase
    {
        public string s;

        public override void Deserialize(NetworkReader reader)
        {
            s = reader.ReadString();
        }
    }

    public class ConnectionGameServerNotify: MessageBase
    {
        public string ipAdress;
        public int port;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(ipAdress);
            writer.Write(port);
        }
    }

    public class MasterServerRsp :MessageBase
    {
        public int type;

        public override void Deserialize(NetworkReader reader)
        {
            type = reader.ReadInt32();
        }
    }

    public class GameServerNotify: MessageBase
    {
        public int maxConnection;
        public int port;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(maxConnection);
            writer.Write(port);
        }
    }
}