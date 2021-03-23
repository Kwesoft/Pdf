using Kwesoft.Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf
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
			var result = new List<byte[]> {
				encoding.GetBytes(_GetPrefix()),
				Data,
				encoding.GetBytes(_GetSuffix())
			};

			return result.SelectMany(x => x).ToArray();
		}


		public override bool Equals(object obj)
		{
			return obj is PdfStream other && ((_properties != null && _properties.Equals(other._properties)) || (_properties == null && other._properties == null)) && ((Data != null && Data.Equals(other.Data)) || (Data == null && other.Data == null));
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Data, _properties);
		}
	}
}
