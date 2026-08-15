using System;
using System.IO;
using NotEnoughAccuracy.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using Object = UnityEngine.Object;

namespace NotEnoughAccuracy;

public static class InGameRenderer
{
	private static GameObject Root { get; } = new("NotEnoughAccuracyRoot", typeof(RectTransform));

	private static Font Font { get; } = new(Path.Combine(Mod.ModEntry.Path, "Fonts", "ingame.ttf"));

	private static TMP_FontAsset FontAsset { get; } = TMP_FontAsset.CreateFontAsset(
		Font,
		128,
		9,
		GlyphRenderMode.SDFAA,
		1024,
		1024
	);

	private static GameObject TextLayer { get; } = new("NotEnoughAccuracyTextLayer", typeof(RectTransform));

	private static Canvas Canvas { get; } = Root.AddComponent<Canvas>();

	private static TextMeshProUGUI ScoreText { get; } = TextLayer.AddComponent<TextMeshProUGUI>();

	public static long Score
	{
		set => ScoreText.text = $"{value / 10000m:0.0000}%";
	}

	public static double FontSize
	{
		set => ScoreText.fontSize = Math.Max((float)value, 1);
	}

	static InGameRenderer()
	{
		Object.DontDestroyOnLoad(Root);
		var rect = Root.GetComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		Canvas.overrideSorting = true;
		Canvas.sortingOrder = 2147483647;
		TextLayer.transform.SetParent(Root.transform, false);
		ScoreText.font = FontAsset;
		ScoreText.text = "0";
		ScoreText.fontSize = 96;
		ScoreText.outlineWidth = 0.25F;
		ScoreText.outlineColor = Color.black;
		ScoreText.color = Color.white;
		ScoreText.raycastTarget = false;
		ScoreText.alignment = TextAlignmentOptions.BottomLeft;
		ScoreText.autoSizeTextContainer = false;
		var rt = ScoreText.rectTransform;
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.zero;
		rt.pivot = Vector2.zero;
		rt.anchoredPosition = Vector2.zero;
		rt.sizeDelta = new Vector2(1920, 1080);
		var mat = ScoreText.fontMaterial;
		mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.25F);
		mat.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
		mat.EnableKeyword(ShaderUtilities.Keyword_Outline);
		ScoreText.UpdateMeshPadding();
		ScoreText.SetMaterialDirty();
	}

	public static unit OnUpdate()
	{
		var ctl = scrController.instance;
		var show = Mod.Enabled && Mod.Settings.DisplayInGame && ctl is { gameworld: true, paused: false };
		ScoreText.gameObject.SetActive(show);
		return [];
	}
}
