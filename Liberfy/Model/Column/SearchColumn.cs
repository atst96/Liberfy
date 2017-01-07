using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class SearchColumn : SearchColumnBase
	{
		public SearchColumn(Account account)
			: base(account, ColumnType.Search)
		{
		}

		public bool UseResultType { get; set; } = true;

		public string ResultType { get; set; } = "recent";

		public bool UseUntil { get; set; } = false;

		public string Until { get; set; }

		public bool UseSinceId { get; set; }

		public long SinceId { get; set; }

		public bool UseMaxId { get; set; }

		public long MaxId { get; set; }


		protected override ColumnProperties CreateProperties()
			=> new ColumnProperties(base.CreateProperties())
			{
				["use_result_type"] = UseResultType,
				["result_type"] = ResultType,
				["use_until"] = UseUntil,
				["until"] = Until,
				["use_since_id"] = UseSinceId,
				["since_id"] = SinceId,
				["use_max_id"] = UseMaxId,
				["max_id"] = MaxId,
			};

		protected override void ApplyProperties(ColumnProperties prop)
		{
			base.ApplyProperties(prop);

			UseResultType = prop.TryGetValue<bool>("use_result_type");
			ResultType= prop.TryGetValue<string>("result_type");
			UseUntil = prop.TryGetValue<bool>("use_until");
			Until = prop.TryGetValue<string>("until");
			UseSinceId = prop.TryGetValue<bool>("use_since_id");
			SinceId = prop.TryGetValue<long>("since_id");
			UseMaxId = prop.TryGetValue<bool>("use_max_id");
			MaxId = prop.TryGetValue<long>("max_id");
		}
	}
}
