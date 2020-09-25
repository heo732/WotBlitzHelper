using Engine;
using System;

namespace Tester
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var requester = new Requester();
            Console.WriteLine(requester.GetTankIdByName("E 100"));
        }
    }
}