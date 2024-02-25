using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaAQ11
{
    internal class Program
    {

        static void Main(string[] args)
        {
            List<Temperature> temperatureList = new List<Temperature> { Temperature.High, Temperature.VeryHigh, Temperature.Normal, Temperature.High,
                                                                        Temperature.High, Temperature.Normal, Temperature.Normal };

            List<bool> headacheList = new List<bool> { true, true, false, true, false, true, false };
            List<bool> nauseaList = new List<bool> { false, true, false, true, true, false, true };

            List<bool> decisionFluList = new List<bool> { true, true, false, true, false, false, false };


            int numberOfCases = decisionFluList.Count();

            List<Case> positiveSet = new List<Case>(); // List of Positive Flu cases // Concept
            List<Case> negativeSet = new List<Case>(); // List of Negative Flu cases 

            for(int i=0; i<numberOfCases; i++)
            {
                if (decisionFluList[i])
                {
                    positiveSet.Add(new Case(i + 1, temperatureList[i], headacheList[i], nauseaList[i], decisionFluList[i]));
                }
                else
                {
                    negativeSet.Add(new Case(i + 1, temperatureList[i], headacheList[i], nauseaList[i], decisionFluList[i]));
                }
            }

            // positiveSet[0] will be the seed



        }
    }
}
