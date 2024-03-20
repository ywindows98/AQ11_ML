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

            List<int> positiveRecord = data.PositiveRecords[0];
            List<int> negativeRecord = data.NegativeRecords[3];

            AQ11 aq = new AQ11(data);

            //List<int?> partialStarDisjunction = aq.CreatePartialStarDisjunction(positiveRecord, negativeRecord);

            //aq.DisplayPartialStarDisjunction(partialStarDisjunction);

            List<List<int?>> partialStarConjunction = aq.CreatePartialStarConjunction(positiveRecord, data.NegativeRecords);
            aq.DispalayPartialStarConjunction(partialStarConjunction);

            Console.WriteLine("=============================\n\n==============");

            List<List<int?>> afterAbsorption = aq.ApplyAbsorptionLawOnConjunction(partialStarConjunction);
            aq.DispalayPartialStarConjunction(afterAbsorption);


            Console.WriteLine(aq.IsRecordCoveredByConjunction(data.NegativeRecords[2], afterAbsorption));
        }
    }
}
