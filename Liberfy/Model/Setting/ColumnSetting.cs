using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	[JsonObject( MemberSerialization.OptIn)]
	internal class ColumnSetting
	{
		[JsonProperty("type")]
		public ColumnType Type { get; set; }

		[JsonProperty("user_id")]
		public long UserId { get; set; }

		[JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
		private ColumnProperties _properties;

		public ColumnProperties Properties
		{
			get { return _properties ?? (_properties = new ColumnProperties()); }
			set { _properties = value; }
		}

		[JsonConstructor]
		private ColumnSetting() { }

		public ColumnSetting(ColumnType type, long userId, ColumnProperties properties = null)
		{
			Type = type;
			UserId = userId;
			_properties = properties;
		}

		public ColumnSetting(ColumnType type, Account account, ColumnProperties properties = null)
		{
			Type = type;
			UserId = account.Id;
			_properties = properties;
		}

		public static ColumnSetting CreateFromDefault(ColumnSetting def, Account account)
		{
			var clone = new ColumnProperties();
			foreach (var prop in def.Properties)
			{
				if (prop.Value != null)
				{
					string key = string.Copy(prop.Key);
					int valueType = (int)prop.Value.GetTypeCode();

					if (valueType >= 2 && valueType <= 15)
						clone.Add(key, prop.Value);
					else if (valueType == 16)
						clone.Add(key, prop.Value.ToString());
				}
			}

			return new ColumnSetting(def.Type, account, clone);
		}
	}
}
