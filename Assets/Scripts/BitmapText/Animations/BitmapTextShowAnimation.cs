using UnityEngine;

[Tag("show")]
public class BitmapTextShowAnimation : BitmapTextAnimation
{
	readonly float m_Speed;
	readonly float m_Size;

	float m_Phase;

	public BitmapTextShowAnimation(BitmapText _BitmapText, TagText.Node _Node) : base(_BitmapText, _Node)
	{
		m_Speed = _Node.GetFloat("speed", 1);
		m_Size  = _Node.GetFloat("size", 3);
	}

	public override void Process()
	{
		m_Phase += Time.deltaTime * m_Speed * 5;
		
		Vector2 offset = new Vector2(2.5f, 5) * CharSize;
		float   step   = 1.0f / m_Size;
		
		foreach (BitmapCharacter character in Characters)
		{
			float phase = m_Phase - character.Index * step;
			
			character.Tint *= new Color(1, 1, 1, phase);
			character.Offset += Vector2.Lerp(offset, Vector2.zero, phase);
		}
		
		Rebuild();
	}

	public override void Restore()
	{
		foreach (BitmapCharacter character in Characters)
		{
			character.Tint   = Color.white;
			character.Offset = Vector2.zero;
		}
	}

	float Evaluate(float _Phase)
	{
		return Mathf.Sin(_Phase * Mathf.PI * 2);
	}
}