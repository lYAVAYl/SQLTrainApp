using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SQLTrainApp.Model.Logic
{
    /// <summary>
    /// Правила валидации логина
    /// </summary>
    public class LoginValidationRule : ValidationRule
    {
        public int MinCharacters { get; set; }
        public int MaxCharacters { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;

            if (string.IsNullOrEmpty(str))
                return new ValidationResult(false, $"Логин не введён");
            else if (str.Length < MinCharacters)
                return new ValidationResult(false, $"Минимальная длина логина - {MinCharacters} символов");
            else if (str.Length > MaxCharacters)
                return new ValidationResult(false, $"Максимальная длина логина - {MaxCharacters} символов");
            else
                return new ValidationResult(true, null);

        }


    }
}
