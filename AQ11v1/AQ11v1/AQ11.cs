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

        // Metóda na nastavenie iného súboru údajov pre trenovanie algoritmu.
        public void SetData(Data data)
        {
            LocalTrainingData = data;
            NumberOfColumns = data.NumberOfColumns;
            NumberOfRecords = data.NumberOfRecords;
        }

        // Metóda na nastavenie iného súboru údajov pre hodnotenie algoritmu.
        public void SetEvaluationData(Data data)
        {
            EvaluationData = data;
        }

        // Metóda aplikácie algoritmu na dáta, ktoré sú aktuálne uložené v inštancii AQ11.
        // Parameter "display" je zodpovedný za rozhodnutie, či sa výsledky zobrazia v konzole.
        public void ApplyAlgorithmOnData(bool display=false)
        {
            fullStar = CreateFullStarDisjunction(LocalTrainingData.PositiveRecords, LocalTrainingData.NegativeRecords);

            PositiveFullStar = TransformFullStarToNumericalPositiveFullStar(fullStar);

            if (display)
            {
                DisplayResultingRule();
            }
        }
        // Metóda aplikácie algoritmu na externé dáta zadané ako parameter.
        // Parameter "display" je zodpovedný za rozhodnutie, či sa výsledky zobrazia v konzole.
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

        // Metóda na zobrazenie výsledného pravidla vytvoreného algoritmom a uloženého v inštancii.
        public void DisplayResultingRule()
        {
            DisplayPositiveFullStarAsRule(PositiveFullStar);
            Console.WriteLine();
        }


        // POMOCNE METÓDY PRE FUNGOVANIE ALGORIMU

        // Metóda na vytvorenie čiastočnej hviezdy-disjunkcie (obálka G - pozitívny príklad voči kontrapríkladu) z jedného pozitívneho záznamu a jedného negatívneho záznamu. Výstupom je čiastočná obálka-disjunkcia.
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

        // Metóda zobrazenia danej ako parameter obálky (obálka G - pozitívny príklad voči kontrapríkladu).
        public void DisplayPartialStarDisjunction(List<int?> partialStarDisjunction)
        {
            for (int i = 0; i < NumberOfColumns - 2; i++)
            {
                Console.Write($" {partialStarDisjunction[i]} |");
            }
            Console.Write("\n");
        }

        // Metóda na vytvorenie čiastočnej obálky-konjunkcie (obálka G - pozitívny príklad voči všetkým kontrapríkladom) z jedného pozitívneho záznamu a všetkych negatívnych záznamov. Výstupom je čiastočná obálka-konjunkcia.
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

        // Metóda zobrazenia danej ako parameter obálky (obálka G - pozitívny príklad voči všetkým kontrapríkladom).
        public void DisplayPartialStarConjunction(List<List<int?>> partialStarConjunction)
        {
            foreach (List<int?> star in partialStarConjunction)
            {
                DisplayPartialStarDisjunction(star);
            }
        }

        // Metóda na kontrolu, či sekundárna obláka môže byť absorbovaná pomocou primárnej obálky. Obálky sú uvedené ako parametre. (obálka G - pozitívny príklad voči kontrapríkladu). Výstup je boolovská hodnota.
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

        // Metóda aplikácie absorpčného zákona na čiastočnú obálku-konjunkciu. Metóda odstraňuje akúkoľvek čiastočnú obálku-disjunkciu, ktorá môže byť absorbovaná z čiastočnej obálky-konjunkcie. Výstupom je zmenená čiastočná obálka-konjunkcia.
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

        // Metóda kontroly, či konkrétny daný záznam je pokrytý danou čiastočnou obálkou-disjunkciou. Výstup je boolovská hodnota.
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

        // Metóda zobrazenia konkrétneho daného záznamu, ktorý nebol pokrytý danou obálkovou disjunkciou.
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

        // Metóda kontroly, či konkrétny daný záznam je pokrytý danou čiastkovou obálkou-konjunkciou. Výstup je boolovská hodnota.
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

        // Metóda na výber záznamov, ktoré sú pokryté obálkou-konjunkciou, zo zoznamu záznamov. Pokryté záznamy sú uložené v druhom zozname. Výstupom je zoznam pokrytých záznamov.
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

        // Metóda na vytvorenie plnej obálky-disjunkcie. (obálka G - všetky pozitívne príklady voči všetkym kontrapríkladom). Výstup je plna obálka-disjunkcia.
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

        // Metóda zobrazenia plnej obálkovej disjunkcie.
        public void DisplayFullStarDisjunction(List<List<List<int?>>> fullStar)
        {
            foreach (List<List<int?>> conjunction in fullStar)
            {
                Console.WriteLine("Conjunction: ");
                DisplayPartialStarConjunction(conjunction);
            }
        }

        // Metóda transformuje každú negáciu v čiastočnej obál-disjunkcii na disjunkcie obsahujúce pozitivne hodnoty. Výstupom je zoznam samostatných pozitívnych disjunkcií pre každý atribút.
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
                        maxIndex = LocalTrainingData.AttributesDict[LocalTrainingData.Headers[i + 1]].Count - 1; // -1 pretože v AttributeDict v každom zozname atributov mame na 0 pozicie null hodnoto pre zjednoduchšenie procesu.
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

        // Metóda na zobrazenie zoznamu pozitívnych disjunkcií.
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

        // Metóda, ktorá používa metódu "TransformNegationsIntoDisjunctions" na transformáciu negativnych disjunkcii v spojkách na pozitivne disjunkcie.
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

        // Metóda zobrazenia konjunkcie s pozitivnymi disjunkciami.
        public void DisplayPositiveConjunction(List<List<List<int?>>> positiveConjucntion)
        {
            Console.WriteLine("Positive conjunction: ");
            for (int i = 0; i < positiveConjucntion.Count; i++)
            {
                DisplayPositiveDisjunctions(positiveConjucntion[i]);
            }
        }

        // Metóda transformácie prvkov obálky-disjunkcie na pozitívne. Výstup je plná obálka-disjunkcia iba s pozitivnymi hodnotami.
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

        // Metóda zobrazenia plnej obálky-disjunkcie.
        public void DisplayPositiveFullStar(List<List<List<List<int?>>>> positiveStar)
        {
            for(int i=0; i<positiveStar.Count; i++)
            {
                DisplayPositiveConjunction(positiveStar[i]);
            }
        }

        // Metóda na zobrazenie úplnej obálky-disjunkcie, ktorá obsahuje iba pozitivne hodnoty ako formátované pravidlo napísané v texte.
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


        // METÓDY HODNOTENIA ALGORITHMU

        // Metóda kontroly či je záznam pokrytý jedinou pozitivnou disjunkciou.
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

        // Metóda kontroly, či je záznam pokrytý konjunkciou, ktorá obsahuje disjunkcie.
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

        // Metóda kontroly, či je záznam pokrytý celou pozitívnou obálkou.
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

        // Metóda vypočíta počet True Positive výsledkov vzhľadom na vstupné pozitívne záznamy a pozitívnu celu obálku.
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

        // Metóda vypočíta počet True Neagtive výsledkov vzhľadom na vstupné negatívne záznamy a pozitívnu celu obálku.
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

        // Metóda vypočíta počet False Positive výsledkov vzhľadom na vstupné negatívne záznamy a pozitívnu celu obálku.
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

        // Metóda vypočíta počet False Negative výsledkov vzhľadom na vstupné pozitívne záznamy a pozitívnu celu obálku.
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

        // Metóda počíta počet TP, FP, FN, TN a Precision, Recall, F1, Accuracy pre uložené EvaluationData v inštancii algoritmu. A uloží výsledky do inštancie algoritmu.
        // Parameter "display" je zodpovedný za rozhodnutie, či sa výsledky zobrazia v konzole.
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

        // Metóda vypočíta počet TP, FP, FN, TN a Precision, Recall, F1, Accuracy pre dané EvaluationData a uloží EvaluationData a výsledky do inštancie algoritmu.
        // Parameter "display" je zodpovedný za rozhodnutie, či sa výsledky zobrazia v konzole.
        public void EvaluateAlgorithm(Data evalData, bool display=false)
        {
            EvaluationData = evalData;
            EvaluateAlgorithm(display);
        }

        // Metóda na zobrazenie výsledkov hodnotenia, ktoré sú uložené v inštancie algoritmu.
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
