using UnityEngine;

public class BitmapTextRender : BitmapTextProcessor
{
	public BitmapTextRender(BitmapText _BitmapText) : base(_BitmapText) { }

	public override void Process()
	{
		Rect  rect        = Rect;
		float totalWidth  = CalcWidth(Lines);
		float totalHeight = CalcHeight(Lines);
		
		Vector2 pivot    = Alignment.GetPivot();
		Vector2 position = new Vector2(0, totalHeight);
		
		float descender = Font.Descender * CharSize;
		
		int index = 0;
		foreach (string line in Lines)
		{
			float lineWidth  = CalcWidth(line);
			float lineHeight = CalcHeight(line);
			
			foreach (char character in line)
			{
				float           characterWidth  = Font.CalcWidth(character) * CharSize;
				float           characterHeight = Font.CalcHeight(character) * CharSize;
				BitmapCharacter bitmapCharacter = Characters[index++];
				BitmapGlyph     bitmapGlyph     = Font.GetGlyph(character);
				if (bitmapCharacter.Enabled && bitmapGlyph != null)
				{
					float offset = lineHeight - descender - bitmapGlyph.Offset * CharSize;
					
					Rect characterRect = new Rect(
						rect.x + (rect.width - lineWidth) * pivot.x + position.x,
						rect.y + (rect.height - totalHeight) * pivot.y + position.y - offset,
						characterWidth,
						characterHeight
					);
					
					Rect characterUV = bitmapGlyph.UV;
					
					bitmapCharacter.Rect  = characterRect;
					bitmapCharacter.UV    = characterUV;
					bitmapCharacter.Color = Color;
				}
				
				position.x += characterWidth + CharSpacing * CharSize;
			}
			position.x =  0;
			position.y -= lineHeight + LineGap * CharSize;
		}
	}
}