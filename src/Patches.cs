using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HarmonyLib;
using NotEnoughAccuracy.Utils;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

namespace NotEnoughAccuracy;

public static class Patches
{
	internal const long FullScore = 1_000_000;

	private static readonly Regex RegexInjectedJudgementText = new(@"\u200B.*?\u200B");

	private static long TileScore { get; set; }

	internal static List<long> Judgements { get; } = [];

	internal static List<long> AccumulatedScores { get; } = [];

	internal static List<long> AccumulatedTiles { get; } = [];

	internal static long CachedScore { get; private set; }

	private static long Divide(long a, long b)
	{
		if (a >= 0) a += b >> 1;
		else a -= b >> 1;
		var r = a / b;
		if ((~b & r & 1) == 0 || a % b != 0) return r;
		return r >= 0 ? r - 1 : r + 1;
	}

	private static unit AddJudgement(long judgement)
	{
		Judgements.Add(judgement);
		var score = AccumulatedScores is [.., var s] ? s : 0;
		var tiles = AccumulatedTiles is [.., var t] ? t : 0;
		unit _ = judgement switch
		{
			-1 => [],
			-2 => [++tiles, score -= 100],
			-3 => [score -= 100],
			-4 => [score -= 50],
			_ => [++tiles, score += judgement]
		};
		AccumulatedScores.Add(score);
		AccumulatedTiles.Add(tiles);
		CacheJudgementResults();
		return [];
	}

	private static unit CacheJudgementResults()
	{
		var score = AccumulatedScores is [.., var s] ? s : 0;
		var tiles = AccumulatedTiles is [.., var t] ? t : 0;
		CachedScore = Divide(FullScore * score, tiles == 0 ? 100 : 100 * tiles);
		InGameRenderer.Score = CachedScore;
		return [];
	}

	private static double RadToMs(double rad, double speed)
	{
		return 60000.0 / Math.PI * rad / speed;
	}

	[HarmonyPatch(typeof(DetailedResults), "GenerateResults")]
	public static class scrController_OnLandOnPortal
	{
		public static void Postfix(ref string __result)
		{
			if (!Mod.Settings.DisplayInDetailedResults) return;
			CacheJudgementResults();
			__result = $"{__result.TrimEnd()}\n{Lang.Translate("Game.DetailedResults", CachedScore / 10000m)}";
		}
	}

	[HarmonyPatch(typeof(scrHitTextMesh), nameof(scrHitTextMesh.Show))]
	public static class scrHitTextMesh_Show
	{
		public static void Prefix(scrHitTextMesh __instance)
		{
			var text = __instance.text.text;
			if (Mod.Settings.DisplayInJudgementTexts && !scrController.instance.playerOne.midspinInfiniteMargin)
			{
				if (Mod.Settings.NoDisplayPerfect && __instance.hitMargin == HitMargin.Perfect)
				{
					__instance.text.text = $"\u200B\u200B{TileScore}\u200B\u200B";
				}
				else
				{
					if (text.Contains("\u200B\u200B"))
					{
						__instance.Init(__instance.hitMargin);
						text = __instance.text.text;
					}

					long? score = __instance.hitMargin switch
					{
						HitMargin.Multipress => null,
						HitMargin.OverPress => -100,
						HitMargin.FailMiss => -100,
						HitMargin.FailOverload => -100,
						HitMargin.TooEarly => -50,
						HitMargin.TooLate => null,
						_ => TileScore
					};

					if (score is null) return;

					text = RegexInjectedJudgementText.Replace(text, "");
					text += $"\u200B {score}\u200B";
					__instance.text.text = text;
				}
			}
			else
			{
				if (text.Contains('\u200B')) __instance.Init(__instance.hitMargin);
			}
		}
	}

	[HarmonyPatch(typeof(scrPlanet), nameof(scrPlanet.SwitchChosen))]
	public static class scrPlanet_SwitchChosen
	{
		public static void Prefix(scrPlanet __instance)
		{
			var rad = __instance.cachedAngle - __instance.targetExitAngle;
			if (!__instance.planetarySystem.isCW) rad = -rad;
			var ms = RadToMs(
				rad,
				__instance.conductor.bpm * __instance.planetarySystem.speed * __instance.conductor.song.pitch
			);
			var msInt = Convert.ToInt32(Math.Abs(ms));
			TileScore = Math.Clamp(100 - Math.Abs(msInt), 0, 100);
		}
	}

	[HarmonyPatch(typeof(scrMarginTracker), nameof(scrMarginTracker.Reset))]
	public static class scrMarginTracker_Reset
	{
		public static void Prefix()
		{
			Judgements.Clear();
			AccumulatedScores.Clear();
			AccumulatedTiles.Clear();
			CacheJudgementResults();
		}
	}

	[HarmonyPatch(typeof(scrMarginTracker), nameof(scrMarginTracker.AddHit))]
	public static class scrMarginTracker_AddHit
	{
		public static void Prefix(HitMargin hit)
		{
			AddJudgement(
				scrController.instance.playerOne.midspinInfiniteMargin
					? -1
					: hit switch
					{
						HitMargin.Multipress => -1,
						HitMargin.OverPress => -3,
						HitMargin.FailMiss => -2,
						HitMargin.FailOverload => -3,
						HitMargin.TooEarly => -4,
						HitMargin.TooLate => -4,
						_ => TileScore
					}
			);
			CacheJudgementResults();
		}
	}

	[HarmonyPatch(typeof(scrMarginTracker), nameof(scrMarginTracker.RevertToLastCheckpoint))]
	public static class scrMarginTracker_RevertToLastCheckpoint
	{
		public static void Postfix(scrMarginTracker __instance)
		{
			var leave = __instance.hitMargins.Count;
			AccumulatedScores.RemoveRange(leave, Judgements.Count - leave);
			AccumulatedTiles.RemoveRange(leave, Judgements.Count - leave);
			Judgements.RemoveRange(leave, Judgements.Count - leave);
			CacheJudgementResults();
		}
	}
}
