using System.Collections.Generic;

namespace NotEnoughAccuracy;

public static class Api
{
	public static IReadOnlyList<long> Judgements => Patches.Judgements.AsReadOnly();

	public static IReadOnlyList<long> Scores => Patches.AccumulatedScores.AsReadOnly();

	public static IReadOnlyList<long> Tiles => Patches.AccumulatedTiles.AsReadOnly();

	public static long Accuracy => Patches.CachedScore;

	public static long FullScore => Patches.FullScore;
}
