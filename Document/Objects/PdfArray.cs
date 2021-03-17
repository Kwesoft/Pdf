using Kwesoft.Pdf.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kwesoft.Pdf.Document.Objects
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
