using UnityEngine;

[Tag("blink")]
public class BitmapTextBlinkAnimation : BitmapTextAnimation
{
	readonly float m_Speed;
	readonly Color m_Source;
	readonly Color m_Target;

	float m_Phase;

	public BitmapTextBlinkAnimation(BitmapText _BitmapText, TagText.Node _Node) : base(_BitmapText, _Node)
	{
		m_Speed  = _Node.GetFloat("speed", 1);
		m_Source = _Node.GetColor("source", new Color(1, 1, 1, 0.25f));
		m_Target = _Node.GetColor("target", new Color(1, 1, 1, 1));
	}

	public override void Process()
	{
		m_Phase += Time.deltaTime * m_Speed;
		
		float phase = Mathf.PingPong(m_Phase, 1);
		
		foreach (BitmapCharacter character in GetCharacters())
			character.Tint *= Color.Lerp(m_Source, m_Target, phase);
		
		Rebuild();
	}

	public override void Restore()
	{
		foreach (BitmapCharacter character in GetCharacters())
			character.Tint = Color.white;
	}
}