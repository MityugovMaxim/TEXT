using UnityEngine;
using System.Collections.Generic;

public abstract class BitmapTextAnimation
{
	protected Rect Rect
	{
		get { return m_BitmapText.GetPixelAdjustedRect(); }
	}

	public List<BitmapCharacter> Characters
	{
		get { return m_Characters; }
	}

	protected float CharSize
	{
		get { return m_BitmapText.CharSize * m_BitmapText.Scale; }
	}

	readonly BitmapText            m_BitmapText;
	readonly List<BitmapCharacter> m_Characters;

	public BitmapTextAnimation(BitmapText _BitmapText, TagText.Node _Node)
	{
		m_BitmapText = _BitmapText;
		m_Characters = new List<BitmapCharacter>();
		
		if (_Node == null)
			return;
		
		int sourceIndex = _Node.Index;
		int targetIndex = _Node.Index + _Node.Length;
		
		foreach (BitmapCharacter character in m_BitmapText.Characters)
		{
			if (character.Enabled && character.Visible && character.Index >= sourceIndex && character.Index <= targetIndex)
				m_Characters.Add(character);
		}
	}

	public abstract void Process();

	public abstract void Restore();

	protected void Rebuild()
	{
		if (m_BitmapText != null)
			m_BitmapText.SetVerticesDirty();
	}

	protected float GetPhaseBySeed(BitmapCharacter _Character, int _Seed, float _Phase)
	{
		if (_Character == null)
			return _Phase;
		
		Random.InitState(_Character.Index + _Seed);
		
		return _Phase - Random.value;
	}

	protected float GetPhaseByPosition(BitmapCharacter _Character, float _Size, float _Phase)
	{
		if (_Character == null)
			return _Phase;
		
		Rect rect = Rect;
		
		return _Phase - (_Character.Rect.center.x - rect.x) / (_Size * CharSize);
	}
}
