using UnityEngine;

public static class RectExtension
{
	public static float GetAspect(this Rect _Rect)
	{
		return _Rect.width / _Rect.height;
	}

	public static Rect Fit(this Rect _Rect, float _Aspect, Alignment _Alignment = Alignment.MiddleCenter)
	{
		float hFit = _Rect.width  * (_Rect.width  / _Aspect);
		float vFit = _Rect.height * (_Rect.height * _Aspect);
		
		Vector2 size = hFit < vFit
			? new Vector2(_Rect.width, _Rect.width / _Aspect)
			: new Vector2(_Rect.height             * _Aspect, _Rect.height);
		Vector2 pivot = _Alignment.GetPivot();
		
		return new Rect(
			_Rect.x + (_Rect.width  - size.x) * pivot.x,
			_Rect.y + (_Rect.height - size.y) * pivot.y,
			size.x,
			size.y
		);
	}

	public static Rect Fill(this Rect _Rect, float _Aspect, Alignment _Alignment = Alignment.MiddleCenter)
	{
		float hFit = _Rect.width  * (_Rect.width  / _Aspect);
		float vFit = _Rect.height * (_Rect.height * _Aspect);
		
		Vector2 size = hFit > vFit
			? new Vector2(_Rect.width, _Rect.width / _Aspect)
			: new Vector2(_Rect.height             * _Aspect, _Rect.height);
		Vector2 pivot = _Alignment.GetPivot();
		
		return new Rect(
			_Rect.x + (_Rect.width  - size.x) * pivot.x,
			_Rect.y + (_Rect.height - size.y) * pivot.y,
			size.x,
			size.y
		);
	}
}