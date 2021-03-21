using Kwesoft.Pdf.UnitTests.Helpers;
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
	class PdfStreamTests
	{
		[Test]
		public void DocumentAssigned()
		{
			var properties = new Mock<PdfDictionary>();
			properties.Setup(x => x.ToString()).Returns("properties");

			var stream = new PdfStream { Properties = properties.Object };
			var document = new Mock<IEditablePdfDocument>();

			stream.Document = document.Object;

			Assert.AreEqual(document.Object, properties.Object.Document);
		}

		[TestCase(true, "properties stream\n[data]\nendstream\n")]
		[TestCase(false, "stream\n[data]\nendstream\n")]
		public void ConvertToString(bool includeProperties, string expected)
		{
			var properties = new Mock<PdfDictionary>();
			properties.Setup(x => x.ToString()).Returns("properties");

			var stream = new PdfStream { Data = new byte[] { 1, 2, 3, 4, 5 } };
			if (includeProperties) stream.Properties = properties.Object;

			Assert.AreEqual(expected, stream.ToString());
		}

		[TestCase(true, "properties stream\n", "\nendstream\n")]
		[TestCase(false, "stream\n", "\nendstream\n")]
		public void GetBytes(bool includeProperties, string expectedPrefix, string expectedSuffix)
		{
			var properties = new Mock<PdfDictionary>();
			properties.Setup(x => x.ToString()).Returns("properties");

			var data = new byte[] { 1, 2, 3, 4, 5 };
			var stream = new PdfStream { Data = data };
			if (includeProperties) stream.Properties = properties.Object;

			var expected = new byte[expectedPrefix.Length + data.Length + expectedSuffix.Length];
			var prefixBytes = Encoding.UTF8.GetBytes(expectedPrefix);
			var suffixBytes = Encoding.UTF8.GetBytes(expectedSuffix);

			Buffer.BlockCopy(prefixBytes, 0, expected, 0, prefixBytes.Length);
			Buffer.BlockCopy(data, 0, expected, prefixBytes.Length, data.Length);
			Buffer.BlockCopy(suffixBytes, 0, expected, prefixBytes.Length + data.Length, suffixBytes.Length);

			expected.AssertAllEqual(stream.GetBytes(Encoding.UTF8));
		}
	}
}
