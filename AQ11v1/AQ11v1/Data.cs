using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace AQ11v1
{
    // Class represents data that were extracted from csv dataset and prepared to be used by an algorithm
    public class Data
    {
        private string datasetPath;
        public List<string> Headers { get; set; }

        private List<List<string>> recordsString;
        public Dictionary<string, List<string>> AttributesDict { get; set; }
        public List<List<int>> Records { get; set; }

        private string targetAttributeName;
        private string targetAttributeValue;
        public List<List<int>> PositiveRecords { get; set; }
        public List<List<int>> NegativeRecords { get; set; }


        public int NumberOfColumns { get; set; }
        public int NumberOfRecords { get; set; }
        public Data(string datasetPath, string targetAttributeName, string targetAttributeValue)
        {
            this.datasetPath = datasetPath;
            this.targetAttributeName = targetAttributeName;
            this.targetAttributeValue = targetAttributeValue;

            ReadDataset();
        }

        public Data(string datasetPath, string targetAttributeName, string targetAttributeValue, Dictionary<string, List<string>> attributesDict)
        {
            this.datasetPath = datasetPath;
            this.targetAttributeName = targetAttributeName;
            this.targetAttributeValue = targetAttributeValue;
            this.AttributesDict = attributesDict;

            ReadDatasetWithExistingDictionary();
        }

        // Method that calls all needed methods from Preprocessor to successfully read data from dataset and store them in needed ways.
        public void ReadDataset()
        {
            Headers = Preprocessor.GetHeaders(datasetPath);

            recordsString = Preprocessor.GetRecordsString(datasetPath);

            AttributesDict = Preprocessor.CreateAttributesDictionary(Headers, recordsString);

            Records = Preprocessor.GetRecordsIndividualNumerical(recordsString, AttributesDict, Headers);

            PositiveRecords = Preprocessor.GetPositiveRecords(Records, AttributesDict, Headers, targetAttributeName, targetAttributeValue);

            NegativeRecords = Preprocessor.GetNegativeRecords(Records, AttributesDict, Headers, targetAttributeName, targetAttributeValue);

            NumberOfColumns = Headers.Count;
            NumberOfRecords = Records.Count;
        }

        // Method that calls all needed methods from Preprocessor to successfully read data from dataset and store them in needed ways.
        // This specific methods uses already existing dictionary that was assigned to this instance beforehand, for the numeric represented values to be synchronized with another existing Data instance.
        // For example for numeric values to represent same attribute values in training Data instance and evaluation Data instance.
        public void ReadDatasetWithExistingDictionary()
        {
            Headers = Preprocessor.GetHeaders(datasetPath);

            recordsString = Preprocessor.GetRecordsString(datasetPath);

            Records = Preprocessor.GetRecordsIndividualNumerical(recordsString, AttributesDict, Headers);

            PositiveRecords = Preprocessor.GetPositiveRecords(Records, AttributesDict, Headers, targetAttributeName, targetAttributeValue);

            NegativeRecords = Preprocessor.GetNegativeRecords(Records, AttributesDict, Headers, targetAttributeName, targetAttributeValue);

            NumberOfColumns = Headers.Count;
            NumberOfRecords = Records.Count;
        }

        // Method to display stored headers in the console
        public void DisplayHeaders()
        {
            string headersString = "";
            for (int i = 0; i < NumberOfColumns; i++)
            {
                headersString += $" {Headers[i]} |";
            }
            Console.WriteLine(headersString);
        }

        // Method to display string values of the records stored in the instance. (Displays whole dataset)
        public void DisplayRecordsString()
        {
            string outputRecord;
            for (int i = 0; i < NumberOfRecords; i++)
            {
                outputRecord = "";
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    outputRecord += $" {recordsString[j][i]} |";
                }
                Console.WriteLine(outputRecord);
            }
        }

        // Method to display numeric records given as a parameter. (Displays whole dataset)
        public void DisplayNumericalRecords(List<List<int>> records)
        {
            string outputRecord;
            foreach (List<int> record in records)
            {
                outputRecord = "";
                for (int i = 0; i < NumberOfColumns; i++)
                {
                    outputRecord += $" {record[i]} |";
                }
                Console.WriteLine(outputRecord);
            }
        }

        // Method to dysplay numeric records stored in the instance. (Displays whole dataset)
        public void DisplayNumericalRecords()
        {
            DisplayNumericalRecords(Records);
        }

        // Method to display positive numeric records. (Ones we want to cover)
        public void DisplayPositiveRecords()
        {
            DisplayNumericalRecords(PositiveRecords);
        }

        // Method to display negative numeric records. (Ones we don`t want to cover)
        public void DisplayNegativeRecords()
        {
            DisplayNumericalRecords(NegativeRecords);
        }

    }
}
