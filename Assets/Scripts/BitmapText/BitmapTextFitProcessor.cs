using UnityEngine;

public class BitmapTextFitProcessor : BitmapTextProcessor
{
	public BitmapTextFitProcessor(BitmapText _BitmapText) : base(_BitmapText) { }

	public override void Process()
	{
		Rect  rect        = Rect;
		float totalWidth  = CalcTotalWidth();
		float totalHeight = CalcTotalHeight();
		
		float minScale = MinSize > 0 ? MinSize / BitmapText.CharSize : 0;
		float maxScale = MaxSize > 0 ? MaxSize / BitmapText.CharSize : float.MaxValue;
		
		float scale = Mathf.Min(
			rect.width / totalWidth,
			rect.height / totalHeight
		);
		
		scale = Mathf.Clamp(scale, minScale, maxScale);
		
		BitmapText.Scale = scale;
		
		foreach (BitmapCharacter character in Characters)
		{
			character.Rect           =  character.Rect.Scale(scale);
			character.LineHeight     *= scale;
			character.BaselineOffset *= scale;
		}
	}
}