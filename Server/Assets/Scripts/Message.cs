using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Message_Server
{
    public class Connection_Notify: MessageBase
    {
        public bool isReady;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(isReady);
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
}