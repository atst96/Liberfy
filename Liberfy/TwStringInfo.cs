using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	public sealed class TwStringInfo : IDisposable
	{
		private int[] _surrogatedIndices;
		private int _surrogateCount;

		private bool _hasSurrogatePairs;
		public bool HasSurrogateParis => _hasSurrogatePairs;

		private string _string;
		public string String => _string;

		private int _length;
		public int Length => _length;

		public TwStringInfo(string text)
		{
			if (text?.Length <= 0)
			{
				_string = string.Empty;
				return;
			}

			_string = text.Normalize(NormalizationForm.FormC);
			_length = String.Length;

			// サロゲートペア文字の文字位置をリストに格納する
			var surrogateIndicesList = new LinkedList<int>();

			for (int charIndex = 0; charIndex < Length; charIndex++)
			{
				if (char.IsHighSurrogate(String[charIndex]))
				{
					surrogateIndicesList.AddLast(charIndex);
					charIndex++;
				}
			}

			_surrogateCount = surrogateIndicesList.Count;
			_hasSurrogatePairs = _surrogateCount > 0;
			_length = String.Length - _surrogateCount;

			if (_hasSurrogatePairs)
			{
				_surrogatedIndices = surrogateIndicesList.ToArray();
			}

			surrogateIndicesList.Clear();
			surrogateIndicesList = null;
		}

		public string Slice(int startIndex)
		{
			if (_hasSurrogatePairs)
			{
				int i = 0;

				int actualStartIndex = startIndex;
				while (i < _surrogateCount && _surrogatedIndices[i] < startIndex)
				{
					actualStartIndex++;
					i++;
				}

				return String.Substring(actualStartIndex);
			}
			else
			{
				return String.Substring(startIndex);
			}
		}

		public string Slice(int startIndex, int endIndex)
		{
			int length = endIndex - startIndex;

			if (_hasSurrogatePairs)
			{
				int i = 0;

				int actualStartIndex = startIndex;
				while (i < _surrogateCount && _surrogatedIndices[i] < actualStartIndex)
				{
					actualStartIndex++;
					i++;
				}

				int actualEndIndex = actualStartIndex + length;
				while (i < _surrogateCount && _surrogatedIndices[i] < actualEndIndex)
				{
					actualEndIndex++;
					i++;
				}

				return String.Substring(actualStartIndex, actualEndIndex - actualStartIndex);
			}
			else
			{
				return String.Substring(startIndex, length);
			}
		}

		public void Dispose()
		{
			_length = 0;
			_string = null;
			_hasSurrogatePairs = false;
			_surrogatedIndices = null;
		}
	}
}
