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

		_ = DoubleOption(
			group,
			ref Mod.Settings.InGameFontSize,
			"Page.Setting.InGameFontSize"
		) is { } fontSize && (InGameRenderer.FontSize = fontSize) is { };
		
		Separator();

		Text(Lang.Translate("Page.Setting.InGamePosition"));

		using (Div(H, N, group, WidthMax))
		{
			using (Div(V, N, group, WidthMin))
			{
				int? toSet = null;

				for (var i = 0; i < 3; i++)
					using (Div(H, N, group, WidthMin))
					{
						for (var j = 0; j < 3; j++)
						{
							var anchor = i * 3 + j;
							var clicked = Button(
								Lang.Translate($"Anchor.{anchor}"),
								anchor == Mod.Settings.InGameAnchor ? ButtonStyle.Primary : ButtonStyle.Element,
								Width(50)
							);
							if (clicked) toSet = anchor;
						}
					}

				if (toSet is not null)
				{
					Save |= Mod.Settings.InGameAnchor = toSet.Value;
					InGameRenderer.Anchor = Mod.Settings.InGameAnchor;
				}
			}

			using (Div(V, N, group, WidthMin))
			{
				using (Div(H, N, group, WidthMin) + Align(0.5))
				{
					Button(Lang.Translate("Anchor.Dummy"), ButtonStyle.Dummy, WidthMin);
					Text(Lang.Translate("Anchor.Anchor"));
				}

				using (Div(H, N, group, WidthMin) + Align(0.5))
				{
					Button(Lang.Translate("Anchor.Dummy"), ButtonStyle.Dummy, WidthMin);
					Text(Lang.Translate("Anchor.OffsetX"), options: WidthMin);
					var changed = StructField(ref Mod.Settings.InGameOffsetX, DoubleFormat(), WidthMin);
					Save |= changed;
					if (changed is not null)
						InGameRenderer.OffsetX = Mod.Settings.InGameOffsetX;
				}

				using (Div(H, N, group, WidthMin) + Align(0.5))
				{
					Button(Lang.Translate("Anchor.Dummy"), ButtonStyle.Dummy, WidthMin);
					Text(Lang.Translate("Anchor.OffsetY"), options: WidthMin);
					var changed = StructField(ref Mod.Settings.InGameOffsetY, DoubleFormat(), WidthMin);
					Save |= changed;
					if (changed is not null)
						InGameRenderer.OffsetY = Mod.Settings.InGameOffsetY;
				}
			}

			Fill();
		}

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
