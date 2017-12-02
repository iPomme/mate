using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Globalization;

public class WebsocketServer : MonoBehaviour
{

    public GameObject pingu;

    public const float lowerbound = 4.8f;
    public const float upperbound = 5.2f;

    private WebSocketServer wssv;

    public static float weight = 0;


    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting websocket server");
        wssv = new WebSocketServer("ws://0.0.0.0:8086");
        wssv.AddWebSocketService<WSWorker>("/");
        wssv.Start();
        Debug.Log("WebSocket started !");
    }

    // Update is called once per frame
    void Update()
    {
        grow();
    }

    void OnDestroy()
    {
        wssv.Stop();
    }

    void grow()
    {
        var width = pingu.GetComponent<Renderer>().bounds.size.x;
        if (weight > 10)
        {
            pingu.transform.localScale = new Vector3(weight, weight, weight);
        }
        else if (width > lowerbound && width < upperbound)
        {
            Debug.Log("GOT IT");
        }
        else if (weight < lowerbound)
        {
            pingu.transform.localScale = new Vector3(weight, weight, weight);
            pingu.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (weight > upperbound)
        {
            pingu.transform.localScale = new Vector3(weight, weight, weight);
            pingu.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            pingu.transform.localScale = new Vector3(weight, weight, weight);
            pingu.GetComponent<Renderer>().material.color = Color.green;
        }
    }
}

public class WSWorker : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Got message");
        Debug.Log(e.Data);
        var weight = Mathf.Abs(float.Parse(e.Data, CultureInfo.InvariantCulture.NumberFormat));
        var factor = weight / 200;
        WebsocketServer.weight = weight;
    }


}
