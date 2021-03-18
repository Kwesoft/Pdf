using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.UnitTests.Internal
{
	[TestFixture]
	class PdfCrossReferenceTableTests
	{
		[Test]
		public void ConvertToString()
		{
			var result = new PdfCrossReferenceTable { 
				ObjectOffsets = new Dictionary<int, int> {
					{ 0, 0 },
					{ 1, 10 },
					{ 2, 20 }
				},
				ObjectCount = 3,
				FirstObjectNumber = 0
			}.ToString();

			Assert.AreEqual("xref\n0 3\n0000000000 65535 f\n0000000010 00000 n\n0000000020 00000 n\n", result);
		}
	}
}
