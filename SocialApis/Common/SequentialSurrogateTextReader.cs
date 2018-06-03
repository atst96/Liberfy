using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    internal class SequentialSurrogateTextReader
    {
        private string _string;
        public String String => this._string;

        private int _cursor = 0;
        public int Cursor => this._cursor;

        public SequentialSurrogateTextReader(string text)
        {
            this._string = text;
        }

        ~SequentialSurrogateTextReader()
        {
            this._cursor = 0;
            this._string = null;
        }

        public int GetNextLength(int length)
        {
            int strLen = this._string.Length;
            int startTextPosition = this._cursor;
            int endTextPosition = startTextPosition + length;

            for (int textIndex = startTextPosition; textIndex < endTextPosition; ++textIndex)
            {
                if (char.IsHighSurrogate(this._string, textIndex))
                {
                    // サロゲートペア文字(UTF-16 2byte + 2byte = 2文字)を1文字としてカウントする
                    ++endTextPosition;
                }
            }

            this._cursor = endTextPosition;

            return endTextPosition - startTextPosition;
        }

        public string ReadLength(int length)
        {
            int startIndex = this._cursor;
            return this._string.Substring(startIndex, this.GetNextLength(length));
        }

        public string ReadToEnd()
        {
            int cursor = this._cursor;
            this._cursor = this._string.Length;

            if (cursor >= this._string.Length)
                return string.Empty;
            else
                return this._string.Substring(cursor);
        }

        public void SkipLength(int length) => this.GetNextLength(length);
    }
}
