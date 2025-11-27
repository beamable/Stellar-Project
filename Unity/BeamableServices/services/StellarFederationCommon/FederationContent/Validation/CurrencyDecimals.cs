using Beamable.Common.Content.Validation;

namespace StellarFederationCommon.FederationContent.Validation
{
    /// <summary>
    /// Stellar currency asset must have name between 1 and 12
    /// </summary>
    public class CurrencyDecimals : ValidationAttribute
    {
        private const int LengthMin = 0;
        private const int LengthMax = 7;
        private const string ValueError = "The field value must be >= {0} and <= {1}.";

        /// <summary>
        /// Stellar currency asset must have name between 1 and 12
        /// </summary>
        /// <param name="args"></param>
        public override void Validate(ContentValidationArgs args)
        {
            var validationField = args.ValidationField;
            var obj = validationField.GetValue();
            var content = args.Content;
            if (!(obj is int value) ||
                !(value >= LengthMin && value <= LengthMax))
            {
                throw new ContentValidationException(content, validationField,
                    string.Format(ValueError, LengthMin, LengthMax));
            }
        }
    }
}