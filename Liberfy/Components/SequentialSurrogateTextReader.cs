using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class SequentialSurrogateTextReader
    {
        public string Content { get; private set; }

        public int RawCursor { get; private set; }

        public SequentialSurrogateTextReader(string content)
        {
            this.Content = content;
        }

        public int GetRawLength(int length)
        {
            int startTextPosition = this.RawCursor;
            int endTextPosition = startTextPosition + length;

            for (int textIndex = startTextPosition; textIndex < endTextPosition; ++textIndex)
            {
                if (char.IsHighSurrogate(this.Content, textIndex))
                {
                    // サロゲートペア文字を1文字としてカウントする
                    ++endTextPosition;
                }
            }

            this.RawCursor = endTextPosition;

            return endTextPosition - startTextPosition;
        }

        public string ReadLength(int length)
        {
            int startIndex = this.RawCursor;

            return this.Content.Substring(startIndex, this.GetRawLength(length));
        }

        public string ReadToEnd()
        {
            int cursor = this.RawCursor;
            this.RawCursor = this.Content.Length;

            return cursor >= this.Content.Length
                ? string.Empty
                : this.Content.Substring(cursor);
        }

        public void SkipLength(int length) => this.GetRawLength(length);
    }
}
