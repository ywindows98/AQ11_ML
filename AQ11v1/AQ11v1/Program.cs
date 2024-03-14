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

            Data data = new Data();

            data.ReadDataset(datasetPath);

            //data.DisplayHeaders();
            //data.DisplayRecordsString();
            data.DisplayNumericalRecords();


        }
    }
}
