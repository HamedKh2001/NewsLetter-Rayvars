using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SharedKernel.Extensions
{
    public static class StringUtilityExtension
    {
        public static string ClearArabicCharacter(this string str)
        {
            str = str.Trim();
            if (string.IsNullOrEmpty(str))
                return null;

            str = str.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
            str = str.Replace("ك", "ک").Replace("دِ", "د").Replace("بِ", "ب").Replace("زِ", "ز").Replace("ذِ", "ذ").Replace("شِ", "ش").Replace("سِ", "س").Replace("ى", "ی").Replace("ي", "ی");

            return str;
        }

        public static bool IsValidIranianNationalCode(this string input)
        {
            if (!Regex.IsMatch(input, @"^\d{10}$"))
                return false;

            var check = Convert.ToInt32(input.Substring(9, 1));
            var sum = Enumerable.Range(0, 9)
                .Select(x => Convert.ToInt32(input.Substring(x, 1)) * (10 - x))
                .Sum() % 11;

            return sum < 2 ? check == sum : check + sum == 11;
        }

        public static bool IsValidPassportNo(this string input)
        {
            string motif = "^(?!^0+$)[a-zA-Z0-9]{3,20}$";
            if (input != null) return Regex.IsMatch(input, motif);
            else return false;
        }

        public static bool IsValidMobile(this string number)
        {
            string motif = @"^(\+[0-9]{12})$";
            if (number != null) return Regex.IsMatch(number, motif);
            else return false;
        }

        public static bool IsUserNameValid(this string userName)
        {
            char[] allowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ".ToCharArray();
            foreach (char item in userName.ToCharArray())
            {
                if (userName.ToCharArray().Any(c => allowedUserNameCharacters.Contains(item)) == false)
                    return false;
            }

            return true;
        }

        public static bool IsEmailValid(this string emailAddress)
        {
            try
            {
                var address = new MailAddress(emailAddress);
                return address.Address == emailAddress;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
