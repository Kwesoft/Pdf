using Kwesoft.Pdf.Helpers;
using System;
using System.Text;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfStream : PdfObject
	{
		private PdfDictionary _properties;
		public PdfDictionary Properties 
		{ 
			get 
			{
				return _properties;
			}
			set 
			{
				_properties = value;
				if (_properties == null) return;
					
				_properties.Parent = this;
			} 
		}

		public byte[] Data { get; set; }

		private string _GetStreamStart() => _properties == null ? PdfKeywords.StreamStart : $"{PdfKeywords.Space}{PdfKeywords.StreamStart}";
		private string _GetPrefix() => $"{_properties}{_GetStreamStart()}{PdfKeywords.LineBreak}";
		private string _GetSuffix() => $"{PdfKeywords.LineBreak}{PdfKeywords.StreamEnd}{PdfKeywords.LineBreak}";
		public override string ToString()
		{
			return $"{_GetPrefix()}[data]{_GetSuffix()}";
		}

		protected override void _DocumentAssigned()
		{
			if (_properties != null)
				_properties.Document = Document;
		}

		internal override byte[] GetBytes(Encoding encoding)
		{
			var prefix = encoding.GetBytes(_GetPrefix());
			var suffix = encoding.GetBytes(_GetSuffix());

			var bytes = new byte[prefix.Length + Data.Length + suffix.Length];

			Buffer.BlockCopy(prefix, 0, bytes, 0, prefix.Length);
			Buffer.BlockCopy(Data, 0, bytes, prefix.Length, Data.Length);
			Buffer.BlockCopy(suffix, 0, bytes, prefix.Length + Data.Length, suffix.Length);

			return bytes;
		}
	}
}
