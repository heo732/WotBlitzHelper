using Engine;
using System;
using System.IO;

namespace Tester
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var requester = new Requester
            {
                AppId = File.ReadAllText(@"..\..\..\..\appId.txt")
            };
            string nickname = "t3mp0";
            Console.WriteLine(requester.GetAccountIdByNickname(nickname));
        }
    }
}