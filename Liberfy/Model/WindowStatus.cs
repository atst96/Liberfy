using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
	[DataContract]
	class WindowStatus
	{
		[IgnoreDataMember]
		public bool IsEmpty => Width <= 0 && Height <= 0;

		[DataMember(Name = "Top")]
		public double? Top { get; set; }

		[DataMember(Name = "Left")]
		public double? Left { get; set; }

		[DataMember(Name = "Width")]
		public double? Width { get; set; }

		[DataMember(Name = "Height")]
		public double? Height { get; set; }

		[DataMember(Name = "State")]
		public WindowState? State { get; set; }
	}
}
