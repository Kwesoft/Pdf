using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.UnitTests
{
	[TestFixture]
	class InMemoryPdfTests
	{
		private Mock<IPdfReader> _reader;
		private Mock<IPdfEditor> _editor;
		private byte[] _data;

		private PdfHeader _header;
		private PdfTrailer _trailer;

		private Mock<PdfIndirectReference> _rootRef;
		private Mock<PdfIndirectReference> _infoRef;
		private Mock<PdfDictionary> _root;
		private Mock<PdfDictionary> _info;

		private InMemoryPdf _inMemoryPdf;

		[SetUp]
		public void Setup()
		{
			_reader = new Mock<IPdfReader>();
			_editor = new Mock<IPdfEditor>();
			_data = new byte[] { 0, 1, 2, 3, 4, 5 };

			_rootRef = new Mock<PdfIndirectReference>();
			_infoRef = new Mock<PdfIndirectReference>();
			_root = new Mock<PdfDictionary>();
			_info = new Mock<PdfDictionary>();

			_header = new PdfHeader();
			_trailer = new PdfTrailer
			{
				TrailerDictionary = new Dictionary<PdfName, PdfObject> {
					{ "Root", _rootRef.Object },
					{ "Info", _infoRef.Object }
				}
			};

			_reader.Setup(x => x.ReadHeader()).Returns(_header);
			_reader.Setup(x => x.ReadTrailer()).Returns(_trailer);

			_reader.Setup(x => x.ReadObject(_rootRef.Object)).Returns(_root.Object);
			_reader.Setup(x => x.ReadObject(_infoRef.Object)).Returns(_info.Object);

			_inMemoryPdf = new InMemoryPdf(_data, _reader.Object, _editor.Object);
		}

		private void AssertArrayEqual<T>(T[] expected, T[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length);
			for (var i = 0; i < expected.Length; i++)
				Assert.AreEqual(expected[i], actual[i]);
		}

		[Test]
		public void ReadOneByte()
		{
			var result = (_inMemoryPdf as IEditablePdfDocument).Read(3);
			Assert.AreEqual(3, result);
		}

		[Test]
		public void ReadManyBytes()
		{
			var result = (_inMemoryPdf as IEditablePdfDocument).Read(2, 3);
			AssertArrayEqual(new byte[] { 2, 3, 4 }, result);
		}

		[Test]
		public void ReadAllBytes()
		{
			var result = _inMemoryPdf.GetBytes();
			AssertArrayEqual(new byte[] { 0, 1, 2, 3, 4, 5 }, result);
		}

		[Test]
		public void Replace()
		{
			_inMemoryPdf.Replace(2, 3, new byte[] { 11 });
			var result = _inMemoryPdf.GetBytes();
			AssertArrayEqual(new byte[] { 0, 1, 11, 5 }, result);
		}

		[Test]
		public void ReadObject()
		{
			var reference = new PdfIndirectReference();
			var expectedResult = new Mock<PdfObject>();
			_reader.Setup(x => x.ReadObject(reference)).Returns(expectedResult.Object);

			var result = (_inMemoryPdf as IEditablePdfDocument).ReadObject(reference);

			Assert.AreEqual(expectedResult.Object, result);
			_reader.Verify(x => x.ReadObject(reference), Times.Once);
		}

		[Test]
		public void AddToDictionary()
		{
			var dictionary = new PdfDictionary();
			var key = new PdfName();
			var value = new Mock<PdfObject>();

			var expectedResult = new Mock<PdfObject>();
			_editor.Setup(x => x.Add(dictionary, key, value.Object)).Verifiable();

			(_inMemoryPdf as IEditablePdfDocument).Add(dictionary, key, value.Object);

			_editor.Verify(x => x.Add(dictionary, key, value.Object), Times.Once);
		}

		[Test]
		public void RemoveFromDictionary()
		{
			var dictionary = new PdfDictionary();
			var key = new PdfName();
			_editor.Setup(x => x.Remove(dictionary, key)).Verifiable();

			(_inMemoryPdf as IEditablePdfDocument).Remove(dictionary, key);

			_editor.Verify(x => x.Remove(dictionary, key), Times.Once);
		}

		[Test]
		public void AddToArray()
		{
			var arr = new PdfArray();
			var value = new Mock<PdfObject>();
			_editor.Setup(x => x.Add(arr, value.Object)).Verifiable();

			(_inMemoryPdf as IEditablePdfDocument).Add(arr, value.Object);

			_editor.Verify(x => x.Add(arr, value.Object), Times.Once);
		}

		[Test]
		public void RemoveFromArrayByIndex()
		{
			var arr = new PdfArray();
			var index = 0;
			_editor.Setup(x => x.Remove(arr, index)).Verifiable();

			(_inMemoryPdf as IEditablePdfDocument).Remove(arr, index);

			_editor.Verify(x => x.Remove(arr, index), Times.Once);
		}

		[Test]
		public void RemoveFromArray()
		{
			var arr = new PdfArray();
			var value = new Mock<PdfObject>();
			_editor.Setup(x => x.Remove(arr, value.Object)).Verifiable();

			(_inMemoryPdf as IEditablePdfDocument).Remove(arr, value.Object);

			_editor.Verify(x => x.Remove(arr, value.Object), Times.Once);
		}

		[Test]
		public void Edit()
		{
			var value = new Mock<PdfObject>();
			var action = new Mock<Action>();
			_editor.Setup(x => x.Edit(value.Object, action.Object)).Verifiable();

			(_inMemoryPdf as IEditablePdfDocument).Edit(value.Object, action.Object);

			_editor.Verify(x => x.Edit(value.Object, action.Object), Times.Once);
		}

		[Test]
		public void Add()
		{
			var value = new Mock<PdfObject>();
			var expectedResult = new PdfIndirectReference();
			_editor.Setup(x => x.Add(value.Object)).Returns(expectedResult);

			var result = (_inMemoryPdf as IEditablePdfDocument).Add(value.Object);

			Assert.AreEqual(expectedResult, result);
			_editor.Verify(x => x.Add(value.Object), Times.Once);
		}


	}
}
