namespace NotEnoughAccuracy.Utils;

using static Yui;

public static class YuiPreset
{
	public static void OptionNameDescription(
		string name,
		bool description
	)
	{
		if (description)
		{
			using var _ = Div(V, options: WidthMin);
			Text(Lang.Translate(name), options: WidthMin);
			Text(Lang.Translate($"{name}.Description"), TextStyle.Secondary, WidthMin);
		}
		else
		{
			Text(Lang.Translate(name), options: WidthMin);
		}
	}

	public static void SwitchOption(
		Sizes sizes,
		ref bool option,
		string name,
		bool description = false,
		bool save = true
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		OptionNameDescription(name, description);
		Fill();
		var result = Switch(ref option);
		if (save) Save |= result;
	}

	public static double? DoubleOption(
		Sizes sizes,
		ref double option,
		string name,
		IStructFormat<double>? format = null,
		bool description = false,
		bool save = true
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		OptionNameDescription(name, description);
		Fill();
		var result = StructField(ref option, format ?? DoubleFormat(), WidthMin);
		if (save) Save |= result;
		return result;
	}

	public static void TextOption(
		Sizes sizes,
		ref string option,
		string name,
		bool description = false,
		bool save = true
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		OptionNameDescription(name, description);
		Fill();
		var result = TextField(ref option, options: WidthMin);
		if (save) Save |= result;
	}

	public static void CheckboxTextOption(
		Sizes sizes,
		ref bool enabled,
		ref string option,
		string name,
		bool description = false,
		bool save = true
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		object? result = null;
		result ??= Checkbox(ref enabled);
		OptionNameDescription(name, description);
		Fill();
		result ??= TextField(ref option, options: WidthMin);
		if (save) Save |= result;
	}

	public static void CheckboxSwitchOption(
		Sizes sizes,
		ref bool enabled,
		ref bool option,
		string name,
		bool description = false,
		bool save = true
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		object? result = null;
		result ??= Checkbox(ref enabled);
		OptionNameDescription(name, description);
		Fill();
		result ??= Switch(ref option, WidthMin);
		if (save) Save |= result;
	}

	public static void CheckboxDoubleOption(
		Sizes sizes,
		ref bool enabled,
		ref double option,
		string name,
		bool description = false,
		IStructFormat<double>? format = null,
		bool save = true
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		object? result = null;
		result ??= Checkbox(ref enabled);
		OptionNameDescription(name, description);
		Fill();
		result ??= StructField(ref option, format ?? DoubleFormat(), WidthMin);
		if (save) Save |= result;
	}

	public static void CheckboxIntOption(
		Sizes sizes,
		ref bool enabled,
		ref int option,
		string name,
		bool description = false,
		IStructFormat<int>? format = null,
		bool save = true
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		object? result = null;
		result ??= Checkbox(ref enabled);
		OptionNameDescription(name, description);
		Fill();
		result ??= StructField(ref option, format ?? IntFormat(), WidthMin);
		if (save) Save |= result;
	}

	public static bool IconText(
		Sizes sizes,
		IconStyle icon,
		string text
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		var result = false;
		result |= Icon(icon);
		result |= Text(Lang.Translate(text), options: WidthMax);
		return result;
	}

	public static bool Collapse(
		Sizes sizes,
		ref bool expanded,
		string text,
		TextStyle style = TextStyle.Normal
	)
	{
		using var _ = Div(H, sizes: sizes, options: WidthMax) + Align(0.5);
		if (ArrowButton(expanded ? ArrowStyle.Down : ArrowStyle.Right)) expanded = !expanded;
		Text(Lang.Translate(text), style, WidthMax);
		return expanded;
	}
}
