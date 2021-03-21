namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfKeywords
	{
		public static string Obj { get; } = "obj";
		public static string EndObj { get; } = "endobj";
		public static string EndObjLine { get; } = "\nendobj\n";
		public static string EOF { get; } = "%%EOF";
		public static string EOFLine { get; } = "\n%%EOF";
		public static string XRef { get; } = "xref";
		public static string StartXRef { get; } = "startxref";
		public static string StartXRefLine { get; } = "\nstartxref\n";
		public static string Trailer { get; } = "trailer";
		public static string TrailerLine { get; } = "\ntrailer\n";
		public static string True { get; } = "true";
		public static string False { get; } = "false";
		public static string Null { get; } = "null";
		public static string DictionaryStart { get; } = "<<";
		public static string DictionaryEnd { get; } = ">>";
		public static string ArrayStart { get; } = "[";
		public static string ArrayEnd { get; } = "]";
		public static string NameStart { get; } = "/";
		public static string StringStart { get; } = "(";
		public static string StringEnd { get; } = ")";
		public static string BackSlash { get; } = "\\";
		public static string HexStringStart { get; } = "<";
		public static string HexStringEnd { get; } = ">";
		public static string LineBreak { get; } = "\n";
		public static string Space { get; } = " ";
		public static string Decimal { get; } = ".";
		public static string Reference { get; } = "R";
		public static string Numbers { get; } = "0123456789";
		public static string StreamStart { get; } = "stream";
		public static string StreamStartLine { get; } = "stream\n";
		public static string StreamEnd { get; } = "endstream";
		public static string StreamEndLines { get; } = "\nendstream\n";
		public static string BigEndianUnicodeMarker { get; } = "FFFE";
	}
}
