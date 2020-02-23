using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitmapText : MaskableGraphic
{
	public override Texture mainTexture
	{
		get
		{
			if (Font != null && Font.Texture != null)
				return Font.Texture;
			return s_WhiteTexture;
		}
	}

	public BitmapFont Font
	{
		get { return m_Font; }
		set
		{
			if (m_Font == value)
				return;
			
			m_Font = value;
			
			SetAllDirty();
		}
	}

	public string Text
	{
		get { return m_Text; }
		set
		{
			if (m_Text == value)
				return;
			
			m_Text = value;
			
			SetAllDirty();
		}
	}

	public List<BitmapLine> Lines
	{
		get { return m_Lines; }
	}

	public List<BitmapCharacter> Characters
	{
		get { return m_Characters; }
	}

	public Alignment Alignment
	{
		get { return m_Alignment; }
		set
		{
			if (m_Alignment == value)
				return;
			
			m_Alignment = value;
			
			SetAllDirty();
		}
	}

	public float LineSpacing
	{
		get { return m_LineSpacing; }
		set
		{
			if (Mathf.Approximately(m_LineSpacing, value))
				return;
			
			m_LineSpacing = value;
			
			SetAllDirty();
		}
	}

	public float CharSpacing
	{
		get { return m_CharSpacing; }
		set
		{
			if (Mathf.Approximately(m_CharSpacing, value))
				return;
			
			m_CharSpacing = value;
			
			SetAllDirty();
		}
	}

	public float CharSize
	{
		get { return m_CharSize; }
		set
		{
			float charSize = Mathf.Max(0, value);
			
			if (Mathf.Approximately(m_CharSize, charSize))
				return;
			
			m_CharSize = charSize;
			
			SetAllDirty();
		}
	}

	public float MinSize
	{
		get { return m_MinSize; }
		set
		{
			float minSize = Mathf.Max(value, 0);
			
			if (Mathf.Approximately(m_MinSize, minSize))
				return;
			
			m_MinSize = minSize;
			
			SetAllDirty();
		}
	}

	public float MaxSize
	{
		get { return m_MaxSize; }
		set
		{
			float maxSize = Mathf.Max(value, 0);
			
			if (Mathf.Approximately(m_MaxSize, maxSize))
				return;
			
			m_MaxSize = maxSize;
			
			SetAllDirty();
		}
	}

	public float Scale { get; set; }

	[SerializeField, HideInInspector] string     m_Text;
	[SerializeField, HideInInspector] BitmapFont m_Font;
	[SerializeField, HideInInspector] Alignment  m_Alignment;
	[SerializeField, HideInInspector] float      m_CharSize = 1;
	[SerializeField, HideInInspector] float      m_CharSpacing;
	[SerializeField, HideInInspector] float      m_LineSpacing;

	[SerializeField] float m_MinSize;
	[SerializeField] float m_MaxSize;

	[SerializeField] bool m_Wrap;
	[SerializeField] bool m_Fit;
	[SerializeField] bool m_Clip;

	BitmapTextProcessor m_PositionHorizontalProcessor;
	BitmapTextProcessor m_PositionVerticalProcessor;
	BitmapTextProcessor m_WrapProcessor;
	BitmapTextProcessor m_FitProcessor;
	BitmapTextProcessor m_AlignProcessor;
	BitmapTextProcessor m_ClipProcessor;

	[NonSerialized] List<BitmapLine>          m_Lines;
	[NonSerialized] List<BitmapCharacter>     m_Characters;
	[NonSerialized] List<BitmapTextAnimation> m_Animations;

	TagText    m_TagText;
	UIVertex[] m_Buffer = new UIVertex[4];

	void Update()
	{
		if (m_Animations != null)
		{
			foreach (BitmapTextAnimation animation in m_Animations)
				animation.Restore();
			
			foreach (BitmapTextAnimation animation in m_Animations)
				animation.Process();
		}
	}

	public override void SetAllDirty()
	{
		SetTextDirty();
		
		base.SetAllDirty();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		SetTextDirty();
		
		base.OnRectTransformDimensionsChange();
	}

	protected override void OnPopulateMesh(VertexHelper _VertexHelper)
	{
		_VertexHelper.Clear();
		
		if (Characters == null)
			return;
		
		foreach (BitmapCharacter character in Characters)
		{
			if (character == null || !character.Enabled || !character.Visible)
				continue;
			
			character.Fill(ref m_Buffer);
			
			_VertexHelper.AddUIVertexQuad(m_Buffer);
		}
	}

	void SetTextDirty()
	{
		ProcessCharacters();
		
		ProcessAnimations();
		
		Wrap();
		
		Fit();
		
		PositionHorizontal();
		
		PositionVertical();
		
		Align();
		
		Clip();
	}

	void ProcessCharacters()
	{
		Scale = 1;
		
		if (m_TagText == null)
			m_TagText = new TagText();
		
		m_TagText.Load(Text);
		
		if (m_Lines == null)
			m_Lines = new List<BitmapLine>();
		else
			m_Lines.Clear();
		
		if (m_Characters == null)
			m_Characters = new List<BitmapCharacter>();
		
		int delta = m_TagText.Text.Length - m_Characters.Count;
		for (int i = 0; i < delta; i++)
			m_Characters.Add(new BitmapCharacter());
		
		for (int i = 0; i < m_Characters.Count; i++)
		{
			if (m_Characters[i] == null)
				m_Characters[i] = new BitmapCharacter();
			
			BitmapCharacter character = m_Characters[i];
			
			character.Index   = -1;
			character.Enabled = false;
			character.Visible = false;
			character.Tint    = Color.white;
			character.Offset  = Vector2.zero;
		}
		
		List<BitmapCharacter> line = new List<BitmapCharacter>();
		int index = 0;
		for (int i = 0; i < m_TagText.Text.Length; i++)
		{
			if (m_TagText.Text[i] == '\n')
			{
				m_Lines.Add(new BitmapLine(line));
				line.Clear();
				
				index++;
				continue;
			}
			
			BitmapCharacter character = m_Characters[index];
			
			character.Index     = i;
			character.Enabled   = true;
			character.Character = m_TagText.Text[i];
			
			BitmapGlyph glyph = Font.GetGlyph(character.Character);
			if (glyph != null)
			{
				Rect glyphRect = glyph.Rect;
				character.Visible        = true;
				character.Rect           = glyphRect.Scale(CharSize);
				character.BaselineOffset = glyph.Offset * CharSize;
				character.LineHeight     = Font.Ascender * CharSize;
				character.UV             = glyph.UV;
				character.Color          = color;
			}
			else if (character.Character == ' ')
			{
				Rect glyphRect = new Rect(0, 0, Font.SpaceWidth, 0);
				
				character.Visible = false;
				character.Rect    = glyphRect.Scale(CharSize);
			}
			
			line.Add(character);
			
			index++;
		}
		m_Lines.Add(new BitmapLine(line));
		line.Clear();
	}

	void ProcessAnimations()
	{
		if (m_Animations == null)
			m_Animations = new List<BitmapTextAnimation>();
		else
			m_Animations.Clear();
		
		foreach (TagText.Node node in m_TagText.Nodes)
		{
			Type type = TagPrebuild.GetTagType(node.Tag);
			if (type == null)
				continue;
			
			BitmapTextAnimation animation = Activator.CreateInstance(type, this, node) as BitmapTextAnimation;
			if (animation == null)
				continue;
			
			m_Animations.Add(animation);
		}
	}

	void Wrap()
	{
		if (!m_Wrap)
			return;
		
		if (m_WrapProcessor == null)
			m_WrapProcessor = new BitmapTextWrapProcessor(this);
		
		m_WrapProcessor.Process();
	}

	void Fit()
	{
		if (!m_Fit)
			return;
		
		if (m_FitProcessor == null)
			m_FitProcessor = new BitmapTextFitProcessor(this);
		
		m_FitProcessor.Process();
	}

	void PositionHorizontal()
	{
		if (m_PositionHorizontalProcessor == null)
			m_PositionHorizontalProcessor = new BitmapTextPositionProcessor(this, BitmapTextPositionProcessor.Direction.Horizontal);
		
		m_PositionHorizontalProcessor.Process();
	}

	void PositionVertical()
	{
		if (m_PositionVerticalProcessor == null)
			m_PositionVerticalProcessor = new BitmapTextPositionProcessor(this, BitmapTextPositionProcessor.Direction.Vertical);
		
		m_PositionVerticalProcessor.Process();
	}

	void Align()
	{
		if (m_AlignProcessor == null)
			m_AlignProcessor = new BitmapTextAlignProcessor(this);
		
		m_AlignProcessor.Process();
	}

	void Clip()
	{
		if (!m_Clip)
			return;
		
		if (m_ClipProcessor == null)
			m_ClipProcessor = new BitmapTextClipProcessor(this);
		
		m_ClipProcessor.Process();
	}
}
