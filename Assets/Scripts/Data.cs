using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class Data
    {
        public string anim;
        public string bcolor;
        public float speed;

        public Data()
        {
            this.anim = "";
            this.bcolor = "";
            this.speed = 1f;
        }

        public int getValueOfAnim()
        {
            if (this.anim.Equals("joy"))
            {
                return 4;
            }
            else if (this.anim.Equals("anger"))
            {
                return 1;
            }
            else if (this.anim.Equals("sad"))
            {
                return 3;
            }
            else if (this.anim.Equals("ini"))
            {
                return 10;
            }
            else if (this.anim.Equals("blink"))
            {
                return 9;
            }
            else if (this.anim.Equals("surprise"))
            {
                return 5;
            }
            else if (this.anim.Equals("up"))
            {
                return 2;
            }
            else if (this.anim.Equals("down"))
            {
                return 6;
            }
            else if (this.anim.Equals("ur"))
            {
                return 7;
            }
            else if (this.anim.Equals("ul"))
            {
                return 8;
            }
            else if (this.anim.Equals("dr"))
            {
                return 11;
            }
            else if (this.anim.Equals("dl"))
            {
                return 12;
            }
            else if (this.anim.Equals("right"))
            {
                return 13;
            }
            else if (this.anim.Equals("left"))
            {
                return 14;
            }
            else
            {
                return 0;
            }
        }

        public Color getValueOfBColor()
        {
            if (this.bcolor.Equals("black"))
            {
                return Color.black;
            }
            else if (this.bcolor.Equals("blue"))
            {
                return Color.blue;
            }
            else if (this.bcolor.Equals("green"))
            {
                return Color.green;
            }
            else if (this.bcolor.Equals("yellow"))
            {
                return Color.yellow;
            }
            else if (this.bcolor.Equals("red"))
            {
                return Color.red;
            }
            else
            {
                return new Color((float) 0.1037736, (float) 0.1037736, (float) 0.1037736);
            }
        }
    }
}