using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Converters;
using System.Windows;

namespace Liberfy
{
	[JsonObject]
	class Setting : NotificationObject
	{
		#region

		public Application app = Application.Current;

		private void SetValue(string key, object value)
		{
			app.Resources[key] = value;
		}

		private T GetValue<T>(string key)
		{
			return (T)app.TryFindResource(key);
		}

		#endregion

		#region Generic settings

		[JsonProperty("CheckUpdate")]
		public bool _checkUpdate = true;

		[JsonProperty("LastUpdateCheck")]
		public DateTime LastUpdateChecked { get; set; }

		[JsonProperty("StayingInTaskTray")]
		public bool StayingInTaskTray { get; set; } = false;

		[JsonProperty("AlwaysShowInTaskbar")]
		public bool AlwaysShowInTaskbar { get; set; } = true;

		[JsonProperty("BackgroundType", ItemConverterType = typeof(StringEnumConverter))]
		private BackgroundType _backgroundType = BackgroundType.None;

		[JsonProperty("ImageAlignmentX")]
		private AlignmentX _imageAlignmentX = AlignmentX.Left;

		[JsonProperty("ImageAlignmentY")]
		private AlignmentY _imageAlignmentY = AlignmentY.Top;

		[JsonProperty("ImageStretch")]
		private Stretch _imageStretch = Stretch.UniformToFill;

		[JsonProperty("ImageOpacity")]
		public double _imageOpacity = 1.0d;

		[JsonProperty("ImagePath")]
		private string _imagePath = string.Empty;

		[JsonProperty("SuppressShutdown")]
		public bool SupressShutdown { get; set; } = false;

		public bool CheckUpdate
		{
			get { return _checkUpdate; }
			set { SetProperty(ref _checkUpdate, value); }
		}

		public BackgroundType BackgroundType
		{
			get { return _backgroundType; }
			set { SetProperty(ref _backgroundType, value); }
		}

		public AlignmentX ImageAlignmentX
		{
			get { return _imageAlignmentX; }
			set { SetProperty(ref _imageAlignmentX, value); }
		}

		public AlignmentY ImageAlignmentY
		{
			get { return _imageAlignmentY; }
			set { SetProperty(ref _imageAlignmentY, value); }
		}

		public Stretch ImageStretch
		{
			get { return _imageStretch; }
			set { SetProperty(ref _imageStretch, value); }
		}

		public double ImageOpacity
		{
			get { return _imageOpacity; }
			set { SetProperty(ref _imageOpacity, value); }
		}

		public string ImagePath
		{
			get { return _imagePath; }
			set { SetProperty(ref _imagePath, value); }
		}

		[JsonProperty("StartupWindowMode")]
		public uint StartupWindowMode { get; set; } = 0;

		#endregion

		#region View settings

		public const string DefaultTimelineFont = "Meiryo, Segoe UI Symbol";
		public const double DefaultTimelineFontSize = 12;

		public const string DefaultUIFont = "Meiryo";
		public const double DefaultUIFontSize = 12;

		[JsonProperty("TimelineUseUIFont")]
		public bool _timelineUseUIFont = true;

		[JsonProperty("TimelineFont")]
		private string _timelineFont = DefaultTimelineFont;

		[JsonProperty("TimelineFontSize")]
		private double _timelineFontSize = DefaultTimelineFontSize;

		[JsonProperty("UIUseSystemFont")]
		public bool _uiUseSystemFont;

		[JsonProperty("UIFont")]
		private string _uiFont = DefaultUIFont;

		[JsonProperty("UIFontSize")]
		private double _uiFontSize = DefaultUIFontSize;

		public bool TimelineUseUIFont
		{
			get { return _timelineUseUIFont; }
			set
			{
				if (SetProperty(ref _timelineUseUIFont, value))
				{
					RaisePropertyChanged(nameof(TimelineFont));
					RaisePropertyChanged(nameof(TimelineFontSize));
				}
			}
		}

