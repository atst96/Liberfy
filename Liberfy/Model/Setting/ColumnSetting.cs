using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	[JsonObject]
	class ColumnSetting
	{
		[JsonProperty("type")]
		public ColumnType Type { get; set; }

		[JsonProperty("user_id")]
		public long UserId { get; set; }

		[JsonProperty("properties")]
		public ColumnProperties Properties { get; set; }
	}
}
