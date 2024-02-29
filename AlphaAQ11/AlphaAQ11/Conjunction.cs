using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaAQ11
{
    public class Conjunction
    {
        public Temperature? Temperature { get; set; }
        public bool? Headache { get; set; }
        public bool? Nausea { get; set; }

        public Conjunction(Temperature? temperature, bool? headache, bool? nausea)
        {
            Temperature = temperature;
            Headache = headache;
            Nausea = nausea;
        }

        public bool IsSame(Conjunction comparedConjunction)
        {
            if (Temperature != comparedConjunction.Temperature)
            {
                return false;
            }

            if (Headache != comparedConjunction.Headache)
            {
                return false;
            }

            if (Nausea != comparedConjunction.Nausea)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $" Temperature: {Temperature} | Headache: {Headache} | Nausea: {Nausea} ";
        }
    }
}
