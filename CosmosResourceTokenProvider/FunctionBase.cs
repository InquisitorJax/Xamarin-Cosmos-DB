using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Web.Http;

namespace CosmosResourceTokenProvider
{
	public abstract class FunctionBase
	{
		protected BadRequestObjectResult LogErrorAndReturnBadObjectResult(string error, ILogger log, Exception ex = default)
		{
			log.Log(LogLevel.Error, ex is null ? $"{error}" : $"{error}. Unhandled exception: {ex}");
			return new BadRequestObjectResult(ex is null ? $"{error}" : $"{error}. Unhandled exception: {ex}");
		}

		protected InternalServerErrorResult LogInternalServerError(ILogger log, Exception ex)
		{
			log.Log(LogLevel.Error, $"{ex.Message}. Unhandled exception: {ex}");
			return new InternalServerErrorResult();
		}

	}
}
