using UnityEngine;

public static class AlignmentExtension
{
	public static Vector2 GetPivot(this Alignment _Alignment)
	{
		switch (_Alignment)
		{
			case Alignment.UpperLeft:
				return new Vector2(0, 1);
			case Alignment.UpperCenter:
				return new Vector2(0.5f, 1);
			case Alignment.UpperRight:
				return new Vector2(1, 1);
			case Alignment.MiddleLeft:
				return new Vector2(0, 0.5f);
			case Alignment.MiddleCenter:
				return new Vector2(0.5f, 0.5f);
			case Alignment.MiddleRight:
				return new Vector2(1, 0.5f);
			case Alignment.LowerLeft:
				return new Vector2(0, 0);
			case Alignment.LowerCenter:
				return new Vector2(0.5f, 0);
			case Alignment.LowerRight:
				return new Vector2(1, 0);
			default:
				return new Vector2(0, 0);
		}
	}
}