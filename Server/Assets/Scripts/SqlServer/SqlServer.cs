﻿using Message_SqlServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SqlServer : MonoBehaviour
{
    public static NetworkConnection conn;

    private const int _max_Connection = 100;
    private NetworkClient myMasterClient;
    private string IPAdress = "127.0.0.1";
    private int Port = 4000;

    void Start()
    {
        startServer();

        SqlConnection.SqlConnect();
        SqlConnection.GetAllDB();
    }

    private void startServer()
    {
        if (NetworkServer.active)
        {
            Log.Instance.Info("服务器已启动，请勿重复开启！");
            return;
        }

        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.Reliable);
        config.AddChannel(QosType.Unreliable);

        HostTopology host = new HostTopology(config, _max_Connection);
        NetworkServer.Configure(host);
        if (NetworkServer.Listen(Port))
        {
            NetworkServer.RegisterHandler(MsgType.Connect, __onConn);
            NetworkServer.RegisterHandler(MsgType.Disconnect, __onDisconn);

            NetworkServer.RegisterHandler(MessageType.LoginReq, __onLoginReq);
        }
        Log.Instance.Info("服务器已开启");

    }

    private void __onConn(NetworkMessage msg)
    {
        NetworkServer.SetClientReady(msg.conn);
        Log.Instance.Info(msg.conn + " 连接");
    }

    private void __onDisconn(NetworkMessage msg)
    {
        Log.Instance.Info("connectionId：" + msg.conn.connectionId + "下线");
    }

    private void __onLoginReq(NetworkMessage msg)
    {
        LoginReq req = msg.ReadMessage<LoginReq>();
        Log.Instance.Info("收到玩家密码请求：" + req.name);

        LoginRsp rsp = new LoginRsp();
        string psd = SqlLogin.GetAccountPassword(req.name);
        if (string.IsNullOrEmpty(psd) || !psd.Equals(req.psd))
        {
            rsp.error = "昵称或密码错误";
        }
        else
        {
            rsp.error = "";
        }
        rsp.connId = req.connId; 
        NetworkServer.SendToClient(msg.conn.connectionId, MessageType.LoginRsp, rsp);
        Log.Instance.Info("回复玩家密码请求：" + rsp.error);
    }
}
