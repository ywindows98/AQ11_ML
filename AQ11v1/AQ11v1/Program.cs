﻿using System;
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
            //definujeme nazov datasetu a cestu ku nemu
            string datasetName = "stroke_dataset_sample.csv";
            string datasetPath = mainProjectFolder + '\\' + datasetName;
            Console.WriteLine("Training dataset path: ");
            Console.WriteLine(datasetPath);

            // Načitame a prespracujeme data.
            Data trainingData = new Data(datasetPath, "stroke", "Yes");

            Console.WriteLine("==============================");
            Console.WriteLine("TRAINING DATASET: ");
            Console.WriteLine("==============================");
            trainingData.DisplayHeaders();
            trainingData.DisplayRecordsString();
            //data.DisplayNumericalRecords();
            Console.WriteLine("\n\n");

            // Vytvoríme inštanciu algoritmu a vložíme do nej dáta.
            AQ11 aq = new AQ11();

            // Applikujeme algoritmus na dáta
            aq.ApplyAlgorithmOnData(trainingData, true);
            //aq.DisplayResultingRule();
            //Console.WriteLine();


            string evaluationName = "stroke_dataset_sample_evaluation.csv";
            string evaluationPath = mainProjectFolder + '\\' + evaluationName;
            Console.WriteLine("Evaluation dataset path: ");
            Console.WriteLine(evaluationPath);

            // Načitame a prespracujeme data.
            Data evaluationData = new Data(evaluationPath, "stroke", "Yes", aq.LocalTrainingData.AttributesDict);

            //Console.WriteLine("==============================");
            //Console.WriteLine("EVALUATION DATASET: ");
            //Console.WriteLine("==============================");
            //evaluationData.DisplayHeaders();
            //evaluationData.DisplayRecordsString();
            //Console.WriteLine("\n");

            aq.EvaluateAlgorithm(evaluationData, true);
            //aq.DisplayEvaluationResults();



            // Použitie algoritmu na jeden záznam
            Console.WriteLine("\n\nGiving opinion on one record: ");
            Console.WriteLine(aq.IsRecordCoveredByPositiveFullStar(evaluationData.Records[38]));

        }
    }
}
