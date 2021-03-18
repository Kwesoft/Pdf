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
	class PdfIntegerTests
	{
		[TestCase((byte)1)]
		[TestCase((int)100000000)]
		[TestCase((long)10000000000000)]
		public void ImplicitOperatorFromFloat(dynamic input)
		{
			var result = (PdfInteger)input;

			Assert.AreEqual((double)input, result.Value);
		}

		[TestCase((byte)1, "1")]
		[TestCase((int)100000000, "100000000")]
		[TestCase((long)10000000000000, "10000000000000")]
		public void ConvertToString(dynamic input, string expected)
		{
			var result = ((PdfInteger)input).ToString();

			Assert.AreEqual(expected, result);
		}
	}
}
