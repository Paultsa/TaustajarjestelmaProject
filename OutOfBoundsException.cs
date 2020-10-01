using System;

namespace Project
{
    public class OutOfBoundsException : Exception
    {
        public OutOfBoundsException()
        {
            Console.WriteLine("OutOfBoundsException thrown!");
        }
    }
}