using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace monoxmlschemabug
{
    class Program
    {
		static Dictionary<string, string> typeValues = new Dictionary<string, string> {
			{"string", "abc"},

			{"normalizedString", "abc"},
			{"token", "abc"},
			{"language", "en"},
			{"Name", "abc"},
			{"NCName", "abc"},
			{"ID", "abc"},
			{"ENTITY", "abc"},
			{"NMTOKEN", "abc"},
			
			{"boolean", "true"},
			{"decimal", "1"},
			{"integer", "1"},
			{"nonPositiveInteger", "0"},
			{"negativeInteger", "-1"},
			{"long", "9223372036854775807"},
			{"int", "2147483647"},
			{"short", "32767"},
			{"byte", "127"},
			{"nonNegativeInteger", "0"},
			{"unsignedLong", "18446744073709551615"},
			{"unsignedInt", "4294967295"},
			{"unsignedShort", "65535"},
			{"unsignedByte", "255"},
			{"positiveInteger", "1"},
			{"float", "1.1"},
			{"double", "1.1"},
			{"time", "00:00:00"},
			{"date", "1999-12-31"},
			{"dateTime", "1999-12-31T00:00:00.000"},
			{"duration", "P1Y2M3DT10H30M"},
			{"gYearMonth", "1999-01"},
			{"gYear", "1999"},
			{"gMonthDay", "--12-31"},
			{"gMonth", "--12"},
			{"gDay", "---31"},
			
			{"base64Binary", "AbCd eFgH IjKl 019+"},
			{"hexBinary", "0123456789ABCDEF"},
			
			{"anyURI", "https://www.server.com"},
			{"QName", "xml:abc"},
		};
		
		const string schemaTemplate = @"
			<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' elementFormDefault='qualified'> 
			        <xs:element name='EL'>
			        	<xs:complexType>
			        		<xs:attribute name='attr' type='xs:{0}' use='required' />
			        	</xs:complexType>
			        </xs:element>
			</xs:schema>";
		
		const string documentTemplate = @"<EL attr='{0}' />";
		
        static void Main(string[] args)
        {
			Console.WriteLine(Environment.Version.ToString());
			var cnt = 0;
			foreach (var type in typeValues.Keys)
			{
				if (CheckDataType(type)) cnt++;
			}
			Console.WriteLine("{0} from {1} succeeded.", cnt, typeValues.Count);
        }
		
		static bool CheckDataType(string type) 
		{
			var schema = string.Format(schemaTemplate, type);
			var document = string.Format(documentTemplate, typeValues[type]);
			
			Console.Write("Check type '{0}' with '{1}' ... ", type, typeValues[type]);
	        
			var schemaSet = new XmlSchemaSet();
            using (var reader = new StringReader(schema))
            {
                schemaSet.Add(XmlSchema.Read(reader, ValidationEventHandler));
            }
			schemaSet.Compile();
			var doc = new XmlDocument();
			using (var reader = new StringReader(document))
			{
            	doc.Load(reader);
			}
			doc.Schemas = schemaSet;
			
			bool err = false;
			try 
			{
				doc.Validate((o, ea) => 
				{ 
					err = true; 
					Console.WriteLine("failed:\n\t{0} ({1})", ea.Message, ea.Severity); 
				});
			} 
			catch (XmlSchemaValidationException ex)
			{
				Console.WriteLine("failed with Exception:\n\t{0}", ex.Message);
				err = true;
			}
			if (!err) 
			{
				Console.WriteLine("success");
			}
			return !err;
		}

        static void ValidationEventHandler(object sender, ValidationEventArgs validationEventArgs)
        {
            Console.WriteLine(validationEventArgs.Severity + ": " + validationEventArgs.Message);
        }
    }
}
