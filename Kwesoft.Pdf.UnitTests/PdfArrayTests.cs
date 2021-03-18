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
	class PdfArrayTests
	{
		[Test]
		public void ImplicitOperator()
		{
			var content = new PdfObject[] { };
			var result = (PdfArray)content;

			Assert.AreEqual(content, result.Value);
		}

		[TestCase(0, "[]")]
		[TestCase(1, "[*]")]
		[TestCase(2, "[* *]")]
		[TestCase(3, "[* * *]")]
		public void ConvertToString(int take, string expected)
		{
			var obj = new Mock<PdfObject>();
			obj.Setup(x => x.ToString()).Returns("*");

			var content = new List<PdfObject> { obj.Object, obj.Object, obj.Object }.Take(take);
			var result = ((PdfArray)content.ToArray()).ToString();
			
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void DocumentAssigned()
		{
			var obj = new Mock<PdfObject>();
			var document = new Mock<IEditablePdfDocument>();

			var arr = (PdfArray)new PdfObject[] { obj.Object };
			arr.Document = document.Object;

			Assert.AreEqual(document.Object, obj.Object.Document);
		}

		[Test]
		public void Add()
		{
			var obj = new Mock<PdfObject>();
			var document = new Mock<IEditablePdfDocument>();
			var arr = new PdfArray();

			arr.Document = document.Object;
			arr.Add(obj.Object);

			document.Verify(x => x.Add(arr, obj.Object));
		}

		[Test]
		public void Remove()
		{
			var obj = new Mock<PdfObject>();
			var document = new Mock<IEditablePdfDocument>();
			var arr = (PdfArray)new PdfObject[] { obj.Object };

			arr.Document = document.Object;
			arr.Remove(obj.Object);

			document.Verify(x => x.Remove(arr, obj.Object));
		}

		[Test]
		public void RemoveByIndex()
		{
			var obj = new Mock<PdfObject>();
			var document = new Mock<IEditablePdfDocument>();
			var arr = (PdfArray)new PdfObject[] { obj.Object };

			arr.Document = document.Object;
			arr.Remove(0);

			document.Verify(x => x.Remove(arr, 0));
		}
	}
}
