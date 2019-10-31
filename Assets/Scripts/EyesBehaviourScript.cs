using Assets.Scripts;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class EyesBehaviourScript : MonoBehaviour
{
    public Camera camera;
    public Text text;
    private Animator anim { get; set; }
    private bool state { get; set; }
    private string[] names { get; set; }
    private MqttClient client;
    private Data message;

    private bool first;
    private bool server;
    private string address;
    private string clientId;

    // Start is called before the first frame update
    [Obsolete]
    private void Start()
    {
        anim = GetComponent<Animator>();
        names = new[] { "Angry 0", "Bored 0", "Sad 0", "Happy 0", "Surprised 0" };
        state = true;
        message = new Data();
        first = true;
        server = false;
        address = GetLocalIPAddress();
        clientId = Guid.NewGuid().ToString();
    }

    private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        var mqtt = System.Text.Encoding.UTF8.GetString(e.Message);
        //{"anim":"ini","bcolor":"blue","extra":false}
        message = JsonUtility.FromJson<Data>(mqtt);
    }

    IEnumerator Fade()
    {
        var save = new Save();
        if (LoadAndSave.DataSaved())
        {
            save = LoadAndSave.LoadData();
            //Debug.Log(save.List[0]);
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
                    message = new Data() { anim = "blink", bcolor = "", extra = false };
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
                message = new Data() { anim = "blink", bcolor = "", extra = false };
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
        text.text = "";
        yield return null;
    }

    // Update is called once per frame
    private void Update()
    {
        var temp = anim.GetCurrentAnimatorStateInfo(0);

        if (!string.IsNullOrEmpty(message.bcolor))
        {
            camera.backgroundColor = message.getValueOfBColor();
        }

        if (message.anim.Equals("exit"))
        {
            client.Disconnect();
            Application.Quit();
        }

        if (temp.IsName("Blink"))
        {
            message = new Data();
        }

        if (!temp.IsName("New State"))
        {
            if (temp.IsName(names[0]) || temp.IsName(names[1]) || temp.IsName(names[2]) || temp.IsName(names[3]) || temp.IsName(names[4]))
            {
                state = true;
            }
            if (!temp.IsName(names[0]) && !temp.IsName(names[1]) && !temp.IsName(names[2]) && !temp.IsName(names[3]) && !temp.IsName(names[4]) && state)
            {
                state = false;
                message = new Data();
                anim.SetInteger("Option", message.getValueOfAnim());
            }
        }

        if (first)
        {
            Debug.Log("first: " + first);
            first = false;
            StartCoroutine("Fade");
        }
        else if (server)
        {
            Debug.Log("server: " + server);
            StopAllCoroutines();
            server = false;
        }
        anim.SetInteger("Option", message.getValueOfAnim());
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}