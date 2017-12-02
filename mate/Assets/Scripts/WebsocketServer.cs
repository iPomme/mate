using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebsocketServer : MonoBehaviour {


	// Use this for initialization
	void Start () {
        Debug.Log("Starting websocket server");
        var wssv = new WebSocketServer("ws://0.0.0.0:8086");
        wssv.AddWebSocketService<WSWorker>("/");
        wssv.Start();
        Debug.Log("WebSocket started !");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class WSWorker : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Got message");
        var msg = e.Data == "BALUS"
                  ? "I've been balused already..."
                  : "I'm not available now.";
        Debug.Log(e.Data);
        Send(msg);
    }
}
