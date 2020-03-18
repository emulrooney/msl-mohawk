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
        public ParsedCsvData<EligibleStudent> ParseStudents()
        {
            using (var reader = new StreamReader(FileData))
            {
                //SETUP     
                ParsedCsvData<EligibleStudent> parsedStudents = new ParsedCsvData<EligibleStudent>();

                int currentLineNumber = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(Delimiter);

                    try
                    {
                        //Validate student number by attempting parse
                        var studentNumber = Int32.Parse(values[0]);

                        //Validate student has proper Mohawk email
                        var studentEmail = values[3].ToLower();
                        var emailPattern = "[a-z]*.[a-z0-9]*\\@mohawkcollege.ca";
                        var emailValidated = Regex.Match(studentEmail, emailPattern);

                        //First&Last name are not validated; some students may have different names the one in their email

                        //Student number should be < 9 digits, email must pass regex
                        if (studentNumber < 1000000000 && emailValidated.Success)
                        {
                            var eligible = new EligibleStudent()
                            {
                                StudentID = studentNumber,
                                FirstName = values[1],
                                LastName = values[2],
                                StudentEmail = values[3]
                            };

                            parsedStudents.ValidList.Add(eligible);
                        }
                    }
                    catch (Exception e)
                    {
                        parsedStudents.InvalidList.Add(new InvalidCsvEntry(currentLineNumber, line));
                    }

                    currentLineNumber++;
                }

                return parsedStudents;
            }
        }

        /// <summary>
        /// Parse incoming keys from CSV file.
        /// </summary>
        public ParsedCsvData<Tuple<string, string>> ParseKeys()
        {
            var parsedKeys = new ParsedCsvData<Tuple<string, string>>();
            int currentLineNumber = 0;

            using (var reader = new StreamReader(FileData))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(Delimiter);

                    try
                    {
                        var key = Tuple.Create(values[0], values[1]);
                        parsedKeys.ValidList.Add(key);
                    }
                    catch (Exception e)
                    {
                        parsedKeys.InvalidList.Add(new InvalidCsvEntry(currentLineNumber, line));
                    }

                    currentLineNumber++;
                }
            }

            return parsedKeys;
        }

        /// <summary>
        /// Parse product names from file
        /// </summary>
        public ParsedCsvData<ProductName> ParseProducts()
        {
            var parsedProducts = new ParsedCsvData<ProductName>();
            int currentLineNumber = 0;

            using (var reader = new StreamReader(FileData))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var productName = new ProductName()
                    {
                        QuantityLimit = 1,
                        Name = line,
                        ActiveStatus = "Actived"
                    };

                    parsedProducts.ValidList.Add(productName);
                    currentLineNumber++;
                
                    //TODO: How should we check for validation?
                }

                return parsedProducts;
            }
        }


    }
}
