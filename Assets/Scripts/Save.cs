using System;
using System.Linq;

namespace Assets.Scripts
{
    [Serializable]
    public class Save
    {
        public string[] address { get; set; }

        public Save()
        {
            address = new string[0];
        }

        public void Add(string ip)
        {
            var list = address.ToList();
            list.Add(ip);
            address = list.ToArray();
        }

        public string GiveMe(string ip)
        {
            foreach (var t in address)
            {
                if (t.StartsWith(ip.Substring(0, ip.LastIndexOf("."[0]))))
                {
                    return t;
                }
            }

            return "";
        }
    }
}