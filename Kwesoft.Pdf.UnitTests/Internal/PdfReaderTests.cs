using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Kwesoft.Pdf.Helpers;
using System.Linq;

namespace Kwesoft.Pdf.UnitTests.Internal
{
	[TestFixture]
	class PdfReaderTests
	{
		PdfReader _reader;
		Mock<IEditablePdfDocument> _document;
		Encoding _encoding;
		
		[SetUp]
		public void Setup()
		{
			_encoding = Encoding.UTF8;
			_document = new Mock<IEditablePdfDocument>();
			_document.SetupGet(x => x.Encoding).Returns(_encoding);
			_document.SetupGet(x => x.Keywords).Returns(new PdfKeywordBytes(Encoding.UTF8));

			_reader = new PdfReader(_document.Object);
		}

		private void _SetupData(byte[] data)
		{
			_document.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((index, length) => data.Skip(index).Take(length).ToArray());
			_document.Setup(x => x.Read(It.IsAny<int>())).Returns<int>((index) => data[index]);
			_document.SetupGet(x => x.Length).Returns(data.Length);
		}

		[TestCase("%PDF-1.4\n*", "1.4")]
		public void ReadVersion(string input, string expected)
		{
			var data = _encoding.GetBytes(input);
			_SetupData(data);

			var result = _reader.ReadVersion();
			Assert.AreEqual(decimal.Parse(expected), result);
		}

		[TestCase("(test)", "test")]
		[TestCase("(te\\(st)", "te(st")]
		[TestCase("(te\\)st)", "te)st")]
		[TestCase("(te\\050st)", "te(st")]
		[TestCase("<41414141>", "AAAA")]
		[TestCase("<FFFE00410041>", "AA")]
		public void ReadObjectFromIndirectReference_String(string data, string expected)
		{
			var result = _ReadObjectFromIndirectReference(data);
			Assert.IsInstanceOf<PdfString>(result);
			Assert.AreEqual(expected, ((PdfString)result).Value);
		}

		private PdfObject _ReadObjectFromIndirectReference(string data) 
		{
			var bytes = Encoding.UTF8.GetBytes($"0 0 obj\n{data}\nendobj\n");
			_SetupData(bytes);

			var crossReferenceTable = new PdfCrossReferenceTable 
			{ 
				ObjectOffsets = new Dictionary<int, int> { { 0, 0 } },
			};

			_document.SetupGet(x => x.CrossReferenceTable).Returns(crossReferenceTable);

			var indirectReference = new PdfIndirectReference
			{
				GenerationNumber = 0,
				ObjectNumber = 0
			};

			return _reader.ReadObject(indirectReference);
		}
	}
}
