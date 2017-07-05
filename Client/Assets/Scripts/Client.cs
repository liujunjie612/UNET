using Message_Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {
    public GameObject loadingImg;

    public static NetworkConnection conn;

    private const int Max_Connection = 100;
    private NetworkClient myClient;
    private string IPAdress = "127.0.0.1";
    private int Port = 3000;

    private int _connectTimes = 0;

    void Start()
    {
        loadingImg.SetActive(true);

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

        Log.Instance.Info("Send connect");
    }

    private void registerHandler()
    {
        myClient.RegisterHandler(MsgType.Connect, __onConn);
        myClient.RegisterHandler(MsgType.Disconnect, __onDisconn);

        myClient.RegisterHandler(MessageType_Client.ConnectToGameServer, __onConnectionToGameServer);
        myClient.RegisterHandler(MessageType_Client.T, __onT);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Notify_T n = new Notify_T();
            n.s = "Liu";
            conn.Send(MessageType_Client.T, n);

            Log.Instance.Info("send Liu");
        }
    }

    private void __onConn(NetworkMessage msg)
    {
        conn = myClient.connection;
        Log.Instance.Info("connected successful");

        MasterServerRsp rsp = new MasterServerRsp();
        myClient.Send(MessageType_Client.MasterServerRsp, rsp);
        Log.Instance.Info("send MasterServerRsp");

        _connectTimes++;
        if (_connectTimes > 1)
            loadingImg.SetActive(false);
    }

    private void __onDisconn(NetworkMessage msg)
    {
        Log.Instance.Info("send disconnected");
    }

    private void __onConnectionToGameServer(NetworkMessage msg)
    {
        ConnectionToGameServerRsp rsp = msg.ReadMessage<ConnectionToGameServerRsp>();

        myClient.ReconnectToNewHost(rsp.ipAdress, rsp.port);

        Log.Instance.Info("切换到GameServer:" + rsp.ipAdress + "  " + rsp.port);
    }

    private void __onT(NetworkMessage msg)
    {
        Notify_T notify = msg.ReadMessage<Notify_T>();

        Log.Instance.Info("Receive：" + notify.s);
    }
}
