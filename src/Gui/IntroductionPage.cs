using NotEnoughAccuracy.Utils;
using static NotEnoughAccuracy.Utils.Yui;

namespace NotEnoughAccuracy.Gui;

public static class IntroductionPage
{
	public static unit Draw()
	{
		Text(Lang.Translate("Page.Introduction.Content"));

		return [];
	}
}
