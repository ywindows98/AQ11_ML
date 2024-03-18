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

        public AQ11(int numberOfColumns, int numberOfRecords)
        {
            NumberOfColumns = numberOfColumns;
            NumberOfRecords = numberOfRecords;
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
            for (int i=0; i<numberOfNegatives; i++)
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
            for(int i=0; i<primaryStar.Count; i++)
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

    }
}
