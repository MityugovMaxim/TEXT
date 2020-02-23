using UnityEngine;
using System.Collections.Generic;

public abstract class BitmapTextAnimation
{
	protected Rect Rect
	{
		get { return m_BitmapText.GetPixelAdjustedRect(); }
	}

	protected float CharSize
	{
		get { return m_BitmapText.CharSize * m_BitmapText.Scale; }
	}

	readonly BitmapText m_BitmapText;
	readonly int        m_Index;
	readonly int        m_Length;

	public BitmapTextAnimation(BitmapText _BitmapText, TagText.Node _Node)
	{
		m_BitmapText = _BitmapText;
		
		if (_Node == null)
			return;
		
		m_Index  = _Node.Index;
		m_Length = _Node.Length;
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
		
		return _Phase - (_Character.Rect.center.x - rect.x) / _Size;
	}

	protected IEnumerable<BitmapCharacter> GetCharacters()
	{
		if (m_BitmapText == null || m_BitmapText.Characters == null)
			yield break;
		
		int index = Mathf.Max(m_Index, 0);
		for (int i = 0; i < m_Length; i++)
		{
			if (index + i < m_BitmapText.Characters.Count)
				yield return m_BitmapText.Characters[index + i];
		}
	}
}
