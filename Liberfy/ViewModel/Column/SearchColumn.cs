using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    class SearchColumn : SearchColumnBase
    {
        public SearchColumn(IAccount account)
            : base(account, ColumnType.Search)
        {
        }

        private bool _useResultType;
        public bool UseResultType
        {
            get => this._useResultType;
            set => this.SetProperty(ref this._useResultType, value);
        }

        private string _resultType;
        public string ResultType
        {
            get => this._resultType;
            set => this.SetProperty(ref this._resultType, value);
        }

        public override ColumnSetting GetOption()
        {
            var opt = base.GetOption();

            opt.SetValue("use_result_type", this._useResultType);
            opt.SetValue("result_type", this._resultType);

            return opt;
        }

        protected override void SetOption(ColumnSetting opt)
        {
            base.SetOption(opt);

            this._useResultType = opt.GetValue<bool>("use_result_type");
            this._resultType = opt.GetValue<string>("result_type");
        }

        public static ReadOnlyDictionary<string, string> SearchTypes { get; } =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                ["tweet"] = "ツイート",
                ["user"] = "ユーザー",
            });

        public static ReadOnlyDictionary<string, string> ResultTypes { get; } =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                ["fixed"] = "すべて",
                ["recent"] = "最近のツイート",
                ["popular"] = "人気のツイート",
            });

        public static IReadOnlyDictionary<string, string> Languages { get; } =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                ["all"] = "すべての言語",
                ["is"] = "アイスランド語(íslenska)",
                ["am"] = "アムハラ語(አማርኛ)",
                ["ar"] = "アラビア語(العربية)",
                ["hy"] = "アルメニア語(հայերեն)",
                ["it"] = "イタリア語(italiano)",
                ["id"] = "インドネシア語(Indonesia)",
                ["ug"] = "ウイグル語(ئۇيغۇرچە)",
                ["ur"] = "ウルドゥー語(اردو)",
                ["et"] = "エストニア語(eesti)",
                ["nl"] = "オランダ語(Nederlands)",
                ["or"] = "オリヤー語(ଓଡ଼ିଆ)",
                ["kn"] = "カンナダ語(ಕನ್ನಡ)",
                ["el"] = "ギリシャ語(Ελληνικά)",
                ["gu"] = "グジャラート語(ગુજરાતી)",
                ["km"] = "クメール語(ខ្មែរ)",
                ["ckb"] = "クルド語(ソラニー) (کوردیی ناوەندی)",
                ["hr"] = "クロアチア語(hrvatski)",
                ["ka"] = "ジョージア語(ქართული)",
                ["sd"] = "シンド語(سنڌي)",
                ["si"] = "シンハラ語(සිංහල)",
                ["sv"] = "スウェーデン語(svenska)",
                ["es"] = "スペイン語(español)",
                ["sk"] = "スロバキア語(slovenčina)",
                ["sl"] = "スロベニア語(slovenščina)",
                ["sr"] = "セルビア語(српски)",
                ["th"] = "タイ語(ไทย)",
                ["tl"] = "タガログ語(Tagalog)",
                ["ta"] = "タミル語(தமிழ்)",
                ["bo"] = "チベット語(བོད་སྐད་)",
                ["dv"] = "ディベヒ語(Divehi)",
                ["te"] = "テルグ語(తెలుగు)",
                ["da"] = "デンマーク語(dansk)",
                ["de"] = "ドイツ語(Deutsch)",
                ["tr"] = "トルコ語(Türkçe)",
                ["ne"] = "ネパール語(नेपाली)",
                ["no"] = "ノルウェー語(norsk)",
                ["ps"] = "パシュトゥー語(پښتو)",
                ["hu"] = "ハンガリー語(magyar)",
                ["pa"] = "パンジャブ語(ਪੰਜਾਬੀ)",
                ["my"] = "ビルマ語(ဗမာ)",
                ["hi"] = "ヒンディー語(हिन्दी)",
                ["fi"] = "フィンランド語(suomi)",
                ["fr"] = "フランス語(français)",
                ["bg"] = "ブルガリア語(български)",
                ["vi"] = "ベトナム語(Tiếng Việt)",
                ["he"] = "ヘブライ語(עברית)",
                ["fa"] = "ペルシア語(فارسی)",
                ["bn"] = "ベンガル語(বাংলা)",
                ["pl"] = "ポーランド語(polski)",
                ["bs"] = "ボスニア語(bosanski)",
                ["pt"] = "ポルトガル語(português)",
                ["mr"] = "マラーティー語(मराठी)",
                ["ml"] = "マラヤーラム語(മലയാളം)",
                ["lo"] = "ラオ語(ລາວ)",
                ["lv"] = "ラトビア語(latviešu)",
                ["lt"] = "リトアニア語(lietuvių)",
                ["ro"] = "ルーマニア語(română)",
                ["ru"] = "ロシア語(русский)",
                ["en"] = "英語(English)",
                ["ko"] = "韓国語(한국어)",
                ["zh"] = "中国語(中文)",
                ["ja"] = "日本語(日本語)",
            });
    }
}
