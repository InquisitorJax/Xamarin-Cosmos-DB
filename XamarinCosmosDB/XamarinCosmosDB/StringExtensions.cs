using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace XamarinCosmosDB
{
	public static class StringExtensions
	{
		public static string GetStringFromFile(this string file, string resourcePrefix, string assemblyName)
		{

			var assembly = Assembly.Load(new AssemblyName(assemblyName));
			resourcePrefix = resourcePrefix.EndsWith(".") ? resourcePrefix : resourcePrefix + ".";
			var stream = assembly.GetManifestResourceStream(resourcePrefix + file);

			if (stream == null)
			{
				Debug.WriteLine($"Unable to load chart from app resource: {resourcePrefix}{file}");
				return null;
			}

			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

	}
}
