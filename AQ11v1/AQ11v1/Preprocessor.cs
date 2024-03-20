using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQ11v1
{
    // Trieda vytvorená špeciálne na prácu so súborom csv a údajmi z neho extrahovanými
    public static class Preprocessor
    {
        // Metóda získa názvy hlavičiek zo súboru csv a vráti zoznam reťazcov obsahujúcich názvy hlavičiek
        public static List<string> GetHeaders(string datasetPath)
        {
            // Používa metódy CsvHelper.
            using (var reader = new StreamReader(datasetPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                List<string> headerTitles = new List<string>();
                headerTitles.Add("Id");
                headerTitles.AddRange(csv.HeaderRecord.ToList());
                return headerTitles;
            }
        }

        // Metóda číta dáta zo súboru csv. Uloží údaje reťazca do Zoznamu zoznamov. Každý zoznam predstavuje stĺpec.
        // K údajom sa pridá aj stĺpec id. Metóda vracia dáta reprezentované zoznamom zoznamov.
        public static List<List<string>> GetRecordsString(string datasetPath)
        {
            using (var reader = new StreamReader(datasetPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                List<string> headerTitles = csv.HeaderRecord.ToList();
                int numberOfColumns = csv.ColumnCount;
                //headerTitles.ForEach(h => Console.WriteLine(csv.GetField(h)));

                List<List<string>> records = new List<List<string>>();
                for (int i = 0; i < numberOfColumns + 1; i++)
                {
                    records.Add(new List<string>());
                }

                int id = 0;
                while (csv.Read())
                {
                    records[0].Add(id.ToString());
                    id++;
                    // Pridanie hodnôt z datasetu do každého zoznamu (stĺpca).
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        records[i + 1].Add(csv.GetField<string>(i));
                    }
                }

                int numberOfRecords = records[0].Count;

                return records;
            }
        }

        // Metóda transformuje počiatočný zoznam zoznamov s nespracovanými dátami na zoznam obsahujúci každý jednotlivý záznam (prípad).
        // Vráti zoznam zoznamov nového formátu.
        public static List<List<string>> TransformRecordsIntoRecordsIndividual(List<List<string>> recordsString)
        {
            List<List<string>> recordsIndividual = new List<List<string>>();
            int numberOfRecords = recordsString[0].Count;
            int numberOfColumns = recordsString.Count;


            for (int i = 0; i < numberOfRecords; i++)
            {
                recordsIndividual.Add(new List<string>());
                // Pridanie všetkých hodnôt atribútov do každého jednotlivého záznamu.
                for (int j = 0; j < numberOfColumns; j++)
                {
                    recordsIndividual[i].Add(recordsString[j][i]);
                }
            }

            return recordsIndividual;
        }

        // Metóda vytvorí slovník na univerzálne uloženie možných hodnôt atribútov a vráti ho. Kľúče sú názvy hlavičiek.
        // indexovanie v setoch sa začina s 1 (0 element je null). Aby bolo ľahšie pracovať s negaciou atributov. 
        public static Dictionary<string, List<string>> CreateAttributesDictionary(List<string> headers, List<List<string>> recordsString)
        {
            Dictionary<string, List<string>> attributesDict = new Dictionary<string, List<string>>();
            int numberOfColumns = recordsString.Count;
            List<string> attrValuesSet = new List<string>();
            // Každému kľúču-hlavičke priradíme množinu jedinečných hodnôt atribútov.
            for (int i = 1; i < numberOfColumns; i++)
            {
                // Získanie všetkých možných hodnôt atribútu v zoznam(set).
                attrValuesSet = recordsString[i].ToHashSet().ToList();
                attrValuesSet.Insert(0, null);
                attributesDict.Add(headers[i], attrValuesSet);
            }

            return attributesDict;
        }

        // Metóda prevodu záznamov reťazca na číselné. Každé číslo predstavuje index hodnoty atribútu v zozname uloženom v slovníku.
        // Vráti Zoznam jednotlivých záznamov (zoznamov) s číselne zastúpenými hodnotami atribútov.
        public static List<List<int>> ConvertRecordsToInt(List<List<string>> recordsIndividual, Dictionary<string, List<string>> attributesDict, List<string> headers)
        {
            List<List<int>> recordsIndividualConverted = new List<List<int>>();

            int numberOfColumns = recordsIndividual[0].Count;
            int numberOfRecords = recordsIndividual.Count;

            for (int i = 0; i < numberOfRecords; i++)
            {
                recordsIndividualConverted.Add(new List<int>());
                // Skopíruje id
                recordsIndividualConverted[i].Add(Int32.Parse(recordsIndividual[i][0]));

                for (int j = 1; j < numberOfColumns; j++)
                {
                    // Ku každej hodnote atribútu reťazca nájdeme index tejto hodnoty uloženej v zozname v slovníku.
                    recordsIndividualConverted[i].Add(attributesDict[headers[j]].FindIndex(x => x == recordsIndividual[i][j]));
                }
            }

            return recordsIndividualConverted;
        }

        // Užitočná metóda na jednoduchšiu transformáciu reťazcových nespracovaných dát na jednotlivé číselné záznamy iba jedným volaním.
        // Vráti Zoznam jednotlivých záznamov (zoznamov) s číselne zastúpenými hodnotami atribútov.
        public static List<List<int>> GetRecordsIndividualNumerical(List<List<string>> recordsString, Dictionary<string, List<string>> attributesDict, List<string> headers)
        {
            List<List<string>> recordsIndividual = TransformRecordsIntoRecordsIndividual(recordsString);
            //DisplayRecordsIndividualString(recordsIndividual);

            List<List<int>> recordsIndividualConverted = ConvertRecordsToInt(recordsIndividual, attributesDict, headers);
            //DisplayNumericalRecords(recordsIndividualConverted);

            return recordsIndividualConverted;
        }

        // Metóda extrakcie pozitívnych záznamov(ktore chceme pokryť) zo zoznamu záznamov.
        public static List<List<int>> GetPositiveRecords(List<List<int>> recordsNumerical, Dictionary<string, List<string>> attributesDict, List<string> headers, string posAttributeName, string posAttributeValue)
        {
            int numberOfColumns = headers.Count;
            int numberOfRecords = recordsNumerical.Count;

            int targetColumnIndex = headers.FindIndex(x => x == posAttributeName);
            if(targetColumnIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute name (target column {posAttributeName})  is not found in the headers list. Name of the attribute has to be identical to the one in the dataset header. It is also, case-sensitive. Make sure the input is correct");
            }
            //Console.WriteLine(targetColumnIndex);
            int targetPositiveValueIndex = attributesDict[posAttributeName].FindIndex(x => x == posAttributeValue);
            if (targetPositiveValueIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute value (target value {posAttributeValue})  is not found in the dictionary under the {posAttributeName} key. Value of the attribute has to be identical to the one in the dataset. It is also, case-sensitive. Make sure the input is correct.");
            }
            //Console.WriteLine(targetPositiveValueIndex);

            List<List<int>> positiveRecords = new List<List<int>>();
            for(int i=0; i<numberOfRecords; i++)
            {
                if (recordsNumerical[i][targetColumnIndex] == targetPositiveValueIndex)
                {
                    positiveRecords.Add(recordsNumerical[i]);
                }
            }

            return positiveRecords;
        }

        // Metóda extrakcie negatívnych záznamov(ktore nechceme pokryť) zo zoznamu záznamov.
        public static List<List<int>> GetNegativeRecords(List<List<int>> recordsNumerical, Dictionary<string, List<string>> attributesDict, List<string> headers, string posAttributeName, string posAttributeValue)
        {
            int numberOfColumns = headers.Count;
            int numberOfRecords = recordsNumerical.Count;

            int targetColumnIndex = headers.FindIndex(x => x == posAttributeName);
            if (targetColumnIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute name (target column {posAttributeName})  is not found in the headers list. Name of the attribute has to be identical to the one in the dataset header. It is also, case-sensitive. Make sure the input is correct");
            }
            //Console.WriteLine(targetColumnIndex);
            int targetPositiveValueIndex = attributesDict[posAttributeName].FindIndex(x => x == posAttributeValue);
            if (targetPositiveValueIndex == -1)
            {
                throw new ArgumentException($"Given positive attribute value (target value {posAttributeValue})  is not found in the dictionary under the {posAttributeName} key. Value of the attribute has to be identical to the one in the dataset. It is also, case-sensitive. Make sure the input is correct.");
            }
            //Console.WriteLine(targetPositiveValueIndex);

            List<List<int>> negativeRecords = new List<List<int>>();
            for (int i = 0; i < numberOfRecords; i++)
            {
                if (recordsNumerical[i][targetColumnIndex] != targetPositiveValueIndex)
                {
                    negativeRecords.Add(recordsNumerical[i]);
                }
            }

            return negativeRecords;
        }
    }
}
