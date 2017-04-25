using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Security;

namespace Liberfy
{
	[JsonObject]
	class SettingBase : NotificationObject
	{
		public static FileStatusResult<T> FromFile<T>(string fileName) where T : SettingBase
		{
			var res = new FileStatusResult<T>();

			try
			{
				res.SetSuccess(JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName, Encoding.UTF8)));
			}
			catch (Exception e)
			{
				res.Set(statusFromException(e), e.Message);
			}

			return res;
		}

		public static FileStatusResult SaveFile<T>(string fileName, T setting) where T : SettingBase
		{
			var res = new FileStatusResult();

			try
			{
				File.WriteAllText(fileName, JsonConvert.SerializeObject(setting, Formatting.Indented));
				res.SetSuccess();
			}
			catch (Exception e)
			{
				res.Set(statusFromException(e), e.Message);
			}

			return res;
		}

		private static FileProcessStatus statusFromException(Exception e)
		{
			if (e is DirectoryNotFoundException || e is FileNotFoundException)
				return FileProcessStatus.FileNotFound;
			else if (e is JsonSerializationException)
				return FileProcessStatus.ParseError;
			else
				return FileProcessStatus.OtherError;
		}
	}

	public enum FileProcessStatus : int
	{
		Success = 0,
		FileNotFound = 1,
		ParseError = 2,
		OtherError = 3,
	}

	struct FileStatusResult
	{
		public FileProcessStatus Status { get; private set; }
		public string ErrorMessage { get; private set; }

		public void SetSuccess()
		{
			Status = FileProcessStatus.Success;
		}

		public void Set(FileProcessStatus error, string message)
		{
			Status = error;
			ErrorMessage = message;
		}
	}

	struct FileStatusResult<T>
	{
		public T Result { get; private set; }
		public FileProcessStatus Status { get; private set; }
		public string ErrorMessage { get; private set; }

		public void SetSuccess(T value)
		{
			Result = value;
			Status = FileProcessStatus.Success;
		}

		public void Set(FileProcessStatus error, string message)
		{
			Status = error;
			ErrorMessage = message;
		}
	}
}
