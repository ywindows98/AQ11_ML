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

            DisplayNotCoverage(record, disjunction);
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



    }
}
