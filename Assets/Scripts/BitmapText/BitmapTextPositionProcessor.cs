public class BitmapTextPositionProcessor : BitmapTextProcessor
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	readonly Direction m_Direction;

	public BitmapTextPositionProcessor(BitmapText _BitmapText, Direction _Direction) : base(_BitmapText)
	{
		m_Direction = _Direction;
	}

	public override void Process()
	{
		switch (m_Direction)
		{
			case Direction.Horizontal:
				PositionHorizontal();
				break;
			
			case Direction.Vertical:
				PositionVertical();
				break;
		}
	}

	void PositionHorizontal()
	{
		float kerning = CharSpacing;
		
		foreach (BitmapLine line in Lines)
		{
			float position = 0;
			foreach (BitmapCharacter character in line)
			{
				character.Rect.x = position;
				if (character.Enabled)
					position += character.Rect.width + kerning;
			}
		}
	}

	void PositionVertical()
	{
		float lineHeight  = LineHeight;
		float lineSpacing = LineSpacing;
		
		float position = 0;
		foreach (BitmapLine line in Lines)
		{
			foreach (BitmapCharacter character in line)
				character.Rect.y = position;
			position -= lineHeight + lineSpacing;
		}
	}
}