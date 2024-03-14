using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace AQ11v1
{
    public class Data
    {
        string datasetPath;
        List<string> Headers { get; set; }

        List<List<string>> recordsString;
        Dictionary<string, List<string>> AttributesDict { get; set; }
        List<List<int>> Records { get; set; }

        int numberOfColumns;
        int numberOfRecords;
        public Data()
        {

        }

        public void ReadDataset(string datasetPath)
        {
            this.datasetPath = datasetPath;
            Headers = Preprocessor.GetHeaders(this.datasetPath);

            recordsString = Preprocessor.GetRecordsString(this.datasetPath);

            AttributesDict = Preprocessor.CreateAttributesDictionary(Headers, recordsString);

            Records = Preprocessor.GetRecordsIndividualNumerical(recordsString, AttributesDict, Headers);

            numberOfColumns = Headers.Count;
            numberOfRecords = Records.Count;
        }

        public void DisplayHeaders()
        {
            string headersString = "";
            for (int i = 0; i < numberOfColumns; i++)
            {
                headersString += $" {Headers[i]} |";
            }
            Console.WriteLine(headersString);
        }

        // method to display records with values represented by strings.
        public void DisplayRecordsString()
        {
            string outputRecord;
            for (int i = 0; i < numberOfRecords; i++)
            {
                outputRecord = "";
                for (int j = 0; j < numberOfColumns; j++)
                {
                    outputRecord += $" {recordsString[j][i]} |";
                }
                Console.WriteLine(outputRecord);
            }
        }

        public void DisplayNumericalRecords()
        {
            string outputRecord;
            foreach (List<int> record in Records)
            {
                outputRecord = "";
                for (int i = 0; i < numberOfColumns; i++)
                {
                    outputRecord += $" {record[i]} |";
                }
                Console.WriteLine(outputRecord);
            }
        }
    }
}
