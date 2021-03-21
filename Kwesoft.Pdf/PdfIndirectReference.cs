using Kwesoft.Pdf.Helpers;
using System;

namespace Kwesoft.Pdf
{
	public class PdfIndirectReference : PdfObject
	{
		public int ObjectNumber { get; internal set; }
		public int GenerationNumber { get; internal set; }


		public override bool Equals(object obj)
		{
			return obj is PdfIndirectReference other && other.ObjectNumber == ObjectNumber && other.GenerationNumber == GenerationNumber;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ObjectNumber, GenerationNumber);
		}

		public override string ToString()
		{
			return $"{ObjectNumber}{PdfKeywords.Space}{GenerationNumber}{PdfKeywords.Space}{PdfKeywords.Reference}";
		}

		public PdfObject Read()
		{
			if (Document == null) return null;
			return Document.ReadObject(this);
		}
	}
}
