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
    // Class created specifically to work with csv files and to preptocess extracted data
    public static class Preprocessor
    {
        // Methods gets names of headers (attributes) from the csv file and returns them as a list of string values
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

        // Method reads data from csv file. Saves them into list of lists. Each list represents one column.
        // Adding id column to data. Method returns data represented by list of lists.
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

        // Method transforms starting list of list with unprocessed data to a list that contains each single record separatel.
        // Returns list of lists in new format.
        public static List<List<string>> TransformRecordsIntoRecordsIndividual(List<List<string>> recordsString)
        {
            List<List<string>> recordsIndividual = new List<List<string>>();
            int numberOfRecords = recordsString[0].Count;
            int numberOfColumns = recordsString.Count;


            for (int i = 0; i < numberOfRecords; i++)
            {
                recordsIndividual.Add(new List<string>());
                for (int j = 0; j < numberOfColumns; j++)
                {
                    recordsIndividual[i].Add(recordsString[j][i]);
                }
            }

            return recordsIndividual;
        }

        // Method creates dictionary to universally store all possible attribute values and returns it. Keys are the names of the attributes (columns).
        // Indexing in sets starts from 1 (0 element is null). It is made for the easier future work with coverting attribute values to negative (1 is positive value of attribute, and -1 is the same value but negative).
        public static Dictionary<string, List<string>> CreateAttributesDictionary(List<string> headers, List<List<string>> recordsString)
        {
            Dictionary<string, List<string>> attributesDict = new Dictionary<string, List<string>>();
            int numberOfColumns = recordsString.Count;
            List<string> attrValuesSet = new List<string>();
            // To each key (attribute) we assign a set of possible attribute values.
            for (int i = 1; i < numberOfColumns; i++)
            {
                attrValuesSet = recordsString[i].ToHashSet().ToList();
                attrValuesSet.Insert(0, null);
                attributesDict.Add(headers[i], attrValuesSet);
            }

            return attributesDict;
        }

        // Method converts string records into records that contain numbers that represent value of an attribute. Each number is an index of the value in the dictionary.
        // Returns list of single records with integer-represented attribute values.
        public static List<List<int>> ConvertRecordsToInt(List<List<string>> recordsIndividual, Dictionary<string, List<string>> attributesDict, List<string> headers)
        {
            List<List<int>> recordsIndividualConverted = new List<List<int>>();

            int numberOfColumns = recordsIndividual[0].Count;
            int numberOfRecords = recordsIndividual.Count;

            for (int i = 0; i < numberOfRecords; i++)
            {
                recordsIndividualConverted.Add(new List<int>());
                // Id is being copied
                recordsIndividualConverted[i].Add(Int32.Parse(recordsIndividual[i][0]));

                for (int j = 1; j < numberOfColumns; j++)
                {
                    // To each string value of an attribute we find index of this value in the list that is stored in the dictionary.
                    recordsIndividualConverted[i].Add(attributesDict[headers[j]].FindIndex(x => x == recordsIndividual[i][j]));
                }
            }

            return recordsIndividualConverted;
        }

        // Utiliry method for easier transformation of uprocessed string data into single numeric records with only one call..
        // Returns list of numeric records (lists).
        public static List<List<int>> GetRecordsIndividualNumerical(List<List<string>> recordsString, Dictionary<string, List<string>> attributesDict, List<string> headers)
        {
            List<List<string>> recordsIndividual = TransformRecordsIntoRecordsIndividual(recordsString);
            //DisplayRecordsIndividualString(recordsIndividual);

            List<List<int>> recordsIndividualConverted = ConvertRecordsToInt(recordsIndividual, attributesDict, headers);
            //DisplayNumericalRecords(recordsIndividualConverted);

            return recordsIndividualConverted;
        }

        // Method to extract only positive records (ones we want to cover) from the list of records.
        public static List<List<int>> GetPositiveRecords(List<List<int>> recordsNumerical, Dictionary<string, List<string>> attributesDict, List<string> headers, string posAttributeName, string posAttributeValue)
        {
            int numberOfColumns = headers.Count;
            int numberOfRecords = recordsNumerical.Count;

            int targetColumnIndex = headers.FindIndex(x => x == posAttributeName);
            if(targetColumnIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute name (target column {posAttributeName})  is not found in the headers list. Name of the attribute has to be identical to the one in the dataset header. It is also, case-sensitive. Make sure the input is correct");
            }
            //Console.WriteLine(targetColumnIndex);
            int targetPositiveValueIndex = attributesDict[posAttributeName].FindIndex(x => x == posAttributeValue);
            if (targetPositiveValueIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute value (target value {posAttributeValue})  is not found in the dictionary under the {posAttributeName} key. Value of the attribute has to be identical to the one in the dataset. It is also, case-sensitive. Make sure the input is correct.");
            }
            //Console.WriteLine(targetPositiveValueIndex);

            List<List<int>> positiveRecords = new List<List<int>>();
            for(int i=0; i<numberOfRecords; i++)
            {
                if (recordsNumerical[i][targetColumnIndex] == targetPositiveValueIndex)
                {
                    positiveRecords.Add(recordsNumerical[i]);
                }
            }

            return positiveRecords;
        }

        // Method to extract only negative records (ones we don`t want to cover) from the list of records.
        public static List<List<int>> GetNegativeRecords(List<List<int>> recordsNumerical, Dictionary<string, List<string>> attributesDict, List<string> headers, string posAttributeName, string posAttributeValue)
        {
            int numberOfColumns = headers.Count;
            int numberOfRecords = recordsNumerical.Count;

            int targetColumnIndex = headers.FindIndex(x => x == posAttributeName);
            if (targetColumnIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute name (target column {posAttributeName})  is not found in the headers list. Name of the attribute has to be identical to the one in the dataset header. It is also, case-sensitive. Make sure the input is correct");
            }
            //Console.WriteLine(targetColumnIndex);
            int targetPositiveValueIndex = attributesDict[posAttributeName].FindIndex(x => x == posAttributeValue);
            if (targetPositiveValueIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute value (target value {posAttributeValue})  is not found in the dictionary under the {posAttributeName} key. Value of the attribute has to be identical to the one in the dataset. It is also, case-sensitive. Make sure the input is correct.");
            }
            //Console.WriteLine(targetPositiveValueIndex);

            List<List<int>> negativeRecords = new List<List<int>>();
            for (int i = 0; i < numberOfRecords; i++)
            {
                if (recordsNumerical[i][targetColumnIndex] != targetPositiveValueIndex)
                {
                    negativeRecords.Add(recordsNumerical[i]);
                }
            }

            return negativeRecords;
        }
    }
}
