using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AQ11v1
{
    public class AQ11
    {
        public int NumberOfColumns { get; set; }
        public int NumberOfRecords { get; set; }

        public Data LocalData { get; set; }
        public List<List<List<int?>>> fullStar { get; set; }
        public List<List<List<List<int?>>>> positiveFullStar { get; set; }

        public AQ11(Data data)
        {
            LocalData = data;
            NumberOfColumns = data.NumberOfColumns;
            NumberOfRecords = data.NumberOfRecords;
        }

        public void SetData(Data data)
        {
            LocalData = data;
        }

        public void ApplyAlgorithmOnData()
        {
            fullStar = CreateFullStarDisjunction(LocalData.PositiveRecords, LocalData.NegativeRecords);

            positiveFullStar = TransformFullStarToNumericalPositiveFullStar(fullStar);
        }

        public void DisplayResultingRule()
        {
            DisplayPositiveFullStarAsRule(positiveFullStar);
        }

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

            return partialStarDisjunction;
        }

        public void DisplayPartialStarDisjunction(List<int?> partialStarDisjunction)
        {
            for (int i = 0; i < NumberOfColumns - 2; i++)
            {
                Console.Write($" {partialStarDisjunction[i]} |");
            }
            Console.Write("\n");
        }

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

        public void DispalayPartialStarConjunction(List<List<int?>> partialStarConjunction)
        {
            foreach (List<int?> star in partialStarConjunction)
            {
                DisplayPartialStarDisjunction(star);
            }
        }

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


        public  List<List<int>> SelectCoveredRecords(List<List<int>> records, List<List<int?>> conjunction)
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

            return fullStar;

        }


        public void DisplayFullStarDisjunction(List<List<List<int?>>> fullStar)
        {
            foreach (List<List<int?>> conjunction in fullStar)
            {
                Console.WriteLine("Conjunction: ");
                DispalayPartialStarConjunction(conjunction);
            }
        }

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
                        maxIndex = LocalData.AttributesDict[LocalData.Headers[i + 1]].Count - 1; // -1 pretože v AttributeDict v každom zozname atributov mame na 0 pozicie null hodnoto pre zjednoduchšenie procesu.
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

        public void DisplayPositiveDisjunctions(List<List<int?>> disjunctions)
        {
            Console.WriteLine("Disjunctions: ");

            for (int i = 0; i < disjunctions.Count; i++)
            {
                Console.WriteLine($"Positive {LocalData.Headers[i + 1]} disjunction: ");
                for (int j = 0; j < disjunctions[i].Count; j++)
                {
                    Console.Write($" {disjunctions[i][j]} |");
                }
                Console.WriteLine();
            }
        }

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

        public void DisplayPositiveConjunction(List<List<List<int?>>> positiveConjucntion)
        {
            Console.WriteLine("Positive conjunction: ");
            for (int i = 0; i < positiveConjucntion.Count; i++)
            {
                DisplayPositiveDisjunctions(positiveConjucntion[i]);
            }
        }

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

        public void DisplayPositiveFullStar(List<List<List<List<int?>>>> positiveStar)
        {
            for(int i=0; i<positiveStar.Count; i++)
            {
                DisplayPositiveConjunction(positiveStar[i]);
            }
        }

        public void DisplayPositiveFullStarAsRule(List<List<List<List<int?>>>> positiveStar)
        {
            bool useOr = false;
            Console.WriteLine("=======================\nFinal rule: \n=======================");
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

                                Console.Write($"({LocalData.Headers[k+1]} : { LocalData.AttributesDict[LocalData.Headers[k+1]][(int)positiveStar[i][j][k][h]] })");
                            }
                        }
                    }
                }
            }
            Console.WriteLine("\n=======================\n\n");
        }



    }
}
