using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class BitmapTextWrap : BitmapTextProcessor
{
	readonly List<string> m_Lines;

	public BitmapTextWrap(BitmapText _BitmapText) : base(_BitmapText)
	{
		m_Lines = new List<string>();
	}

	public override void Process()
	{
		Rect rect = Rect;
		
		m_Lines.Clear();
		
		foreach (string line in Lines)
			m_Lines.AddRange(Wrap(line, rect.width));
		
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
		
		if (CalcWidth(_Text) <= _Width)
		{
			yield return _Text;
			yield break;
		}
		
		string text = _Text.TrimEnd(TrimChars);
		if (CalcWidth(text) <= _Width)
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
			if (lineBuilder.Length > 0 && lineWidth + wordWidth > _Width)
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