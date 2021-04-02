using Kwesoft.Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf
{
	internal class PdfReader : IPdfReader
	{
		private readonly IEditablePdfDocument _document;

		public PdfReader(IEditablePdfDocument document)
		{
			_document = document;
		}

		public decimal ReadVersion()
		{
			var content = _document.Encoding.GetString(_document.Read(0, _document.Find(2, window => window[0] == 10 && window[1] != 37)));

			switch (content.Split('\n')[0])
			{
				case "%PDF-1.4": return 1.4M;
				default: throw new Exception("Invalid document");
			};
		}

		public PdfObject ReadObject(PdfIndirectReference reference)
		{
			var index = _document.Find(_document.Keywords.LineBreak, _document.CrossReferenceTable.ObjectOffsets[reference.ObjectNumber]) + 1;
			var result = _ReadObject(index, null);
			if (_document.ByteEquals(index + result.Length, _document.Keywords.EndObjLine)) return result;
			var streamStart = _document.Find(_document.Keywords.StreamStartLine, index + result.Length);
			var end = _document.Find(_document.Keywords.StreamEndLines, streamStart);
			var stream = new PdfStream
			{
				Properties = (PdfDictionary)result,
				Offset = index,
				Length = end - index,
				Data = _document.Read(streamStart + 1, end - _document.Keywords.StreamEndLines.Length - streamStart),
				Document = _document
			};
			result.Parent = stream;
			result.Offset = 0;
			return stream;
		}


		public PdfCrossReferenceTable ReadCrossReferenceTable(PdfTrailer trailer)
		{
			var pos = trailer.CrossReferenceTableOffset;
			var length = trailer.Index - pos;
			var crossReferenceData = _document.Encoding.GetString(_document.Read(pos, length - 1)).Split(new[] { PdfKeywords.LineBreak }, StringSplitOptions.RemoveEmptyEntries);

			var crossReferenceTableSummary = crossReferenceData[1].Split(new[] { PdfKeywords.Space }, StringSplitOptions.RemoveEmptyEntries);
			var firstObjectNumber = int.Parse(crossReferenceTableSummary[0]);
			var objectCount = int.Parse(crossReferenceTableSummary[1]);

			var result = new PdfCrossReferenceTable
			{
				ObjectOffsets = new Dictionary<int, int>(),
				Offset = pos,
				Length = length,
				FirstObjectNumber = firstObjectNumber,
				ObjectCount = objectCount,
				Document = _document
			};

			for (var i = 2; i < crossReferenceData.Length; i++)
			{
				result.ObjectOffsets.Add(firstObjectNumber + i - 2, int.Parse(crossReferenceData[i].Split(new[] { PdfKeywords.Space }, StringSplitOptions.RemoveEmptyEntries)[0]));
			}

			return result;
		}

		public PdfTrailer ReadTrailer()
		{
			var eofPos = _document.FindReverse(_document.Keywords.EOFLine);

			var startxrefPos = _document.FindReverse(_document.Keywords.StartXRefLine, eofPos);
			var startxrefData = _document.Encoding.GetString(_document.Read(startxrefPos + 11, eofPos - (startxrefPos + 11)));

			var trailerPos = _document.FindReverse(_document.Keywords.TrailerLine, startxrefPos) + 1;
			var result = new PdfTrailer
			{
				Offset = trailerPos,
				Length = eofPos - trailerPos,
				CrossReferenceTableOffset = int.Parse(startxrefData),
				Document = _document
			};
			result.TrailerDictionary = _ReadDictionary(trailerPos + 8, result);
			return result;
		}

		private PdfObject _ReadObject(int index, PdfObject parent)
		{
			if(_document.Length >= index + 4)
			{
				var raw = _document.Encoding.GetString(_document.Read(index, 4));
				if(string.Equals(PdfKeywords.True, raw,StringComparison.InvariantCultureIgnoreCase))
					return new PdfBool { Value = true, Offset = index - (parent?.Index ?? 0), Length = _document.Keywords.True.Length, Parent = parent };
				if (string.Equals(PdfKeywords.Null, raw, StringComparison.InvariantCultureIgnoreCase))
					return new PdfNull { Offset = index - (parent?.Index ?? 0), Length = _document.Keywords.Null.Length, Parent = parent };
				if (_document.Length >= index + 5)
				{
					raw =  $"{raw}{_document.Encoding.GetString(_document.Read(index + 4, 1))}";
					if(string.Equals(PdfKeywords.False, raw, StringComparison.InvariantCultureIgnoreCase))
						return new PdfBool { Value = false, Offset = index - (parent?.Index ?? 0), Length = _document.Keywords.False.Length, Parent = parent };
				}
			}
			
			if (_document.ByteEquals(index, _document.Keywords.DictionaryStart)) return _ReadDictionary(index, parent);
			if (_document.ByteEquals(index, _document.Keywords.StringStart)) return _ReadString(index, parent);
			if (_document.ByteEquals(index, _document.Keywords.HexStringStart)) return _ReadHexString(index, parent);
			if (_document.ByteEquals(index, _document.Keywords.ArrayStart)) return _ReadArray(index, parent);
			if (_document.ByteEquals(index, _document.Keywords.NameStart)) return _ReadName(index, parent);

			var pos = index;
			var isDecimal = false;

			while (true)
			{
				if (_document.ByteEquals(pos, _document.Keywords.Decimal))
				{
					isDecimal = true;
				}
				else if (_document.ByteEquals(pos, _document.Keywords.DictionaryEnd) || _document.ByteEquals(pos, _document.Keywords.LineBreak) || _document.ByteEquals(pos, _document.Keywords.ArrayEnd))
				{
					if (isDecimal)
						return _ReadDouble(index, pos - index, parent);
					return _ReadInteger(index, pos - index, parent);
				}
				else if (_document.ByteEquals(pos, _document.Keywords.Space))
				{
					if (isDecimal)
						return _ReadDouble(index, pos - index, parent);

					var i = 1;
					while (_IsNumeric(pos + i))
						i++;

					if (_document.ByteEquals(pos + i, _document.Keywords.Space) && _document.ByteEquals(pos + i + 1, _document.Keywords.Reference))
						return _ReadIndirectReference(index, (pos + i + 2) - index, parent);

					return _ReadInteger(index, pos - index, parent);
				}
				pos++;
			}
		}

		private PdfArray _ReadArray(int index, PdfObject parent)
		{
			var pos = index + 1;
			var list = new List<PdfObject>();
			var result = new PdfArray(list)
			{
				Offset = index - (parent?.Index ?? 0),
				Parent = parent,
				Document = _document
			};

			while (!_document.ByteEquals(pos, _document.Keywords.ArrayEnd))
			{
				var obj = _ReadObject(pos, result);
				pos += obj.Length;
				list.Add(obj);
				while (_document.ByteEquals(pos, _document.Keywords.Space) || _document.ByteEquals(pos, _document.Keywords.LineBreak))
					pos++;
			}

			result.Length = pos - index + 1;
			return result;
		}

		private PdfDictionary _ReadDictionary(int index, PdfObject parent)
		{
			if (!_document.ByteEquals(index, _document.Keywords.DictionaryStart)) throw new Exception("Invalid dictionary");
			var pos = index + 2;
			var dictionary = new Dictionary<PdfName, PdfObject>();
			var result = new PdfDictionary(dictionary)
			{
				Offset = index - (parent?.Index ?? 0),
				Parent = parent,
				Document = _document
			};

			while (!_document.ByteEquals(pos, _document.Keywords.DictionaryEnd))
			{
				if (_document.ByteEquals(pos, _document.Keywords.NameStart))
				{
					var key = _ReadName(pos, result);
					pos += key.Length + 1;
					var obj = _ReadObject(pos, result);
					dictionary.Add(key, obj);
					pos += obj.Length;
				}
				else
				{
					pos++;
				}
			}
			result.Length = (pos + 2) - index;
			return result;
		}

		private PdfDouble _ReadDouble(int index, int length, PdfObject parent)
		{
			return new PdfDouble
			{
				Value = double.Parse(_document.Encoding.GetString(_document.Read(index, length))),
				Offset = index - (parent?.Index ?? 0),
				Length = length,
				Parent = parent,
				Document = _document
			};
		}

		private PdfIndirectReference _ReadIndirectReference(int index, int length, PdfObject parent)
		{
			var text = _document.Encoding.GetString(_document.Read(index, length));
			var indirectReferenceData = text.Split(' ');
			return new PdfIndirectReference
			{
				ObjectNumber = int.Parse(indirectReferenceData[0]),
				GenerationNumber = int.Parse(indirectReferenceData[1]),
				Offset = index - (parent?.Index ?? 0),
				Length = length,
				Parent = parent,
				Document = _document
			};
		}

		private PdfInteger _ReadInteger(int index, int length, PdfObject parent)
		{
			return new PdfInteger
			{
				Value = long.Parse(_document.Encoding.GetString(_document.Read(index, length))),
				Offset = index - (parent?.Index ?? 0),
				Length = length,
				Parent = parent,
				Document = _document
			};
		}

		private static string _ReadSanitisedName(string input)
		{
			var output = "";
			var i = 0;

			while (i < input.Length)
			{
				if (input[i] == '#')
				{
					output = $"{output}{(char)Convert.ToByte(input.Substring(i + 1, 2), 16)}";
					i += 3;
				}
				else if (input[i] == '/')
				{
					output = $"{output}{input[i + 1]}";
					i += 2;
				}
				else
				{
					var e1 = input.IndexOf('#', i);
					var e2 = input.IndexOf('/', i);
					e1 = e1 == -1 ? input.Length : e1;
					e2 = e2 == -1 ? input.Length : e2;
					var e = e1 < e2 ? e1 : e2;
					output = $"{output}{input.Substring(i, e-i)}";
					i = e;
				}
			}

			return new string(output.ToArray());
		}


		private PdfName _ReadName(int index, PdfObject parent)
		{
			var end = _document.Find(1, b => b.ByteEquals(_document.Keywords.Space) || b.ByteEquals(_document.Keywords.ArrayEnd) || b.ByteEquals(_document.Keywords.DictionaryEnd) || b.ByteEquals(_document.Keywords.LineBreak), index + 1);
			return new PdfName
			{
				Value = _ReadSanitisedName(_document.Encoding.GetString(_document.Read(index + 1, end - index - 1))),
				Offset = index - (parent?.Index ?? 0),
				Length = end - index,
				Parent = parent,
				Document = _document
			};
		}

		private PdfString _ReadString(int index, PdfObject parent) 
		{
			var output = "";
			var i = index + 1;

			while (true)
			{
				var b = _document.Read(i, 1);
				if (b.ByteEquals(_document.Keywords.StringEnd))
				{
					return new PdfString
					{
						Offset = index - (parent?.Index ?? 0),
						Length = i + 1 - index,
						Value = output,
						Parent = parent,
						Document = _document
					};
				}

				if (b.ByteEquals(_document.Keywords.BackSlash))
				{
					if(_document.Length >= i + 4)
					{
						var oct = _document.Read(i + 1, 3);
						if (oct.All(_IsNumeric))
						{
							output = $"{output}{_document.Encoding.GetString(_document.Encoding.GetString(oct).OctToByteArray())}";
							i += 4;
						}
						else
						{
							output = $"{output}{_document.Encoding.GetString(oct.Take(1).ToArray())}";
							i += 2;
						}
					}
					else
					{
						output = $"{output}{_document.Read(i + 1, 1)}";
						i += 2;
					}
				}
				var e1 = _document.Find(_document.Keywords.StringEnd, i);
				var e2 = _document.Find(_document.Keywords.BackSlash, i);
				e1 = e1 == -1 ? _document.Length : e1;
				e2 = e2 == -1 ? _document.Length : e2;
				var e = e1 < e2 ? e1 : e2;
				output = $"{output}{_document.Encoding.GetString(_document.Read(i, e - i))}";
				i = e;
			}
		}

		private PdfString _ReadHexString(int index, PdfObject parent) 
		{
			var end = _document.Find(_document.Keywords.HexStringEnd, index + 1);
			var data = _document.Read(index + 1, end - index - 1);
			var decoded = _document.Encoding.GetString(data);

			var value = data.Length >= 4 && data.ByteEquals(_document.Keywords.BigEndianUnicodeMarker)
				? Encoding.BigEndianUnicode.GetString(decoded.Substring(4).HexToByteArray())
				: _document.Encoding.GetString(decoded.HexToByteArray());

			return new PdfString
			{
				Offset = index - (parent?.Index ?? 0),
				Length = end - index + 1,
				Value = value,
				Parent = parent,
				Document = _document
			};
		}


		private bool _IsNumeric(int index)
		{
			return _IsNumeric(_document.Read(index));
		}

		private bool _IsNumeric(byte s)
		{
			return _document.Keywords.Numbers.Contains(s);
		}
	}
}
