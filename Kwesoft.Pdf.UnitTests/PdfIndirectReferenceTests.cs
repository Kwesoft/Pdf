using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Text;

namespace Kwesoft.Pdf.UnitTests
{
	[TestFixture]
	class PdfIndirectReferenceTests
	{
		[Test]
		public void ConvertToString()
		{
			var obj = new PdfIndirectReference { ObjectNumber = 1, GenerationNumber = 2 };
			var result = obj.ToString();
			Assert.AreEqual("1 2 R", result);
		}

		[Test]
		public void Read()
		{
			var document = new Mock<IEditablePdfDocument>();
			var obj = new Mock<PdfObject>();
			var indirectReference = new PdfIndirectReference { ObjectNumber = 1, GenerationNumber = 2, Document = document.Object };

			document.Setup(x => x.ReadObject(indirectReference)).Returns(obj.Object);

			var x = indirectReference.Read();

			document.Verify(x => x.ReadObject(indirectReference), Times.Once);
			Assert.AreSame(obj.Object, x);
		}

	}
}
