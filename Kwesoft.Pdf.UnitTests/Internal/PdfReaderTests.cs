using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Kwesoft.Pdf.Helpers;
using System.Linq;
using Kwesoft.Pdf.UnitTests.Helpers;

namespace Kwesoft.Pdf.UnitTests.Internal
{
	[TestFixture]
	class PdfReaderTests
	{
		PdfReader _reader;
		Mock<IEditablePdfDocument> _document;
		Encoding _encoding;
		
		[SetUp]
		public void Setup()
		{
			_encoding = Encoding.UTF8;
			_document = new Mock<IEditablePdfDocument>();
			_document.SetupGet(x => x.Encoding).Returns(_encoding);
			_document.SetupGet(x => x.Keywords).Returns(new PdfKeywordBytes(Encoding.UTF8));

			_reader = new PdfReader(_document.Object);
		}

		private void _SetupData(byte[] data)
		{
			_document.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((index, length) => data.Skip(index).Take(length).ToArray());
			_document.Setup(x => x.Read(It.IsAny<int>())).Returns<int>((index) => data[index]);
			_document.SetupGet(x => x.Length).Returns(data.Length);
		}

		[TestCase("%PDF-1.4\n*", "1.4")]
		public void ReadVersion(string input, string expected)
		{
			var data = _encoding.GetBytes(input);
			_SetupData(data);

			var result = _reader.ReadVersion();
			Assert.AreEqual(decimal.Parse(expected), result);
		}

		[TestCase("(test)", "test")]
		[TestCase("(te\\(st)", "te(st")]
		[TestCase("(te\\)st)", "te)st")]
		[TestCase("(te\\050st)", "te(st")]
		[TestCase("<41414141>", "AAAA")]
		[TestCase("<FFFE00410041>", "AA")]
		public void ReadObject_String(string data, string expected)
		{
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfString>(result);
			Assert.AreEqual(expected, ((PdfString)result).Value);
		}


		[TestCase("true", true)]
		[TestCase("True", true)]
		[TestCase("TRUE", true)]
		[TestCase("false", false)]
		[TestCase("False", false)]
		[TestCase("FALSE", false)]
		public void ReadObject_Bool(string data, bool expected)
		{
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfBool>(result);
			Assert.AreEqual(expected, ((PdfBool)result).Value);
		}

		[TestCase("null")]
		[TestCase("Null")]
		[TestCase("NULL")]
		public void ReadObject_Null(string data)
		{
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfNull>(result);
		}


		[TestCase("0", 0)]
		[TestCase("100", 100)]
		[TestCase("10000000000", 10000000000)]
		[TestCase("-100", -100)]
		[TestCase("-0", 0)]
		public void ReadObject_Int(string data, long expected)
		{
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfInteger>(result);
			Assert.AreEqual(expected, ((PdfInteger)result).Value);
		}

		[TestCase("0.0", 0)]
		[TestCase("100.123", 100.123)]
		[TestCase("10000000000.1234", 10000000000.1234)]
		[TestCase("-100.123", -100.123)]
		[TestCase("-0.0", 0)]
		public void ReadObject_Double(string data, double expected)
		{
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfDouble>(result);
			Assert.AreEqual(expected, ((PdfDouble)result).Value);
		}

		[TestCase("/test", "test")]
		[TestCase("/te//st", "te/st")]
		[TestCase("/te#20st", "te st")]
		public void ReadObject_Name(string data, string expected)
		{
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfName>(result);
			Assert.AreEqual(expected, ((PdfName)result).Value);
		}


		[TestCase("[1 2 3]", 1, 2, 3)]
		[TestCase("[(a) (b) (c)]", "a", "b", "c")]
		[TestCase("[1  2 (c) (d)]", 1, 2, "c", "d")]
		[TestCase("[1   2 (c)\n\n(d)]", 1, 2, "c", "d")]
		public void ReadObject_Array(string data, params dynamic[] expected)
		{
			var expectedObjects = expected.Select(e => (PdfObject)e);
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfArray>(result);
			expectedObjects.ToList().AssertAllEqual(((PdfArray)result).Value);
		}

		[Test]
		public void ReadObject_ArrayNested()
		{
			var result = _ReadObject("[[1] 2 [(a) [(b) []] (c)]]");
			Assert.IsInstanceOf<PdfArray>(result);
			var outer = (PdfArray)result;
			Assert.AreEqual(3, outer.Value.Count);
			Assert.IsInstanceOf<PdfArray>(outer.Value[0]);
			var nested1 = (PdfArray)outer.Value[0];
			Assert.AreEqual(1, nested1.Offset);
			Assert.AreEqual(3, nested1.Length);
			Assert.AreEqual(1, nested1.Value.Count);
			Assert.AreEqual((PdfObject)1, nested1.Value[0]);
			Assert.AreEqual((PdfObject)2, outer.Value[1]);
			Assert.IsInstanceOf<PdfArray>(outer.Value[2]);
			var nested2 = (PdfArray)outer.Value[2];
			Assert.AreEqual(7, nested2.Offset);
			Assert.AreEqual(18, nested2.Length);
			Assert.AreEqual(3, nested2.Value.Count);
			Assert.AreEqual((PdfObject)"a", nested2.Value[0]);
			Assert.IsInstanceOf<PdfArray>(nested2.Value[1]);
			var nested3 = (PdfArray)nested2.Value[1];
			Assert.AreEqual(5, nested3.Offset);
			Assert.AreEqual(8, nested3.Length);
			Assert.AreEqual(2, nested3.Value.Count);
			Assert.AreEqual((PdfObject)"b", nested3.Value[0]);
			Assert.IsInstanceOf<PdfArray>(nested3.Value[1]);
			var nested4 = (PdfArray)nested3.Value[1];
			Assert.AreEqual(5, nested4.Offset);
			Assert.AreEqual(2, nested4.Length);
			Assert.AreEqual(0, nested4.Value.Count);
			Assert.AreEqual((PdfObject)"c", nested2.Value[2]);
		}

