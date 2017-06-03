using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	internal static class UILocalize
	{
		public static LocalizeDictionary<MuteType> MuteTypes { get; }
			= new LocalizeDictionary<MuteType>(new Dictionary<object, string>
			{
				[MuteType.Content]    = "内容",
				[MuteType.UserId]     = "ユーザID",
				[MuteType.ScreenName] = "@ユーザ名",
				[MuteType.ViewName]   = "表示名",
				[MuteType.Client]     = "クライアント",
			});

		public static LocalizeDictionary<SearchMode> MuteMatchTypes { get; }
			= new LocalizeDictionary<SearchMode>(new Dictionary<object, string>
			{
				[SearchMode.Partial]  = "部分一致",
				[SearchMode.Forward]  = "前方一致",
				[SearchMode.Backward] = "後方一致",
				[SearchMode.Perfect]  = "完全一致",
				[SearchMode.Regex]    = "正規表現",
			});
	}

	public class LocalizeDictionary<T> : ReadOnlyDictionary<object, string>
	{
		public LocalizeDictionary(IDictionary<object, string> dictionary)
			: base(dictionary)
		{
		}

		public string GetName(T key)
		{
			return this[key];
		}
	}
}
