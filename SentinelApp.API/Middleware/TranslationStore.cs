
namespace SentinelApp.API.Middleware
{
	public static class TranslationStore
	{
		private static ITranslationProvider _provider;

		public static void Configure(ITranslationProvider provider)
		{
			_provider = provider;
		}
        public static string GetValidLanguage(string preferredLanguage)
        {
            if (string.IsNullOrWhiteSpace(preferredLanguage))
                return "en-gb";

            var lang = preferredLanguage.ToLower();

            var filePath = Path.Combine("Resources", $"translations_{lang}.json");

            if (File.Exists(filePath))
                return lang;

            return "en-gb"; 
        }
        public static string Get(string key)
		{
			//EnsureInitialized();
			return _provider.Get(key);
		}
		public static string Get(string key, object args)
		{
			//EnsureInitialized();
			var template = _provider.Get(key);

			if (args == null)
				return template;

			foreach (var prop in args.GetType().GetProperties())
			{
				template = template.Replace(
					$"{{{prop.Name}}}",
					prop.GetValue(args)?.ToString() ?? ""
				);
			}

			return template;
		}
		//private static void EnsureInitialized()
		//{
		//	if (_provider == null)
		//		throw new InvalidOperationException(
		//			"TranslationStore is not initialized. Call Configure() in Program.cs");
		//}
	}
}
