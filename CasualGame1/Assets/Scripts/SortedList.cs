using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PathSortedList
{
    class PathList
    {
        List<KeyValuePair<int, Vector2>> keys = new List<KeyValuePair<int, Vector2>>();


        public void Insert(int newKey, Vector2 newValue)
        {
            if (keys.Count == 0) { keys.Add(new KeyValuePair<int, Vector2>(newKey, newValue)); return; }

            int index = keys.Count;
            while(index != 0 && keys[index - 1].Key > newKey) { index--; }

            if(index == keys.Count) { keys.Add(new KeyValuePair<int, Vector2>(newKey, newValue)); }

            else { keys.Insert(index, new KeyValuePair<int, Vector2>(newKey, newValue)); }

        }

        public KeyValuePair<int, Vector2> Pop()
        {
            KeyValuePair<int, Vector2> temp = keys[0];

            keys.RemoveAt(0);

            return temp;
        }

        public int GetSize()
        {
            return keys.Count;
        }

        public void PrintList()
        {
            for(int i = 0; i < keys.Count; i++)
            {
                Debug.Log(keys[i].Key);
            }
            Debug.Log(" ");
        }
    }
}
