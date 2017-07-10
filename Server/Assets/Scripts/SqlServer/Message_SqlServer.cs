using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Message_SqlServer
{
    public class LoginReq : MessageBase
    {
        public string name;
        public string psd;

        public int connId;

        public override void Deserialize(NetworkReader reader)
        {
            name = reader.ReadString();
            psd = reader.ReadString();

            connId = reader.ReadInt32();
        }
    }

    public class LoginRsp: MessageBase
    {
        public string error;

        public int connId;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(error);
            writer.Write(connId);
        }
    }
}