using Kwesoft.Pdf.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Kwesoft.Pdf
{
	internal class PdfCrossReferenceTable : PdfObject
	{
		public Dictionary<int, int> ObjectOffsets { get; set; }
		public int ObjectCount { get; set; }
		public int FirstObjectNumber { get; set; }

		public override string ToString()
		{
			var entries = string.Join(PdfKeywords.LineBreak, ObjectOffsets.Select(kvp => {
				var offset = kvp.Value.ToString().PadLeft(10, '0');
				var generationNumber = kvp.Value == 0 ? "65535" : "00000";
				var flag = kvp.Value == 0 ? "f" : "n";
				return $"{offset} {generationNumber} {flag}";
			}));

			return $"{PdfKeywords.XRef}{PdfKeywords.LineBreak}{FirstObjectNumber}{PdfKeywords.Space}{ObjectCount}{PdfKeywords.LineBreak}{entries}{PdfKeywords.LineBreak}";
		}
	}
}
