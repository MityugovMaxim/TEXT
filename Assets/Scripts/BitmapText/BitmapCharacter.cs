using System;
using UnityEngine;

[Serializable]
public class BitmapCharacter
{
	public Rect    Rect;
	public Rect    UV;
	public Color32 Color;
	public bool    Enabled;
	public int     Index;

	public Color   Tint   = UnityEngine.Color.white;
	public Vector2 Offset = Vector2.zero;

	public void Fill(ref UIVertex[] _Buffer)
	{
		Color   color  = Color * Tint;
		Vector3 normal = Vector3.forward;
		
		_Buffer[0].position = new Vector2(Rect.xMin, Rect.yMin) + Offset;
		_Buffer[0].uv0      = new Vector2(UV.xMin, UV.yMin);
		_Buffer[0].color    = color;
		_Buffer[0].normal   = normal;
		
		_Buffer[1].position = new Vector2(Rect.xMax, Rect.yMin) + Offset;
		_Buffer[1].uv0      = new Vector2(UV.xMax, UV.yMin);
		_Buffer[1].color    = color;
		_Buffer[1].normal   = normal;
		
		_Buffer[2].position = new Vector2(Rect.xMax, Rect.yMax) + Offset;
		_Buffer[2].uv0      = new Vector2(UV.xMax, UV.yMax);
		_Buffer[2].color    = color;
		_Buffer[2].normal   = normal;
		
		_Buffer[3].position = new Vector2(Rect.xMin, Rect.yMax) + Offset;
		_Buffer[3].uv0      = new Vector2(UV.xMin, UV.yMax);
		_Buffer[3].color    = color;
		_Buffer[3].normal   = normal;
	}
}