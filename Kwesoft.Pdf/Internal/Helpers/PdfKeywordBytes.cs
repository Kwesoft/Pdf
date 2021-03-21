using System.Text;

namespace Kwesoft.Pdf.Helpers
{

	internal class PdfKeywordBytes
	{
		public byte[] Obj { get; }
		public byte[] EndObj { get; }
		public byte[] EndObjLine { get; }
		public byte[] EOF { get; }
		public byte[] EOFLine { get; }
		public byte[] XRef { get; }
		public byte[] StartXRef { get; }
		public byte[] StartXRefLine { get; }
		public byte[] Trailer { get; }
		public byte[] TrailerLine { get; }
		public byte[] True { get; }
		public byte[] False { get; }
		public byte[] Null { get; }
		public byte[] DictionaryStart { get; }
		public byte[] DictionaryEnd { get; }
		public byte[] ArrayStart { get; }
		public byte[] ArrayEnd { get; }
		public byte[] NameStart { get; }
		public byte[] StringStart { get; }
		public byte[] StringEnd { get; }
		public byte[] BackSlash { get; }
		public byte[] HexStringStart { get; }
		public byte[] HexStringEnd { get; }
		public byte[] LineBreak { get; }
		public byte[] Space { get; }
		public byte[] Decimal { get; }
		public byte[] Reference { get; }
		public byte[] Numbers { get; }
		public byte[] StreamStart { get; }
		public byte[] StreamStartLine { get; }
		public byte[] StreamEnd { get; }
		public byte[] StreamEndLines { get; }
		public byte[] BigEndianUnicodeMarker { get; }

		public PdfKeywordBytes(Encoding encoding)
		{
			Obj = encoding.GetBytes(PdfKeywords.Obj);
			EndObj = encoding.GetBytes(PdfKeywords.EndObj);
			EndObjLine = encoding.GetBytes(PdfKeywords.EndObjLine);
			EOF = encoding.GetBytes(PdfKeywords.EOF);
			EOFLine = encoding.GetBytes(PdfKeywords.EOFLine);
			XRef = encoding.GetBytes(PdfKeywords.XRef);
			StartXRef = encoding.GetBytes(PdfKeywords.StartXRef);
			StartXRefLine = encoding.GetBytes(PdfKeywords.StartXRefLine);
			Trailer = encoding.GetBytes(PdfKeywords.Trailer);
			TrailerLine = encoding.GetBytes(PdfKeywords.TrailerLine);
			True = encoding.GetBytes(PdfKeywords.True);
			False = encoding.GetBytes(PdfKeywords.False);
			Null = encoding.GetBytes(PdfKeywords.Null);
			DictionaryStart = encoding.GetBytes(PdfKeywords.DictionaryStart);
			DictionaryEnd = encoding.GetBytes(PdfKeywords.DictionaryEnd);
			ArrayStart = encoding.GetBytes(PdfKeywords.ArrayStart);
			ArrayEnd = encoding.GetBytes(PdfKeywords.ArrayEnd);
			NameStart = encoding.GetBytes(PdfKeywords.NameStart);
			StringStart = encoding.GetBytes(PdfKeywords.StringStart);
			StringEnd = encoding.GetBytes(PdfKeywords.StringEnd);
			HexStringStart = encoding.GetBytes(PdfKeywords.HexStringStart);
			HexStringEnd = encoding.GetBytes(PdfKeywords.HexStringEnd);
			LineBreak = encoding.GetBytes(PdfKeywords.LineBreak);
			Space = encoding.GetBytes(PdfKeywords.Space);
			Decimal = encoding.GetBytes(PdfKeywords.Decimal);
			Reference = encoding.GetBytes(PdfKeywords.Reference);
			Numbers = encoding.GetBytes(PdfKeywords.Numbers);
			StreamStart = encoding.GetBytes(PdfKeywords.StreamStart);
			StreamStartLine = encoding.GetBytes(PdfKeywords.StreamStartLine);
			StreamEnd = encoding.GetBytes(PdfKeywords.StreamEnd);
			StreamEndLines = encoding.GetBytes(PdfKeywords.StreamEndLines);
			BackSlash = encoding.GetBytes(PdfKeywords.BackSlash);
			BigEndianUnicodeMarker = encoding.GetBytes(PdfKeywords.BigEndianUnicodeMarker);
		}
	}
}
