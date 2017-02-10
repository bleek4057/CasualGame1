using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PathHeap
{
    class Heap
    {
        List<int> key = new List<int>();
        List<Vector2> values = new List<Vector2>();

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
            if (key[i] < key[j])
            {
                int temp = key[i];
                key[i] = key[j];
                key[j] = temp;

                Vector2 tempV = values[i];
                values[i] = values[j];
                values[j] = tempV;
            }

            HeapifyUp(j);
        }

        private void HeapifyDn(int i)
        {
            int j;

            // If no children...
            if (GetLeft(i) > key.Count - 1) return;

            // If no right child...
            if (GetRight(i) > key.Count - 1)
            {
                j = GetLeft(i);
            }
            else
            {
                // If both right and left children
                j = (key[GetLeft(i)] < key[GetRight(i)]) ? (GetLeft(i)) : (GetRight(i));
            }

            if (key[i] > key[j])
            {
                int temp = key[i];
                key[i] = key[j];
                key[j] = temp;

                Vector2 tempV = values[i];
                values[i] = values[j];
                values[j] = tempV;
            }

            HeapifyDn(j);
        }

        public void Insert(int newKey, Vector2 newValue)
        {
            key.Add(newKey);
            values.Add(newValue);
            HeapifyUp(key.Count - 1);
        }

        public KeyValuePair<int, Vector2> Pop()
        {
            KeyValuePair<int, Vector2> temp = new KeyValuePair<int, Vector2>(key[0], values[0]);

            key[0] = key[key.Count - 1];
            key.RemoveAt(key.Count - 1);

            values[0] = values[values.Count - 1];
            values.RemoveAt(values.Count - 1);

            HeapifyDn(0);

            return temp;
        }

        public int GetSize()
        {
            return key.Count;
        }
    }
}
