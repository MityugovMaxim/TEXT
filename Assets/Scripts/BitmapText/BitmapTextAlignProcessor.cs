using UnityEngine;

public class BitmapTextAlignProcessor : BitmapTextProcessor
{
	public BitmapTextAlignProcessor(BitmapText _BitmapText) : base(_BitmapText) { }

	public override void Process()
	{
		Rect rect = Rect;
		
		rect = new Rect(
			rect.x,
			rect.y + rect.height,
			rect.width,
			rect.height
		);
		
		Vector2 pivot = Alignment.GetPivot();
		
		float totalHeight = CalcTotalHeight();
		
		foreach (BitmapLine line in Lines)
		{
			float lineWidth = CalcWidth(line);
			foreach (BitmapCharacter character in line)
			{
				Rect characterRect = character.Rect;
				
				// Horizontal
				characterRect.x += rect.x + (rect.width - lineWidth) * pivot.x;
				
				// Vertical
				characterRect.y += rect.y - (rect.height - totalHeight) * pivot.y;
				
				character.Rect = characterRect;
			}
		}
	}
}