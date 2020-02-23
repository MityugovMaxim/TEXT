using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class BitmapTextBestFit : BitmapTextProcessor
{
	const float TOLERANCE = 0.001f;

	readonly List<string> m_Lines;

	public BitmapTextBestFit(BitmapText _BitmapText) : base(_BitmapText)
	{
		m_Lines = new List<string>();
	}

	public override void Process()
	{
		Rect rect = Rect;
		
		float scale = Scale;
		
		Scale = 1;
		
		float minScale = MinSize > 0 ? MinSize / CharSize : 0;
		float maxScale = MaxSize > 0 ? MaxSize / CharSize : Mathf.Max(rect.width, rect.height);
		
		m_Lines.Clear();
		m_Lines.AddRange(Lines);
		
		while (maxScale - minScale > TOLERANCE)
		{
			Scale = minScale + (maxScale - minScale) / 2;
			
			m_Lines.Clear();
			
			foreach (string line in Lines)
				m_Lines.AddRange(Wrap(line, rect.width));
			
			float width  = CalcWidth(m_Lines);
			float height = CalcHeight(m_Lines);
			
			if (width < rect.width && height < rect.height)
			{
				scale = Scale;
				minScale = Scale;
				continue;
			}
			if (width > rect.width || height > rect.height)
			{
				maxScale = Scale;
				continue;
			}
			
			break;
		}
		
		Scale = scale;
		
		m_Lines.Clear();
		
		foreach (string line in Lines)
			m_Lines.AddRange(Wrap(line, rect.width));
		
		Scale = Mathf.Floor(scale / TOLERANCE) * TOLERANCE;
		
		Lines.Clear();
		Lines.AddRange(m_Lines);
	}

	IEnumerable<string> Wrap(string _Text, float _Width)
	{
		if (string.IsNullOrEmpty(_Text))
		{
			yield return string.Empty;
			yield break;
		}
		
		if (CalcWidth(_Text) < _Width)
		{
			yield return _Text;
			yield break;
		}
		
		string text = _Text.TrimEnd(TrimChars);
		if (CalcWidth(text) < _Width)
		{
			yield return text;
			yield break;
		}
		
		float lineWidth = 0;
		
		HashSet<char> trimChars = new HashSet<char>(TrimChars);
		
		StringBuilder lineBuilder = new StringBuilder();
		
		foreach (string word in Words(text))
		{
			float wordWidth = CalcWidth(word);
			if (lineBuilder.Length > 0 && lineWidth + wordWidth >= _Width)
			{
				yield return lineBuilder.ToString().TrimEnd(TrimChars);
				
				lineWidth = 0;
				lineBuilder.Clear();
				
				if (word.Length == 1 && trimChars.Contains(word[0]))
					continue;
			}
			
			lineWidth += wordWidth + CharSpacing * CharSize;
			lineBuilder.Append(word);
		}
		yield return lineBuilder.ToString().TrimEnd(TrimChars);
	}

	static IEnumerable<string> Words(string _Text)
	{
		StringBuilder wordBuilder = new StringBuilder();
		
		foreach (char character in _Text)
		{
			if (char.IsWhiteSpace(character))
			{
				if (wordBuilder.Length > 0)
				{
					yield return wordBuilder.ToString();
					wordBuilder.Clear();
				}
				
				yield return character.ToString();
				
			}
			else
			{
				wordBuilder.Append(character);
			}
		}
		yield return wordBuilder.ToString();
	}
}