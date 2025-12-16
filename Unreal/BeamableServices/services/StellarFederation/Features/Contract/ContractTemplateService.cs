using System;
using HandlebarsDotNet;
using StellarFederationCommon.Extensions;

namespace Beamable.StellarFederation.Features.Contract;

public static class ContractTemplateService
{
    private static bool _initialized;
    public static void Initialize()
    {
        if (_initialized) return;

        Handlebars.RegisterHelper("toUpperCase", (writer, _, parameters) =>
            WriteTransformed(writer, parameters, s => FederationContentExtensions.SanitizeModuleName(s).ToUpperInvariant()));

        Handlebars.RegisterHelper("toLowerCase", (writer, _, parameters) =>
            WriteTransformed(writer, parameters, s => FederationContentExtensions.SanitizeModuleName(s).ToLowerInvariant()));

        Handlebars.RegisterHelper("toStructName", (writer, _, parameters) =>
            WriteTransformed(writer, parameters, s =>
            {
                var cleaned = FederationContentExtensions.SanitizeModuleName(s);
                return string.IsNullOrEmpty(cleaned) ? cleaned
                    : char.ToUpperInvariant(cleaned[0]) + cleaned[1..];
            }));

        Handlebars.RegisterHelper("if_not_zero", (writer, options, context, args) =>
        {
            var left = Convert.ToInt64(args[0]);
            if (left > 0)
                options.Template(writer, context);
            else
                options.Inverse(writer, context);
        });

        Handlebars.RegisterHelper("if_zero", (writer, options, context, args) =>
        {
            var value = Convert.ToInt64(args[0]);
            if (value == 0)
                options.Template(writer, context);
            else
                options.Inverse(writer, context);
        });

        _initialized = true;
    }

    private static void WriteTransformed(EncodedTextWriter writer, Arguments parameters, Func<string, string> transform)
    {
        if (parameters.Length > 0 && parameters[0] is string input)
        {
            writer.Write(transform(input));
        }
    }
}