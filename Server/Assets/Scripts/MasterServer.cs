using Message_MasterServer;
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

    private List<NetworkConnection> _gameServerList = new List<NetworkConnection>();
    private Dictionary<int, GameServerVo> _gameServerPlayersDic = new Dictionary<int, GameServerVo>();

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

            NetworkServer.RegisterHandler(MessageType_MasterServer.T, __onT);
            NetworkServer.RegisterHandler(MessageType_MasterServer.MasterServerRsp, __onMasterServerRsp);
        }
        Log.Instance.Info("服务器已开启");
    }

    private void __onMaxConnectionInputChanged(string input)
    {
        _maxConnection = int.Parse(input);
    }

    private void __onPortInputChanged(string input)
    {
        _port = int.Parse(input);
    }

    private void clientConnenctToGameServer(NetworkConnection cnn, string ipAdress, int port)
    {
        ConnectionGameServerNotify notify = new ConnectionGameServerNotify();
        notify.ipAdress = ipAdress;
        notify.port = port;

        NetworkServer.SendToClient(cnn.connectionId, MessageType_MasterServer.ConnectionGameServerNotify, notify);
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

    private void __onMasterServerRsp(NetworkMessage msg)
    {
        MasterServerRsp rsp = msg.ReadMessage<MasterServerRsp>();

        Log.Instance.Info("type:" + rsp.type);
        if (rsp.type == 0)
        {
            int p = -1;
            for (int i = 0; i < _gameServerList.Count; i++)
            {
                if (_gameServerPlayersDic[(_gameServerList[i].connectionId)].playerCount < 2)
                {
                    p = _gameServerPlayersDic[(_gameServerList[i].connectionId)].port;
                    _gameServerPlayersDic[(_gameServerList[i].connectionId)].playerCount++;
                    break;
                }
            }

            if(p == -1)
            {
                //这是服务器满员了，需要创建新的服务器
                Log.Instance.Info("所有服务器满员了，需要创建新的服务器");
                return;
            }
            clientConnenctToGameServer(msg.conn, "127.0.0.1", p);
        }
        else
        {
            GameServerNotify notify = new GameServerNotify();
            notify.maxConnection = 100;
            notify.port = _gameServerList.Count + 1000;

            NetworkServer.SendToClient(msg.conn.connectionId, MessageType_MasterServer.GameServerNotify, notify);

            _gameServerList.Add(msg.conn);
            GameServerVo v = new GameServerVo ();
            v.port = notify.port;
            v.playerCount = 0;
            _gameServerPlayersDic.Add(msg.conn.connectionId, v);
        }
    }
}
