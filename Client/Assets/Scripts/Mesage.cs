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
}
