using System;

namespace Project
{
    public class OutOfBoundsException : Exception
    {
        public int[] moveIndex {get; set;}
        public string playerName {get; set;}
        public OutOfBoundsException(string name, int[] movePos)
        {
            playerName = name;
            moveIndex = movePos;
            Console.WriteLine("\nOutOfBoundsException thrown!");
        }
    }
}