using Groundforce.Services.Models;
using System.Collections.Generic;

namespace Groundforce.Common.Utilities.Helpers
{
    public static class Util
    {
        public static PageMetaData Paginate(int page, int per_page, int total)
        {
            int total_page = total % per_page == 0 ? total / per_page : total / per_page + 1;
            var result = new PageMetaData()
            {
                Page = page,
                PerPage = per_page,
                Total = total,
                TotalPages = total_page
            };
            return result;
        }


        public static Dictionary<string, byte[]> ConvertToHash(string verificationCode)
        {
            byte[] codeSalt, codeHash;

            // convert to a hash value and generate and salt
            using (var hash = new System.Security.Cryptography.HMACSHA512())
            {
                codeSalt = hash.Key;
                codeHash = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(verificationCode));
            }

            var obj = new Dictionary<string, byte[]>();
            obj.Add("codeSalt", codeSalt);
            obj.Add("codeHash", codeHash);


            return obj;

        }

        public static bool CompareWithHash(EmailVerification verificationObj, string newCode)
        {
            byte[] codeSalt = new byte[0]; byte[] codeHash = new byte[0];
            bool codeMatch = true;

            // compare the hash
            using (var hash = new System.Security.Cryptography.HMACSHA512(verificationObj.CodeSalt))
            {
                var hashGenerated = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newCode));
                for (int i = 0; i < hashGenerated.Length; i++)
                {
                    if (hashGenerated[i] != verificationObj.CodeHash[i])
                    {
                        codeMatch = false;
                        break;
                    }
                }
            }

            return codeMatch;

        }
    }
}
