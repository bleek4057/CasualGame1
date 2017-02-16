using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PathSortedList
{
    class SortedPathList
    {
        List<KeyValuePair<int, Vector2>> keys = new List<KeyValuePair<int, Vector2>>();

        static int GetParent(int i)
        {
            return (i - 1) / 2;
        }

        static int GetLeft(int i)
        {
            return 2 * i + 1;
        }

        static int GetRight(int i)
        {
            return 2 * i + 2;
        }

        private void HeapifyUp(int i)
        {
            if (i <= 0) return;

            //get the index of the parent in the list
            int j = GetParent(i);

            //check the values at the indexes
            if (keys[i].Key < keys[j].Key)
            {
                KeyValuePair<int, Vector2> temp = keys[i];
                keys[i] = keys[j];
                keys[j] = temp;
            }

            HeapifyUp(j);
        }

        private void HeapifyDn(int i)
        {
            int j;

            // If no children...
            if (GetLeft(i) > keys.Count - 1) return;

            // If no right child...
            if (GetRight(i) > keys.Count - 1)
            {
                j = GetLeft(i);
            }
            else
            {
                // If both right and left children
                j = (keys[GetLeft(i)].Key < keys[GetRight(i)].Key) ? (GetLeft(i)) : (GetRight(i));
            }

            if (keys[i].Key > keys[j].Key)
            {
                KeyValuePair<int, Vector2> temp = keys[i];
                keys[i] = keys[j];
                keys[j] = temp;
            }

            HeapifyDn(j);
        }

        public void Insert(int newKey, Vector2 newValue)
        {
            keys.Add(new KeyValuePair<int, Vector2>(newKey, newValue));
            HeapifyUp(keys.Count - 1);
        }

        public KeyValuePair<int, Vector2> Pop()
        {
            KeyValuePair<int, Vector2> temp = keys[0];

            keys[0] = keys[keys.Count - 1];
            keys.RemoveAt(keys.Count - 1);

            HeapifyDn(0);

            return temp;
        }

        public int GetSize()
        {
            return keys.Count;
        }
    }
}
