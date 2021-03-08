using Kwesoft.Pdf.Document;
using Kwesoft.Pdf.Document.Objects;
using Kwesoft.Pdf.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf
{

	public class PdfEditor : IPdfEditor
	{
		IEditablePdfDocument _document;

		internal PdfEditor(IEditablePdfDocument document)
		{
			_document = document;
		}

		public void Add(PdfDictionary dictionary, PdfName key, PdfObject value)
		{
			_Edit(dictionary, obj => {
				obj.Value.Add(key, value);
			});
		}
		public PdfIndirectReference Add(PdfObject obj)
		{
			var maxObjectNumber = _document.CrossReferenceTable.ObjectOffsets.Max(o => o.Key);
			var maxIndirectReference = _document.CrossReferenceTable.ObjectOffsets[maxObjectNumber];
			var objectNumber = maxObjectNumber + 1;

			var prefix = _document.Encoding.GetBytes($"{objectNumber}{PdfKeywords.Space}0{PdfKeywords.Space}{PdfKeywords.Obj}{PdfKeywords.LineBreak}");
			var data = obj.GetBytes(_document.Encoding);
			var suffix = _document.Encoding.GetBytes($"{PdfKeywords.EndObj}{PdfKeywords.LineBreak}");
			var bytes = new byte[prefix.Length + data.Length + suffix.Length];

			Buffer.BlockCopy(prefix, 0, bytes, 0, prefix.Length);
			Buffer.BlockCopy(data, 0, bytes, prefix.Length, data.Length);
			Buffer.BlockCopy(suffix, 0, bytes, prefix.Length + data.Length, suffix.Length);

			var index = _document.CrossReferenceTable.Index;
			_document.CrossReferenceTable.ObjectOffsets.Add(objectNumber, index);

			obj.Offset = index + prefix.Length;
			obj.Length = bytes.Length;

			_document.Replace(index, 0, bytes);
			_AdjustReferences(index, bytes.Length);

			return new PdfIndirectReference
			{
				GenerationNumber = 0,
				ObjectNumber = objectNumber,
				Offset = index,
				Length = bytes.Length,
				Parent = null
			};
		}

		public void Add(PdfArray array, PdfString value)
		{
			throw new NotImplementedException();
		}

		private void _AdjustObjectLength(PdfObject obj, int adjustBy, bool adjustReferences)
		{
			obj.Length += adjustBy;
			if (obj.Parent != null)
				_AdjustLength(obj.Parent, adjustBy, adjustReferences);
		}

		private void _AdjustReferences(int after, int adjustBy)
		{
			var keys = new int[_document.CrossReferenceTable.ObjectOffsets.Count];
			_document.CrossReferenceTable.ObjectOffsets.Keys.CopyTo(keys, 0);

			foreach (var key in keys)
			{
				var offset = _document.CrossReferenceTable.ObjectOffsets[key];
				if (offset > after)
				{
					_document.CrossReferenceTable.ObjectOffsets[key] = offset + adjustBy;
				}
			}

			var oldCrossReferenceTableLength = _document.CrossReferenceTable.Length;
			_document.CrossReferenceTable.Offset += adjustBy;

			_Edit(_document.CrossReferenceTable, obj => {
				obj.ObjectCount = obj.ObjectOffsets.Count;
			}, false);

			_document.Trailer.Offset += adjustBy + (_document.CrossReferenceTable.Length - oldCrossReferenceTableLength);

			_Edit(_document.Trailer, obj => {
				obj.CrossReferenceTableOffset = _document.CrossReferenceTable.Index;
				obj.TrailerDictionary.Value["Size"] = new PdfInteger { Value = _document.CrossReferenceTable.ObjectCount };
			}, false);
		}

		private void _AdjustLength(PdfObject obj, int adjustBy, bool adjustReferences)
		{
			_AdjustObjectLength(obj, adjustBy, adjustReferences);
			if(adjustReferences) _AdjustReferences(obj.Index, adjustBy);
		}

		private void _Edit<TPdfObject>(TPdfObject value, Action<TPdfObject> edit, bool adjustReferences = true) where TPdfObject : PdfObject
		{
			if(value.Parent != null)
			{
				_Edit(value.Parent, _ => edit(value), adjustReferences);
				return;
			}
			var oldIndex = value.Index;
			var oldLength = value.Length;
			edit(value);
			var newRawValue = _document.Encoding.GetBytes(value.ToString());
			var newLength = newRawValue.Length;
			_document.Replace(oldIndex, oldLength, newRawValue);
			_AdjustLength(value, newLength - oldLength, adjustReferences);
		}

		public void Edit<TPdfObject>(TPdfObject value, Action<TPdfObject> edit) where TPdfObject : PdfObject
		{
			_Edit(value, edit);
		}

		public void Save(string filename)
		{
			_document.Save(filename);
		}
	}
}
