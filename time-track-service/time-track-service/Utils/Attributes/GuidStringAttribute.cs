using System;
using System.ComponentModel.DataAnnotations;


namespace time_track_service.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GuidStringAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return null;
            
            if (!Guid.TryParse(value.ToString(), out Guid result) && result != Guid.Empty)
            {
                return new ValidationResult("Guid cannot be empty.");
            }

            return null;
            
        }
    }
}