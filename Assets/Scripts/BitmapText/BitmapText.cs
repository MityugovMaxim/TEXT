using System;
using System.Text;
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

	public List<string> Lines
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

	public float Scale
	{
		get { return m_Scale; }
		set { m_Scale = value; }
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

	[SerializeField, HideInInspector] string     m_Text;
	[SerializeField, HideInInspector] BitmapFont m_Font;
	[SerializeField, HideInInspector] Alignment  m_Alignment;
	[SerializeField, HideInInspector] float      m_CharSize = 1;
	[SerializeField, HideInInspector] float      m_CharSpacing;
	[SerializeField, HideInInspector] float      m_LineSpacing;

	[SerializeField] float m_MinSize;
	[SerializeField] float m_MaxSize;

	[SerializeField] bool m_WrapEnabled;
	[SerializeField] bool m_FitEnabled;
	[SerializeField] bool m_ClipEnabled;

	[SerializeField] float m_Scale = 1;

	BitmapTextProcessor m_Wrap;
	BitmapTextProcessor m_Fit;
	BitmapTextProcessor m_BestFit;
	BitmapTextProcessor m_Render;
	BitmapTextProcessor m_Clip;

	[NonSerialized] List<string>              m_Lines;
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
		
		foreach (BitmapCharacter characterInfo in Characters)
		{
			if (characterInfo == null || !characterInfo.Enabled)
				continue;
			
			characterInfo.Fill(ref m_Buffer);
			
			_VertexHelper.AddUIVertexQuad(m_Buffer);
		}
	}

	void SetTextDirty()
	{
		SetLinesDirty();
		
		WordWrap();
		
		Fit();
		
		BestFit();
		
		SetAnimationsDirty();
		
		SetCharactersDirty();
		
		Render();
		
		Clip();
	}

	void SetLinesDirty()
	{
		if (m_TagText == null)
			m_TagText = new TagText();
		
		m_TagText.Load(Text);
		
		if (m_Lines == null)
			m_Lines = new List<string>();
		else
			m_Lines.Clear();
		
		if (string.IsNullOrEmpty(m_TagText.Text))
			return;
		
		StringBuilder lineBuilder = new StringBuilder();
		
		foreach (char character in m_TagText.Text)
		{
			lineBuilder.Append(character);
			
			if (character != '\n')
				continue;
			
			m_Lines.Add(lineBuilder.ToString());
			
			lineBuilder.Clear();
		}
		m_Lines.Add(lineBuilder.ToString());
	}

	void SetAnimationsDirty()
	{
		if (m_Animations == null)
			m_Animations = new List<BitmapTextAnimation>();
		
		foreach (BitmapTextAnimation animation in m_Animations)
			animation.Restore();
		
		m_Animations.Clear();
		
		if (m_TagText == null || m_TagText.Nodes == null)
			return;
		
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

	void SetCharactersDirty()
	{
		if (m_Characters == null)
			m_Characters = new List<BitmapCharacter>();
		
		for (int i = 0; i < m_Characters.Count; i++)
		{
			if (m_Characters[i] == null)
				m_Characters[i] = new BitmapCharacter();
			
			m_Characters[i].Index   = -1;
			m_Characters[i].Enabled = false;
		}
		
		if (Lines == null)
			return;
		
		int index = 0;
		foreach (string line in Lines)
		foreach (char character in line)
		{
			if (index >= m_Characters.Count)
				m_Characters.Add(new BitmapCharacter());
			
			m_Characters[index].Index   = index;
			m_Characters[index].Enabled = true;
			index++;
		}
	}

	void BestFit()
	{
		if (!m_WrapEnabled || !m_FitEnabled)
			return;
		
		if (m_BestFit == null)
			m_BestFit = new BitmapTextBestFit(this);
		
		m_BestFit.Process();
	}

	void WordWrap()
	{
		if (!m_WrapEnabled || m_FitEnabled)
			return;
		
		if (m_Wrap == null)
			m_Wrap = new BitmapTextWrap(this);
		
		m_Wrap.Process();
	}

	void Fit()
	{
		if (!m_FitEnabled || m_WrapEnabled)
			return;
		
		if (m_Fit == null)
			m_Fit = new BitmapTextFit(this);
		
		m_Fit.Process();
	}

	void Render()
	{
		if (m_Render == null)
			m_Render = new BitmapTextRender(this);
		
		m_Render.Process();
	}

	void Clip()
	{
		if (!m_ClipEnabled)
			return;
		
		if (m_Clip == null)
			m_Clip = new BitmapTextClip(this);
		
		m_Clip.Process();
	}
}
