using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf.Document.Objects
{
	public abstract class PdfObject<T> : PdfObject
	{
		private T _value { get; set; }
		
		public T Value 
		{ 
			get
			{
				return _value;
			}
			set 
			{
				if (this.Document != null)
				{
					this.Document.Edit(this, () => _value = value);
				}
				else
				{
					_value = value;
				}
			}
		}
	}

	public abstract class PdfObject
	{
		internal int Offset { get; set; }
		internal int Index => (Parent?.Index ?? 0) + Offset;
		internal int Length { get; set; }
		private IEditablePdfDocument _document { get; set; }
		internal IEditablePdfDocument Document
		{ 
			get 
			{
				return _document;
			} 
			set 
			{
				_document = value;
				_DocumentAssigned();
			} 
		}
		internal PdfObject Parent { get; set; }

		protected virtual void _DocumentAssigned()
		{
		}

		internal virtual byte[] GetBytes(Encoding encoding) {
			return encoding.GetBytes(ToString());
		}

		public static implicit operator PdfObject(bool value)
		{
			return new PdfBool { Value = value };
		}

		public static implicit operator PdfObject(byte value)
		{
			return new PdfInteger { Value = value };
		}

		public static implicit operator PdfObject(int value)
		{
			return new PdfInteger { Value = value };
		}

		public static implicit operator PdfObject(long value)
		{
			return new PdfInteger { Value = value };
		}

		public static implicit operator PdfObject(float value)
		{
			return new PdfDouble { Value = value };
		}

		public static implicit operator PdfObject(double value)
		{
			return new PdfDouble { Value = value };
		}

		public static implicit operator PdfObject(decimal value)
		{
			return new PdfDouble { Value = (double)value };
		}

		public static implicit operator PdfObject(string value)
		{
			return new PdfString { Value = value };
		}

		public static implicit operator PdfObject(PdfObject[] value)
		{
			return new PdfArray(value.ToList());
		}

		public static implicit operator PdfObject(List<PdfObject> value)
		{
			return new PdfArray(value);
		}

		public static implicit operator PdfObject(Dictionary<PdfName, PdfObject> value)
		{
			return new PdfDictionary(value);
		}
	}
}
