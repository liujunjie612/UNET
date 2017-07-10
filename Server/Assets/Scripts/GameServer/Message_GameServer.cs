using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Message_GameServer
{
    /// <summary>
    /// 告诉MasterServer类型
    /// </summary>
    public class MasterServerRsp: MessageBase
    {
        private int type = 1;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(type);
        }
    }

    public class Notify_T : MessageBase
    {
        public string s;

        public override void Deserialize(NetworkReader reader)
        {
            s = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(s);
        }
    }

    public class GameServerNotify: MessageBase
    {
        public int maxConnection;
        public int port;

        public override void Deserialize(NetworkReader reader)
        {
            maxConnection = reader.ReadInt32();
            port = reader.ReadInt32();
        }
    }

    public class PlayerOfflineNotify: MessageBase
    {
        public int playerConnId;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(playerConnId);
        }
    }

    public class GameServerOpenedNotify: MessageBase
    {
        private bool opened = true;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(opened);
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
