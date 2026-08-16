using NotEnoughAccuracy.Utils;
using static NotEnoughAccuracy.Utils.Yui;
using static NotEnoughAccuracy.Utils.YuiPreset;

namespace NotEnoughAccuracy.Gui;

public static class SettingPage
{
	private static SizesGroup.Holder Group { get; } = new();

	public static unit Draw()
	{
		var group = Group.Begin();

		SwitchOption(
			group,
			ref Mod.Settings.DisplayInGame,
			"Page.Setting.DisplayInGame"
		);

		Separator();

		var newInGameFontSize = DoubleOption(
			group,
			ref Mod.Settings.InGameFontSize,
			"Page.Setting.InGameFontSize"
		);

		if (newInGameFontSize is not null) InGameRenderer.FontSize = newInGameFontSize.Value;

		Separator();

		SwitchOption(
			group,
			ref Mod.Settings.DisplayInDetailedResults,
			"Page.Setting.DisplayInDetailedResults"
		);

		Separator();

		SwitchOption(
			group,
			ref Mod.Settings.DisplayInJudgmentTexts,
			"Page.Setting.DisplayInJudgmentTexts"
		);

		Separator();

		SwitchOption(
			group,
			ref Mod.Settings.NoDisplayPerfect,
			"Page.Setting.NoDisplayPerfect"
		);

		return [];
	}
}
