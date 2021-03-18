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
	class PdfNullTests
	{
		[Test]
		public void ConvertToString()
		{
			var result = new PdfNull().ToString();

			Assert.AreEqual("null", result);
		}
	}
}
