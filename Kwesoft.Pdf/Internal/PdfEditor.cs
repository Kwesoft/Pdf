using Kwesoft.Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf
{

	internal class PdfEditor : IPdfEditor
	{
		IEditablePdfDocument _document;

		internal PdfEditor(IEditablePdfDocument document)
		{
			_document = document;
		}

		PdfIndirectReference IPdfEditor.Add(PdfObject obj)
		{
			var maxObjectNumber = _document.CrossReferenceTable.ObjectOffsets.Any() 
				? _document.CrossReferenceTable.ObjectOffsets.Max(o => o.Key)
				: -1;
			var objectNumber = maxObjectNumber + 1;

			var prefix = _document.Encoding.GetBytes($"{objectNumber}{PdfKeywords.Space}0{PdfKeywords.Space}{PdfKeywords.Obj}{PdfKeywords.LineBreak}");

			var bytes = new List<byte[]> {
				prefix,
				obj.GetBytes(_document.Encoding),
				_document.Encoding.GetBytes($"{PdfKeywords.LineBreak}{PdfKeywords.EndObj}{PdfKeywords.LineBreak}")
			}.SelectMany(x => x).ToArray();

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
				Parent = null,
				Document = _document
			};
		}

		void IPdfEditor.Add(PdfDictionary dictionary, PdfName key, PdfObject value)
		{
			_Edit(dictionary, () => dictionary.Value.Add(key, value));
		}

		void IPdfEditor.Remove(PdfDictionary dictionary, PdfName key)
		{
			_Edit(dictionary, () =>
				dictionary.Value.Remove(key));
		}

		void IPdfEditor.Add(PdfArray array, PdfObject value)
		{
			_Edit(array, () => array.Value.Add(value));
		}

		void IPdfEditor.Remove(PdfArray array, PdfObject value)
		{
			_Edit(array, () => array.Value.Remove(value));
		}

		void IPdfEditor.Remove(PdfArray array, int index)
		{
			_Edit(array, () => array.Value.RemoveAt(index));
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

			_Edit(_document.CrossReferenceTable, () => {
				_document.CrossReferenceTable.ObjectCount = _document.CrossReferenceTable.ObjectOffsets.Count;
			}, false);

			_document.Trailer.Offset += adjustBy + (_document.CrossReferenceTable.Length - oldCrossReferenceTableLength);

			_Edit(_document.Trailer, () => {
				_document.Trailer.CrossReferenceTableOffset = _document.CrossReferenceTable.Index;
				_document.Trailer.TrailerDictionary.Value["Size"] = new PdfInteger { Value = _document.CrossReferenceTable.ObjectCount };
			}, false);
		}

		private void _AdjustLength(PdfObject obj, int adjustBy, bool adjustReferences)
		{
			_AdjustObjectLength(obj, adjustBy, adjustReferences);
			if(adjustReferences) _AdjustReferences(obj.Index, adjustBy);
		}

		private void _Edit<TPdfObject>(TPdfObject value, Action edit, bool adjustReferences = true) where TPdfObject : PdfObject
		{
			if(value.Parent != null)
			{
				_Edit(value.Parent, edit, adjustReferences);
				return;
			}
			var oldIndex = value.Index;
			var oldLength = value.Length;
			edit();
			var newRawValue = value.GetBytes(_document.Encoding);
			var newLength = newRawValue.Length;
			_document.Replace(oldIndex, oldLength, newRawValue);
			_AdjustLength(value, newLength - oldLength, adjustReferences);
		}

		void IPdfEditor.Edit<TPdfObject>(TPdfObject value, Action edit)
		{
			_Edit(value, edit);
		}
	}
}
