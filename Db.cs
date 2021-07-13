using System;
using System.Collections.Generic;
using System.Linq;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using System.Globalization;


namespace ChatBot
{
    public class LogData
    {
        
        public string userInput { get; }
        
        public string botInput { get; }
        public string matchedTable { get; }

        public LogData(string input, string botinput, string table)
        {
            userInput = input;
            botInput = botinput;
            matchedTable = table;
           
        }
    }
    
    public class Db
    {
        private static string _authSecret = "*****";
        private static string _basePath = "*****";

        private static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = _authSecret,
            BasePath = _basePath
        };

        private static IFirebaseClient client;


        /// <summary>
        /// Starts the firebase connection
        /// </summary>
        public static void OpenDb()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                throw;
            }
        }

        /// <summary>
        /// Gets user's inputs for a specific table
        /// </summary>
        /// <param name="tableName">the table we want to get the inputs for</param>
        /// <returns>a list of the user's inputs</returns>
        public static List<string> GetUsersInputs(string tableName)
        {
            return client.Get($"questions/{tableName}").ResultAs<List<string>>();
        }

        /// <summary>
        /// Gets bot's response for a specific question
        /// </summary>
        /// <param name="tableName">the table name</param>
        /// <returns>the response</returns>
        public static string[] GetResponse(string tableName)
        {
            return client.Get($"responses/{tableName}").ResultAs<string[]>();
        }

        /// <summary>
        /// Gets all the tables in the database at a given path
        /// </summary>
        /// <param name="path">path in the database</param>
        /// <returns>the tables at a given path</returns>
        public static List<string> GetAllTables(string path)
        {
            var getter = client.Get(path);
            Dictionary<string, List<string>> data =
                JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(getter.Body);
            List<string> tables = new List<string>();
            foreach (var couple in data)
            {
                tables.Add(couple.Key);
            }

            return tables;
        }

        /// <summary>
        /// Adds confused data to the database
        /// </summary>
        /// <param name="userInput">the confused data</param>
        public static void AddConfusedData(string userInput)
        {
            string tableName = "unknown";
            var getter = client.Get(tableName).ResultAs<string[]>();
            int i = getter?.Length ?? 0;
            var setter = client.Set($"{tableName}/{i}", userInput);
        }

        public static void Log(string userInput, string botInput, string matchedTable)
        {
            DateTime date = DateTime.Now;
            string now = date.ToString("F");
            var setter = client.Set($"z-log/{now}", new LogData(userInput, botInput, matchedTable));
        }

        // <----------------------------------------------------------------------------------------------->
        // <----------------------------------------------------------------------------------------------->
        // <----------------------------------------------------------------------------------------------->
        // <------------------------------U-S-E-D---B-Y---A-D-M-I-N---O-N-L-Y------------------------------>
        // <----------------------------------------------------------------------------------------------->

        /*
        /// <summary>
        /// Gets unknown data
        /// </summary>
        /// <returns>an array with this unknown data</returns>
        public static string[] GetUnknownData()
        {
            return client.Get("unknown").ResultAs<string[]>();
        }

        /// <summary>
        /// Adds user's input
        /// </summary>
        /// <param name="userInput">the input of the input</param>
        /// <param name="intent">intent of the input</param>
        /// <param name="chapter">chapter of the input</param>
        /// <param name="topic">topic of the input</param>
        public static void AddUsersInput(string userInput, string intent, string chapter, string topic)
        {
            string tableName = $"questions/{intent}-{chapter}-{topic}";
            var getter = client.Get(tableName).ResultAs<string[]>();
            int i = getter?.Length ?? 0;
            var setter = client.Set($"{tableName}/{i}", userInput);
        }

        /// <summary>
        /// Adds bot's response for a question
        /// </summary>
        /// <param name="tablename">the table in which we want to add the response to</param>
        /// <param name="response">the response to add</param>
        public static void AddResponses(string tablename, string[] response)
        {
            client.Set($"responses/{tablename}", response);
        }

        /// <summary>
        /// Deletes unknown data
        /// </summary>
        /// <param name="i">index of the data to delete</param>
        public static void DeleteUnknownData(int i)
        {
            client.Delete($"unknown/{i}");
        }

        /// <summary>
        /// Deletes all the tables at a given path
        /// </summary>
        public static void DeleteAllTables(string path)
        {
            List<string> tables = GetAllTables(path);
            foreach (var table in tables.ToList())
            {
                client.Delete(table);
            }
        }

        /// <summary>
        /// Deletes a table from the database
        /// </summary>
        /// <param name="tableName">the table to delete</param>
        public static void DeleteTable(string tableName)
        {
            client.Delete(tableName);
        }
        */
    }
}