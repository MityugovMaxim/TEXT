using UnityEngine;
using System.Collections.Generic;

public abstract class BitmapTextProcessor
{
	protected static readonly char[] TrimChars = { ' ', '\t' };

	protected BitmapText BitmapText
	{
		get { return m_BitmapText; }
	}

	protected Color Color
	{
		get { return BitmapText.color; }
	}

	protected List<string> Lines
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

	protected Vector2 Pivot
	{
		get { return BitmapText.rectTransform.pivot; }
	}

	protected float LineGap
	{
		get { return Font.LineGap + BitmapText.LineSpacing; }
	}

	protected float CharSpacing
	{
		get { return Font.Kerning + BitmapText.CharSpacing; }
	}

	protected float CharSize
	{
		get { return BitmapText.CharSize * BitmapText.Scale; }
	}

	protected float Scale
	{
		get { return BitmapText.Scale; }
		set { BitmapText.Scale = value; }
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

	public BitmapTextProcessor(BitmapText _BitmapText)
	{
		m_BitmapText = _BitmapText;
	}

	public abstract void Process();

	protected float CalcWidth(ICollection<string> _Lines)
	{
		float width = 0;
		if (_Lines != null)
		{
			foreach (string line in _Lines)
				width = Mathf.Max(width, CalcWidth(line));
		}
		return width;
	}

	protected float CalcHeight(ICollection<string> _Lines)
	{
		float height = 0;
		int count = 0;
		if (_Lines != null)
		{
			foreach (string line in _Lines)
			{
				height += CalcHeight(line);
				
				count++;
			}
		}
		return height + LineGap * CharSize * Mathf.Max(0, count - 1);
	}

	protected float CalcWidth(string _Text)
	{
		float width = 0;
		if (!string.IsNullOrEmpty(_Text))
		{
			foreach (char character in _Text)
			{
				if (character == '\n')
					continue;
				
				width += Font.CalcWidth(character) + CharSpacing;
			}
			width = Mathf.Max(0, width - CharSpacing);
		}
		return width * CharSize;
	}

	protected float CalcHeight(string _Text)
	{
		return (Font.Ascender + Font.Descender) * CharSize;
	}

	protected void Rebuild()
	{
		m_BitmapText.SetVerticesDirty();
	}

	protected void Repaint()
	{
		m_BitmapText.SetVerticesDirty();
	}
}