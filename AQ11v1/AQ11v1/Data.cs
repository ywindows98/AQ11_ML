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

        // Metóda, ktorá volá všetky potrebné metódy z predprocesora na úspešné načítanie údajov z datasetu a ich uloženie v rôznych variantoch.
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
        
        // Metóda na zobrazenie hlavičiek(atribútov) údajov v konzole.
        public void DisplayHeaders()
        {
            string headersString = "";
            for (int i = 0; i < NumberOfColumns; i++)
            {
                headersString += $" {Headers[i]} |";
            }
            Console.WriteLine(headersString);
        }

        // Metóda na zobrazenie reťazcových hodnôt záznamov extrahovaných z datasetu.
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

        // Metóda na zobrazenie číselných záznamov.
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

        // Metóda na zobrazenie číselných záznamov transformovaných z nespracovaných reťazcových údajov.
        public void DisplayNumericalRecords()
        {
            DisplayNumericalRecords(Records);
        }

        // Metóda na zobrazenie pozitívnych číselných záznamov. (Ktore chceme pokryť)
        public void DisplayPositiveRecords()
        {
            DisplayNumericalRecords(PositiveRecords);
        }

        // Metóda na zobrazenie negatívnych číselných záznamov. (Ktore nechceme pokryť)
        public void DisplayNegativeRecords()
        {
            DisplayNumericalRecords(NegativeRecords);
        }

    }
}
