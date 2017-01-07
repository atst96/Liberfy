using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	[JsonObject]
	class PageItem : NotificationObject
	{
		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonIgnore]
		public FluidCollection<ColumnBase> Columns { get; } = new FluidCollection<ColumnBase>();

		internal ColumnSetting[] GetSettings()
		{
			return Columns.Select((c) => c.ToSetting()).ToArray();
		}

		internal void SetSettings(ColumnSetting[] settings)
		{
			Columns.Clear();

			if (settings == null)
				return;

			foreach (var setting in settings)
			{
				var column = ColumnBase.FromSettings(setting);

				Columns.Add(column);
			}
		}
	}
}
