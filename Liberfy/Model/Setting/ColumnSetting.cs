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

		private ColumnProperties _properties = new ColumnProperties();

		[JsonProperty("properties")]
		public ColumnProperties Properties
		{
			get { return _properties; }
			set { _properties = value != null ? new ColumnProperties(value) : new ColumnProperties(); }
		}

		[JsonConstructor]
		private ColumnSetting() { }

		public ColumnSetting(ColumnType type, long userId, ColumnProperties properties = null)
		{
			Type = type;
			UserId = userId;
			Properties = properties;
		}

		public ColumnSetting(ColumnType type, Account account, ColumnProperties properties = null)
		{
			Type = type;
			UserId = account.Id;
			Properties = properties;
		}
	}
}
