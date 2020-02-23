using UnityEngine;
using System.Collections.Generic;

public abstract class BitmapTextProcessor
{
	protected BitmapText BitmapText
	{
		get { return m_BitmapText; }
	}

	protected List<BitmapLine> Lines
	{
		get { return BitmapText.Lines; }
	}

	protected List<BitmapCharacter> Characters
	{
		get { return BitmapText.Characters; }
	}

	protected BitmapFont Font
	{
		get { return BitmapText.Font; }
	}

	protected Alignment Alignment
	{
		get { return BitmapText.Alignment; }
	}

	protected Rect Rect
	{
		get { return BitmapText.GetPixelAdjustedRect(); }
	}

	protected float LineHeight
	{
		get { return (Font.Ascender + Font.Descender) * CharSize; }
	}

	protected float LineSpacing
	{
		get { return (Font.LineGap + BitmapText.LineSpacing) * CharSize; }
	}

	protected float CharSpacing
	{
		get { return (Font.Kerning + BitmapText.CharSpacing) * CharSize; }
	}

	protected float CharSize
	{
		get { return BitmapText.CharSize * BitmapText.Scale; }
	}

	protected float MinSize
	{
		get { return BitmapText.MinSize; }
	}

	protected float MaxSize
	{
		get { return BitmapText.MaxSize; }
	}

	readonly BitmapText m_BitmapText;

	protected BitmapTextProcessor(BitmapText _BitmapText)
	{
		m_BitmapText = _BitmapText;
	}

	public abstract void Process();

	protected float CalcTotalWidth()
	{
		float width = 0;
		foreach (BitmapLine line in Lines)
			width = Mathf.Max(width, CalcWidth(line));
		return width;
	}

	protected float CalcTotalHeight()
	{
		return Lines.Count * LineHeight + Mathf.Max(0, Lines.Count - 1) * LineSpacing;
	}

	protected float CalcWidth(BitmapLine _Line)
	{
		if (_Line == null || _Line.Count == 0)
			return 0;
		
		int   count = 0;
		float width = 0;
		foreach (BitmapCharacter character in _Line)
		{
			if (!character.Enabled)
				continue;
			
			width += character.Rect.width;
			
			count++;
		}
		return width + Mathf.Max(0, count - 1) * CharSpacing;
	}

	protected float CalcWidth(BitmapCharacter[] _Characters)
	{
		if (_Characters == null || _Characters.Length == 0)
			return 0;
		
		int   count = 0;
		float width = 0;
		foreach (BitmapCharacter character in _Characters)
		{
			if (!character.Enabled)
				continue;
			
			width += character.Rect.width;
			
			count++;
		}
		return width + Mathf.Max(0, count - 1) * CharSpacing;
	}
}