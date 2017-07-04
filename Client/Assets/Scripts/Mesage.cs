using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Message_Client
{
    public class Notify_T : MessageBase
    {
        public string s;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(s);
        }
    }

    public class ConnectionToGameServerRsp: MessageBase
    {
        public string ipAdress;
        public int port;

        public override void Deserialize(NetworkReader reader)
        {
            ipAdress = reader.ReadString();
            port = reader.ReadInt32(); 
        }
    }

    public class MasterServerRsp: MessageBase
    {
        private int type = 0;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(type);
        }
    }
}
