using Microsoft.AspNetCore.Http;
using MSL_APP.Models;
using MSL_APP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MSL_APP
{
    class LicenseParser
    {
        public char Delimiter { get; set; } = ';'; //Default ';'
        public Stream FileData { get; set; }

        private static readonly List<string> fileWhitelist = new List<string>() 
        {
            "text/plain"
        };

        /// <summary>
        /// Receives the intended file, then parses based on given delimiter
        /// </summary>
        /// <param name="file">Uploaded file to parse</param>
        /// <param name="delimiter">Char to break up csv entries</param>
        public LicenseParser(IFormFile file, char delimiter)
        {
            if (fileWhitelist.Contains(file.ContentType))
            {
                Delimiter = delimiter;
                FileData = file.OpenReadStream();
            }
            else
            {
                throw new InvalidDataException("Uploaded file must be a plain text document.");
            }
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

                            parsedStudents.ValidList.Add(currentLineNumber.ToString(), eligible);
                        }
                    }
                    catch (Exception e)
                    {
                        parsedStudents.InvalidList.Add(currentLineNumber.ToString(), line);
                    }

                    currentLineNumber++;
                }

                return parsedStudents;
            }
        }

        /// <summary>
        /// Parse keys. If a key is in the format 'productName;key' (or with a different delimiter)
        /// it'll be added to the ValidList. If there are too many or too few values, the values
        /// will be added to the invalid list.
        /// </summary>
        /// <returns>ParsedCsvData with valid keys as tuples to be added to db in controller</returns>
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

                    if (values.Length == 2)
                    {
                        var key = Tuple.Create(values[0], values[1]);
                        parsedKeys.ValidList.Add(currentLineNumber.ToString(), key);
                    }
                    else
                        parsedKeys.InvalidList.Add(currentLineNumber.ToString(), line);

                    currentLineNumber++;
                }
            }

            return parsedKeys;
        }

        /// <summary>
        /// Parses products based on list.
        /// </summary>
        /// <returns>Parsed data containing list of valid and invalid Product names </returns>
        public ParsedCsvData<Product> ParseProducts()
        {
            var parsedProducts = new ParsedCsvData<Product>();
            int currentLineNumber = 0;

            using (var reader = new StreamReader(FileData))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var product = new Product()
                    {
                        QuantityLimit = 1,
                        Name = line,
                        ActiveStatus = "Active"
                    };

                    parsedProducts.ValidList.Add(currentLineNumber.ToString(), product);
                    currentLineNumber++;
                
                    //TODO: How should we check for validation?
                }

                return parsedProducts;
            }
        }


    }
}
