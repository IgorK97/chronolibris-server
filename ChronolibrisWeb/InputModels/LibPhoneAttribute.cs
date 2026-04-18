//using System.ComponentModel.DataAnnotations;
//using PhoneNumbers;

//namespace ChronolibrisWeb.InputModels
//{
//    public class LibPhoneAttribute : ValidationAttribute
//    {
//        private readonly string _defaultRegion;

//        public LibPhoneAttribute(string defaultRegion = "RU")
//        {
//            _defaultRegion = defaultRegion;
//        }

//        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
//        {
//            if (value is not string phoneNumber || string.IsNullOrWhiteSpace(phoneNumber))
//            {
//                return ValidationResult.Success;
//            }

//            try
//            {
//                var util = PhoneNumberUtil.GetInstance();
//                var number = util.Parse(phoneNumber, _defaultRegion);

//                if (util.IsValidNumber(number))
//                {
//                    return ValidationResult.Success;
//                }
//            }
//            catch (NumberParseException)
//            {
//            }

//            return new ValidationResult(ErrorMessage ?? "Некорректный формат номера телефона");
//        }
//    }
//}
