using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	internal class TextBoxController
	{
		public EventHandler<string> InsertHandler;
		public EventHandler<int> SetCaretIndexHandler;
		public EventHandler FocusHandler;

		public void Insert(string text)
		{
			InsertHandler?.Invoke(this, text);
		}

		public void SetCaretIndex(int caretIndex)
		{
			SetCaretIndexHandler?.Invoke(this, caretIndex);
		}

		public void Focus()
		{
			FocusHandler?.Invoke(this, EventArgs.Empty);
		}
	}
}
