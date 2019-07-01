namespace Willow.API.Events.ControllerData
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EventData : IValidatableObject
    {
        public string  Title { get; set; }
        public string Description { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(Title)) yield return new ValidationResult(
                $"The {Title} is required", 
                new[] {nameof(Title)}
                );
        }
    }
}