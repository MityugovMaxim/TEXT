using UnityEngine;

[Tag("rainbow")]
public class BitmapTextRainbowAnimation : BitmapTextAnimation
{
	readonly float    m_Speed;
	readonly Gradient m_Gradient;

	float m_Phase;

	public BitmapTextRainbowAnimation(BitmapText _BitmapText, TagText.Node _Node) : base(_BitmapText, _Node)
	{
		const float step = 1.0f / 7.0f;
		
		m_Speed = _Node.GetFloat("speed", 1);
		m_Gradient = new Gradient();
		m_Gradient.alphaKeys = new GradientAlphaKey[]
		{
			new GradientAlphaKey(1, 0), 
			new GradientAlphaKey(1, 1), 
		};
		m_Gradient.colorKeys = new GradientColorKey[]
		{
			new GradientColorKey(new Color(1.0f, 0.0f, 0.0f), step * 0),
			new GradientColorKey(new Color(1.0f, 0.5f, 0.0f), step * 1),
			new GradientColorKey(new Color(1.0f, 1.0f, 0.0f), step * 2),
			new GradientColorKey(new Color(0.0f, 1.0f, 0.0f), step * 3),
			new GradientColorKey(new Color(0.0f, 0.5f, 1.0f), step * 4),
			new GradientColorKey(new Color(0.0f, 0.0f, 1.0f), step * 5),
			new GradientColorKey(new Color(1.0f, 0.0f, 1.0f), step * 6),
			new GradientColorKey(new Color(1.0f, 0.0f, 0.0f), step * 7),
		};
	}

	public override void Process()
	{
		m_Phase = Mathf.Repeat(m_Phase + Time.deltaTime * m_Speed, 1);
		
		foreach (BitmapCharacter character in Characters)
			character.Tint *= m_Gradient.Evaluate(m_Phase);
		
		Rebuild();
	}

	public override void Restore()
	{
		foreach (BitmapCharacter character in Characters)
			character.Tint = Color.white;
	}
}
