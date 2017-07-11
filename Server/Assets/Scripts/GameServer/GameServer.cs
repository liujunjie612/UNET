using Message_GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameServer : MonoBehaviour {

    public static NetworkConnection masterConn;

    private const int _max_Connection = 100;
    private NetworkClient myMasterClient;
    private string _masterServerIPAdress = "127.0.0.1";
    private int _masterServerPort = 3000;


    public static NetworkConnection sqlConn;

    private NetworkClient mySqlClient;
    private string _sqlServerIPAdress = "127.0.0.1";
    private int _sqlServerPort = 4000;

    private int _playerCount = 0;

    void Start()
    {
        connectMasterServer();

        connectSqlServer();

        registerHandler();
    }

    private void connectMasterServer()
    {
        ConnectionConfig conf = new ConnectionConfig();
        conf.AddChannel(QosType.Reliable);
        conf.AddChannel(QosType.Unreliable);
        myMasterClient = new NetworkClient();
        myMasterClient.Configure(conf, 1);

        myMasterClient.Connect(_masterServerIPAdress, _masterServerPort);

        Log.Instance.Info("send masterServer connect");
    }


    private void connectSqlServer()
    {
        ConnectionConfig conf = new ConnectionConfig();
        conf.AddChannel(QosType.Reliable);
        conf.AddChannel(QosType.Unreliable);
        mySqlClient = new NetworkClient();
        mySqlClient.Configure(conf, 1);

        mySqlClient.Connect(_sqlServerIPAdress, _sqlServerPort);

        Log.Instance.Info("send sqlServer connect");
    }
    

    private void registerHandler()
    {
        myMasterClient.RegisterHandler(MsgType.Connect, __onMasterConn);
        myMasterClient.RegisterHandler(MessageType.GameServerNotify, __onStartServer);

        mySqlClient.RegisterHandler(MsgType.Connect, __onSqlConn);
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
            NetworkServer.RegisterHandler(MessageType.MasterServerRsp, __onMasterServerRsp);
            NetworkServer.RegisterHandler(MsgType.Disconnect, __onDisconn);


            NetworkServer.RegisterHandler(MessageType.T, __onT);
        }
        Log.Instance.Info("服务器已开启");

        GameServerOpenedNotify n = new GameServerOpenedNotify();
        myMasterClient.Send(MessageType.GameServerOpenedNotify, n);
        Log.Instance.Info("发送服务器已开启通知");
    }

    private void __onMasterConn(NetworkMessage msg)
    {
        masterConn = myMasterClient.connection;
        Log.Instance.Info("connect masterServer successful");

        MasterServerRsp rsp = new MasterServerRsp();

        myMasterClient.Send(MessageType.MasterServerRsp, rsp);
        Log.Instance.Info("send MasterServerRsp");
    }

    private void __onSqlConn(NetworkMessage msg)
    {
        sqlConn = mySqlClient.connection;
        Log.Instance.Info("connect sqlServer successful");
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
        myMasterClient.Send(MessageType.PlayerOfflineNotify, notify);
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
        NetworkServer.SendToClient(msg.conn.connectionId, MessageType.T, t);
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
        {
            myMasterClient.Disconnect();
            Application.Quit();
        }
    }
}
