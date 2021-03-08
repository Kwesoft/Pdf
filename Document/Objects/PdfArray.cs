using System.Collections.Generic;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfArray : PdfObject
	{
		public List<PdfObject> Value { get; set; }

		public override string ToString()
		{
			throw new System.NotImplementedException();
		}
	}
}
