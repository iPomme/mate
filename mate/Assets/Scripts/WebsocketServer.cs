using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;


public class WebsocketServer : MonoBehaviour {

	public  GameObject pingu;
   

	// Use this for initialization
	void Start () {
        WSWorker.pingu = pingu;
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
	static public GameObject pingu;
	protected override void OnMessage(MessageEventArgs e)

    {
        Debug.Log("Got message");
        Debug.Log(e.Data);
        var weight = Mathf.Abs(float.Parse(e.Data));
		var factor = weight / 200;
       pingu.transform.localScale = new Vector3(factor, factor, factor);
    }
}
