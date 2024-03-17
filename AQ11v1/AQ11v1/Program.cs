using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace AQ11v1
{
    internal class Program
    {

        // Funkcia získa a vráti reťazec cesty do hlavného priečinka projektu
        static string GetMainProjectFolder()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string mainProjectFolder = (Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName)).FullName;
            return mainProjectFolder;
        }

        static void Main(string[] args)
        {

            string mainProjectFolder = GetMainProjectFolder();
            //define dataset name and path
            string datasetName = "stroke_data_sample.csv";
            string datasetPath = mainProjectFolder + '\\' + datasetName;
            Console.WriteLine(datasetPath);

            Data data = new Data(datasetPath, "stroke", "Yes");

            //data.DisplayHeaders();
            //data.DisplayRecordsString();
            data.DisplayNumericalRecords();

            //data.DisplayPositiveRecords();
            //data.DisplayNegativeRecords();

            int numberOfColumns = data.NumberOfColumns;

            List<int> positiveRecord = data.PositiveRecords[0];
            List<int> negativeRecord = data.NegativeRecords[3];

            List<int?> partialStarDisjunction = new List<int?>();
            for(int i=1; i<numberOfColumns-1; i++)
            {
                if (positiveRecord[i] != negativeRecord[i])
                {
                    partialStarDisjunction.Add(-negativeRecord[i]);
                }
                else
                {
                    partialStarDisjunction.Add(null);
                }
            }

            for(int i=0; i<numberOfColumns-2; i++)
            {
                Console.Write($" {partialStarDisjunction[i]} |");
            }
            Console.Write("\n");

        }
    }
}
