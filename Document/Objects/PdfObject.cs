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
	}
}
