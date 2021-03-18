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
	class PdfStringTests
	{
		[Test]
		public void ImplicitOperator()
		{
			var result = (PdfString)"test";

			Assert.AreEqual("test", result.Value);
		}

		[TestCase("test", "(test)")]
		[TestCase("te\\st()", "(te\\134st\\050\\051)")]
		public void ConvertToString(string input, string expected)
		{
			var result = ((PdfString)input).ToString();

			Assert.AreEqual(expected, result);
		}
	}
}
