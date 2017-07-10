using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Message_SqlServer
{
    public class Login : MessageBase
    {
        public string name;
        public string psd;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(name);
            writer.Write(psd);
        }

        public override void Deserialize(NetworkReader reader)
        {
            name = reader.ReadString();
            psd = reader.ReadString();
        }
    }
}