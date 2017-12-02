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

    public static float weight = 0.1f;


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
        computeWeigthAccordingKeyboard();
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
            weight = 0.1f;
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
            weight = Mathf.Max((float)(weight / 1.003), 0.1f);
        }
        else if (weight > upperbound)
        {
            pingu.transform.localScale = new Vector3(weight, weight, weight);
            pingu.GetComponent<Renderer>().material.color = Color.red;
            weight = Mathf.Max((float)(weight / 1.003), 0.1f);
        }
        else
        {
            pingu.transform.localScale = new Vector3(weight, weight, weight);
            pingu.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    void computeWeigthAccordingKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            weight = (float)(weight * 1.4);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            weight = Mathf.Max((float)(weight / 1.4), 0.1f);
        }
    }
}

public class WSWorker : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        var msg = e.Data;

        string[] splitted = msg.Split(':');
        string header = splitted[0];
        float value = Mathf.Abs(float.Parse(splitted[1], CultureInfo.InvariantCulture.NumberFormat));
        if (header == "S")
        { // SCALE
            WebsocketServer.weight = Mathf.Max(value, 0.1f);
        }
        else if (header.StartsWith("GY"))
        { // GYRO
            if (header == "GY-X")
            {
                Debug.Log("Got X axis (" + value + ")");
            }
            else if (header == "GY-Y")
            {
                Debug.Log("Got Y axis (" + value + ")");

            }
            else if (header == "GY-H")
            {
                Debug.Log("Got H axis (" + value + ")");

            }
        }
        else if (header.StartsWith(""))
        { // GYRO

        }


        Debug.Log("WW: " + WebsocketServer.weight);
    }


}
