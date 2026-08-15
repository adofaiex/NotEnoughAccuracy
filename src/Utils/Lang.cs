using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NotEnoughAccuracy.Utils;

public static class Lang
{
	public const string DefaultLanguage = "en_US";

	static Lang()
	{
		using var _ = Mod.Info.Begin("Languages.Load");

		foreach (var file in Directory.EnumerateFiles(Path.Combine(Mod.ModEntry.Path, "Languages")))
		{
			var fileName = Path.GetFileName(file);
			if (!fileName.ToLower().EndsWith(".lang")) continue;
			var language = new Language(fileName[..^5]);
			if (!language.NotLanguage) LanguageList.Add(language);
			LanguageMap[language.Code] = language;
			Mod.Info.Log($"found language: {language.Code} {language.Name}");
		}
	}

	public static List<Language> LanguageList { get; } = [];

	private static Dictionary<string, Language> LanguageMap { get; } = [];

	private static Language SelectedLanguage
	{
		get
		{
			var code = Mod.Settings.Language;
			if (LanguageMap.TryGetValue(code, out var language)) return language;
			Mod.Warn.Log($"language {code} not found, defaulting to {DefaultLanguage}");
			Mod.Settings.Language = DefaultLanguage;
			Mod.Settings.Save();
			return SelectedLanguage;
		}
	}

	public static string Translate(string key, params object?[] args)
	{
		return SelectedLanguage.Translate(key, args) ?? key;
	}

	public class Language
	{
		public Language(string code)
		{
			Code = code;

			var lines = File.ReadAllLines(Path.Combine(Mod.ModEntry.Path, "Languages", $"{code}.lang"), Encoding.UTF8);

			foreach (var line in lines)
			{
				var trimmed = line.Trim();
				if (trimmed.IsNullOrEmpty() || trimmed.StartsWith('#')) continue;
				var split = trimmed.Split('=', 2);
				var key = split[0];
				var value = split[1]
					.Replace("\\n", "\n")
					.Replace("\\s", " ")
					.Replace("\\/", "\\");
				if (Translations.TryGetValue(key, out var translation)) value = $"{translation}\n{value}";
				Translations[key] = value;
			}

			Name = Translations.GetValueOrDefault("Name", code);

			NotLanguage = Translations.ContainsKey("NotLanguage");

			if (Translations.TryGetValue("Parents", out var parentsString))
				Parents.AddRange(parentsString.Split(' '));
		}

		public string Code { get; }

		public string Name { get; }

		public bool NotLanguage { get; }

		private Dictionary<string, string> Translations { get; } = [];

		private List<string> Parents { get; } = [];

		// ReSharper disable once MemberHidesStaticFromOuterClass
		public string? Translate(string key, params object?[] args)
		{
			var translation = Translations.GetValueOrDefault(key);
			if (translation is not null) return string.Format(translation, args);

			foreach (var parent in Parents)
			{
				translation = LanguageMap.GetValueOrDefault(parent).Translate(key, args);
				if (translation is not null) return translation;
			}

			return null;
		}
	}
}
