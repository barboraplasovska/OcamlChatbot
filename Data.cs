using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot
{
    public class Data
    {
        public readonly List<string> Intents;
        public readonly List<string> Chapters;
        public readonly List<string> Topics;
        public readonly List<string> TableNames;
        public readonly List<(string tablename, List<string> inputs)> Tables;

        /// <summary>
        /// Data constructor
        /// </summary>
        public Data()
        {
            Db.OpenDb();
            Intents = new List<string>();
            Chapters = new List<string>();
            Topics = new List<string>();
            Tables = new List<(string tablename, List<string> inputs)>();
            TableNames = Db.GetAllTables("questions");
            TableNames.ForEach(table =>
            {
                string[] split = table.Split("-");
                if (!Intents.Contains(split[0])) Intents.Add(split[0]);
                if (split[1] != "" && Chapters.Contains(split[1])) Chapters.Add(split[1]);
                if (split[2] != "" && !Topics.Contains(split[2])) Topics.Add(split[2]);
                Tables.Add((table, Db.GetUsersInputs(table)));
            });
        }

        /// <summary>
        /// gets the unchecked tables
        /// </summary>
        /// <param name="checkedTables">tables that were already checked</param>
        /// <returns>returns unchecked tables</returns>
        public List<string> GetUncheckedTables(List<string> checkedTables)
        {
            List<string> tables = new List<string>();
            foreach (var t in TableNames)
            {
                bool stop = false;
                for (int i = 0; i < checkedTables.Count && !stop; i++)
                {
                    if (checkedTables[i] == t) stop = true;
                }

                if (!stop) tables.Add(t);
            }

            return tables;
        }

        /// <summary>
        /// gets the key tables
        /// </summary>
        /// <param name="keywords">keywords that have to be present in the table name</param>
        /// <returns>returns the tables containing the parameters passed</returns>
        public List<string> GetKeyTables(UserInput input)
        {
            List<string> keywords = FindKeywords(input);
            List<string> tables = new List<string>();
            foreach (var t in TableNames)
            {
                string[] split = t.Split('-');
                bool stop = false;
                for (int j = 0; j < keywords.Count && !stop; j++)
                {
                    if (split[0] == keywords[j] || split[1] == keywords[j] || split[2] == keywords[j])
                    {
                        tables.Add(t);
                        stop = true;
                    }
                }
            }

            if (tables.Count == 0) tables = new List<string> {"greetings--", "quit--"};
            return tables;
        }

        /// <summary>
        /// Depending on the user's input it finds Intents, Chapters and Topics into which the bot will look first
        /// </summary>
        /// <param name="input">the user's input</param>
        /// <returns>the list of key words</returns>
        private List<string> FindKeywords(UserInput input)
        {
            List<string> res = new List<string>();
            foreach (var word in input.SplitedInput)
            {
                string temp;
                if (Intents.Contains(word)) res.Add(word);
                if (Chapters.Contains(word)) res.Add(word);
                if (Topics.Contains(word)) res.Add(word);
                if ((temp = Chapters.Find(chapter => chapter.Contains(word))) != null) res.Add(temp);
                //if ((temp = Topics.Find(chapter => chapter.Contains(word))) != null) res.Add(temp);
            }

            return res;
        }
    }
}