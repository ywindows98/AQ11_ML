using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AlphaAQ11
{
    internal class Program
    {

        static List<PartialStar> ConjunctSetWithStar(List<PartialStar> set, PartialStar addingStar)
        {
            List<PartialStar> resultingSet = new List<PartialStar>();
            PartialStar tempStar= null;
            foreach (PartialStar star in set)
            {
                if(star.Temperature != null)
                {
                    if (addingStar.Temperature != null && star.Temperature == addingStar.Temperature)
                    {
                        tempStar = new PartialStar(star.Temperature, null, null);
                        if (!resultingSet.Contains(tempStar))
                        {
                            resultingSet.Add(tempStar);
                        }
                    }

                    if(addingStar.Headache != null)
                    {
                        tempStar = new PartialStar(star.Temperature, addingStar.Headache, null);
                        if (!resultingSet.Contains(tempStar))
                        {
                            resultingSet.Add(tempStar);
                        }
                    }

                    if (addingStar.Nausea != null)
                    {
                        tempStar = new PartialStar(star.Temperature, null, addingStar.Nausea);
                        if (!resultingSet.Contains(tempStar))
                        {
                            resultingSet.Add(tempStar);
                        }
                    }
                }


                if (star.Headache != null)
                {
                    if (addingStar.Temperature != null)
                    {
                        resultingSet.Add(new PartialStar(addingStar.Temperature, star.Headache, null));
                    }

                    if (addingStar.Headache != null && star.Headache == addingStar.Headache)
                    {
                        resultingSet.Add(new PartialStar(null, star.Headache, null));
                    }

                    if (addingStar.Nausea != null)
                    {
                        resultingSet.Add(new PartialStar(null, star.Headache, addingStar.Nausea));
                    }
                }


                if (star.Nausea != null)
                {
                    if (addingStar.Temperature != null)
                    {
                        resultingSet.Add(new PartialStar(addingStar.Temperature, null, star.Nausea));
                    }

                    if (addingStar.Headache != null)
                    {
                        resultingSet.Add(new PartialStar(null, addingStar.Headache, star.Nausea));
                    }

                    if (addingStar.Nausea != null && star.Nausea == addingStar.Nausea)
                    {
                        resultingSet.Add(new PartialStar(null, null, star.Nausea));
                    }
                }
            }
            return resultingSet;
        }

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

            PartialStar OneToThird = new PartialStar(positiveSet[0], negativeSet[0]);
            Console.WriteLine(OneToThird);

            PartialStar OneToFifth = new PartialStar(positiveSet[0], negativeSet[1]);
            Console.WriteLine(OneToFifth);

            PartialStar OneToSixth = new PartialStar(positiveSet[0], negativeSet[2]);
            Console.WriteLine(OneToSixth);

            PartialStar OneToSeventh = new PartialStar(positiveSet[0], negativeSet[3]);
            //Console.WriteLine(OneToSeventh);

            List<PartialStar> set1 = new List<PartialStar>();
            set1.Add(OneToThird);

            Console.WriteLine("\n\n");

            List<PartialStar> conjuctionResult = ConjunctSetWithStar(set1, OneToFifth);
            conjuctionResult.ForEach(ps => Console.WriteLine(ps));

            Console.WriteLine("\n\n");
            conjuctionResult = ConjunctSetWithStar(conjuctionResult, OneToSixth);
            conjuctionResult.ForEach(ps => Console.WriteLine(ps));

            PartialStar part1 = new PartialStar(Temperature.NotNormal, null, null);
            Console.WriteLine("\n\n");
            Console.WriteLine(part1.IsSame(conjuctionResult[0]));
            
            
        }
    }
}
