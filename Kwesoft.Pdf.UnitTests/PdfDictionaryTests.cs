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
	class PdfDictionaryTests
	{
		[Test]
		public void ImplicitOperator()
		{
			var content = new Dictionary<PdfName, PdfObject> { };
			var result = (PdfDictionary)content;

			Assert.AreEqual(content, result.Value);
		}

		[TestCase(0, "<<>>")]
		[TestCase(1, "<<0 *>>")]
		[TestCase(2, "<<0 *\n1 *>>")]
		[TestCase(3, "<<0 *\n1 *\n2 *>>")]
		public void ConvertToString(int count, string expected)
		{
			var content = new Dictionary<PdfName, PdfObject>();
			for(var i = 0; i < count; i++)
			{
				var key = new Mock<PdfName>();
				key.Setup(x => x.ToString()).Returns($"{i}");

				var obj = new Mock<PdfObject>();
				obj.Setup(x => x.ToString()).Returns("*");
				
				content.Add(key.Object, obj.Object);
			}
			var result = ((PdfDictionary)content).ToString();
			
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void DocumentAssigned()
		{
			var key = new Mock<PdfName>(); 
			var obj = new Mock<PdfObject>();
			var document = new Mock<IEditablePdfDocument>();

			var dictionary = (PdfDictionary)new Dictionary<PdfName, PdfObject>() { { key.Object, obj.Object } };
			dictionary.Document = document.Object;

			Assert.AreEqual(document.Object, key.Object.Document);
			Assert.AreEqual(document.Object, obj.Object.Document);
		}

		[Test]
		public void Add()
		{
			var key = new Mock<PdfName>();
			var obj = new Mock<PdfObject>();
			var document = new Mock<IEditablePdfDocument>();
			var dictionary = new PdfDictionary();

			dictionary.Document = document.Object;
			dictionary.Add(key.Object, obj.Object);

			document.Verify(x => x.Add(dictionary, key.Object, obj.Object));
		}

		[Test]
		public void Remove()
		{
			var key = new Mock<PdfName>();
			var obj = new Mock<PdfObject>();
			var document = new Mock<IEditablePdfDocument>();
			var dictionary = (PdfDictionary)new Dictionary<PdfName, PdfObject>() { { key.Object, obj.Object } };

			dictionary.Document = document.Object;
			dictionary.Remove(key.Object);

			document.Verify(x => x.Remove(dictionary, key.Object));
		}
	}
}
