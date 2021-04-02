using Kwesoft.Pdf.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf
{
	public class PdfArray : PdfObject, IEnumerable<PdfObject>
	{
		internal List<PdfObject> Value { get; }

		public PdfArray()
		{
			Value = new List<PdfObject>();
		}

		internal PdfArray(List<PdfObject> value)
		{
			Value = value;
		}

		public override bool Equals(object obj)
		{
			return obj is PdfArray other && ((Value != null && Value.Equals(other.Value)) || (Value == null && other.Value == null));
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static implicit operator PdfArray(List<PdfObject> value)
		{
			return new PdfArray(value);
		}

		public static implicit operator PdfArray(PdfObject[] value)
		{
			return new PdfArray(value.ToList());
		}

		public override string ToString()
		{
			var values = string.Join(PdfKeywords.Space, Value.Select(v => v.ToString()));
			return $"{PdfKeywords.ArrayStart}{values}{PdfKeywords.ArrayEnd}";
		}

		internal override byte[] GetBytes(Encoding encoding)
		{
			var result = new List<byte[]>(Value.Count * 2 + 1)
			{
				encoding.GetBytes(PdfKeywords.ArrayStart)
			};
			var first = true;
			foreach (var value in Value)
			{
				if (!first) result.Add(encoding.GetBytes(PdfKeywords.Space));
				result.Add(value.GetBytes(encoding));
				first = false;
			}
			result.Add(encoding.GetBytes(PdfKeywords.ArrayEnd));
			return result.SelectMany(x => x).ToArray();
		}

		protected override void _DocumentAssigned()
		{
			if(Value != null)
				foreach (var value in Value)
				{
					if (value != null)
						value.Document = Document;
				}
		}
		public void Add(PdfObject obj)
		{
			if(Document != null)
			{
				Document.Add(this, obj);
			}
			else
			{
				Value.Add(obj);
			}
		}
	

		public void Remove(int index)
		{
			if (Document != null)
			{
				Document.Remove(this, index);
			}
			else
			{
				Value.RemoveAt(index);
			}
		}
		public void Remove(PdfObject obj)
		{
			if (Document != null)
			{
				Document.Remove(this, obj);
			}
			else
			{
				Value.Remove(obj);
			}
		}

		public IEnumerator<PdfObject> GetEnumerator()
		{
			return Value.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
