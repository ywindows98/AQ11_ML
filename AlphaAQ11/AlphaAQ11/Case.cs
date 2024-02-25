using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaAQ11
{
    public class Case
    {
        public int Id { get; set; }
        public Temperature Temperature { get; set; }
        public bool Headache { get; set; }
        public bool Nausea { get; set; }
        public bool DecisionFlu { get; set; }
        public Case(int id, Temperature temperature, bool headache, bool nausea)
        {
            Id = id;
            Temperature = temperature;
            Headache = headache;
            Nausea = nausea;
        }

        public Case(int id, Temperature temperature, bool headache, bool nausea, bool decisionFlu)
        {
            Id = id;
            Temperature = temperature;
            Headache = headache;
            Nausea = nausea;
            DecisionFlu = decisionFlu;
        }

        public override string ToString()
        {
            return $" Id: {Id} | Temperature: {Temperature} | Headache: {Headache} | Nausea: {Nausea} | Flu: {DecisionFlu} ";
        }
    }
}
