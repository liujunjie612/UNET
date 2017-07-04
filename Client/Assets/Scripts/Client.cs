﻿using Message_Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour {

    public static NetworkConnection conn;

    private const int Max_Connection = 100;
    private NetworkClient myClient;
    private string IPAdress = "127.0.0.1";
    private int Port = 3000;

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

        Debug.Log("Send connect");
    }

    private void registerHandler()
    {
        myClient.RegisterHandler(MsgType.Connect, __onConn);
        myClient.RegisterHandler(MsgType.Disconnect, __onDisconn);

        myClient.RegisterHandler(MessageType_Client.ConnectToGameServer, __onConnectionToGameServer);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Notify_T n = new Notify_T();
            n.s = "Liu";
            conn.Send(MessageType_Client.T, n);

            Debug.Log("send Liu");
        }
    }

    private void __onConn(NetworkMessage msg)
    {
        conn = myClient.connection;
        Debug.Log("connected successful");

        MasterServerRsp rsp = new MasterServerRsp();
        myClient.Send(MessageType_Client.MasterServerRsp, rsp);
        Debug.Log("send MasterServerRsp");
        
    }

    private void __onDisconn(NetworkMessage msg)
    {
        Debug.Log("disconnected");
    }

    private void __onConnectionToGameServer(NetworkMessage msg)
    {
        ConnectionToGameServerRsp rsp = msg.ReadMessage<ConnectionToGameServerRsp>();

        myClient.ReconnectToNewHost(rsp.ipAdress, rsp.port);

        Debug.Log("切换到GameServer:" + rsp.ipAdress + "  " + rsp.port);
    }
}
