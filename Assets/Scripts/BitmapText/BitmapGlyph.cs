using System;
using UnityEngine;

[Serializable]
public class BitmapGlyph
{
	public string Name
	{
		get { return m_Name; }
	}

	public string[] Aliases
	{
		get { return m_Aliases; }
	}

	public int Width
	{
		get { return m_Width; }
	}

	public int Height
	{
		get { return m_Height; }
	}

	public int Offset
	{
		get { return m_Offset; }
	}

	public Rect UV
	{
		get { return m_UV; }
	}

	[SerializeField, HideInInspector] string   m_Name;
	[SerializeField, HideInInspector] string[] m_Aliases;
	[SerializeField, HideInInspector] int      m_Width;
	[SerializeField, HideInInspector] int      m_Height;
	[SerializeField, HideInInspector] bool[]   m_Glyph;
	[SerializeField, HideInInspector] int      m_Offset;
	[SerializeField, HideInInspector] Rect     m_UV;

	public BitmapGlyph(
		string _Name,
		int    _Width,
		int    _Height
	)
	{
		m_Name   = _Name;
		m_Width  = _Width;
		m_Height = _Height;
		
		m_Glyph = new bool[m_Width * m_Height];
	}
}