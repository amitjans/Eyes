using Assets.Scripts;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using socket.io;
using UnityEngine.UI;

public class EyesBehaviourScript : MonoBehaviour
{
    public Camera camera;
    private Animator anim { get; set; }
    private Data message;

    private bool msg;
    private float noanim;
    private float nextmax;

    // Start is called before the first frame update
    [Obsolete]
    private void Start()
    {
        anim = GetComponent<Animator>();
        message = new Data();
        msg = false;
        noanim = 0;
        nextmax = 0;
        Connect();
    }

    public void Connect()
    {
        var temp = Application.absoluteURL;
        var serverUrl = (temp.EndsWith("/") ? temp.Substring(0, temp.Length - 1) : temp) + ":3000";
        Debug.Log("Ip: " + serverUrl);
        var socket = Socket.Connect(serverUrl);

        // receive "news" event
        socket.On("messages", (string data) => {
            Debug.Log(data);
            if (data.Contains("anim"))
            {
                message = JsonUtility.FromJson<Data>(data);
                msg = true;
            }
        });
    }

    // Update is called once per frame
    private void Update()
    {
        if (!string.IsNullOrEmpty(message.bcolor))
        {
            camera.backgroundColor = message.getValueOfBColor();
        }

        if (message.anim.Equals("exit"))
        {
            Application.Quit();
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Blink"))
        {
            anim.SetInteger("Option", 0);
        }

        if (msg && !anim.GetCurrentAnimatorStateInfo(0).IsName("Blink"))
        {
            anim.speed = message.speed;
            anim.SetInteger("Option", message.getValueOfAnim());
            msg = false;
            noanim = 0;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("New State"))
        {
            noanim += Time.deltaTime;
            if (noanim >= nextmax)
            {
                anim.SetInteger("Option", 9);
                noanim = 0;
                nextmax = Random.Range(5f, 7f);
            }
        }
    }
}