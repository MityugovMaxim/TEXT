using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "Font", menuName = "BitmapText/Font")]
public class BitmapFont : ScriptableObject, IEnumerable<BitmapGlyph>
{
	const char MISSING_SYMBOL = '□';

	public float SpaceWidth
	{
		get { return m_SpaceWidth; }
	}

	public float Ascender
	{
		get { return m_Ascender; }
	}

	public float Descender
	{
		get { return m_Descender; }
	}

	public float CapHeight
	{
		get { return m_CapHeight; }
	}

	public float Kerning
	{
		get { return m_Kerning; }
	}

	public float LineGap
	{
		get { return m_LineGap; }
	}

	public Texture2D Texture
	{
		get { return m_Texture; }
	}

	[SerializeField] float m_SpaceWidth;
	[SerializeField] float m_Ascender;
	[SerializeField] float m_Descender;
	[SerializeField] float m_CapHeight;
	[SerializeField] float m_Kerning;
	[SerializeField] float m_LineGap;

	[SerializeField, HideInInspector] Texture2D         m_Texture;
	[SerializeField, HideInInspector] List<BitmapGlyph> m_Glyphs;

	[NonSerialized] Dictionary<char, BitmapGlyph> m_Cache;

	public static BitmapFont Create(int _Width, int _Height)
	{
		BitmapFont font = CreateInstance<BitmapFont>();
		font.m_Glyphs = new List<BitmapGlyph>();
		return font;
	}

	void Unpack()
	{
		m_Cache = new Dictionary<char, BitmapGlyph>();
		
		if (m_Glyphs == null)
			return;
		
		foreach (BitmapGlyph glyph in m_Glyphs)
		{
			if (glyph == null)
				continue;
				
			if (glyph.Name.Length == 1)
				m_Cache[glyph.Name[0]] = glyph;
				
			if (glyph.Aliases == null)
				continue;
				
			foreach (string alias in glyph.Aliases)
			{
				if (alias.Length == 1)
					m_Cache[alias[0]] = glyph;
			}
		}
	}

	public BitmapGlyph GetGlyph(char _Character)
	{
		if (m_Cache == null)
			Unpack();
		
		if (m_Cache.ContainsKey(_Character))
			return m_Cache[_Character];
		
		if (m_Cache.ContainsKey(MISSING_SYMBOL))
			return m_Cache[MISSING_SYMBOL];
		
		return null;
	}

	public void AddGlyph(BitmapGlyph _Glyph)
	{
		m_Glyphs.Add(_Glyph);
		
		Sort();
	}

	public void RemoveGlyph(BitmapGlyph _Glyph)
	{
		m_Glyphs.Remove(_Glyph);
		
		Sort();
	}

	public float CalcWidth(char _Character)
	{
		if (char.IsWhiteSpace(_Character))
			return SpaceWidth;
		
		if (_Character == '\n')
			return 0;
		
		BitmapGlyph glyph = GetGlyph(_Character);
		
		return glyph != null ? glyph.Width : 0;
	}

	public float CalcHeight(char _Character)
	{
		if (char.IsWhiteSpace(_Character))
			return Ascender;
		
		if (_Character == '\n')
			return 0;
		
		BitmapGlyph glyph = GetGlyph(_Character);
		
		return glyph != null ? glyph.Height : 0;
	}

	public void Reset()
	{
		m_Cache = null;
	}

	public void Sort()
	{
		Regex regex = new Regex("[0-9]+");
		m_Glyphs.Sort(
			(_A, _B) =>
			{
				string a = regex.Replace(
					_A.Name,
					_Match => _Match.Value.PadLeft(10, '0')
				);
				string b = regex.Replace(
					_B.Name,
					_Match => _Match.Value.PadLeft(10, '0')
				);
				return String.Compare(a, b, StringComparison.Ordinal);
			}
		);
	}

	public IEnumerator<BitmapGlyph> GetEnumerator()
	{
		return m_Glyphs.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_Glyphs.GetEnumerator();
	}
}
