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

    public class LoginReq : MessageBase
    {
        public string name;
        public string psd;

        public int connId;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(name);
            writer.Write(psd);

            writer.Write(connId);
        }

        public override void Deserialize(NetworkReader reader)
        {
            name = reader.ReadString();
            psd = reader.ReadString();
        }
    }

    public class LoginRsp : MessageBase
    {
        public string error;

        public int connId;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(error);
        }

        public override void Deserialize(NetworkReader reader)
        {
            error = reader.ReadString();
            connId = reader.ReadInt32();
        }
    }
}