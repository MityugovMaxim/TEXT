using UnityEngine;

[Tag("shake")]
public class BitmapTextShakeAnimation : BitmapTextAnimation
{
	const int DIRECTIONS_LENGTH = 32;

	readonly float m_Speed;
	readonly float m_Size;
	readonly float m_Amplitude;

	Vector2[] m_Directions;
	float     m_Phase;

	public BitmapTextShakeAnimation(BitmapText _BitmapText, TagText.Node _Node) : base(_BitmapText, _Node)
	{
		m_Speed     = _Node.GetFloat("speed", 1);
		m_Size      = _Node.GetFloat("size", 200);
		m_Amplitude = _Node.GetFloat("amplitude", 2);
		
		m_Directions = new Vector2[DIRECTIONS_LENGTH];
		for (int i = 0; i < m_Directions.Length; i++)
			m_Directions[i] = Random.insideUnitCircle;
	}

	public override void Process()
	{
		m_Phase += Time.deltaTime * m_Speed;
		
		foreach (BitmapCharacter character in Characters)
		{
			float phase = Mathf.Repeat(GetPhaseBySeed(character, 128812048, m_Phase), 1);
			
			character.Offset += m_Amplitude * CharSize * Utility.Lerp(m_Directions, phase);
		}
		
		Rebuild();
	}

	public override void Restore()
	{
		foreach (BitmapCharacter character in Characters)
			character.Offset = Vector2.zero;
	}
}