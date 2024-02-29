using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaAQ11
{
    public class PartialStar
    {
        public Temperature? Temperature { get; set; }
        public bool? Headache { get; set; }
        public bool? Nausea { get; set; }

        public PartialStar(Temperature? temperature, bool? headache, bool? nausea)
        {
            Temperature = temperature;
            Headache = headache;
            Nausea = nausea;
        }

        public PartialStar(Case positive, Case negative)
        { 
            if (positive.Temperature != negative.Temperature)
            {
                Temperature = negative.Temperature.GetOpposite();
            }

            if (positive.Headache != negative.Headache)
            {
                Headache = !negative.Headache;
            }

            if (positive.Nausea != negative.Nausea)
            {
                Nausea = !negative.Nausea;
            }
        }

        public bool IsSame(PartialStar comparedStar)
        {
            if(Temperature != comparedStar.Temperature)
            {
                return false;
            }

            if(Headache != comparedStar.Headache)
            {
                return false;
            }

            if(Nausea != comparedStar.Nausea)
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
