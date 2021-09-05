# Liberfy (Alpha version)
開発中のWindows向けSNSクライアントアプリケーションです。

## 対応サービス
- [Twitter](https://developer.twitter.com/)
- [Mastodon](https://github.com/tootsuite)
<!-- - [Frost](https://github.com/Frost-Dev/Frost)（予定）-->

## 仕様
- マルチアカウント対応
- マルチカラム対応
- 画像キャッシュ
- (Twitter) 疑似UserStream
- その他（随時追加）

## ビルドについて
ソースコードのビルド時には以下の点にご注意ください。
- NuGetパッケージの復元を行う。  
　実行ファイルの出力先にSQLite.Interop.dll(`OUTPUT_DIR\[x86 or x64]\SQLite.Interop.dll`)が存在しない場合に起動時にDllNotFoundException例外が発生します。

- `/Liberfy/Config.cs`内のTwitterクラスの定数 `@ConsumerKey` と `@ConsumerSecret` のコメントを解除し、有効なアクセスキーを設定してください。  
```csharp:/Liberfy/Config.cs
static class Config
{
    static class Twitter
    {
        //public const string @ConsumerKey = "";
        //public const string @ConsumerSecret = "";
    }
}
```

## ライセンス
- Liberfy のソースコードは[MITライセンス](https://github.com/atst1996/Liberfy/blob/master/LICENSE)の下で配布しています。
- Liberfy では以下のライブラリおよびデータを使用しています。各ライセンスについては[NOTICE.md](https://github.com/atst1996/Liberfy/blob/master/NOTICE.md)に記載しています。
　- Emoji.Wpf
  - Font Awesome
  - Livet
  - MessagePack for C#
  - NowPlayingLib
  - SgmlReader
  - ToriatamaText
  - Utf8Json
  - WPF NotifyIcon
  - XamlBehaviors for WPF

---

#### 名称の由来
`Liberfy`という名称は英語で"自由"という意味の`Liberty`と`Freedom`を合わせたものです。
