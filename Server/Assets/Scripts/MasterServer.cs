using Message_Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MasterServer : MonoBehaviour
{
    public InputField maxConnectionInput;
    public InputField portInput;
    public Button startServerBtn;


    private int _maxConnection = 100;
    private int _port = 3000;

    void Start()
    {
        maxConnectionInput.text = _maxConnection.ToString();
        portInput.text = _port.ToString();

        startServerBtn.onClick.AddListener(this.startServer);
        maxConnectionInput.onValueChanged.AddListener(this.__onMaxConnectionInputChanged);
        portInput.onValueChanged.AddListener(this.__onPortInputChanged);
    }

    private void startServer()
    {
        if(NetworkServer.active)
        {
            Log.Instance.Info("服务器已启动，请勿重复开启！");
            return;
        }

        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.Reliable);
        config.AddChannel(QosType.Unreliable);

        HostTopology host = new HostTopology(config, _maxConnection);
        NetworkServer.Configure(host);
        if (NetworkServer.Listen(_port))
        {
            NetworkServer.RegisterHandler(MsgType.Connect, __onConn);
            NetworkServer.RegisterHandler(MsgType.Disconnect, __onDisconn);

            NetworkServer.RegisterHandler(MessageType_Server.T, __onT);
        }
    }

    private void __onMaxConnectionInputChanged(string input)
    {
        _maxConnection = int.Parse(input);
    }

    private void __onPortInputChanged(string input)
    {
        _port = int.Parse(input);
    }

    private void __onConn(NetworkMessage msg)
    {
        NetworkServer.SetClientReady(msg.conn);
        Log.Instance.Info("玩家：" + msg.conn + "上线");
    }

    private void __onDisconn(NetworkMessage msg)
    {
        Log.Instance.Info("玩家：" + msg.conn + "下线");
    }

    private void __onT(NetworkMessage msg)
    {
        Notify_T n = msg.ReadMessage<Notify_T>();

        Log.Instance.Info("Receive：" + n.s);
    }
}
