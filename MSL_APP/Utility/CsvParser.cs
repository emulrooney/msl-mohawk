using Microsoft.AspNetCore.Http;
using MSL_APP.Models;
using MSL_APP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MSL_APP
{
    class CsvParser
    {
        public char Delimiter { get; set; }
        public Stream FileData { get; set; }

        /// <summary>
        /// Receives the intended file, then parses based on given delimiter
        /// </summary>
        /// <param name="file">Uploaded file to parse</param>
        /// <param name="delimiter">Char to break up csv entries</param>
        public CsvParser(IFormFile file, char delimiter)
        {
            Delimiter = delimiter;
            FileData = file.OpenReadStream();
        }

        /// <summary>
        /// Parse incoming students from CSV file.
        /// </summary>
        public void ParseStudents()
        {
            using (var reader = new StreamReader(FileData))
            {
                //SETUP     
                List<InvalidCsvEntry> invalidEntries = new List<InvalidCsvEntry>(); 
                List<string> students = new List<string>();
                int currentLineNumber = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    //Split line on delimiter' 
                    //Accommodate multiple entries on one line just in case
                    var values = line.Split(Delimiter);

                    foreach (String value in values)
                    {
                        
                        //Validate student has proper Mohawk email
                        var student = value.ToLower();
                        var pattern = "[a-z]*.[a-z0-9]*\\@mohawkcollege.ca";
                        var match = Regex.Match(student, pattern);

                        //Put valid entries in one list and invalid entries in another
                        if (match.Success)
                            students.Add(student);
                        else
                            invalidEntries.Add(new InvalidCsvEntry(currentLineNumber, student));
                    }

                    currentLineNumber++;
                }

                //TODO
                //Wipe old 'eligible' table
                //Replace eligible table with contents of valid list
                //Display invalid entries on front-end so user knows where errors are

                Console.WriteLine($"Got {students.Count} valid students");
                Console.WriteLine($"Got {invalidEntries.Count} invalid students");
            }
        }

        /// <summary>
        /// Parse incoming keys from CSV file.
        /// </summary>
        public void ParseKeys()
        {
            using (var reader = new StreamReader(FileData))
            {
                //SETUP 
                List<ProductKey> keys = new List<ProductKey>();

                while (!reader.EndOfStream) { 

                    var line = reader.ReadLine();
                    var values = line.Split(Delimiter);

                    foreach (String item in values)
                    {
                        var validKey = new ProductKey
                        {
                            Key = values[1]
                        };

                        keys.Add(validKey);
                    }

                    //TODO: Add handling of multiple entries on single line
                    //TODO: Add check for how many values in line
                }

                //TODO Check DB for any keys already being 

                Console.WriteLine($"Got {keys.Count} valid product keys");
            }
        }

        /// <summary>
        /// Parse product names from file
        /// </summary>
        public ParsedCsvData ParseProducts()
        {
            using (var reader = new StreamReader(FileData))
            {
                var data = new ParsedCsvData();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var productLine = line.Split(Delimiter);

                    foreach (String product in line.Split(Delimiter))
                    {
                        data.ValidList.Add(product);
                    }
                }

                return data;
            }
        }


    }
}
