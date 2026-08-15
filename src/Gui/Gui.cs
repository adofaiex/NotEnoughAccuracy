using System;
using NotEnoughAccuracy.Utils;
using static NotEnoughAccuracy.Utils.Yui;

namespace NotEnoughAccuracy.Gui;

public static class Gui
{
	private static int _currentPage;

	private static string[] PageNames =>
	[
		Lang.Translate("Page.Language.Name"),
		Lang.Translate("Page.Setting.Name"),
		Lang.Translate("Page.Introduction.Name")
	];

	private static Func<unit>[] PageRenderers { get; } =
	[
		LanguagePage.Draw,
		SettingPage.Draw,
		IntroductionPage.Draw
	];

	public static unit Draw()
	{
		EnsureTexturesAlive();

		using (Div(V, P))
		{
			using (Div(H))
			{
				Space(4);
				Selector(ref _currentPage, PageNames, options: WidthMin);
				Fill();
			}

			using (Div(V, B, options: WidthMax))
			{
				using (Div(V))
				{
					Text(PageNames[_currentPage], TextStyle.Title);
					Separator();
					PageRenderers[_currentPage]();
				}
			}
		}

		return [];
	}
}
