using UnityEngine;
using UnityEngine.UI;

public class BitmapTextClipProcessor : BitmapTextProcessor
{
	public override void Process()
	{
		Rect rect = Rect;
		foreach (BitmapCharacter character in Characters)
		{
			Rect characterUV   = character.UV;
			Rect characterRect = character.RealRect;
			
			// Left clip
			if (characterRect.xMin < rect.xMin && characterRect.xMax > rect.xMin)
			{
				float delta = Mathf.Abs(rect.xMin - characterRect.xMin);
				float phase = delta / characterRect.width;
				characterUV = new Rect(
					characterUV.x + characterUV.width * phase,
					characterUV.y,
					characterUV.width * (1 - phase),
					characterUV.height
				);
				characterRect = new Rect(
					characterRect.x + delta,
					characterRect.y,
					characterRect.width - delta,
					characterRect.height
				);
			}
			
			// Right clip
			if (characterRect.xMin < rect.xMax && characterRect.xMax > rect.xMax)
			{
				float delta = Mathf.Abs(characterRect.xMax - rect.xMax);
				float phase = delta / characterRect.width;
				characterUV = new Rect(
					characterUV.x,
					characterUV.y,
					characterUV.width * (1 - phase),
					characterUV.height
				);
				characterRect = new Rect(
					characterRect.x,
					characterRect.y,
					characterRect.width - delta,
					characterRect.height
				);
			}
			
			// Bottom clip
			if (characterRect.yMin < rect.yMin && characterRect.yMax > rect.yMin)
			{
				float delta = Mathf.Abs(rect.yMin - characterRect.yMin);
				float phase = delta / characterRect.height;
				characterUV = new Rect(
					characterUV.x,
					characterUV.y + characterUV.height * phase,
					characterUV.width,
					characterUV.height * (1 - phase)
				);
				characterRect = new Rect(
					characterRect.x,
					characterRect.y + delta,
					characterRect.width,
					characterRect.height - delta
				);
			}
			
			// Top clip
			if (characterRect.yMax > rect.yMax && characterRect.yMin < rect.yMax)
			{
				float delta = Mathf.Abs(characterRect.yMax - rect.yMax);
				float phase = delta / characterRect.height;
				characterUV = new Rect(
					characterUV.x,
					characterUV.y,
					characterUV.width,
					characterUV.height * (1 - phase)
				);
				characterRect = new Rect(
					characterRect.x,
					characterRect.y,
					characterRect.width,
					characterRect.height - delta
				);
			}
			
			character.UV       = characterUV;
			character.RealRect = characterRect;
			
			character.Enabled &= rect.Overlaps(characterRect);
		}
	}

	public BitmapTextClipProcessor(BitmapText _BitmapText) : base(_BitmapText) { }
}