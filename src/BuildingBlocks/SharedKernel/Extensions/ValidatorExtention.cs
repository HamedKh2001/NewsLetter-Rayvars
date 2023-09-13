using PhoneNumbers;

namespace SharedKernel.Extensions
{
    public static class ValidatorExtention
    {
        public static bool ValidatePhoneNumber(this string phoneNumber)
        {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

            try
            {
                PhoneNumber parsedNumber = phoneNumberUtil.Parse(phoneNumber, null);

                if (phoneNumberUtil.IsValidNumber(parsedNumber))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
