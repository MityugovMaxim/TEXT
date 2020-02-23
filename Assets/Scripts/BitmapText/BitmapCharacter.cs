using System;
using UnityEngine;

[Serializable]
public class BitmapCharacter
{
	public Rect RealRect
	{
		get
		{
			return new Rect(
				Rect.x,
				Rect.y + BaselineOffset - LineHeight,
				Rect.width,
				Rect.height
			);
		}
		set
		{
			Rect = new Rect(
				value.x,
				value.y - BaselineOffset + LineHeight,
				value.width,
				value.height
			);
		}
	}

	public int   Index;
	public bool  Enabled;
	public bool  Visible;
	public char  Character;
	public float LineHeight;
	public float BaselineOffset;
	public Rect  UV;

	public Rect    Rect;
	public Color32 Color;

	public Color   Tint   = UnityEngine.Color.white;
	public Vector2 Offset = Vector2.zero;

	public void Fill(ref UIVertex[] _Buffer)
	{
		Rect    rect   = RealRect;
		Color   color  = Color * Tint;
		Vector3 normal = Vector3.back;
		
		rect.position += Offset;
		
		_Buffer[0].position = new Vector2(rect.xMin, rect.yMin);
		_Buffer[0].uv0      = new Vector2(UV.xMin, UV.yMin);
		_Buffer[0].color    = color;
		_Buffer[0].normal   = normal;
		
		_Buffer[1].position = new Vector2(rect.xMin, rect.yMax);
		_Buffer[1].uv0      = new Vector2(UV.xMin, UV.yMax);
		_Buffer[1].color    = color;
		_Buffer[1].normal   = normal;
		
		_Buffer[2].position = new Vector2(rect.xMax, rect.yMax);
		_Buffer[2].uv0      = new Vector2(UV.xMax, UV.yMax);
		_Buffer[2].color    = color;
		_Buffer[2].normal   = normal;
		
		_Buffer[3].position = new Vector2(rect.xMax, rect.yMin);
		_Buffer[3].uv0      = new Vector2(UV.xMax, UV.yMin);
		_Buffer[3].color    = color;
		_Buffer[3].normal   = normal;
	}
}