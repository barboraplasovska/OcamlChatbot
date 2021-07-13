using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Run.ConsoleBot();
        }
    }

    
    class Run
    {

        public static void ConsoleBot()
        {
            string[] intro =
            {
                "Welcome!",
                "Ask me a question or write something"
            };
            foreach (string line in intro)
            {
                Console.WriteLine($"Bot: {line}");
            }

            bool shutdown = false;
            Data data = new Data();
            Bot bot = new Bot(data);
            while (!shutdown)
            {
                Console.Write("User: ");
                string user = Console.ReadLine();
                string[] output = bot.Run(user);
                foreach (string line in output)
                {
                    Console.WriteLine($"Bot: {line}");
                }

                shutdown = bot.Stop;
            }
        }
    }
}