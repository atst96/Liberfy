using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
	[JsonObject]
	class WindowStatus
	{
		[JsonIgnore]
		public bool IsEmpty => Width <= 0 && Height <= 0;

		[JsonProperty("Top")]
		public double? Top { get; set; }

		[JsonProperty("Left")]
		public double? Left { get; set; }

		[JsonProperty("Width")]
		public double? Width { get; set; }

		[JsonProperty("Height")]
		public double? Height { get; set; }

		[JsonProperty("State")]
		public WindowState? State { get; set; }
	}
}
