using UnityEngine;

public static class Utility
{
	public static Vector2 Lerp(Vector2[] _Vertices, float _Phase)
	{
		if (_Vertices == null || _Vertices.Length == 0)
			return Vector2.zero;

		int length = _Vertices.Length;
		int source = Mathf.FloorToInt(length * _Phase);
		int target = (source + 1) % _Vertices.Length;
		
		float min = (float)source / length;
		float max = (float)(source + 1) / length;
		
		return Vector2.Lerp(
			_Vertices[source],
			_Vertices[target],
			Remap01(_Phase, min, max)
		);
	}

	public static float Remap(float _Value, float _SourceMin, float _SourceMax, float _TargetMin, float _TargetMax)
	{
		return _TargetMin + (_Value - _SourceMin) / (_SourceMax - _SourceMin) * (_TargetMax - _TargetMin);
	}

	public static float Remap01(float _Value, float _SourceMin, float _SourceMax)
	{
		return (_Value - _SourceMin) / (_SourceMax - _SourceMin);
	}
}
