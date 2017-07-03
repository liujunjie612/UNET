using Message_Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MasterServer : MonoBehaviour
{
    private int MaxConnection = 100;
    private int Port = 3000;

    void Start()
    {
        startServer();
    }

    private void startServer()
    {
        ConnectionConfig config = new ConnectionConfig();
        config.AddChannel(QosType.Reliable);
        config.AddChannel(QosType.Unreliable);
        
        HostTopology host = new HostTopology(config, MaxConnection);
        NetworkServer.Configure(host);
        if (NetworkServer.Listen(Port))
        {

            NetworkServer.RegisterHandler(MsgType.Connect, OnConn);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconn);

            NetworkServer.RegisterHandler(MessageType_Server.T, __onT);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Servewr:" + NetworkServer.active + NetworkServer.connections.Count);
        }
    }

    private void OnConn(NetworkMessage msg)
    {
        NetworkServer.SetClientReady(msg.conn);

        Connection_Notify n = new Connection_Notify ();
        n.isReady = true;
        NetworkServer.SendToClient(msg.conn.connectionId, MsgType.Connect, n);
        Debug.Log("玩家：" + msg.conn + "上线");
    }

    private void OnDisconn(NetworkMessage msg)
    {
        Debug.Log("玩家：" + msg.conn + "下线");
    }

    private void __onT(NetworkMessage msg)
    {
        Notify_T n = msg.ReadMessage<Notify_T>();

        Debug.Log("Receive：" + n.s);
    }
}
