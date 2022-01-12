using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABankAdmin.Core.Utils
{
    public class GeneratePassword
    {
        //default    length = 8,  only digit
        //e.g.
        //with lowercase    Generate(5,true)
        //with uppercase    Generate(5,false,true)
        //with special character    Generate(5,false,false,true)
        public static string Generate(int length = 8, Boolean useLowercase = false, Boolean useUppercase = false, Boolean useSpecialchar = false)
        {
            // Create a string of characters, numbers, special characters that allowed in the password
            string lower = "abcdefghijkmnopqrstuvwxyz";
            string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            string special = "@#%&$^";// special characters defined by Bank

            Random random = new Random();
            List<char> chars = new List<char>();
            string validChars = "123456789";
            //one number
            chars.Insert(random.Next(0, chars.Count),
                validChars[random.Next(0, validChars.Length)]);

            if (useLowercase)
            {
                validChars = validChars.Insert(validChars.Length, lower);
                chars.Insert(random.Next(0, chars.Count),
                    lower[random.Next(0, lower.Length)]);
            }
            if (useUppercase)
            {
                validChars = validChars.Insert(validChars.Length, upper);
                chars.Insert(random.Next(0, chars.Count),
                    upper[random.Next(0, upper.Length)]);
            }
            if (useSpecialchar)
            {
                validChars = validChars.Insert(validChars.Length, special);
                chars.Insert(random.Next(0, chars.Count),
                    special[random.Next(0, special.Length)]);
            }

            // Select one random character at a time from the string and create an array of chars  
            
            for (int i = chars.Count; i < length; i++)
            {
                chars.Insert(random.Next(0, chars.Count),
                    validChars[random.Next(0, validChars.Length)]);
            }
            return new string(chars.ToArray());
        }
    }
}
