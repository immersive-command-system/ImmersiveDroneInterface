namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MenuController : MonoBehaviour
    {

        public List<UnityEngine.UI.Text> menuUI;
        public Up up;
        public Down down;

        public List<string> Items;
        public int current;
        private int displayLimit = 5;

        private void Start()
        {
            current = 0;
            display();
        }

        private void display()
        {
            if (current < 0)
            {
                current = 0;
            }

            if (current >= Items.Count)
            {
                current = 0;
            }

            for (int i = 0; i < 5; i++)
            {
                if (current + i < Items.Count && current + i >= 0)
                {
                    menuUI[i].text = Items[current + i];
                }
                else
                {
                    menuUI[i].text = "...";
                }
            }
        }

        public void addItem(string item)
        {
            Items.Add(item);
        }

        public void removeItem(string item)
        {
            Items.Remove(item);
        }

        public void scrollDown()
        {
            current += 5;
            display();
        }

        public void scrollUp()
        {
            current -= 5;
            display();
        }

    }
}
