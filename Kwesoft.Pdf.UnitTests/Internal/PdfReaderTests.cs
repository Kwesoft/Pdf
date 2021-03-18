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
			_reader = new PdfReader(_document.Object);
		}

		[TestCase("%PDF-1.4\n*", "1.4")]
		public void ReadVersion(string input, string expected)
		{
			var data = _encoding.GetBytes(input);
			_document.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((index, length) => data.Skip(index).Take(length).ToArray());
			_document.Setup(x => x.Read(It.IsAny<int>())).Returns<int>((index) => data[index]);

			_document.SetupGet(x => x.Length).Returns(data.Length);

			var result = _reader.ReadVersion();
			Assert.AreEqual(decimal.Parse(expected), result);
		}
	}
}
