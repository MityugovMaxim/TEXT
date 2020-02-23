using System.Collections.Generic;
using UnityEngine;

public class BitmapTextWrapProcessor : BitmapTextProcessor
{
	readonly List<BitmapLine>      m_Lines;
	readonly List<BitmapLine>      m_LineGroup;
	readonly List<BitmapCharacter> m_Line;

	readonly BitmapTextPositionProcessor m_PositionProcessor;

	public BitmapTextWrapProcessor(BitmapText _BitmapText) : base(_BitmapText)
	{
		m_Lines     = new List<BitmapLine>();
		m_LineGroup = new List<BitmapLine>();
		m_Line      = new List<BitmapCharacter>();
		
		m_PositionProcessor = new BitmapTextPositionProcessor(BitmapText, BitmapTextPositionProcessor.Direction.Horizontal);
	}

	public override void Process()
	{
		Rect rect = Rect;
		
		m_Lines.Clear();
		m_LineGroup.Clear();
		m_Line.Clear();
		
		foreach (BitmapLine line in Lines)
		{
			if (CalcWidth(line) <= rect.width)
			{
				m_Lines.Add(line);
				continue;
			}
			
			float lineWidth = 0;
			
			foreach (BitmapCharacter[] word in line.GetWords())
			{
				float wordWidth = CalcWidth(word);
				
				if (m_Line.Count > 0 && lineWidth + wordWidth > rect.width)
				{
					lineWidth = 0;
					
					m_LineGroup.Add(new BitmapLine(m_Line));
					m_Line.Clear();
				}
				
				lineWidth += wordWidth + CharSpacing;
				
				m_Line.AddRange(word);
			}
			m_LineGroup.Add(new BitmapLine(m_Line));
			m_Line.Clear();
			
			if (m_LineGroup.Count >= 2)
			{
				m_LineGroup[0].TrimEnd();
				
				for (int i = 1; i < m_LineGroup.Count - 1; i++)
					m_LineGroup[i].Trim();
				
				m_LineGroup[m_LineGroup.Count - 1].TrimStart();
				
				for (int i = m_LineGroup.Count - 1; i >= 0; i--)
				{
					if (m_LineGroup[i].IsEmpty)
						m_LineGroup.RemoveAt(i);
				}
			}
			
			m_Lines.AddRange(m_LineGroup);
			m_LineGroup.Clear();
		}
		
		Lines.Clear();
		Lines.AddRange(m_Lines);
		
		m_PositionProcessor.Process();
	}
}