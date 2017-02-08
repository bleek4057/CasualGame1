using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathHeap
{
    class Heap
    {
        List<int> key = new List<int>();

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
            int j = GetParent(i);
            if (key[i] < key[j])
            {
                int temp = key[i];
                key[i] = key[j];
                key[j] = temp;
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
            }

            HeapifyDn(j);
        }

        public void Insert(int newKey)
        {
            key.Add(newKey);
            HeapifyUp(key.Count - 1);
        }

        public int Pop()
        {
            int temp = key[0];

            key[0] = key[key.Count - 1];
            key.RemoveAt(key.Count - 1);
            HeapifyDn(0);

            return temp;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Heap b = new Heap();

            Console.WriteLine("Testing Heap:");
            Console.WriteLine();

            string userInput = "";
            while (userInput != "quit")
            {
                Console.WriteLine();
                Console.WriteLine("-- Commands: quit, print, insert... --");
                userInput = Console.ReadLine();

                if (userInput == "insert")
                {
                    bool parsed = false;
                    while (parsed == false)
                    {
                        Console.WriteLine("Enter a value to add to the heap.");
                        int key;
                        parsed = int.TryParse(Console.ReadLine(), out key);
                        Console.WriteLine();
                        if (parsed != true)
                            Console.WriteLine("Please enter an integer.");
                        else
                        {
                            b.Insert(key);
                            Console.WriteLine("Inserted key : " + key);
                        }
                    }
                }
                if (userInput == "print")
                {
                    Heap newHeap = new Heap();
                    try
                    {
                        while (true)
                        {
                            int t = b.Pop();
                            newHeap.Insert(t);
                            Console.WriteLine(t);
                        }
                    }
                    catch
                    {

                    }
                    b = newHeap;
                }
            }
        }
    }
}