		public string TimelineFont
		{
			get
			{
				return _timelineUseUIFont
				  ? UIFont : string.IsNullOrEmpty(_timelineFont)
				  ? _timelineFont : _timelineFont = DefaultTimelineFont;
			}
			set { SetProperty(ref _timelineFont, value); }
		}

		public double TimelineFontSize
		{
			get { return _timelineUseUIFont ? UIFontSize : _timelineFontSize; }
			set { SetProperty(ref _timelineFontSize, value); }
		}

		public bool UIUseSystemFont
		{
			get { return _uiUseSystemFont; }
			set
			{
				if (SetProperty(ref _uiUseSystemFont, value))
				{
					RaisePropertyChanged(nameof(UIFont));
					RaisePropertyChanged(nameof(UIFontSize));
					RaisePropertyChanged(nameof(TimelineFont));
					RaisePropertyChanged(nameof(TimelineFontSize));
				}
			}
		}

		public string UIFont
		{
			get
			{
				return _uiUseSystemFont
				  ? "" : !string.IsNullOrEmpty(_uiFont)
				  ? _uiFont : _uiFont = DefaultUIFont;
			}
			set { SetProperty(ref _uiFont, value); }
		}

		public double UIFontSize
		{
			get { return _uiUseSystemFont ? double.NaN : _uiFontSize; }
			set { SetProperty(ref _uiFontSize, value); }
		}

		[JsonProperty("ShowImageInTimeline")]
		public bool ShowImageInTimeline
		{
			get { return GetValue<bool>("ShowImageInTimeline"); }
			set
			{
				SetValue("ShowImageInTimeline", value);
				RaisePropertyChanged(nameof(ShowImageInTimeline));
			}
		}

		[JsonProperty("ShowImageInDetail")]
		public bool ShowImageInDetail
		{
			get { return GetValue<bool>("ShowImageInDetail"); }
			set
			{
				SetValue("ShowImageInDetail", value);
				RaisePropertyChanged("ShowImageInDetail");
			}
		}

		[JsonProperty("RelativeTime")]
		public bool RelativeTime
		{
			get { return GetValue<bool>("RelativeTime"); }
			set { SetValue("RelativeTime", value); }
		}

		[JsonProperty("TextFormattingMode")]
		public TextFormattingMode TextFormattingMode
		{
			get { return GetValue<TextFormattingMode>("TextFormattingMode"); }
			set
			{
				SetValue("TextFormattingMode", value);
				RaisePropertyChanged("TextFormattingMode");
			}
		}

		[JsonProperty("EnableTimelineAnimation")]
		public bool EnableTimelineAnimation { get; set; } = true;

		[JsonProperty("DisableAnimationAtTerminalConnection")]
		public bool DisableAnimationAtTerminalConnection { get; set; } = false;

		[JsonProperty("ShowActionButtonInTimeline")]
		public bool ShowActionButtonInTimeline
		{
			get { return GetValue<bool>("ShowActionButtonInTimeline"); }
			set { SetValue("ShowActionButtonInTimeline", value); }
		}

		#endregion

		#region Format settings

		const string DefaultNowPlayingFormat = @"%artist% - %name% / %album% #NowPlaying";

		[JsonProperty("NowPlayingFormat")]
		public string NowPlayingFormat { get; set; }

		[JsonProperty("InsertThumbnailAtNowPlaying")]
		private bool _insertThumbnailAtNowPlaying;

		public bool InsertThumbnailAtNowPlayying
		{
			get { return _insertThumbnailAtNowPlaying; }
			set { SetProperty(ref _insertThumbnailAtNowPlaying, value); }
		}

		#endregion
	}

	enum BackgroundType
	{
		[JsonProperty("none")]
		None,

		[JsonProperty("color")]
		Color,

		[JsonProperty("picture")]
		Picture,
	}
}
