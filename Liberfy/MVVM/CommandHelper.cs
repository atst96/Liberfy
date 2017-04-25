using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Liberfy
{
	class CommandHelper
	{
		public static void Dispose(params Command[] commands)
		{
			foreach (var command in commands)
			{
				command.Dispose();
			}
		}
	}
}
