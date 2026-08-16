using JetBrains.Annotations;
using NotEnoughAccuracy.Utils;
using UnityModManagerNet;

namespace NotEnoughAccuracy;

[NoReorder]
public class Settings : UnityModManager.ModSettings
{
	public string Language = Lang.DefaultLanguage;

	public bool DisplayInGame = true;

	public double InGameFontSize = 96.0;

	public bool DisplayInDetailedResults = true;

	public bool DisplayInJudgmentTexts = true;

	public bool NoDisplayPerfect = true;

	public unit Save()
	{
		Save(Mod.ModEntry);
		return [];
	}

	public override void Save(UnityModManager.ModEntry modEntry)
	{
		Save(this, modEntry);
	}

	public static Settings Load()
	{
		return Load<Settings>(Mod.ModEntry);
	}
}
