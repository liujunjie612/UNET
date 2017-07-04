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
}
