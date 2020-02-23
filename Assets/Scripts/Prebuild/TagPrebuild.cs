using System;
using System.Collections.Generic;

public static class TagPrebuild
{
	static readonly Dictionary<string, Type> m_TagTypes = new Dictionary<string, Type>()
	{
		{ "blink", typeof(BitmapTextBlinkAnimation) },
		{ "rainbow", typeof(BitmapTextRainbowAnimation) },
		{ "shake", typeof(BitmapTextShakeAnimation) },
		{ "show", typeof(BitmapTextShowAnimation) },
		{ "wave", typeof(BitmapTextWaveAnimation) },
	};

	public static Type GetTagType(string _Tag)
	{
		return m_TagTypes.ContainsKey(_Tag) ? m_TagTypes[_Tag] : null;
	}
}
