using System.Collections.Generic;

namespace NotEnoughAccuracy;

public static class Api
{
	// [Api Version 1]
	//
	// Judgments: the list of judgments to the current tile
	// Judgements: kept in v1 for compatibility
	// Scores: the prefix sum of Judgments
	// Tiles: the counts of tiles hit (one mid-spin is counted as one tile), corresponding to Judgments
	// Accuracy & FullScore: NEAcc = Accuracy / FullScore * 100%
	//
	// Don't assume the value of FullScore. Divide by it every time.

	public static long ApiVersion => 1;

	public static IReadOnlyList<long> Judgments => Patches.Judgment.AsReadOnly();

	public static IReadOnlyList<long> Judgements => Judgments;

	public static IReadOnlyList<long> Scores => Patches.AccumulatedScores.AsReadOnly();

	public static IReadOnlyList<long> Tiles => Patches.AccumulatedTiles.AsReadOnly();

	public static long Accuracy => Patches.CachedScore;

	public static long FullScore => Patches.FullScore;
}
