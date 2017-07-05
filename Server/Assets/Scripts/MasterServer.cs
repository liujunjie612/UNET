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

    private bool _onOpeningGameServer = false;
    private bool _onHandling = false;
    private List<NetworkMessage> _catchMsgList = new List<NetworkMessage>();
    private List<NetworkMessage> _handleMsgList = new List<NetworkMessage>();

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

            NetworkServer.RegisterHandler(MessageType_MasterServer.MasterServerRsp, __onMasterServerRsp);
            NetworkServer.RegisterHandler(MessageType_MasterServer.PlayerOfflineNotify, __onPlayerOfflineNotify);
            NetworkServer.RegisterHandler(MessageType_MasterServer.GameServerOpenedNotify, __onGameServerOpenedNotify);
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

    IEnumerator handleCatchMsgList()
    {
        Log.Instance.Info(_handleMsgList.Count + "  " + _catchMsgList.Count);
        _onHandling = true;
        _onOpeningGameServer = false;
        _handleMsgList = _catchMsgList;
        _catchMsgList = new List<NetworkMessage>();
        Log.Instance.Info(_handleMsgList.Count + "  " + _catchMsgList.Count);
        while(_handleMsgList.Count > 0)
        {
            if(_onOpeningGameServer)
            {
                for(int i =0;i<_handleMsgList.Count;i++)
                {
                    _catchMsgList.Add(_handleMsgList[i]);
                }
                _onHandling = false;
                yield break;
            }

            yield return 0;
            sendClient(_handleMsgList[0]);
            _handleMsgList.RemoveAt(0);
        }

        _onHandling = false;
    }

    private void __onConn(NetworkMessage msg)
    {
        NetworkServer.SetClientReady(msg.conn);
        Log.Instance.Info(msg.conn + " 连接");
    }

    private void __onDisconn(NetworkMessage msg)
    {
        bool server = false;
        for (int i = 0; i < _gameServerList.Count;i++ )
        {
            if(_gameServerList[i].connectionId == msg.conn.connectionId)
            {
                server = true;
                _gameServerList.RemoveAt(i);
                break;
            }
        }

        if (server)
        {
            _gameServerPlayersDic.Remove(msg.conn.connectionId);
            Log.Instance.Info("服务器关闭了：" + msg.conn.connectionId);
        }
        else
        {
            Log.Instance.Info("玩家：" + msg.conn + "下线");
        }
    }

    private void __onMasterServerRsp(NetworkMessage msg)
    {
        MasterServerRsp rsp = msg.ReadMessage<MasterServerRsp>();

        if (rsp.type == 0)
            Log.Instance.Info("玩家连线 type:" + rsp.type);
        else
            Log.Instance.Info("服务器连线 type：" + rsp.type);

        if (rsp.type == 0)
        {
            if(_onOpeningGameServer || _onHandling)
            {
                _catchMsgList.Add(msg);
                return;
            }

            sendClient(msg);
        }
        else
        {
            GameServerNotify notify = new GameServerNotify();
            notify.maxConnection = 100;
            notify.port = _gameServerList.Count + 1000;

            NetworkServer.SendToClient(msg.conn.connectionId, MessageType_MasterServer.GameServerNotify, notify);

            Log.Instance.Info("开启服务器端口 port：" + notify.port);

            _gameServerList.Add(msg.conn);
            GameServerVo v = new GameServerVo ();
            v.port = notify.port;
            v.playerCount = 0;
            _gameServerPlayersDic.Add(msg.conn.connectionId, v);
        }
    }

    private void sendClient(NetworkMessage msg)
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

        if (p == -1)
        {
            //这是服务器满员了，需要创建新的服务器
            Log.Instance.Info("所有服务器满员了，需要创建新的服务器");
            Application.OpenURL(@"E:\UNET\Server\GameServer.exe");
            _onOpeningGameServer = true;
            _catchMsgList.Add(msg);
            return;
        }
        clientConnenctToGameServer(msg.conn, "127.0.0.1", p);
        Log.Instance.Info("玩家连接到 port：" + p);
    }

    private void __onPlayerOfflineNotify(NetworkMessage msg)
    {
        PlayerOfflineNotify notify = msg.ReadMessage<PlayerOfflineNotify>();

        if(_gameServerPlayersDic.ContainsKey(msg.conn.connectionId))
        {
            _gameServerPlayersDic[msg.conn.connectionId].playerCount--;
        }
    }

    private void __onGameServerOpenedNotify(NetworkMessage msg)
    {
        GameServerOpenedNotify notify = msg.ReadMessage<GameServerOpenedNotify>();
        Log.Instance.Info("服务器已开启通知");
        StartCoroutine("handleCatchMsgList");
    }
}
