using System;
using System.Collections.Generic;
using System.IO;

namespace SprintJiraAdapter
{
    class CSVParse
    {

        internal static List<List<string>> ParseCsvFile(string path)
        {
            var allIssues = new List<List<string>>();
            using (var readFile = new StreamReader(path))
            {
                var header = ParseCsvLine(readFile.ReadLine());

                string line;
                while ((line = readFile.ReadLine()) != null)
                {
                    var newIssue = ParseCsvLine(line);

                    if (string.IsNullOrEmpty(newIssue[0]))
                    {
                        for(int i = 0; i < newIssue.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(newIssue[i]))
                            {
                                allIssues[allIssues.Count - 1][i] += (Environment.NewLine + newIssue[i]);
                            }
                        }
                    }
                    else
                    {
                        ValidateCsvLine(newIssue);
                        allIssues.Add(newIssue);
                    }
                }
            }
            return allIssues;
        }

        private static void PrintIssue(List<List<string>> allIssues, List<string > headers)
        {
            
            foreach (var issue in allIssues)
            {
               for (int indexForHeaderList = 0; indexForHeaderList < headers.Count; indexForHeaderList++)
               {
                   Console.WriteLine(headers[indexForHeaderList] + ": " + issue[indexForHeaderList]);
               }
               
                Console.Write(Environment.NewLine + "--------------------------------------------------------------------------------" + Environment.NewLine);
            }
            Console.WriteLine("-----End of Log-----");
            Console.ReadLine();
        }


        private static List<string> ParseCsvLine(string line)
        {
            var lineArray = line.Split('\t');

            var newIssue = new List<string>();

            foreach (string item in lineArray)
            {
                newIssue.Add(item);
            }

            return newIssue;
        }

        private static void ValidateCsvLine(List<string> issue)
        {
            var temp = 0;
            if (!int.TryParse(issue[0], out temp))
            {
                throw new Exception("Non number in first column.");
            }
            
        }
    }
}
