using UnityEngine;
using System.Collections.Generic;

public class BitmapTextTruncate : BitmapTextProcessor
{
	readonly string m_TruncateChars;

	readonly List<string> m_Lines;

	public BitmapTextTruncate(BitmapText _BitmapText) : base(_BitmapText)
	{
		m_TruncateChars = "…";
		m_Lines         = new List<string>();
	}

	public BitmapTextTruncate(BitmapText _BitmapText, string _TruncateChars) : base(_BitmapText)
	{
		m_TruncateChars = _TruncateChars;
		m_Lines         = new List<string>();
	}

	public override void Process()
	{
		m_Lines.Clear();
		
		Rect rect = Rect;
		
		m_Lines.AddRange(Truncate(rect.width));
		
		Lines.Clear();
		Lines.AddRange(m_Lines);
	}

	IEnumerable<string> Truncate(float _Width)
	{
		foreach (string line in Lines)
		{
			if (string.IsNullOrEmpty(line))
			{
				yield return line;
				continue;
			}
			
			float lineWidth = CalcWidth(line);
			
			if (lineWidth <= _Width)
			{
				yield return line;
				continue;
			}
			
			for (int length = line.Length; length >= 0; length--)
			{
				string text = line.Substring(0, length).TrimEnd(TrimChars) + m_TruncateChars;
				
				if (CalcWidth(text) <= _Width)
				{
					yield return text;
					yield break;
				}
			}
			
			yield break;
		}
	}
}