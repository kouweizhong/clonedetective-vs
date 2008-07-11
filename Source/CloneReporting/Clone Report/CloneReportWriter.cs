using System;
using System.Text;
using System.Xml;

namespace CloneDetective.CloneReporting
{
	/// <summary>
	/// This class encapsulates the logic of writing a ConQAT clone report to XML.
	/// </summary>
	internal static class CloneReportWriter
	{
		/// <summary>
		/// Writes a clone report to the given XML file.
		/// </summary>
		/// <param name="fileName">The fully qualified path to the ConQAT clone report file.</param>
		/// <param name="cloneReport">The clone report to be written.</param>
		public static void Write(string fileName, CloneReport cloneReport)
		{
			XmlDocument doc = new XmlDocument();

			Write(doc, cloneReport);

			// We want to write the clone report in the same encoding as ConQAT would.
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.GetEncoding("ISO-8859-1")))
			{
				// Since we want to be able to read the result we will
				// use formatted output.
				writer.Formatting = Formatting.Indented;
				doc.Save(writer);
			}
		}

		/// <summary>
		/// Writes a clone report to the given XML document.
		/// </summary>
		/// <param name="doc">The XML document to write to.</param>
		/// <param name="cloneReport">The clone report to be written.</param>
		private static void Write(XmlDocument doc, CloneReport cloneReport)
		{
			// Create root node.
			XmlNode reportNode = doc.AppendChild(doc.CreateElement("cloneReport", Resources.CloneReportSchemaNamespace));

			// Write all source files.
			foreach (SourceFile sourceFile in cloneReport.SourceFiles)
			{
				// Create source file node and write attributes.
				XmlNode sourceFileNode = reportNode.AppendChild(doc.CreateElement("sourceFile", Resources.CloneReportSchemaNamespace));
				sourceFileNode.Attributes.Append(doc.CreateAttribute("id")).Value = XmlConvert.ToString(sourceFile.Id);
				sourceFileNode.Attributes.Append(doc.CreateAttribute("path")).Value = sourceFile.Path;
				sourceFileNode.Attributes.Append(doc.CreateAttribute("length")).Value = XmlConvert.ToString(sourceFile.Length);
				sourceFileNode.Attributes.Append(doc.CreateAttribute("fingerprint")).Value = sourceFile.Fingerprint;
			}

			// Write all clone classes.
			foreach (CloneClass cloneClass in cloneReport.CloneClasses)
			{
				// Create clone class node and write attributes.
				XmlNode cloneClassNode = reportNode.AppendChild(doc.CreateElement("cloneClass", Resources.CloneReportSchemaNamespace));
				cloneClassNode.Attributes.Append(doc.CreateAttribute("id")).Value = XmlConvert.ToString(cloneClass.Id);
				cloneClassNode.Attributes.Append(doc.CreateAttribute("normalizedLength")).Value = XmlConvert.ToString(cloneClass.NormalizedLength);
				cloneClassNode.Attributes.Append(doc.CreateAttribute("fingerprint")).Value = cloneClass.Fingerprint;

				// Write all key-value pairs of the current clone class.
				XmlNode valuesNode = cloneClassNode.AppendChild(doc.CreateElement("values", Resources.CloneReportSchemaNamespace));
				foreach (CloneClassValue value in cloneClass.Values)
				{
					XmlNode valueNode = valuesNode.AppendChild(doc.CreateElement("value", Resources.CloneReportSchemaNamespace));
					valueNode.Attributes.Append(doc.CreateAttribute("key")).Value = value.Key;
					valueNode.Attributes.Append(doc.CreateAttribute("value")).Value = value.Value;
					valueNode.Attributes.Append(doc.CreateAttribute("type")).Value = value.Type;
				}

				// Write all clones of the current clone class.
				foreach (Clone clone in cloneClass.Clones)
				{
					XmlNode cloneNode = cloneClassNode.AppendChild(doc.CreateElement("clone", Resources.CloneReportSchemaNamespace));
					cloneNode.Attributes.Append(doc.CreateAttribute("sourceFileId")).Value = XmlConvert.ToString(clone.SourceFile.Id);
					cloneNode.Attributes.Append(doc.CreateAttribute("startLine")).Value = XmlConvert.ToString(clone.StartLine);
					cloneNode.Attributes.Append(doc.CreateAttribute("lineCount")).Value = XmlConvert.ToString(clone.LineCount);
					cloneNode.Attributes.Append(doc.CreateAttribute("startUnitIndexInFile")).Value = XmlConvert.ToString(clone.LineCount);
					cloneNode.Attributes.Append(doc.CreateAttribute("lengthInUnits")).Value = XmlConvert.ToString(clone.LineCount);
					cloneNode.Attributes.Append(doc.CreateAttribute("gaps")).Value = clone.Gaps;
					cloneNode.Attributes.Append(doc.CreateAttribute("fingerprint")).Value = clone.Fingerprint;
				}
			}
		}
	}
}
