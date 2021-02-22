using Newtonsoft.Json;
using Skuld.Core.Models;
using System;

namespace Skuld.API.Models
{
	public class ResponseEventResult<T> : EventResult<T>
	{
		public string Warning { get; set; }

		public static ResponseEventResult<T> FromSuccess<T>(T data)
			=> new()
			{
				Successful = true,
				Data = data
			};

		public static ResponseEventResult<T> FromFailure<T>(T data, string reason)
			=> new()
			{
				Data = data,
				Successful = false,
				Error = reason
			};

		public static ResponseEventResult<T> FromFailureException<T>(T data, string reason, Exception ex)
			=> new()
			{
				Data = data,
				Successful = false,
				Error = reason,
				Exception = ex
			};

		public ResponseEventResult<T> WithWarning(string warning)
		{
			Warning = warning;
			return this;
		}

		public ResponseEventResult<T> WithData(T data)
		{
			Data = data;
			return this;
		}

		public ResponseEventResult<T> WithException(Exception exception)
		{
			Exception = exception;
			return this;
		}

		public ResponseEventResult<T> WithError(string error)
		{
			Error = error;
			return this;
		}

		public override string ToJson()
		{
			return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Include,
				Formatting = Formatting.Indented
			});
		}
	}
}
