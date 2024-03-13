using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace AQ11v1
{
    internal class Program
    {
        static string GetMainProjectFolder()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string mainProjectFolder = (Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName)).FullName;
            return mainProjectFolder;
        }
        static List<string> GetHeaders(string datasetPath)
        {
            using (var reader = new StreamReader(datasetPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                List<string> headerTitles = new List<string>();
                headerTitles.Add("Id");
                headerTitles.AddRange(csv.HeaderRecord.ToList());
                return headerTitles;
            }
        }

        static void DisplayHeaders(List<string> headers)
        {
            string headersString = "";
            for (int i = 0; i < headers.Count; i++)
            {
                headersString += $" {headers[i]} |";
            }
            Console.WriteLine(headersString);
        }

        static List<List<string>> GetRecordsString(string datasetPath)
        {
            using (var reader = new StreamReader(datasetPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                List<string> headerTitles = csv.HeaderRecord.ToList();
                int numberOfColumns = csv.ColumnCount;
                //headerTitles.ForEach(h => Console.WriteLine(csv.GetField(h)));

                //create emplty lists to contain all the records
                List<List<string>> records = new List<List<string>>();
                for (int i = 0; i < numberOfColumns + 1; i++)
                {
                    records.Add(new List<string>());
                }

                int id = 0;
                while (csv.Read())
                {
                    records[0].Add(id.ToString());
                    id++;
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        records[i+1].Add(csv.GetField<string>(i));
                    }
                }

                int numberOfRecords = records[0].Count;

                return records;
            }
        }

        // method to display records with values represented by strings.
        static void DisplayRecordString(List<List<string>> records)
        {
            int numberOfColumns = records.Count;
            int numberOfRecords = records[0].Count;

            string outputRecord;
            for (int i = 0; i < numberOfRecords; i++)
            {
                outputRecord = "";
                for (int j = 0; j < numberOfColumns; j++)
                {
                    outputRecord += $" {records[j][i]} |";
                }
                Console.WriteLine(outputRecord);
            }
        }


        static void Main(string[] args)
        {
            string mainProjectFolder = GetMainProjectFolder();
            //define dataset name and path
            string datasetName = "stroke_data_sample.csv";
            string datasetPath = mainProjectFolder + '\\' + datasetName;
            Console.WriteLine(datasetPath);


            List<string> headers = GetHeaders(datasetPath);
            DisplayHeaders(headers);

            List<List<string>> recordsString = GetRecordsString(datasetPath);
            DisplayRecordString(recordsString);

            List<string> S = recordsString[0].ToHashSet().ToList();
            //Console.WriteLine(S[1]);


        }
    }
}
