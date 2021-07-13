using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;

namespace ChatBot
{
    public class UserInput
    {
        public string Input { get; }
        public string[] SplitedInput { get; }


        /// <summary>
        /// UserInput constructor
        /// </summary>
        /// <param name="input">User's input</param>
        public UserInput(string input)
        {
            Input = FormatInput(input);
            SplitedInput = Split(Input);
        }


        /// <summary>
        /// Formats the user's input
        /// </summary>
        /// <param name="input"> user's input</param>
        /// <returns> returns the formated  input</returns>
        private string FormatInput(string input)
        {
            string formatedInput = input.ToLower().Trim();
           List<string> replaceables = new List<string> {"?", ",", "."};
            for (int i = 0; i < replaceables.Count; i++)
                formatedInput = formatedInput.Replace(replaceables[i], "");
            formatedInput = formatedInput.ToLower().Trim();
            formatedInput = TrimSpacesBetweenString(formatedInput);
            string[] split = Split(formatedInput);
            formatedInput = "";
            int ind = 0;
            foreach (var word in split)
            {
                if (ind == 0) 
                    formatedInput += word;
                else 
                    formatedInput += $" {word}";
                ind++;
            }
            return formatedInput;
        }

        /// <summary>
        /// Removes double/triple ect spaces between words in a string
        /// </summary>
        /// <param name="str"> sentence</param>
        /// <returns> returns the sentence without spaces</returns>
        private string TrimSpacesBetweenString(string str)
        {
            var splitStr = str.Split(new string[] {" "}, StringSplitOptions.None);
            string result = string.Empty;
            foreach (var word in splitStr)
            {
                var trim = word.Trim();
                if (!string.IsNullOrEmpty(trim))
                {
                    result = $"{result}{trim} ";
                }
            }

            return result.Trim();
        }

        /// <summary>
        /// Splits the user's input and removes unnecessary characters and words
        /// </summary>
        /// <param name="input">input to split</param>
        /// <returns>an array representing the split input</returns>
        private string[] Split(string input)
        {
            string[] split = input.Split(' ');
            List<string> temp = new List<string>();
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i]!= "a" && split[i] != "the" && split[i] != "is" && split[i] != "are" &&
                    split[i] != "i"&&split[i] != "an")  temp.Add(split[i]);
            }

            return temp.ToArray();
        }
    }
}