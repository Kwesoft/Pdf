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
	class PdfNameTests
	{
		[Test]
		public void ImplicitOperator()
		{
			var result = (PdfName)"test";

			Assert.AreEqual("test", result.Value);
		}

		[TestCase("test", "/test")]
		[TestCase("te/\\s\nt () <> {} [] #", "/te#2F#5Cs#0At#20#28#29#20#3C#3E#20#7B#7D#20#5B#5D#20#23")]
		public void ConvertToString(string input, string expected)
		{
			var result = ((PdfName)input).ToString();

			Assert.AreEqual(expected, result);
		}
	}
}
