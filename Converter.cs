using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FHIR_Formatter
{
    internal class Converter
    {
        internal static string ToXml(string input, string inForm)
        {
            Resource resource;
            if (inForm.Equals("json"))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                resource = FromJson(input);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            } else
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                resource = FromXml(input);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            SerializerSettings settings = new()
            {
                TrimWhiteSpacesInXml = true,
                AppendNewLine = true,
                Pretty = true
            };
            FhirXmlSerializer serializer = new(settings);
            return serializer.SerializeToString(resource);
        }

        internal static string ToJson(string input, string inForm)
        {
            Resource? resource;
            if (inForm.Equals("json"))
            {
                resource = FromJson(input);
            }
            else
            {
                resource = FromXml(input);
            }
            if (resource == null)
            {
                return String.Empty;
            }
            SerializerSettings settings = new()
            {
                AppendNewLine = true,
                Pretty = true
            };
            FhirJsonSerializer serializer = new(settings);
            return serializer.SerializeToString(resource);
        }

        private static Resource? FromJson(string input)
        {
            try
            {
                FhirJsonParser parser = new();
                return parser.Parse<Resource>(input);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Resource? FromXml(string input)
        {
            try
            {
                FhirXmlParser parser = new();
                return parser.Parse<Resource>(input);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
