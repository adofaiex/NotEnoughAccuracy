using JetBrains.Annotations;
using UnityModManagerNet;

namespace NotEnoughAccuracy;

public static class Main
{
	public static UnityModManager.ModEntry? ModEntryToLoad { get; private set; }

	[UsedImplicitly]
	public static bool Load(UnityModManager.ModEntry modEntry)
	{
		ModEntryToLoad = modEntry;
		Mod.Info.Log("completed loading process");
		return true;
	}
}
