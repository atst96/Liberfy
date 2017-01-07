using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	partial class App
	{
		internal static ClientContent Client { get; } = new ClientContent();
	}

	internal class ClientContent : NotificationObject
	{
		public ClientContent()
		{
		}

		public FluidCollection<PageItem> Pages { get; } = new FluidCollection<PageItem>();
	}
}
