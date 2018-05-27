using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	public sealed class TwStringInfo
	{
		private short[] _surrogatedIndices;
		private int _surrogateCount;

		private string _string;
		public string String => _string;

		private int _length;
		public int Length => _length;

		private bool _hasSurrogatePairs;
		public bool HasSurrogateParis => _hasSurrogatePairs;

		public TwStringInfo(string text)
		{
			if (text?.Length <= 0)
			{
				this._string = string.Empty;
				return;
			}

			this._string = text.Normalize(NormalizationForm.FormC);
			this._length = this._string.Length;

            // サロゲートペア文字の文字位置をリストに格納する
            var surrogateIndicesList = new LinkedList<short>();

			for (short charIndex = 0; charIndex < this._length; ++charIndex)
			{
				if (char.IsHighSurrogate(this._string, charIndex))
				{
					surrogateIndicesList.AddLast(charIndex);
					++charIndex;
				}
			}

			this._surrogateCount = surrogateIndicesList.Count;
			this._hasSurrogatePairs = this._surrogateCount > 0;
			this._length = String.Length - this._surrogateCount;

			if (this._hasSurrogatePairs)
			{
				this._surrogatedIndices = surrogateIndicesList.ToArray();
			}

			surrogateIndicesList.Clear();
			surrogateIndicesList = null;
		}

		public string Slice(int startIndex)
		{
			if (_hasSurrogatePairs)
			{
				int startSurListIndex = 0;
				int actualStartIndex = startIndex;
				while (startSurListIndex < _surrogateCount && _surrogatedIndices[startSurListIndex] < startIndex)
				{
					actualStartIndex++;
					startSurListIndex++;
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

			if (this._hasSurrogatePairs)
			{
                int surrogateListIndex = 0;
                var surrogateList = this._surrogatedIndices;

                // サロゲートペアを含めたstartIndexを計算する
				int actStartIndex = startIndex;
				while (surrogateListIndex < surrogateList.Length && surrogateList[surrogateListIndex] < actStartIndex)
				{
					++actStartIndex;
                    ++surrogateListIndex;
				}

                // サロゲートペアを含めたendIndexを計算する
				int actEndIndex = actStartIndex + length;
				while (surrogateListIndex < surrogateList.Length && surrogateList[surrogateListIndex] < actEndIndex)
				{
					++actEndIndex;
                    ++surrogateListIndex;
				}

				return this._string.Substring(actStartIndex, actEndIndex - actStartIndex);
			}
			else
			{
				return this._string.Substring(startIndex, length);
			}
		}

		~TwStringInfo()
		{
			this._length = 0;
			this._string = null;
			this._hasSurrogatePairs = false;
			this._surrogatedIndices = null;
		}
	}
}
