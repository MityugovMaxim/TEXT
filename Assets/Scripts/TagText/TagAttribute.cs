using System;

[AttributeUsage(AttributeTargets.Class)]
public class TagAttribute : Attribute
{
	public readonly string[] Tags;

	public TagAttribute(params string[] _Tags)
	{
		Tags = _Tags;
	}
}