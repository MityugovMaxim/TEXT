using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BitmapLine : IEnumerable<BitmapCharacter>
{
	public BitmapCharacter this[int _Index]
	{
		get { return m_Characters[_Index]; }
	}

	public int Count
	{
		get { return m_Characters.Count; }
	}

	public bool IsEmpty
	{
		get { return m_Characters.Count == 0 || m_Characters.All(_Character => !_Character.Enabled); }
	}

	readonly List<BitmapCharacter> m_Characters;

	public BitmapLine()
	{
		m_Characters = new List<BitmapCharacter>();
	}

	public BitmapLine(int _Capacity)
	{
		m_Characters = new List<BitmapCharacter>(_Capacity);
	}

	public BitmapLine(IEnumerable<BitmapCharacter> _Collection)
	{
		m_Characters = new List<BitmapCharacter>(_Collection);
	}

	public void Add(BitmapCharacter _Character)
	{
		m_Characters.Add(_Character);
	}

	public void AddRange(IEnumerable<BitmapCharacter> _Collection)
	{
		m_Characters.AddRange(_Collection);
	}

	public void Remove(BitmapCharacter _Character)
	{
		m_Characters.Remove(_Character);
	}

	public void RemoveAt(int _Index)
	{
		m_Characters.RemoveAt(_Index);
	}

	public void RemoveAll(Predicate<BitmapCharacter> _Match)
	{
		m_Characters.RemoveAll(_Match);
	}

	public void RemoveRange(int _Index, int _Count)
	{
		m_Characters.RemoveRange(_Index, _Count);
	}

	public void Clear()
	{
		m_Characters.Clear();
	}

	public void Trim()
	{
		TrimStart();
		
		TrimEnd();
	}

	public void TrimStart()
	{
		for (int i = 0; i < m_Characters.Count; i++)
		{
			BitmapCharacter character = m_Characters[i];
			
			if (!character.Enabled)
				continue;
			
			if (character.Character == ' ')
				character.Enabled = false;
			else
				break;
		}
	}

	public void TrimEnd()
	{
		for (int i = m_Characters.Count - 1; i >= 0; i--)
		{
			BitmapCharacter character = m_Characters[i];
			
			if (!character.Enabled)
				continue;
			
			if (character.Character == ' ')
				character.Enabled = false;
			else
				break;
		}
	}

	public IEnumerable<BitmapCharacter> GetSpaces()
	{
		foreach (BitmapCharacter character in m_Characters)
		{
			if (!character.Enabled)
				continue;
			
			if (character.Character == ' ')
				yield return character;
		}
	}

	public IEnumerable<BitmapCharacter[]> GetWords()
	{
		List<BitmapCharacter> word = new List<BitmapCharacter>();
		foreach (BitmapCharacter character in m_Characters)
		{
			if (!character.Enabled)
				continue;
			
			if (character.Character == ' ')
			{
				yield return word.ToArray();
				
				yield return new BitmapCharacter[] { character };
				
				word.Clear();
				
				continue;
			}
			word.Add(character);
		}
		yield return word.ToArray();
	}

	public IEnumerator<BitmapCharacter> GetEnumerator()
	{
		return m_Characters.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}