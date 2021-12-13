using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magneto.AzureFunctions.Stat
{
    public class Response
    {
        public Response(int mutants, int humans, decimal ratio)
        {
            count_Human_dna = humans;
            count_mutant_dna = mutants;
            Ratio = ratio;
        }
        public int count_mutant_dna { get; set; }
        public int count_Human_dna { get; set; }
        public decimal Ratio { get; set; }
    }
}
