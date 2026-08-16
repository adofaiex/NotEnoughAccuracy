using System.Collections.Generic;
using System.IO;

namespace NotEnoughAccuracy.ReplayCompatibility;

public static class ReplayStoragePluginV1
{
	private const int Version = 1;

	private static Dictionary<string, ReplayerContext> Replayers { get; } = [];

	internal static ReplayerContext? Replaying { get; private set; }

	internal static int ReplayingOffset { get; set; }

	public static string GetNamespace(string replayer)
	{
		return "NotEnoughAccuracyV1";
	}

	private static bool EnsureEnabled()
	{
		if (Mod.Enabled) return true;
		Replaying = null;
		Replayers.Clear();
		return false;
	}

	private static ReplayerContext GetContext(string replayer)
	{
		if (Replayers.TryGetValue(replayer, out var context)) return context;
		return Replayers[replayer] = new ReplayerContext();
	}

	public static void OnStartRecording(string replayer, int tileId)
	{
		if (!EnsureEnabled()) return;
		var ctx = GetContext(replayer);
		ctx.TileId = tileId;
		ctx.Judgments.Clear();
		ctx.TileIds.Clear();
		ctx.CurrentMode = ReplayerContext.Mode.Record;
		Mod.Info.Log($"[{replayer}] started recording replay");
	}

	public static byte[]? OnStopRecording(string replayer)
	{
		if (!EnsureEnabled()) return null;
		var ctx = GetContext(replayer);
		if (ctx.CurrentMode != ReplayerContext.Mode.Record) return null;
		ctx.CurrentMode = ReplayerContext.Mode.None;
		using var stream = new MemoryStream();
		var writer = new BinaryWriter(stream);
		writer.Write(Version);
		writer.Write(ctx.TileId);
		writer.Write(Api.Judgments.Count);

		foreach (var judgment in Patches.Judgment)
			writer.Write((sbyte)judgment);

		var baseTileId = ctx.TileId;
		foreach (var tileId in Patches.TileIds)
		{
			writer.Write((sbyte)(tileId - baseTileId));
			baseTileId = tileId;
		}

		writer.Close();
		Mod.Info.Log($"[{replayer}] stopped recording replay");
		return stream.ToArray();
	}

	public static void OnLoadReplay(string replayer, byte[]? data)
	{
		if (!EnsureEnabled()) return;
		var ctx = GetContext(replayer);
		ctx.Judgments.Clear();
		ctx.TileIds.Clear();
		if (data is null)
		{
			ctx.CurrentMode = ReplayerContext.Mode.None;
			return;
		}

		using var stream = new MemoryStream(data);
		var reader = new BinaryReader(stream);
		if (reader.ReadInt32() != Version)
			throw new InvalidDataException("invalid NEA replay version");
		ctx.TileId = reader.ReadInt32();
		var count = reader.ReadInt32();

		for (var i = 0; i < count; i++)
			ctx.Judgments.Add(reader.ReadSByte());

		var currTileId = ctx.TileId;
		for (var i = 0; i < count; i++)
		{
			currTileId += reader.ReadSByte();
			ctx.TileIds.Add(currTileId);
		}

		ctx.CurrentMode = ReplayerContext.Mode.LoadedReplay;
		Mod.Info.Log($"[{replayer}] loaded replay");
	}

	public static void OnUnloadReplay(string replayer)
	{
		if (!EnsureEnabled()) return;
		var ctx = GetContext(replayer);
		ctx.CurrentMode = ReplayerContext.Mode.None;
		ctx.Judgments.Clear();
		ctx.TileIds.Clear();
		Mod.Info.Log($"[{replayer}] unloaded replay");
	}

	public static void OnStartReplaying(string replayer, int tileId)
	{
		if (!EnsureEnabled()) return;
		var ctx = GetContext(replayer);
		if (ctx.CurrentMode != ReplayerContext.Mode.LoadedReplay) return;
		ReplayingOffset = 0;
		while (ReplayingOffset < ctx.Judgments.Count && ctx.TileIds[ReplayingOffset] < tileId)
			++ReplayingOffset;
		ctx.CurrentMode = ReplayerContext.Mode.Replaying;
		Mod.Info.Log($"[{replayer}] started playing replay");
	}

	public static void OnStopReplaying(string replayer)
	{
		if (!EnsureEnabled()) return;
		var ctx = GetContext(replayer);
		ctx.CurrentMode = ctx.CurrentMode switch
		{
			ReplayerContext.Mode.Replaying => ReplayerContext.Mode.LoadedReplay,
			ReplayerContext.Mode.LoadedReplay => ReplayerContext.Mode.LoadedReplay,
			_ => ReplayerContext.Mode.None
		};
		Mod.Info.Log($"[{replayer}] stopped playing replay");
	}

	internal class ReplayerContext
	{
		public enum Mode
		{
			None,
			Record,
			LoadedReplay,
			Replaying
		}

		public Mode CurrentMode
		{
			get;
			set
			{
				Replaying = value == Mode.Replaying ? this : null;
				field = value;
			}
		} = Mode.None;

		public int TileId { get; set; }

		public List<long> Judgments { get; } = [];

		public List<int> TileIds { get; } = [];
	}
}
