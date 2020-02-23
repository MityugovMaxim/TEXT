using System.Collections.Generic;
using UnityEngine;

public class BitmapTextFit : BitmapTextProcessor
{
	public override void Process()
	{
		Scale = 1;
		
		
		Rect  rect        = Rect;
		float totalWidth  = CalcWidth(Lines);
		float totalHeight = CalcHeight(Lines);
		
		float scale = Mathf.Min(
			rect.width / totalWidth,
			rect.height / totalHeight
		);
		float minScale = MinSize > 0 ? MinSize / CharSize : 0;
		float maxScale = MaxSize > 0 ? MaxSize / CharSize : float.MaxValue;
		
		Scale = Mathf.Clamp(scale, minScale, maxScale);
	}

	public BitmapTextFit(BitmapText _BitmapText) : base(_BitmapText) { }
}