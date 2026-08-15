using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using NotEnoughAccuracy.Utils;
using UnityModManagerNet;
using Logger = NotEnoughAccuracy.Utils.Logger;

namespace NotEnoughAccuracy;

[UsedImplicitly]
[NoReorder]
public static class Mod
{
	[UsedImplicitly] public static UnityModManager.ModEntry ModEntry { get; } = Main.ModEntryToLoad!;

	public static bool Enabled => ModEntry.Enabled;

	[UsedImplicitly] public static UnityModManager.ModEntry.ModLogger ModLogger { get; private set; } = ModEntry.Logger;

	private static LoggerContext LoggerContext { get; } = new("    ");

	[UsedImplicitly] public static Logger Info { get; } = new(LoggerContext, ModLogger.Log);

	[UsedImplicitly] public static Logger Warn { get; } = new(LoggerContext, ModLogger.Warning);

	[UsedImplicitly] public static Logger Error { get; } = new(LoggerContext, ModLogger.Error);

	[UsedImplicitly] public static Logger Crit { get; } = new(LoggerContext, ModLogger.Critical);

	[UsedImplicitly] public static Harmony Harmony { get; } = new(ModEntry.Info.Id);

	[UsedImplicitly] public static Settings Settings { get; } = Settings.Load();

	static Mod()
	{
		using (Info.Begin("Mod.Initialize"))
		{
			ModEntry.OnToggle = OnToggle;
			ModEntry.OnGUI = _ => Gui.Gui.Draw();
			ModEntry.OnUpdate = (_, _) => InGameRenderer.OnUpdate();
		}
	}

	private static bool OnToggle(UnityModManager.ModEntry _, bool value)
	{
		if (value)
			using (Info.Begin("Mod.Enable"))
			{
				Harmony.PatchAll(Assembly.GetExecutingAssembly());
			}
		else
			using (Info.Begin("Mod.Disable"))
			{
				Harmony.UnpatchAll(ModEntry.Info.Id);
			}

		return true;
	}
}
