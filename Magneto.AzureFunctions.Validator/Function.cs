using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Http;

namespace Magneto.AzureFunctions.Validator
{
    public static class Function
    {
        [FunctionName("Validator")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                DnaDto data = JsonConvert.DeserializeObject<DnaDto>(requestbody);
                char[] dnac = Environment.GetEnvironmentVariable("Letters").ToArray();
                int secuenceMin = int.Parse(Environment.GetEnvironmentVariable("SecuenceMin"));
                int secuenceLetters = int.Parse(Environment.GetEnvironmentVariable("SecuenceLetters"));
                bool res = IsMutant(data, dnac, secuenceMin, secuenceLetters);
                return (res ? new HttpResponseMessage(System.Net.HttpStatusCode.OK) : new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden));

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
        }
        public static bool IsMutant(DnaDto data, char[] dnac, int secuenceMin, int secuenceLetters)
        {
            List<char> chars = data.dna.SelectMany(x => x).ToList();
            int counChat = data.dna[0].Count(); int countLetter = dnac.Count(); int secuence = 0;
            for (int r = 0; (r < countLetter && secuence < secuenceMin); r++)
            {
                char letter = dnac[r];
                int count = chars.Count(x => x == letter);
                for (int i = 0; (i < count && secuence < secuenceMin); i++)
                {
                    int position = chars.IndexOf(letter);
                    secuence += ValidateSecuenceRight(chars, letter, position, counChat, secuenceLetters);
                    secuence += ValidateSecuenceBelow(chars, letter, position, counChat, secuenceLetters);
                    secuence += ValidateSecuenceBelowRight(chars, letter, position, counChat, secuenceLetters);
                    secuence += ValidateSecuenceBelowLeft(chars, letter, position, counChat, secuenceLetters);
                    chars[position] = '1';
                }
            }
            return (secuence < secuenceMin ? false : true);
        }
        private static int ValidateSecuenceRight(List<char> list, char letter, int index, int countChar, int secuenceLetters)
        {
            var availablePositions = (list.Count() - (index + 1)) % countChar;
            if (availablePositions >= (secuenceLetters - 1))
            {
                if (list.Skip(index).Take(secuenceLetters).Count(x => x == letter) == secuenceLetters)
                    return 1;
            }
            return 0;
        }
        private static int ValidateSecuenceBelow(List<char> list, char letter, int index, int countChar, int secuenceLetters)
        {
            int rows = (list.Count() / countChar) - (secuenceLetters - 1);
            int positionMax = rows * countChar;
            if (index < positionMax)
            {
                int sum = 0;
                for (int i = 1; i < secuenceLetters; i++)
                {
                    if (list[index + (countChar * i)].Equals(letter))
                        sum++;
                    else
                        return 0;
                }
                if (sum == secuenceLetters - 1)
                    return 1;
            }
            return 0;
        }
        private static int ValidateSecuenceBelowRight(List<char> list, char letter, int index, int countChar, int secuenceLetters)
        {
            var availablePositions = (list.Count() - (index + 1)) % countChar;
            int rows = (list.Count() / countChar) - (secuenceLetters - 1);
            int positionMax = rows * countChar;
            if (availablePositions >= (secuenceLetters - 1) && index < positionMax)
            {
                int sum = 0;
                for (int i = 1; i < secuenceLetters; i++)
                {
                    if (list[index + (countChar * i) + i].Equals(letter))
                        sum++;
                    else
                        return 0;
                }
                if (sum == secuenceLetters - 1)
                    return 1;
                else
                    return 0;
            }
            return 0;
        }
        private static int ValidateSecuenceBelowLeft(List<char> list, char letter, int index, int countChar, int secuenceLetters)
        {
            var availablePositions = (list.Count() - (index + 1)) % countChar;
            int rows = (list.Count() / countChar) - (secuenceLetters - 1);
            int positionMax = rows * countChar;
            if (availablePositions <= (secuenceLetters - 1) && index < positionMax)
            {
                int sum = 0;
                for (int i = 1; i < secuenceLetters; i++)
                {
                    if (list[index + (countChar * i) - i].Equals(letter))
                        sum++;
                    else
                        return 0;
                }
                if (sum == secuenceLetters - 1)
                    return 1;
                else
                    return 0;
            }
            return 0;
        }
    }
}
