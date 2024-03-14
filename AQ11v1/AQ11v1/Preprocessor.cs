using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQ11v1
{
    public static class Preprocessor
    {
        public static List<string> GetHeaders(string datasetPath)
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
        public static List<List<string>> GetRecordsString(string datasetPath)
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
                        records[i + 1].Add(csv.GetField<string>(i));
                    }
                }

                int numberOfRecords = records[0].Count;

                return records;
            }
        }

        public static List<List<string>> TransformRecordsIntoRecordsIndividual(List<List<string>> recordsString)
        {
            List<List<string>> recordsIndividual = new List<List<string>>();
            int numberOfRecords = recordsString[0].Count;
            int numberOfColumns = recordsString.Count;


            for (int i = 0; i < numberOfRecords; i++)
            {
                recordsIndividual.Add(new List<string>());
                for (int j = 0; j < recordsString.Count; j++)
                {
                    recordsIndividual[i].Add(recordsString[j][i]);
                }
            }

            return recordsIndividual;
        }

        public static void DisplayRecordsIndividualString(List<List<string>> recordsIndividual)
        {
            int numberOfRecords = recordsIndividual.Count;
            for (int i = 0; i < numberOfRecords; i++)
            {
                recordsIndividual[i].ForEach(r => Console.Write($" {r} |"));
                Console.WriteLine();
            }
        }

        public static Dictionary<string, List<string>> CreateAttributesDictionary(List<string> headers, List<List<string>> recordsString)
        {
            Dictionary<string, List<string>> attributesDict = new Dictionary<string, List<string>>();
            int numberOfColumns = recordsString.Count;
            List<string> attrValuesSet = new List<string>();
            for (int i = 1; i < numberOfColumns; i++)
            {
                attrValuesSet = recordsString[i].ToHashSet().ToList();
                attributesDict.Add(headers[i], attrValuesSet);
            }

            return attributesDict;
        }

        public static List<List<int>> ConvertRecordsToInt(List<List<string>> recordsIndividual, Dictionary<string, List<string>> attributesDict, List<string> headers)
        {
            List<List<int>> recordsIndividualConverted = new List<List<int>>();

            int numberOfColumns = recordsIndividual[0].Count;
            int numberOfRecords = recordsIndividual.Count;
            for (int i = 0; i < numberOfRecords; i++)
            {
                recordsIndividualConverted.Add(new List<int>());
                //copies id
                recordsIndividualConverted[i].Add(Int32.Parse(recordsIndividual[i][0]));
                for (int j = 1; j < numberOfColumns; j++)
                {
                    recordsIndividualConverted[i].Add(attributesDict[headers[j]].FindIndex(x => x == recordsIndividual[i][j]));
                }
            }

            return recordsIndividualConverted;
        }

        public static List<List<int>> GetRecordsIndividualNumerical(List<List<string>> recordsString, Dictionary<string, List<string>> attributesDict, List<string> headers)
        {
            List<List<string>> recordsIndividual = TransformRecordsIntoRecordsIndividual(recordsString);
            //DisplayRecordsIndividualString(recordsIndividual);

            List<List<int>> recordsIndividualConverted = ConvertRecordsToInt(recordsIndividual, attributesDict, headers);
            //DisplayNumericalRecords(recordsIndividualConverted);

            return recordsIndividualConverted;
        }

    }
}
