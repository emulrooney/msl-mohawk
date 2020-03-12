using CSV_Parser;
using MSL_APP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CSVParser
{
    class CSVParser
    {
        public char Delimiter { get; set; }
        public string FilePath { get; set; }

        /// <summary>
        /// Standard constructor. Devs should pass the path to the uploaded object and the delimiter character.
        /// TODO: Might make more sense to pass the uploaded object directly... investigate later
        /// </summary>
        /// <param name="filePath">Path to .txt for parser to examine</param>
        /// <param name="delimiter">Char to break up csv entries</param>
        public CSVParser(string filePath, char delimiter)
        {
            FilePath = filePath;
            Delimiter = delimiter;
        }

        /// <summary>
        /// debug, remove on completion
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");

            var studentParser = new CSVParser(debugPath + "students.txt", ';');
            var keyParser = new CSVParser(debugPath + "keys.txt", ';');
            var productParser = new CSVParser(debugPath + "products.txt", ';');

            studentParser.ParseStudents();
            keyParser.ParseKeys();
            productParser.ParseProducts();

            Console.WriteLine("Parsing complete.");
            Console.ReadKey();
        }

        /// <summary>
        /// Parse incoming students from CSV file.
        /// </summary>
        public void ParseStudents()
        {
            using (var reader = new StreamReader(FilePath))
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
            using (var reader = new StreamReader(FilePath))
            {
                //SETUP 
                List<ProductKey> keys = new List<ProductKey>();

                while (!reader.EndOfStream) { 

                    var line = reader.ReadLine();
                    var values = line.Split(Delimiter);

                    //TODO: Add handling of multiple entries on single line
                    //TODO: Add check for how many values in line

                    var validKey = new ProductKey
                    {
                        Name = values[0],
                        Key = values[1]
                    };

                    keys.Add(validKey);
                }

                //TODO Check DB for any keys already being 

                Console.WriteLine($"Got {keys.Count} valid product keys");
            }
        }

        /// <summary>
        /// Parse product names from file
        /// </summary>
        public void ParseProducts()
        {
            using (var reader = new StreamReader(FilePath))
            {
                List<string> products = new List<string>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var productLine = line.Split(Delimiter);

                    foreach (String product in line.Split(Delimiter))
                    {
                        products.Add(product);
                    }
                }

                //TODO Remove console readings here
                Console.WriteLine($"Got {products.Count} valid product names");
            }
        }


    }
}
