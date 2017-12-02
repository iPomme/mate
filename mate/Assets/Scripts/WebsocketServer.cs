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

    public static Vector4 position = new Vector4(0, 0, 0, 0);

    private float moveCoeff = 5;


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
        computePositionAccordingKeyboard();
        move();

        computeWeigthAccordingKeyboard();
        grow();
    }

    void OnDestroy()
    {
        wssv.Stop();
    }

    bool has(float v) {
        return v <= -1 || v >= 1;
    }

    void move()
    {
        if (has(position.x))
        {
            var sens = position.x > 0 ? Vector3.forward : Vector3.back;
            pingu.transform.Translate(sens * Time.deltaTime * 10);
        }
        if (has(position.y))
        {
            pingu.transform.Rotate(0, Time.deltaTime * 40 * position.y, 0);
        }
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
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
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

    void computePositionAccordingKeyboard()
    {
        position = new Vector4(0, 0, 0, 0);

        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                position.x = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                position.x = -1;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                position.y = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                position.y = 1;
            }
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
                WebsocketServer.position.x = value;
            }
            else if (header == "GY-Y")
            {
                WebsocketServer.position.y = value;
            }
            else if (header == "GY-Z")
            {
                WebsocketServer.position.z = value;
            }
            else if (header == "GY-H")
            {
                WebsocketServer.position.w = value;
            }
        }
        else if (header.StartsWith(""))
        { // GYRO

        }


        Debug.Log("WW: " + WebsocketServer.weight);
    }


}
