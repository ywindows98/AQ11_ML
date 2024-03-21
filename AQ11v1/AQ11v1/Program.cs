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
            //string datasetName = "stroke_data_sample.csv";
            //string datasetPath = mainProjectFolder + '\\' + datasetName;
            //Console.WriteLine(datasetPath);

            //Data data = new Data(datasetPath, "stroke", "Yes");

            string datasetName = "cvic.csv";
            string datasetPath = mainProjectFolder + '\\' + datasetName;
            Console.WriteLine(datasetPath);

            Data data = new Data(datasetPath, "Target", "P");

            //data.DisplayHeaders();
            //data.DisplayRecordsString();
            data.DisplayNumericalRecords();

            Console.WriteLine();
            //data.DisplayPositiveRecords();
            //Console.WriteLine();
            //data.DisplayNegativeRecords();
            //Console.WriteLine();

            List<int> positiveRecord = data.PositiveRecords[0];
            //List<int> negativeRecord = data.NegativeRecords[0];

            AQ11 aq = new AQ11(data);

            //List<int?> partialStarDisjunction = aq.CreatePartialStarDisjunction(positiveRecord, negativeRecord);

            //aq.DisplayPartialStarDisjunction(partialStarDisjunction);

            //List<List<int?>> partialStarConjunction = aq.CreatePartialStarConjunction(positiveRecord, data.NegativeRecords);
            //aq.DispalayPartialStarConjunction(partialStarConjunction);

            //Console.WriteLine("=============================\n\n==============");

            //List<List<int?>> afterAbsorption = aq.ApplyAbsorptionLawOnConjunction(partialStarConjunction);
            //aq.DispalayPartialStarConjunction(afterAbsorption);


            //Console.WriteLine(aq.IsRecordCoveredByConjunction(data.NegativeRecords[7], afterAbsorption));

            //List<List<int>> coveredRecords = aq.SelectCoveredRecords(aq.LocalData.PositiveRecords, afterAbsorption);
            //Console.WriteLine("Covered records: ");
            //data.DisplayNumericalRecords(coveredRecords);

            List<List<List<int?>>> fullStar = aq.CreateFullStarDisjunction(aq.LocalData.PositiveRecords, aq.LocalData.NegativeRecords);

            aq.DisplayFullStarDisjunction(fullStar);



        }
    }
}
