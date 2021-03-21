using Kwesoft.Pdf.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Kwesoft.Pdf
{
	public class PdfDictionary : PdfObject
	{
		internal Dictionary<PdfName, PdfObject> Value { get; }

		public PdfDictionary()
		{
			Value = new Dictionary<PdfName, PdfObject>();
		}

		public PdfDictionary(Dictionary<PdfName, PdfObject> value)
		{
			Value = value;
		}

		public override bool Equals(object obj)
		{
			return obj is PdfDictionary other && ((Value != null && Value.Equals(other.Value)) || (Value == null && other.Value == null));
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static implicit operator PdfDictionary(Dictionary<PdfName, PdfObject> value)
		{
			return new PdfDictionary(value);
		}

		public PdfObject this[PdfName key] => Value[key];
		public int Count => Value.Count;
		public bool ContainsKey(PdfName key) => Value.ContainsKey(key);

		private string _GetValues()
		{
			if (Value.Count == 0) return "";
			var values = string.Join(PdfKeywords.LineBreak, Value.Select(kvp => $"{kvp.Key}{PdfKeywords.Space}{kvp.Value}"));
			return $"{values}";
		}

		public override string ToString()
		{
			var values = _GetValues();
			return $"{PdfKeywords.DictionaryStart}{values}{PdfKeywords.DictionaryEnd}";
		}

		protected override void _DocumentAssigned()
		{
			if(Value != null)
				foreach(var kvp in Value)
				{
					if (kvp.Key != null)
						kvp.Key.Document = Document;
					if (kvp.Value != null)
						kvp.Value.Document = Document;
				}
		}

		public void Add(PdfName key, PdfObject value)
		{
			if(Document != null)
			{
				Document.Add(this, key, value);
			}
			else
			{
				Value.Add(key, value);
			}
		}

		public void Remove(PdfName key)
		{
			if (Document != null)
			{
				Document.Remove(this, key);
			}
			else
			{
				Value.Remove(key);
			}
		}
	}
}
