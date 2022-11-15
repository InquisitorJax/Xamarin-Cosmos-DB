using CosmosResourceTokenProvider.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CosmosResourceTokenProvider.Startup))]

namespace CosmosResourceTokenProvider
{
	// doc: https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection

	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddSingleton<IDataRepository, DataRepository>();
		}
	}
}
