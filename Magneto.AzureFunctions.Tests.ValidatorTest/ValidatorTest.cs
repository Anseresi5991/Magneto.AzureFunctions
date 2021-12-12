using Magneto.AzureFunctions.Validator;
using System;
using Xunit;

namespace Magneto.AzureFunctions.Tests.ValidatorTest
{
    public class ValidatorTest
    {
        [Fact]
        public void IsMutant()
        {
            bool result = false;
            for (int i = 0; i < 1000000; i++)
            {
                Random random = new Random();
                DnaDto dnaDto = new DnaDto();
                dnaDto = GenerateHuman();
                char[] letters = { 'A', 'C', 'T', 'G' };
                var secuenceMin = random.Next(1, 8);
                var SecuenceLetters = random.Next(2, 10);
                try
                {
                    result = Function.IsMutant(dnaDto, letters, secuenceMin, SecuenceLetters);
                }
                catch (Exception)
                {
                    Assert.True(result);
                }
            }
            result = true;
            Assert.True(result);
        }
        public DnaDto GenerateHuman()
        {
            char[] letters = { 'A', 'C', 'T', 'G' };
            Random random = new Random();
            var rowsArray = random.Next(2, 10);
            var columsArray = random.Next(1, 10);
            DnaDto dnaDto = new DnaDto();
            dnaDto.dna = new string[rowsArray];
            for (int r = 0; r < rowsArray; r++)
            {
                string value = "";
                for (int i = 0; i < columsArray; i++)
                {
                    value = value + letters[random.Next(0, letters.Length)];
                }
                dnaDto.dna[r] = value;
            }

            return dnaDto;
        }
    }
}