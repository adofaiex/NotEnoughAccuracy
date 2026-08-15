using System.Collections.Generic;
using System.Linq;
using NotEnoughAccuracy.Utils;
using static NotEnoughAccuracy.Utils.Yui;

namespace NotEnoughAccuracy.Gui;

public static class LanguagePage
{
	private static List<(string, string)> Languages { get; } =
		[.. Lang.LanguageList.Select(lang => (lang.Code, $"{lang.Code} {lang.Name}"))];

	public static unit Draw()
	{
		using (Div(H))
		{
			using (Div(V, options: WidthMin))
			{
				Save |= Selector(ref Mod.Settings.Language, Languages, options: WidthMax);
			}

			Fill();
		}

		return [];
	}
}
