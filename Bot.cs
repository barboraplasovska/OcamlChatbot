using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace ChatBot
{
    public class Bot
    {
        public bool Stop => _stop;
        private bool _stop;
        private (int score, string tablename) _bestMatch;
        private Data _data;
        private UserInput _input;
        private List<(int score, string tablename)> _scoredTables;

        /// <summary>
        /// Bot constructor
        /// </summary>
        /// <param name="data">Data from bots database</param>
        public Bot(Data data)
        {
            _data = data;
            _stop = false;
        }


        /// <summary>
        /// Searches the sentence in the table and returns whether it is present or not
        /// </summary>
        /// <param name="tableName">the table to search in</param>
        /// <returns>true if present in the table</returns>
         private bool SentenceInTable(string tableName)
        {
            List<string> userInputs = _data.Tables.Find(table => table.tablename == tableName).inputs;
            foreach (var sentence in userInputs)
            {
                string[] tempInput = sentence.Split(' ');
                int score = _input.SplitedInput.Length == tempInput.Length ? 0 : -1;
                int length = tempInput.Length > _input.SplitedInput.Length ? _input.SplitedInput.Length : tempInput.Length;
                for (int i = 0; i < length; i++)
                {
                    if (tempInput[i] == _input.SplitedInput[i]) score++;
                }

                score = (score / _input.SplitedInput.Length) * 100;
                if (score == 100) return true;
            }

            return false;
        }

         
         

        /// <summary>
        /// Scores the given table
        /// </summary>
        /// <param name="tablename">the table to score</param>
        /// <returns>the score of the table</returns>
        private int CalculateTableScore(string tablename)
        {
            List<string> tableInputs = _data.Tables.Find(table => table.tablename == tablename).inputs;
            int score = 0;
            for (int i = 0; i < tableInputs.Count; i++)
            {
                string[] tempInput = tableInputs[i].Split(' ');
                bool sameLength = _input.SplitedInput.Length == tempInput.Length;
                int temp = sameLength ? 0 : -1;
                for (int j = 0; j < _input.SplitedInput.Length; j++)
                {
                    if (tempInput.Contains(_input.SplitedInput[j])) temp++;
                }

                temp *= 100 / _input.SplitedInput.Length;
                if (temp == 100) return score;
                if (temp > score) score = temp;
            }
            return score;
        }


        /// <summary>
        /// Goes through the list of scored tables and returns the best match
        /// </summary>
        /// <returns>table with the highest score</returns>
        private (int, string) Best()
        {
            (int score, string) best = _bestMatch;
            for (int i = 0; i < _scoredTables.Count; i++)
            {
                if (_scoredTables[i].score > best.score) best = _scoredTables[i];
            }
            return best;
        }
        
        
        /// <summary>
        /// Searches the given table and changes _bestMatch if a match was found
        /// </summary>
        /// <param name="tables">tables to search in</param>
        private void SearchTables(List<string> tables)
        {
            if (tables.Count > 0)
            {
                string bestMatch = tables.Find(SentenceInTable);
                if (bestMatch == null)
                {
                    tables.ForEach(table => _scoredTables.Add((CalculateTableScore(table), table)));
                    _bestMatch = Best();
                }
                else _bestMatch = (100,bestMatch);
            }
        }


        /// <summary>
        /// Gets bots response from the database according to the best matched table
        /// </summary>
        /// <returns>returns bot response as a string array</returns>
        private string[] GetResponse()
        {
            string[] botResponse = Db.GetResponse(_bestMatch.tablename);

            if (botResponse == null) botResponse = new[] {"This response will be added soon! :P"};
            else if (_bestMatch.tablename == "quit--")
            {
                _stop = true;
                botResponse = new[] {botResponse[new Random().Next(botResponse.Length)]};
            }
            else if (_bestMatch.tablename == "unknown" || _bestMatch.score <= 90)
            {
                Db.AddConfusedData(_input.Input);
                botResponse = Db.GetResponse("unknown");
                botResponse = new[] {botResponse[new Random().Next(botResponse.Length)]};
            }
            if (_bestMatch.tablename == "greetings--" || _bestMatch.tablename == "chitchat--" || _bestMatch.tablename == "chitchat--thanks" )
            { botResponse = new[] {botResponse[new Random().Next(botResponse.Length)]};
            }
            else if (_bestMatch.tablename == "---") botResponse = new[] {"Something is wrong"};
            return botResponse;
        }
        
        /// <summary>
        /// Runs the whole bot
        /// </summary>
        /// <param name="userinput"> the user's input</param>
        /// <returns>returns bot's response</returns>
        public string[] Run(string userinput)
        {
            _input = new UserInput(userinput);
            _bestMatch = (0, "unknown");
            _scoredTables = new List<(int score, string tablename)>();
            List<string> keyTables = _data.GetKeyTables(_input);
            List<string> uncheckedTables = _data.GetUncheckedTables(keyTables);
            SearchTables(keyTables);
            if (_bestMatch.tablename == "unknown" || _bestMatch.score < 80) SearchTables(uncheckedTables);
            Db.Log(userinput,_input.Input,_bestMatch.tablename);
            return GetResponse();
        }
    }
}