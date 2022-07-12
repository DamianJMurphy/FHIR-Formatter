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
                resource = FromJson(input);
            } else
            {
                resource = FromXml(input);
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
            Resource resource;
            if (inForm.Equals("json"))
            {
                resource = FromJson(input);
            }
            else
            {
                resource = FromXml(input);
            }
            SerializerSettings settings = new()
            {
                AppendNewLine = true,
                Pretty = true
            };
            FhirJsonSerializer serializer = new(settings);
            return serializer.SerializeToString(resource);
        }

        private static Resource FromJson(string input)
        {
            FhirJsonParser parser = new();
            return parser.Parse<Resource>(input);
        }

        private static Resource FromXml(string input)
        {
            FhirXmlParser parser = new();
            return parser.Parse<Resource>(input);
        }
    }
}