		[TestCase("<</a 1 /b 2>>", "a", 1, "b", 2)]
		[TestCase("<</a\n1\n/b\n2>>", "a", 1, "b", 2)]
		public void ReadObject_Dictionary(string data, params dynamic[] expected)
		{
			var expectedKvps = Enumerable.Range(0, expected.Length / 2).Select(i => new KeyValuePair<PdfName, PdfObject>((PdfName)expected[i * 2], (PdfObject)expected[i * 2 + 1])).ToList();
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfDictionary>(result);
			var kvps = ((PdfDictionary)result).Value.ToList();
			Assert.AreEqual(expectedKvps.Count, kvps.Count);
			for(var i = 0; i < expectedKvps.Count; i++)
			{
				Assert.AreEqual(expectedKvps[i].Key, kvps[i].Key);
				Assert.AreEqual(expectedKvps[i].Value, kvps[i].Value);
			}
		}

		[Test]
		public void ReadObject_DictionaryNested()
		{
			var result = _ReadObject("<</a [1] /b 2 /c <</a 11 /b <</a 111>> /c 13>>>>");
			Assert.IsInstanceOf<PdfDictionary>(result);

			var outer = ((PdfDictionary)result).Value.ToList();
			Assert.AreEqual(3, outer.Count);
			Assert.AreEqual("a", outer[0].Key.Value);
			Assert.AreEqual("b", outer[1].Key.Value);
			Assert.AreEqual("c", outer[2].Key.Value);
			Assert.IsInstanceOf<PdfArray>(outer[0].Value);
			var arr = (PdfArray)outer[0].Value;
			Assert.AreEqual(5, arr.Offset);
			Assert.AreEqual(3, arr.Length);
			Assert.AreEqual(1, arr.Value.Count);
			Assert.AreEqual((PdfObject)1, arr.Value[0]);
			Assert.AreEqual((PdfObject)2, outer[1].Value);
			Assert.IsInstanceOf<PdfDictionary>(outer[2].Value);
			var nested1 = (PdfDictionary)outer[2].Value;
			var nested1values = nested1.Value.ToList();
			Assert.AreEqual(17, nested1.Offset);
			Assert.AreEqual(29, nested1.Length);
			Assert.AreEqual(3, nested1values.Count);
			Assert.AreEqual("a", nested1values[0].Key.Value);
			Assert.AreEqual("b", nested1values[1].Key.Value);
			Assert.AreEqual("c", nested1values[2].Key.Value);
			Assert.AreEqual((PdfObject)11, nested1values[0].Value);
			Assert.IsInstanceOf<PdfDictionary>(nested1values[1].Value);
			var nested2 = (PdfDictionary)nested1values[1].Value;
			var nested2values = nested2.Value.ToList();
			Assert.AreEqual(11, nested2.Offset);
			Assert.AreEqual(10, nested2.Length);
			Assert.AreEqual(1, nested2values.Count);
			Assert.AreEqual("a", nested2values[0].Key.Value);
			Assert.AreEqual((PdfObject)111, nested2values[0].Value);
			Assert.AreEqual((PdfObject)13, nested1values[2].Value);
		}

		[TestCase("<<>>\nstream\n123\nendstream", "31-32-33")]
		public void ReadObject_Stream(string data, string expected)
		{
			var result = _ReadObject(data);
			Assert.IsInstanceOf<PdfStream>(result);
			var hex = BitConverter.ToString(((PdfStream)result).Data);
			Assert.AreEqual(expected, hex);
		}

		private PdfObject _ReadObject(string data) 
		{
			var bytes = Encoding.UTF8.GetBytes($"0 0 obj\n{data}\nendobj\n");
			_SetupData(bytes);

			var crossReferenceTable = new PdfCrossReferenceTable 
			{ 
				ObjectOffsets = new Dictionary<int, int> { { 0, 0 } },
			};

			_document.SetupGet(x => x.CrossReferenceTable).Returns(crossReferenceTable);

			var indirectReference = new PdfIndirectReference
			{
				GenerationNumber = 0,
				ObjectNumber = 0
			};

			var result = _reader.ReadObject(indirectReference);
			Assert.AreEqual(result.Length, data.Length);
			Assert.AreEqual(result.Offset, 8);

			return result;
		}
	}
}
