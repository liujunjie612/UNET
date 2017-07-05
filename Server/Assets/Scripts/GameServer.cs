using Message_GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameServer : MonoBehaviour {

    public static NetworkConnection conn;

    private const int Max_Connection = 100;
    private NetworkClient myClient;
    private string IPAdress = "127.0.0.1";
    private int Port = 3000;

    private int _playerCount = 0;

    void Start()
    {
        connect();

        registerHandler();
    }

    private void connect()
    {
        ConnectionConfig conf = new ConnectionConfig();
        conf.AddChannel(QosType.Reliable);
        conf.AddChannel(QosType.Unreliable);
        myClient = new NetworkClient();
        myClient.Configure(conf, 1);

        myClient.Connect(IPAdress, Port);

        Log.Instance.Info("send connect");
    }

    

    private void registerHandler()
    {
        myClient.RegisterHandler(MsgType.Connect, __onMasterConn);
        myClient.RegisterHandler(MessageType_GameServer.GameServerNotify, __onStartServer);

    }

    private void __onStartServer(NetworkMessage msg)
    {
        GameServerNotify notify = msg.ReadMessage<GameServerNotify>();

        if (NetworkServer.active)
        {
            Log.Instance.Info("服务器已启动，请勿重复开启！");
            return;
        }

        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.Reliable);
        config.AddChannel(QosType.Unreliable);

        HostTopology host = new HostTopology(config, notify.maxConnection);
        NetworkServer.Configure(host);
        if (NetworkServer.Listen(notify.port))
        {
            NetworkServer.RegisterHandler(MsgType.Connect, __onConn);
            NetworkServer.RegisterHandler(MessageType_GameServer.MasterServerRsp, __onMasterServerRsp);
            NetworkServer.RegisterHandler(MsgType.Disconnect, __onDisconn);

            NetworkServer.RegisterHandler(MessageType_GameServer.T, __onT);
        }
        Log.Instance.Info("服务器已开启");

        GameServerOpenedNotify n = new GameServerOpenedNotify();
        myClient.Send(MessageType_GameServer.GameServerOpenedNotify, n);
        Log.Instance.Info("发送服务器已开启通知");
    }

    private void __onMasterConn(NetworkMessage msg)
    {
        conn = myClient.connection;
        Log.Instance.Info("connect successful");

        MasterServerRsp rsp = new MasterServerRsp();

        myClient.Send(MessageType_GameServer.MasterServerRsp, rsp);
        Log.Instance.Info("send MasterServerRsp");
    }

    private void __onConn(NetworkMessage msg)
    {
        NetworkServer.SetClientReady(msg.conn);
        _playerCount++;
        Log.Instance.Info("玩家：" + msg.conn + "上线");
    }

    private void __onDisconn(NetworkMessage msg)
    {
        //玩家下线通知MasterServer，更新GameServer人数
        PlayerOfflineNotify notify = new PlayerOfflineNotify();
        notify.playerConnId = msg.conn.connectionId;
        myClient.Send(MessageType_GameServer.PlayerOfflineNotify, notify);
        _playerCount--;

        Log.Instance.Info("玩家：" + msg.conn + "下线，剩余在线玩家数：" + _playerCount);

        if (_playerCount <= 0)
            StartCoroutine("shutDown");
    }

    private void __onMasterServerRsp(NetworkMessage msg)
    {
        //不处理
    }

    private void __onT(NetworkMessage msg)
    {
        Notify_T n = msg.ReadMessage<Notify_T>();

        Log.Instance.Info("Receive From" + msg.conn.connectionId + "：" + n.s);

        Notify_T t = new Notify_T();
        t.s = "junjie cool";
        NetworkServer.SendToClient(msg.conn.connectionId, MessageType_GameServer.T, t);
        Log.Instance.Info("send to " + msg.conn.connectionId + " :" + t.s);
    }

    /// <summary>
    /// 服务器所连的客户端为0时，断开连接
    /// </summary>
    /// <returns></returns>
    IEnumerator shutDown()
    {
        yield return new WaitForSeconds(10);

        if (_playerCount <= 0)
            Application.Quit();
    }
}
