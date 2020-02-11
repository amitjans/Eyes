using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Random = UnityEngine.Random;

public class EyesBehaviourScript : MonoBehaviour
{
    public Camera camera;
    public Text text;
    public Animator extras;
    private Animator anim { get; set; }
    private MqttClient client;
    private Data message;

    private bool first;
    private bool server;
    private string address;
    private string clientId;
    private bool msg;
    private float noanim;
    private float nextmax;

    // Start is called before the first frame update
    [Obsolete]
    private void Start()
    {
        anim = GetComponent<Animator>();
        message = new Data();
        first = true;
        server = false;
        msg = false;
        noanim = 0;
        nextmax = 0;
        address = LocalIpAddress.GetLocalIPAddress();
        clientId = Guid.NewGuid().ToString();
    }

    private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        var mqtt = System.Text.Encoding.UTF8.GetString(e.Message);
        //{"anim":"ini","bcolor":"blue","extra":false}
        message = JsonUtility.FromJson<Data>(mqtt);
        msg = true;
    }

    private IEnumerator Fade()
    {
        var save = new Save();
        if (LoadAndSave.DataSaved())
        {
            save = LoadAndSave.LoadData();
            if (!string.IsNullOrEmpty(save.GiveMe(address)))
            {
                try
                {
                    var addr = save.GiveMe(address);
                    client = new MqttClient(addr, 1883, false, null);
                    // register to message received
                    client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                    client.Connect(clientId);
                    client.Subscribe(new string[] { "eve/eyes" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                    message = new Data() { anim = "blink", bcolor = "", speed = 1f };
                    msg = true;
                    server = true;
                    text.text = addr;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        var temp = address.Substring(0, address.LastIndexOf("."[0]) + 1);
        for (var ip = 2; ip < 255 && !server; ip++)
        {
            try
            {
                text.text = temp + ip;
                Debug.Log(temp + ip);
                client = new MqttClient(temp + ip, 1883, false, null);
                // register to message received
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                client.Connect(clientId);
                client.Subscribe(new string[] { "eve/eyes" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                message = new Data() { anim = "blink", bcolor = "", speed = 1f };
                msg = true;
                server = true;
                save.Add(temp + ip);
                LoadAndSave.SaveData(save);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            yield return new WaitForSeconds(.001f);
        }
        yield return null;
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
            client.Disconnect();
            Application.Quit();
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Blink"))
        {
            anim.SetInteger("Option", 0);
        }

        if (msg)
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
                nextmax = Random.Range(4f, 7f);
            }
        }
        Debug.Log(noanim);

        if (first)
        {
            first = false;
            StartCoroutine("Fade");
        }
        else if (server)
        {
            text.text = "";
            StopAllCoroutines();
            server = false;
        }
    }
}