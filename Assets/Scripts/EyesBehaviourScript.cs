using Assets.Scripts;
using System;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class EyesBehaviourScript : MonoBehaviour
{
    public Camera camera;
    private Animator anim { get; set; }
    private bool state { get; set; }
    private string[] names { get; set; }
    private MqttClient client;
    private Data message;

    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        names = new[] { "Angry 0", "Bored 0", "Sad 0", "Happy 0", "Surprised 0" };
        state = true;
        message = new Data();

        client = new MqttClient("127.0.0.1", 1883, false, null);

        // register to message received
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        // subscribe to the topic "/home/temperature" with QoS 2
        client.Subscribe(new string[] { "eve/eyes" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
    }

    private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        var mqtt = System.Text.Encoding.UTF8.GetString(e.Message);
        //{"anim":"ini","bcolor":"blue","extra":false}
        //Debug.Log("Received: " + mqtt);
        message = JsonUtility.FromJson<Data>(mqtt);
    }

    // Update is called once per frame
    private void Update()
    {
        var temp = anim.GetCurrentAnimatorStateInfo(0);

        if (!string.IsNullOrEmpty(message.bcolor))
        {
            camera.backgroundColor = message.getValueOfBColor();
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
        anim.SetInteger("Option", message.getValueOfAnim());
    }
}