using Kwesoft.Pdf.UnitTests.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf.UnitTests.Internal
{
	[TestFixture]
	class PdfEditorTests
	{
		private Mock<IEditablePdfDocument> _document;
		private IPdfEditor _pdfEditor;
		private PdfCrossReferenceTable _crossReferenceTable;
		private PdfTrailer _trailer;


		[SetUp]
		public void Setup()
		{
			_document = new Mock<IEditablePdfDocument>();
			_trailer = new PdfTrailer {
				CrossReferenceTableOffset = 10,
				TrailerDictionary = new PdfDictionary()
			};
			_crossReferenceTable = new PdfCrossReferenceTable
			{
				ObjectOffsets = new Dictionary<int, int>(),
				Offset = 10
			};

			_document.SetupGet(x => x.CrossReferenceTable).Returns(_crossReferenceTable);
			_document.SetupGet(x => x.Encoding).Returns(Encoding.UTF8);
			_document.SetupGet(x => x.Trailer).Returns(_trailer);

			_pdfEditor = new PdfEditor(_document.Object);
		}

		[Test]
		public void AddPdfObject()
		{
			var obj = new Mock<PdfObject>();
			var data = "123";
			var bytes = Encoding.UTF8.GetBytes(data);

			var expected = Encoding.UTF8.GetBytes($"0 0 obj\n{data}\nendobj\n");

			obj.Setup(x => x.GetBytes(Encoding.UTF8)).Returns(bytes);

			_pdfEditor.Add(obj.Object);
			_document.Verify(x => x.Replace(10, 0, It.Is<byte[]>(b => b.Length == expected.Length && Enumerable.Range(0, b.Length).All(i => b[i] == expected[i]))));
			
			Assert.AreEqual(10 + expected.Length, _crossReferenceTable.Offset);
			Assert.AreEqual(1, ((PdfInteger)_trailer.TrailerDictionary["Size"]).Value);
		}

		[Test]
		public void AddToDictionary()
		{
			var obj = new Mock<PdfObject>();
			var dict = new PdfDictionary {
				Length = 3,
				Offset = 5
			};
			
			var data = "123"; 
			var bytes = Encoding.UTF8.GetBytes(data);
			var expected = Encoding.UTF8.GetBytes($"<</a 123>>");
			obj.Setup(x => x.GetBytes(Encoding.UTF8)).Returns(bytes);

			_pdfEditor.Add(dict, "a", obj.Object);
			_document.Verify(x => x.Replace(5, 3, It.Is<byte[]>(b => b.Length == expected.Length && Enumerable.Range(0, b.Length).All(i => b[i] == expected[i]))));
		}

		[Test]
		public void RemoveFromDictionary()
		{
			var obj = new Mock<PdfObject>();
			var dict = new PdfDictionary
			{
				Length = 3,
				Offset = 5
			};
			dict.Add("a", obj.Object);

			var data = "123";
			var bytes = Encoding.UTF8.GetBytes(data);
			var expected = Encoding.UTF8.GetBytes($"<<>>");
			obj.Setup(x => x.GetBytes(Encoding.UTF8)).Returns(bytes);

			_pdfEditor.Remove(dict, "a");
			_document.Verify(x => x.Replace(5, 3, It.Is<byte[]>(b => b.Length == expected.Length && Enumerable.Range(0, b.Length).All(i => b[i] == expected[i]))));
		}

		[Test]
		public void AddToArray()
		{
			var obj = new Mock<PdfObject>();
			var arr = new PdfArray
			{
				Length = 3,
				Offset = 5
			};

			var data = "123";
			var bytes = Encoding.UTF8.GetBytes(data);
			var expected = Encoding.UTF8.GetBytes($"[123]");
			obj.Setup(x => x.GetBytes(Encoding.UTF8)).Returns(bytes);

			_pdfEditor.Add(arr, obj.Object);
			_document.Verify(x => x.Replace(5, 3, It.Is<byte[]>(b => b.Length == expected.Length && Enumerable.Range(0, b.Length).All(i => b[i] == expected[i]))));
		}

		[Test]
		public void RemoveFromArray()
		{
			var obj = new Mock<PdfObject>();
			var arr = new PdfArray
			{
				Length = 3,
				Offset = 5
			};
			arr.Add(obj.Object);

			var data = "123";
			var bytes = Encoding.UTF8.GetBytes(data);
			var expected = Encoding.UTF8.GetBytes($"[]");
			obj.Setup(x => x.GetBytes(Encoding.UTF8)).Returns(bytes);

			_pdfEditor.Remove(arr, obj.Object);
			_document.Verify(x => x.Replace(5, 3, It.Is<byte[]>(b => b.Length == expected.Length && Enumerable.Range(0, b.Length).All(i => b[i] == expected[i]))));
		}

		[Test]
		public void RemoveFromArrayByIndex()
		{
			var obj = new Mock<PdfObject>();
			var arr = new PdfArray
			{
				Length = 3,
				Offset = 5
			};
			arr.Add(obj.Object);

			var data = "123";
			var bytes = Encoding.UTF8.GetBytes(data);
			var expected = Encoding.UTF8.GetBytes($"[]");
			obj.Setup(x => x.GetBytes(Encoding.UTF8)).Returns(bytes);

			_pdfEditor.Remove(arr, 0);
			_document.Verify(x => x.Replace(5, 3, It.Is<byte[]>(b => b.Length == expected.Length && Enumerable.Range(0, b.Length).All(i => b[i] == expected[i]))));
		}
	}
}
