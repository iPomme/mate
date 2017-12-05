using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Globalization;
using UnityEngine.UI;

public class WebsocketServer : MonoBehaviour
{

    public const float TranslationDamper = 1;
    public const float RotationDamper = 1;

    public GameObject x_val;
    public GameObject y_val;
    public GameObject b_val;
    public GameObject weight_val;
    public GameObject particle;
    public GameObject fire;

    public const float lowerbound = 4.8f;
    public const float upperbound = 5.2f;

    public GameObject pingu;

    private WebSocketServer wssv;

    public static float weight = 0.1f;

    public static Vector4 position = new Vector4(0, 0, 0, 0);

    public static float explosion = 0;

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
        //computePositionAccordingKeyboard();
        move();

        //computeWeightAccordingKeyboard();
        //grow();

        updateHUD();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            explosion = 1;     // FORWARD
        }
        if (explosion > 0.5f){
            explosion = 0;
            pingu.GetComponent<Animator>().enabled = true;
            pingu.GetComponent<AudioSource>().enabled = true;
            particle.SetActive(true);
            fire.SetActive(true);
        }
    }

    void OnDestroy()
    {
        wssv.Stop();
    }

    void move()
    {
        if (Mathf.Abs(position.x) >= TranslationDamper)
        {
            var sens = position.x > 0 ? Vector3.forward : Vector3.back;
            pingu.transform.Translate(sens * Time.deltaTime);
        }
        if (Mathf.Abs(position.y) >= RotationDamper)
        {
            pingu.transform.Rotate(0, Time.deltaTime * 40 * position.y / Mathf.Abs(position.y), 0);
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

    void updateHUD(){
        x_val.GetComponent<Text>().text = "X: " + position.x.ToString();
        y_val.GetComponent<Text>().text = "Y: " + position.y.ToString();
        b_val.GetComponent<Text>().text = "B: " + explosion;
        weight_val.GetComponent<Text>().text = "W: " + weight.ToString();
    }

    void computeWeightAccordingKeyboard()
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
        Debug.Log("computePositionAccordingKeyboard");
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                position = new Vector4(TranslationDamper, 0, 0, 0);     // FORWARD
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                position = new Vector4(-TranslationDamper, 0, 0, 0);    // BACKWARD
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                position = new Vector4(0, -RotationDamper, 0, 0);       // LEFT
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                position = new Vector4(0, RotationDamper, 0, 0);        // RIGHT
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
        float value = float.Parse(splitted[1], CultureInfo.InvariantCulture.NumberFormat);
        if (header == "S")
        { // SCALE
            WebsocketServer.weight = Mathf.Max(value, 0.1f);
        }
        else if (header.StartsWith("GY"))
        { // GYRO
            if (header == "GY-X")
            {
                //value -= 100;
                WebsocketServer.position.x = value;
                Debug.Log("Got " + header + " (" + value + ")");
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
				//Debug.Log("Got " + header + " (" + value + ")");
            }
			
        }
        else if (header.StartsWith("BA"))
        { // BAROMETER
            if (header == "BA-EXPLOSION")
            {
                WebsocketServer.explosion = value;
            }
        }


        //Debug.Log("WW: " + WebsocketServer.weight);
    }


}
