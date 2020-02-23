using UnityEngine;

[Tag("wave")]
public class BitmapTextWaveAnimation : BitmapTextAnimation
{
	readonly float m_Speed;
	readonly float m_Size;
	readonly float m_Amplitude;

	float m_Phase;

	public BitmapTextWaveAnimation(BitmapText _BitmapText, TagText.Node _Node) : base(_BitmapText, _Node)
	{
		m_Speed     = _Node.GetFloat("speed", 1);
		m_Size      = _Node.GetFloat("size", 150);
		m_Amplitude = _Node.GetFloat("amplitude", 2);
	}

	public override void Process()
	{
		m_Phase += Time.deltaTime * m_Speed;
		
		foreach (BitmapCharacter character in Characters)
		{
			float phase = GetPhaseByPosition(character, m_Size, m_Phase);
			
			character.Offset += Evaluate(phase) * m_Amplitude * CharSize * Vector2.up;
		}
		
		Rebuild();
	}

	public override void Restore()
	{
		foreach (BitmapCharacter character in Characters)
			character.Offset = Vector2.zero;
	}

	float Evaluate(float _Phase)
	{
		return Mathf.Sin(_Phase * Mathf.PI * 2);
	}
}