using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.UnitTests.Internal
{
	[TestFixture]
	class PdfTrailerTests
	{
		[Test]
		public void ConvertToString()
		{
			var trailerDictionary = new Mock<PdfDictionary>();

			var trailer = new PdfTrailer
			{
				CrossReferenceTableOffset = 100,
				TrailerDictionary = trailerDictionary.Object
			};

			trailerDictionary.Setup(x => x.ToString()).Returns("trailerdictionary");

			var result = trailer.ToString();

			Assert.AreEqual("trailer\ntrailerdictionary\nstartxref\n100", result);
		}

		[Test]
		public void Root()
		{
			var indirectReference = new Mock<PdfIndirectReference>();
			var trailerDictionary = (PdfDictionary)new Dictionary<PdfName, PdfObject> { { "Root", indirectReference.Object } };

			var trailer = new PdfTrailer
			{
				TrailerDictionary = trailerDictionary
			};

			var result = trailer.Root;

			Assert.AreEqual(indirectReference.Object, result);
		}

		[Test]
		public void Info()
		{
			var indirectReference = new Mock<PdfIndirectReference>();
			var trailerDictionary = (PdfDictionary)new Dictionary<PdfName, PdfObject> { { "Info", indirectReference.Object } };

			var trailer = new PdfTrailer
			{
				TrailerDictionary = trailerDictionary
			};

			var result = trailer.Info;

			Assert.AreEqual(indirectReference.Object, result);
		}
	}
}
