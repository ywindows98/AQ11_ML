using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace AQ11v1
{
    // Trieda, ktorá predstavuje údaje extrahované z datasetu.
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

        // Metóda, ktorá volá všetky potrebné metódy z predprocesora na úspešné načítanie údajov z datasetu a ich uloženie v rôznych variantoch.
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

        // Metóda na zobrazenie hlavičiek(atribútov) údajov v konzole.
        public void DisplayHeaders()
        {
            string headersString = "";
            for (int i = 0; i < numberOfColumns; i++)
            {
                headersString += $" {Headers[i]} |";
            }
            Console.WriteLine(headersString);
        }

        // Metóda na zobrazenie reťazcových hodnôt záznamov extrahovaných z datasetu.
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

        // Metóda zobrazenia číselných záznamov transformovaných z nespracovaných reťazcových údajov.
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
