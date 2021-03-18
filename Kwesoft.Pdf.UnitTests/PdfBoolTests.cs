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
	class PdfBoolTests
	{
		[TestCase(true)]
		[TestCase(false)]
		public void ImplicitOperator(bool input)
		{
			var result = (PdfBool)input;

			Assert.AreEqual(input, result.Value);
		}

		[TestCase(true, "true")]
		[TestCase(false, "false")]
		public void ConvertToString(bool input, string expected)
		{
			var result = ((PdfBool)input).ToString();

			Assert.AreEqual(expected, result);
		}
	}
}
