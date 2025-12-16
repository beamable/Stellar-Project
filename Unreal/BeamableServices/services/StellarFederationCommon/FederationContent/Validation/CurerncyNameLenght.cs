using Beamable.Common.Content.Validation;

namespace StellarFederationCommon.FederationContent.Validation
{
    /// <summary>
    /// Stellar currency asset must have name between 1 and 12
    /// </summary>
    public class CurrencyNameLength : ValidationAttribute
    {
        private const int StringLengthMin = 1;
        private const int StringLengthMax = 12;
        private const string ValueError = "The field string length must be >= {0} and <= {1}.";

        /// <summary>
        /// Stellar currency asset must have name between 1 and 12
        /// </summary>
        /// <param name="args"></param>
        public override void Validate(ContentValidationArgs args)
        {
            var validationField = args.ValidationField;
            var obj = validationField.GetValue();
            var content = args.Content;
            if (!(obj is string stringValue) ||
                !(stringValue.Length >= StringLengthMin && stringValue.Length <= StringLengthMax))
            {
                throw new ContentValidationException(content, validationField,
                    string.Format(ValueError, StringLengthMin, StringLengthMax));
            }
        }
    }
}