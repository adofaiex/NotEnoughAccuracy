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

	public static int Anchor
	{
		set
		{
			var (align, anchor) = value switch
			{
				0 => (TextAlignmentOptions.TopLeft, new Vector2(0.0F, 1.0F)),
				1 => (TextAlignmentOptions.Top, new Vector2(0.5F, 1.0F)),
				2 => (TextAlignmentOptions.TopRight, new Vector2(1.0F, 1.0F)),
				3 => (TextAlignmentOptions.Left, new Vector2(0.0F, 0.5F)),
				4 => (TextAlignmentOptions.Center, new Vector2(0.5F, 0.5F)),
				5 => (TextAlignmentOptions.Right, new Vector2(1.0F, 0.5F)),
				6 => (TextAlignmentOptions.BottomLeft, new Vector2(0.0F, 0.0F)),
				7 => (TextAlignmentOptions.Bottom, new Vector2(0.5F, 0.0F)),
				8 => (TextAlignmentOptions.BottomRight, new Vector2(1.0F, 0.0F)),
				_ => (TextAlignmentOptions.TopLeft, new Vector2(0.0F, 1.0F))
			};
			ScoreText.alignment = align;
			ScoreText.rectTransform.anchorMin = anchor;
			ScoreText.rectTransform.anchorMax = anchor;
			ScoreText.rectTransform.pivot = anchor;
		}
	}

	public static double OffsetX
	{
		set => ScoreText.rectTransform.anchoredPosition =
			new Vector2((float)value, ScoreText.rectTransform.anchoredPosition.y);
	}

	public static double OffsetY
	{
		set => ScoreText.rectTransform.anchoredPosition =
			new Vector2(ScoreText.rectTransform.anchoredPosition.x, -(float)value);
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
		FontSize = Mod.Settings.InGameFontSize;
		ScoreText.outlineWidth = 0.25F;
		ScoreText.outlineColor = Color.black;
		ScoreText.color = Color.white;
		ScoreText.raycastTarget = false;
		ScoreText.alignment = TextAlignmentOptions.BottomLeft;
		ScoreText.autoSizeTextContainer = false;
		Anchor = Mod.Settings.InGameAnchor;
		OffsetX = Mod.Settings.InGameOffsetX;
		OffsetY = Mod.Settings.InGameOffsetY;
		ScoreText.rectTransform.sizeDelta = new Vector2(1920, 1080);
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
