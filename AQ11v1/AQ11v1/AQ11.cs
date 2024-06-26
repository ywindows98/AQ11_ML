﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AQ11v1
{   
    // Class represents the algorithm
    public class AQ11
    {
        public int NumberOfColumns { get; set; }
        public int NumberOfRecords { get; set; }

        public Data LocalTrainingData { get; set; }

        public Data EvaluationData { get; set; }
        public List<List<List<int?>>> fullStar { get; set; }
        public List<List<List<List<int?>>>> PositiveFullStar { get; set; }


        public int NumberOfTP { get; set; }
        public int NumberOfFP { get; set; }
        public int NumberOfFN { get; set; }
        public int NumberOfTN { get; set; }

        public float Precision { get; set; }
        public float Recall { get; set; }
        public float F1 { get; set; }
        public float Accuracy { get; set; }

        public AQ11()
        {

        }
        public AQ11(Data data)
        {
            SetData(data);
        }

        // Method to set another training data instance for an algorithm and to avctualize number of columns and number of records
        public void SetData(Data data)
        {
            LocalTrainingData = data;
            NumberOfColumns = data.NumberOfColumns;
            NumberOfRecords = data.NumberOfRecords;
        }

        // Method to set another evaluation data instance for the algorithm
        public void SetEvaluationData(Data data)
        {
            EvaluationData = data;
        }

        // Method to apply algirithm on data that are currently stored in the algorithm instance
        // If display parameter is true, resulting rule will be displayed in the console automatically
        public void ApplyAlgorithmOnData(bool display=false)
        {
            fullStar = CreateFullStarDisjunction(LocalTrainingData.PositiveRecords, LocalTrainingData.NegativeRecords);

            PositiveFullStar = TransformFullStarToNumericalPositiveFullStar(fullStar);

            if (display)
            {
                DisplayResultingRule();
            }
        }

        // Method applies algorithm on data that are not currently stored in the AQ11 instance, but on new Data instance
        // If display parameter is true, resulting rule will be displayed in the console automatically
        public void ApplyAlgorithmOnData(Data data, bool display=false)
        {
            LocalTrainingData = data;
            NumberOfColumns = data.NumberOfColumns;
            NumberOfRecords = data.NumberOfRecords;

            fullStar = CreateFullStarDisjunction(LocalTrainingData.PositiveRecords, LocalTrainingData.NegativeRecords);

            PositiveFullStar = TransformFullStarToNumericalPositiveFullStar(fullStar);

            if (display)
            {
                DisplayResultingRule();
            }
        }

        // Method to display in the console resulting rule that is stored in the instance
        public void DisplayResultingRule()
        {
            DisplayPositiveFullStarAsRule(PositiveFullStar);
            Console.WriteLine();
        }

        // Method to create partial star disjunction(star G - positive example and counterexample)
        // Output is the star that is represented as list of integer values
        public List<int?> CreatePartialStarDisjunction(List<int> positiveRecord, List<int> negativeRecord)
        {
            List<int?> partialStarDisjunction = new List<int?>();
            for (int i = 1; i < NumberOfColumns - 1; i++)
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

            bool log = true;
            for (int i = 0; i < partialStarDisjunction.Count; i++)
            {
                if (partialStarDisjunction[i]!=null)
                {
                    log = false;
                }
            }

            if (log)
            {
                Console.WriteLine($"Positive record id: {positiveRecord[0]} and negative record id: {negativeRecord[0]} are the same. Algorithm can not train on this dataset.");
                
            }

            return partialStarDisjunction;
        }

        // Method to display given partial star disjuncion in the console
        public void DisplayPartialStarDisjunction(List<int?> partialStarDisjunction)
        {
            for (int i = 0; i < NumberOfColumns - 2; i++)
            {
                Console.Write($" {partialStarDisjunction[i]} |");
            }
            Console.Write("\n");
        }

        // Method to create partial star conjunction (star G - positive example and all counterexamples)
        // Output is a star represented as a list of lists with integer values
        public List<List<int?>> CreatePartialStarConjunction(List<int> positiveRecord, List<List<int>> negativeRecords)
        {
            List<List<int?>> partialStarConjunction = new List<List<int?>>();

            int numberOfNegatives = negativeRecords.Count;

            List<int?> partialStarDisjunction = new List<int?>();
            for (int i = 0; i < numberOfNegatives; i++)
            {
                partialStarDisjunction = CreatePartialStarDisjunction(positiveRecord, negativeRecords[i]);
                partialStarConjunction.Add(partialStarDisjunction);
            }

            return partialStarConjunction;
        }

        // Method to display given partial star conjunction
        public void DisplayPartialStarConjunction(List<List<int?>> partialStarConjunction)
        {
            foreach (List<int?> star in partialStarConjunction)
            {
                DisplayPartialStarDisjunction(star);
            }
        }

        // Method to check if given secondary can be absorbed by given primary star. (star G - positive example and counterexample)
        // Output is boolean value
        private bool CanBeAbsorbed(List<int?> primaryStar, List<int?> secondaryStar)
        {
            for (int i = 0; i < primaryStar.Count; i++)
            {
                if (primaryStar[i] != null)
                {
                    if (secondaryStar[i] != primaryStar[i])
                    {
                        return false;
                    }
                }
            }

            //Console.WriteLine("======");
            //DisplayPartialStarDisjunction(primaryStar);
            //Console.WriteLine("DELETES");
            //DisplayPartialStarDisjunction(secondaryStar);
            //Console.WriteLine();
            return true;
        }
        
        // Method to apply absorption law on partial star conjunction. Method removes each partial star disjunction that can be absorbed
        // Output is the partial star conjunction after the absorption law
        public List<List<int?>> ApplyAbsorptionLawOnConjunction(List<List<int?>> partialStarConjucntion)
        {
            List<List<int?>> absorptionList = new List<List<int?>>();

            for (int i = 0; i < partialStarConjucntion.Count; i++)
            {
                absorptionList.Clear();
                for (int j = 0; j < partialStarConjucntion.Count; j++)
                {
                    if (i != j)
                    {
                        if (CanBeAbsorbed(partialStarConjucntion[i], partialStarConjucntion[j]))
                        {
                            absorptionList.Add(partialStarConjucntion[j]);
                        }
                    }
                }

                for (int k = 0; k < absorptionList.Count; k++)
                {
                    i = 0;
                    partialStarConjucntion.Remove(absorptionList[k]);
                }
            }


            return partialStarConjucntion;
        }

        // Method to check if given record (example) is covered by given disjunction
        // Outout is boolean value
        public bool IsRecordCoveredByDisjunction(List<int> record, List<int?> disjunction)
        {
            for (int i = 0; i < disjunction.Count; i++)
            {
                if (disjunction[i] != null)
                {
                    if (disjunction[i] > 0)
                    {
                        if (record[i + 1] == disjunction[i])
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (record[i + 1] != disjunction[i]*(-1))
                        {
                            return true;
                        }
                    }
                }
            }

            //DisplayNotCoverage(record, disjunction);
            return false;
        }

        // Method to display specific given record that was not covered by given disjunction
        public void DisplayNotCoverage(List<int> record, List<int?> disjunction)
        {
            Console.WriteLine("=====================");
            Console.WriteLine("Record:");
            for (int i = 0; i < record.Count; i++)
            {
                Console.Write($" {record[i]} |");
            }
            Console.WriteLine("\nWas not covered by:");
            for (int i = 0; i < disjunction.Count(); i++)
            {
                Console.Write($" {disjunction[i]} |");
            }
            Console.WriteLine();
        }

        // Method to check if given record is covered by given partial star conjunction
        // Output is boolean value
        public bool IsRecordCoveredByConjunction(List<int> record, List<List<int?>> conjunction)
        {
            for(int i=0; i<conjunction.Count; i++)
            {
                if (!IsRecordCoveredByDisjunction(record, conjunction[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // Method to select records that were covered by given conjunction from given records list
        // Output is list of records
        public List<List<int>> SelectCoveredRecords(List<List<int>> records, List<List<int?>> conjunction)
        {
            List<List<int>> coveredRecords = new List<List<int>>();

            foreach (List<int> record in records)
            {
                if(IsRecordCoveredByConjunction(record, conjunction))
                {
                    coveredRecords.Add(record);
                }
            }

            return coveredRecords;
        }

        // Method to create full star disjunction (star G - all positive examples and all negative counterexamples)
        // Output is list of partial star conjunctions
        public List<List<List<int?>>> CreateFullStarDisjunction(List<List<int>> positiveRecords, List<List<int>> negativeRecords)
        {
            List<List<int>> positiveRecordsTemp = positiveRecords;
            List<List<List<int?>>> fullStar = new List<List<List<int?>>>();

            List<List<int?>> conjunctionTemp = new List<List<int?>>();
            List<List<int>> coveredTemp = new List<List<int>>();
            while (positiveRecords.Count != 0)
            {
                conjunctionTemp = CreatePartialStarConjunction(positiveRecordsTemp[0], negativeRecords);
                conjunctionTemp = ApplyAbsorptionLawOnConjunction(conjunctionTemp);

                fullStar.Add(conjunctionTemp);

                coveredTemp = SelectCoveredRecords(positiveRecordsTemp, conjunctionTemp);

                foreach(List<int> record in coveredTemp)
                {
                    positiveRecordsTemp.Remove(record);
                }
            }
            //Console.WriteLine("FullstarDisjunction is created");
            return fullStar;

        }

        // Method to display full star disjunction in the console
        public void DisplayFullStarDisjunction(List<List<List<int?>>> fullStar)
        {
            foreach (List<List<int?>> conjunction in fullStar)
            {
                Console.WriteLine("Conjunction: ");
                DisplayPartialStarConjunction(conjunction);
            }
        }

        // Method to transform each negative value in partial star disjunction into new disjunction that contains positive values
        public List<List<int?>> TransformNegationsIntoDisjunctions(List<int?> negations)
        {
            List<List<int?>> disjunctions = new List<List<int?>>();
            List<int?> disjunctionTemp = new List<int?>();
            int maxIndex = 0;
            for (int i = 0; i < negations.Count; i++)
            {
                if (negations[i]!=null)
                {
                    disjunctionTemp = new List<int?>();
                    if (negations[i]<0)
                    {
                        maxIndex = LocalTrainingData.AttributesDict[LocalTrainingData.Headers[i + 1]].Count - 1; // -1 because in AttributeDict in each list of attribute values we have null on index 0 for process simplification
                        for (int j=1; j<=maxIndex; j++)
                        {
                            if (negations[i] * (-1) != j)
                            {
                                disjunctionTemp.Add(j);
                            }
                        }
                    }
                    else
                    {
                        disjunctionTemp.Add(negations[i]);
                    }
                }
                else
                {
                    disjunctionTemp = new List<int?>();
                }

                disjunctions.Add(disjunctionTemp);
            }

            return disjunctions;
        }

        // Method to display list of positive disjunctions
        public void DisplayPositiveDisjunctions(List<List<int?>> disjunctions)
        {
            Console.WriteLine("Disjunctions: ");

            for (int i = 0; i < disjunctions.Count; i++)
            {
                Console.WriteLine($"Positive {LocalTrainingData.Headers[i + 1]} disjunction: ");
                for (int j = 0; j < disjunctions[i].Count; j++)
                {
                    Console.Write($" {disjunctions[i][j]} |");
                }
                Console.WriteLine();
            }
        }

        // Method that uses method TransformNegationsIntoDisjunctions to transform negative disjunctions in given conjunction into positive disjunctions
        public List<List<List<int?>>> TransformNegativeConjunctionIntoPositive(List<List<int?>> conjucntion)
        {
            List<List<List<int?>>> positiveConjunction = new List<List<List<int?>>>();
            //List<List<int?>> disjunctionTemp;

            for(int i=0; i<conjucntion.Count; i++)
            {
                positiveConjunction.Add(TransformNegationsIntoDisjunctions(conjucntion[i]));

            }

            return positiveConjunction;
        }

        // Method to display given conjunction with positive dinsjunctions in the console
        public void DisplayPositiveConjunction(List<List<List<int?>>> positiveConjucntion)
        {
            Console.WriteLine("Positive conjunction: ");
            for (int i = 0; i < positiveConjucntion.Count; i++)
            {
                DisplayPositiveDisjunctions(positiveConjucntion[i]);
            }
        }

        // Method to transform given full star with negative values into fullstart that contains positive values that specify the coverage
        // Disjuncton / Conjunctions / Disjunctions / Disjunctions
        public List<List<List<List<int?>>>> TransformFullStarToNumericalPositiveFullStar(List<List<List<int?>>> fullStar)
        {
            List<List<List<List<int?>>>> positiveStar = new List<List<List<List<int?>>>>();
            for (int i = 0; i < fullStar.Count; i++)
            {
                positiveStar.Add(TransformNegativeConjunctionIntoPositive(fullStar[i]));
            }

            return positiveStar;
        }

        // Method to display given full star disjunction in the console
        public void DisplayPositiveFullStar(List<List<List<List<int?>>>> positiveStar)
        {
            for(int i=0; i<positiveStar.Count; i++)
            {
                DisplayPositiveConjunction(positiveStar[i]);
            }
        }

        // Method to display given positive full star, as a formatted text rule that can be understood by a user, in the console.
        public void DisplayPositiveFullStarAsRule(List<List<List<List<int?>>>> positiveStar)
        {
            bool useOr = false;
            Console.WriteLine("=======================\nFINAL RULE: \n=======================");
            for(int i=0; i<positiveStar.Count; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine("\n=======================");
                    Console.WriteLine(" OR");
                    Console.WriteLine("=======================");
                }
                for(int j=0; j < positiveStar[i].Count; j++)
                {

                    if (j != 0)
                    {
                        Console.WriteLine("\n--------------------");
                        Console.WriteLine(" AND");
                        Console.WriteLine("--------------------");
                    }
                    useOr = false;
                    for(int k=0; k < positiveStar[i][j].Count; k++)
                    {
                        if (positiveStar[i][j][k].Count !=0)
                        {
                            if (useOr)
                            {
                                Console.WriteLine(" or");
                            }
                            else
                            {
                                useOr = true;
                            }

                            for(int h=0; h < positiveStar[i][j][k].Count; h++)
                            {
                                if (h != 0) Console.Write(" or ");

                                Console.Write($"({LocalTrainingData.Headers[k+1]} : { LocalTrainingData.AttributesDict[LocalTrainingData.Headers[k+1]][(int)positiveStar[i][j][k][h]] })");
                            }
                        }
                    }
                }
            }
            Console.WriteLine("\n=======================\n\n");
        }


        // METHODS OF ALGORITHM EVALUATION

        // Method to check if given record is covered by the given positive disjunction (that contains other positive disjuntions)
        public bool IsRecordCoveredByPositiveDisjunction(List<int> record, List<List<int?>> disjunction)
        {
            for (int i = 1; i < record.Count-1; i++)
            {
                if (disjunction[i - 1].Count != 0)
                {
                    if (disjunction[i - 1].Contains(record[i]))
                    {
                        //Console.WriteLine($" {EvaluationData.Headers[i]} : {record[i]} ");
                        return true;
                    }
                }
            }

            //Console.WriteLine("Was not covered");
            return false;
        }

        // Method to check if given record is covered by the given positive conjucntion that contains disjunctions
        public bool IsRecordCoveredByPositiveConjunction(List<int> record, List<List<List<int?>>> conjunction)
        {
            for (int i=0; i<conjunction.Count; i++)
            {
                if (!IsRecordCoveredByPositiveDisjunction(record, conjunction[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // Method to check if given record is covered by the given positive full star
        public bool IsRecordCoveredByPositiveFullStar(List<int> record, List<List<List<List<int?>>>> positiveStar)
        {
            for (int i=0; i<positiveStar.Count; i++)
            {
                if (IsRecordCoveredByPositiveConjunction(record, positiveStar[i]))
                {
                    return true;
                }
            }

            return false;
        }
        public bool IsRecordCoveredByPositiveFullStar(List<int> record)
        {
            for (int i = 0; i < PositiveFullStar.Count; i++)
            {
                if (IsRecordCoveredByPositiveConjunction(record, PositiveFullStar[i]))
                {
                    return true;
                }
            }

            return false;
        }

        // Method to calculate number of True Positive rusults given the list of positive records and positive full start that represents a rule
        public int GetNumberOfTruePositives(List<List<int>> positiveRecords, List<List<List<List<int?>>>> positiveStar)
        {
            int numberOfTP = 0;

            for (int i=0; i<positiveRecords.Count; i++)
            {
                if (IsRecordCoveredByPositiveFullStar(positiveRecords[i], positiveStar))
                {
                    numberOfTP += 1;
                }
            }


            return numberOfTP;
        }

        // Method to calculate number of True Negative rusults given the list of negative records and positive full start that represents a rule
        public int GetNumberOfTrueNegatives(List<List<int>> negativeRecords, List<List<List<List<int?>>>> positiveStar)
        {
            int numberOfTN =negativeRecords.Count;

            for (int i = 0; i < negativeRecords.Count; i++)
            {
                if (IsRecordCoveredByPositiveFullStar(negativeRecords[i], positiveStar))
                {
                    numberOfTN -= 1;
                }
            }

            
            return numberOfTN;
        }

        // Method to calculate number of False Positive rusults given the list of negative records and positive full start that represents a rules
        public int GetNumberOfFalsePositives(List<List<int>> negativeRecords, List<List<List<List<int?>>>> positiveStar)
        {
            int numberOfFP = 0;

            for (int i = 0; i < negativeRecords.Count; i++)
            {
                if (IsRecordCoveredByPositiveFullStar(negativeRecords[i], positiveStar))
                {
                    numberOfFP += 1;
                }
            }
            return numberOfFP;
        }

        // Method to calculate number of False Negative rusults given the list of positive records and positive full start that represents a rule
        public int GetNumberOfFalseNegatives(List<List<int>> positiveRecords, List<List<List<List<int?>>>> positiveStar)
        {
            int numberOfFN = positiveRecords.Count;

            for (int i = 0; i < positiveRecords.Count; i++)
            {
                if (IsRecordCoveredByPositiveFullStar(positiveRecords[i], positiveStar))
                {
                    numberOfFN -= 1;
                }
            }

            return numberOfFN;
        }

        // Method calculates numbres of TP, FP, FN, TN and Precision, Recall, F1, Accuracy metrics for the positive full star (rule) and evaluation data that are currently stored in the algorithm instance
        // Stores results in the instance of the algorithm
        // If display parameter is true, resulting rule will be displayed in the console automatically
        public void EvaluateAlgorithm(bool display = false)
        {
            NumberOfTP = GetNumberOfTruePositives(EvaluationData.PositiveRecords, PositiveFullStar);
            NumberOfFP = GetNumberOfFalsePositives(EvaluationData.NegativeRecords, PositiveFullStar);
            NumberOfFN = GetNumberOfFalseNegatives(EvaluationData.PositiveRecords, PositiveFullStar);
            NumberOfTN = GetNumberOfTrueNegatives(EvaluationData.NegativeRecords, PositiveFullStar);

            Precision = ((float)NumberOfTP) / ((float)NumberOfTP + (float)NumberOfFP);
            Recall = ((float)NumberOfTP) / ((float)NumberOfTP + (float)NumberOfFN);

            F1 = 2f * ((Precision * Recall) / (Precision + Recall));

            Accuracy = ((float)NumberOfTP + (float)NumberOfTN) / ((float)(NumberOfTP + NumberOfFP + NumberOfFN + NumberOfTN));

            if (display)
            {
                DisplayEvaluationResults();
            }
        }

        // Method calculates numbres of TP, FP, FN, TN and Precision, Recall, F1, Accuracy metrics for the positive full star (rule) that is currently stored in the algorithm instance
        // and for the given evaluation data as a parameter 
        // Stores results in the instance of the algorithm
        // If display parameter is true, resulting rule will be displayed in the console automatically
        public void EvaluateAlgorithm(Data evalData, bool display=false)
        {
            EvaluationData = evalData;
            EvaluateAlgorithm(display);
        }

        // Method to display in the console results of evaluation that are stored in the instance
        public void DisplayEvaluationResults()
        {
            Console.WriteLine("====================");
            Console.WriteLine("EVALUATION RESULTS:");
            Console.WriteLine($"  TP: {NumberOfTP}  |  FP: {NumberOfFP}");
            Console.WriteLine($"  FN: {NumberOfFN}  |  TN: {NumberOfTN}");
            Console.WriteLine("--------------------");
            Console.WriteLine($"  Precision: {Precision}");
            Console.WriteLine($"  Recall: {Recall}");
            Console.WriteLine($"  F1: {F1}");
            Console.WriteLine($"  Accuracy: {Accuracy}");
            Console.WriteLine("====================");
        }

    }
}
