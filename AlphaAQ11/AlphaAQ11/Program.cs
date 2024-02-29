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

        static bool ContainsStar(List<PartialStar> set, PartialStar star)
        {
            foreach (PartialStar st in set)
            {
                if (st.IsSame(star))
                {
                    return true;
                }
            }

            return false;
        }


        //static List<PartialStar> ConjunctSetWithStar(List<PartialStar> set, PartialStar addingStar)
        //{
        //    List<PartialStar> resultingSet = new List<PartialStar>();
        //    PartialStar tempStar= null;
        //    foreach (PartialStar star in set)
        //    {
        //        if(star.Temperature != null)
        //        {
        //            if (addingStar.Temperature != null && star.Temperature == addingStar.Temperature)
        //            {
        //                tempStar = new PartialStar(star.Temperature, null, null);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }

        //            if(addingStar.Headache != null)
        //            {
        //                tempStar = new PartialStar(star.Temperature, addingStar.Headache, null);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }

        //            if (addingStar.Nausea != null)
        //            {
        //                tempStar = new PartialStar(star.Temperature, null, addingStar.Nausea);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }
        //        }


        //        if (star.Headache != null)
        //        {
        //            if (addingStar.Temperature != null)
        //            {
        //                tempStar = new PartialStar(addingStar.Temperature, star.Headache, null);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }

        //            if (addingStar.Headache != null && star.Headache == addingStar.Headache)
        //            {
        //                tempStar = new PartialStar(null, star.Headache, null);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }

        //            if (addingStar.Nausea != null)
        //            {
        //                tempStar = new PartialStar(null, star.Headache, addingStar.Nausea);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }
        //        }


        //        if (star.Nausea != null)
        //        {
        //            if (addingStar.Temperature != null)
        //            {
        //                tempStar = new PartialStar(addingStar.Temperature, null, star.Nausea);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }

        //            if (addingStar.Headache != null)
        //            {
        //                tempStar = new PartialStar(null, addingStar.Headache, star.Nausea);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }

        //            if (addingStar.Nausea != null && star.Nausea == addingStar.Nausea)
        //            {
        //                tempStar = new PartialStar(null, null, star.Nausea);
        //                if (!ContainsStar(resultingSet, tempStar))
        //                {
        //                    resultingSet.Add(tempStar);
        //                }
        //            }
        //        }
        //    }
        //    return resultingSet;
        //}


        //static List<PartialStar> ConjunctSetWithStar(List<PartialStar> set, PartialStar addingStar)
        //{
        //    List<PartialStar> resultingSet = new List<PartialStar>();
        //    PartialStar tempStar = null;
        //    foreach (PartialStar star in set)
        //    {
        //        if (addingStar.Temperature != null)
        //        {
        //            if (star.Temperature == null)
        //            {
        //                tempStar = new PartialStar(addingStar.Temperature, star.Headache, star.Nausea);
        //                resultingSet.Add(tempStar);
        //            }
        //            else
        //            {
        //                resultingSet.Add(star);
        //            }
        //        }

        //        if(addingStar.Headache != null)
        //        {
        //            if(star.Headache == null)
        //            {
        //                tempStar = new PartialStar(star.Temperature, addingStar.Headache, star.Nausea);
        //                resultingSet.Add(tempStar);
        //            }
        //            else
        //            {
        //                resultingSet.Add(star);
        //            }
        //        }

        //        if (addingStar.Nausea != null)
        //        {
        //            if (star.Nausea == null)
        //            {
        //                tempStar = new PartialStar(star.Temperature, star.Headache, addingStar.Nausea);
        //                resultingSet.Add(tempStar);
        //            }
        //            else
        //            {
        //                resultingSet.Add(star);
        //            }
        //        }
        //    }
        //    return resultingSet;
        //}


        static List<Conjunction> ConjunctSetWithStar(List<Conjunction> set, PartialStar addingStar)
        {
            List<Conjunction> resultingSet = new List<Conjunction>();
            Conjunction tempStar = null;
            foreach (Conjunction star in set)
            {
                
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

            List<Conjunction> set1 = new List<Conjunction>();
            set1.Add(new Conjunction(OneToThird.Temperature, null, null));
            set1.Add(new Conjunction(null, OneToThird.Headache, null));
            set1.Add(new Conjunction(null, null, OneToThird.Nausea));

            Console.WriteLine("\n\n");

            set1.ForEach(con => Console.WriteLine(con));

            //List<Conjunction> conjuctionResult = ConjunctSetWithStar(set1, OneToFifth);
            //conjuctionResult.ForEach(ps => Console.WriteLine(ps));

            //Console.WriteLine("\n\n");
            //conjuctionResult = ConjunctSetWithStar(conjuctionResult, OneToSixth);
            //conjuctionResult.ForEach(ps => Console.WriteLine(ps));

            //Console.WriteLine("\n\n");
            //conjuctionResult = ConjunctSetWithStar(conjuctionResult, OneToSeventh);
            //conjuctionResult.ForEach(ps => Console.WriteLine(ps));



        }
    }
}
