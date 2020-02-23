using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.ProjectWindowCallback;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

[CanEditMultipleObjects]
[CustomEditor(typeof(BitmapFont))]
public class BitmapFontEditor : Editor
{
	class DoCreateBitmapGlyph : PopupWindowContent
	{
		Rect       m_Rect;
		BitmapFont m_Font;
		string     m_Name;
		int        m_Width;
		int        m_Height;

		public DoCreateBitmapGlyph(Rect _Rect, BitmapFont _Font)
		{
			m_Rect   = _Rect;
			m_Font   = _Font;
			m_Width  = 4;
			m_Height = 4;
			foreach (BitmapGlyph glyph in m_Font)
			{
				m_Width  = Mathf.Max(m_Width, glyph.Width);
				m_Height = Mathf.Max(m_Height, glyph.Height);
			}
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(m_Rect.width, 83);
		}

		public override void OnGUI(Rect _Rect)
		{
			m_Name   = EditorGUILayout.TextField("Name", m_Name);
			m_Width  = EditorGUILayout.IntField("Width", m_Width);
			m_Height = EditorGUILayout.IntField("Height", m_Height);
			
			m_Name = !string.IsNullOrEmpty(m_Name) ? m_Name.Substring(0, 1) : string.Empty;
			
			m_Width  = Mathf.Max(4, m_Width);
			m_Height = Mathf.Max(4, m_Height);
			
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Create", EditorStyles.miniButtonLeft))
			{
				Create();
				Cancel();
			}
			
			if (GUILayout.Button("Cancel", EditorStyles.miniButtonRight))
			{
				Cancel();
			}
			
			EditorGUILayout.EndHorizontal();
		}

		void Create()
		{
			if (m_Font == null)
			{
				Debug.LogError("Create BitmapGlyph failed. Font can't be null.");
				return;
			}
			
			if (string.IsNullOrEmpty(m_Name))
			{
				Debug.LogError("Create BitmapGlyph failed. Name can't be null or empty.");
				return;
			}
			
			BitmapGlyph glyph = new BitmapGlyph(m_Name, m_Width, m_Height);
			
			m_Font.AddGlyph(glyph);
		}

		void Cancel()
		{
			if (editorWindow != null)
				editorWindow.Close();
		}
	}

	SerializedProperty SelectedGlyph
	{
		get
		{
			if (m_SelectedGlyph == -1)
				return null;
			
			SerializedProperty glyphsProperty = serializedObject.FindProperty("m_Glyphs");
			
			if (m_SelectedGlyph < 0 || m_SelectedGlyph >= glyphsProperty.arraySize)
				return null;
			
			return glyphsProperty.GetArrayElementAtIndex(m_SelectedGlyph);
		}
	}

	bool      m_CellValue;
	int       m_SelectedGlyph;
	int       m_GlyphGroupIndex;
	List<int> m_GlyphGroup;

	ReorderableList m_Aliases;
	Texture2D       m_Preview;

	void OnEnable()
	{
		m_CellValue       = false;
		m_SelectedGlyph   = -1;
		m_GlyphGroupIndex = -1;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		GUIContent content = new GUIContent("Create glyph");
		GUIStyle   style   = GUI.skin.button;
		Rect       rect    = GUILayoutUtility.GetRect(content, style);
		if (GUI.Button(rect, content, style))
			PopupWindow.Show(rect, new DoCreateBitmapGlyph(rect, target as BitmapFont));
		
		if (GUILayout.Button("Bake"))
			Bake();
		
		DrawGlyphs();
		
		serializedObject.ApplyModifiedProperties();
	}

	public override bool HasPreviewGUI()
	{
		SerializedProperty glyphsProperty = serializedObject.FindProperty("m_Glyphs");
		
		return glyphsProperty != null && glyphsProperty.arraySize > 0;
	}

	public override void OnPreviewGUI(Rect _Rect, GUIStyle _Background)
	{
		BitmapFont font = target as BitmapFont;
		
		if (font == null)
			return;
		
		EditorGUI.DrawPreviewTexture(_Rect, font.Texture, null, ScaleMode.ScaleToFit);
	}

	void DrawGlyphs()
	{
		DrawGlyphsGroups();
		
		SerializedProperty glyphsProperty = serializedObject.FindProperty("m_Glyphs");
		
		if (m_SelectedGlyph < 0 || m_SelectedGlyph >= glyphsProperty.arraySize)
			return;
		
		GUILayout.Space(5);
		
		DrawGlyphPreview();
		
		GUILayout.Space(5);
		
		DrawGlyphParameters();
		
		DrawGlyphAliases();
		
		DrawGlyphToolbar();
		
		GUILayout.Space(5);
		
		DrawGlyphCanvas();
		
		GUILayout.Space(15);
		
		DrawGlyphControl();
	}

	void DrawGlyphsGroups()
	{
		const int lettersGroup     = 0;
		const int digitsGroup      = 1;
		const int punctuationGroup = 2;
		const int symbolsGroup     = 3;
		
		EditorGUILayout.BeginHorizontal();
		
		int glyphGroup = m_GlyphGroupIndex;
		
		GUI.backgroundColor = glyphGroup == lettersGroup ? Color.grey : Color.white;
		if (GUILayout.Button("LETTERS", EditorStyles.miniButtonLeft))
			glyphGroup = glyphGroup != lettersGroup ? lettersGroup : -1;
		
		GUI.backgroundColor = m_GlyphGroupIndex == digitsGroup ? Color.grey : Color.white;
		if (GUILayout.Button("DIGITS", EditorStyles.miniButtonMid))
			glyphGroup = glyphGroup != digitsGroup ? digitsGroup : -1;
		
		GUI.backgroundColor = glyphGroup == punctuationGroup ? Color.grey : Color.white;
		if (GUILayout.Button("PUNCTUATION", EditorStyles.miniButtonMid))
			glyphGroup = glyphGroup != punctuationGroup ? punctuationGroup : -1;
		
		GUI.backgroundColor = glyphGroup == symbolsGroup ? Color.grey : Color.white;
		if (GUILayout.Button("SYMBOLS", EditorStyles.miniButtonRight))
			glyphGroup = glyphGroup != symbolsGroup ? symbolsGroup : -1;
		GUI.backgroundColor = Color.white;
		
		EditorGUILayout.EndHorizontal();
		
		if (m_GlyphGroup == null)
			m_GlyphGroup = new List<int>();
		
		if (m_GlyphGroupIndex != glyphGroup)
		{
			m_GlyphGroupIndex = glyphGroup;
			
			m_GlyphGroup.Clear();
			
			SerializedProperty glyphsProperty = serializedObject.FindProperty("m_Glyphs");
			for (int i = 0; i < glyphsProperty.arraySize; i++)
			{
				SerializedProperty glyphProperty = glyphsProperty.GetArrayElementAtIndex(i);
				
				if (glyphProperty == null)
					continue;
				
				SerializedProperty nameProperty = glyphProperty.FindPropertyRelative("m_Name");
				
				if (nameProperty == null)
					continue;
				
				string name = nameProperty.stringValue;
				
				if (string.IsNullOrEmpty(name))
					continue;
				
				switch (m_GlyphGroupIndex)
				{
					case lettersGroup:
						if (char.IsLetter(name, 0))
							m_GlyphGroup.Add(i);
						break;
					case digitsGroup:
						if (char.IsDigit(name, 0))
							m_GlyphGroup.Add(i);
						break;
					case punctuationGroup:
						if (char.IsPunctuation(name, 0))
							m_GlyphGroup.Add(i);
						break;
					case symbolsGroup:
						if (!char.IsLetter(name, 0) && !char.IsDigit(name, 0) && !char.IsPunctuation(name, 0))
							m_GlyphGroup.Add(i);
						break;
				}
			}
		}
		
		if (m_GlyphGroupIndex == -1)
			return;
		
		DrawGlyphsGroup();
	}

	void DrawGlyphsGroup()
	{
		if (m_GlyphGroup == null || m_GlyphGroup.Count == 0)
			return;
		
		SerializedProperty glyphsProperty = serializedObject.FindProperty("m_Glyphs");
		
		int width  = 20;
		int height = Mathf.CeilToInt(m_GlyphGroup.Count * (1.0f / width));
		
		switch (Event.current.type)
		{
			case EventType.KeyDown:
				int index = m_GlyphGroup.IndexOf(m_SelectedGlyph);
				if (Event.current.keyCode == KeyCode.LeftArrow)
					index -= 1;
				if (Event.current.keyCode == KeyCode.RightArrow)
					index += 1;
				if (Event.current.keyCode == KeyCode.UpArrow)
					index -= width;
				if (Event.current.keyCode == KeyCode.DownArrow)
					index += width;
				
				index = Mathf.Clamp(index, 0, m_GlyphGroup.Count - 1);
				
				SelectGlyph(m_GlyphGroup[index]);
				Repaint();
				Event.current.Use();
				
				break;
		}
		
		for (int y = 0; y < height; y++)
		{
			Rect  rect = GUILayoutUtility.GetAspectRect(width);
			float size = Mathf.Min(rect.width, rect.height);
			for (int x = 0; x < width; x++)
			{
				int index = x + width * y;
				
				if (index >= m_GlyphGroup.Count)
					break;
				
				index = m_GlyphGroup[index];
				
				SerializedProperty glyphProperty = glyphsProperty.GetArrayElementAtIndex(index);
				
				SerializedProperty nameProperty = glyphProperty.FindPropertyRelative("m_Name");
				
				GUI.backgroundColor = m_SelectedGlyph == index
					? new Color(0.25f, 0.5f, 1, 1)
					: Color.white;
				
				Rect buttonRect = new Rect(rect.x + size * x, rect.y, size, size);
				
				if (GUI.Button(buttonRect, nameProperty.stringValue))
				{
					GUI.FocusControl(null);
					SelectGlyph(index);
				}
				
				GUI.backgroundColor = Color.white;
			}
		}
	}

	void DrawGlyphParameters()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		SerializedProperty nameProperty   = glyphProperty.FindPropertyRelative("m_Name");
		SerializedProperty offsetProperty = glyphProperty.FindPropertyRelative("m_Offset");
		
		EditorGUI.BeginChangeCheck();
		
		nameProperty.stringValue = EditorGUILayout.TextField(nameProperty.displayName, nameProperty.stringValue);
		if (EditorGUI.EndChangeCheck())
			SortGlyphs();
		
		EditorGUILayout.PropertyField(offsetProperty);
	}

	void DrawGlyphAliases()
	{
		if (m_Aliases != null)
			m_Aliases.DoLayoutList();
	}

	void DrawGlyphPreview()
	{
		if (m_Preview == null)
			return;
		
		Rect rect = GUILayoutUtility.GetRect(0, 64, GUILayout.Height(64));
		
		float   size     = Mathf.Max(m_Preview.width, m_Preview.height);
		float[] sizes    = { 64, 32, 16 };
		float   position = 0;
		
		for (int i = 0; i < sizes.Length; i++)
		{
			float scale  = sizes[i] / size;
			float width  = m_Preview.width * scale;
			float height = m_Preview.height * scale;
			
			GUI.DrawTexture(
				new Rect(
					rect.x + position,
					rect.y + rect.height - height,
					width,
					height
				),
				m_Preview,
				ScaleMode.ScaleToFit
			);
			
			position += width + 5;
		}
	}

	void DrawGlyphToolbar()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		EditorGUILayout.BeginHorizontal();
		
		DrawGlyphTool(
			"MOVE",
			() => MoveGlyph(-1, 0),
			() => MoveGlyph(1, 0),
			() => MoveGlyph(0, -1),
			() => MoveGlyph(0, 1)
		);
		
		GUILayout.Space(5);
		
		DrawGlyphTool(
			"CROP",
			() => ResizeGlyph(1, 0, 0, 0),
			() => ResizeGlyph(0, 1, 0, 0),
			() => ResizeGlyph(0, 0, 1, 0),
			() => ResizeGlyph(0, 0, 0, 1)
		);
		
		GUILayout.Space(5);
		
		DrawGlyphTool(
			"EXTEND",
			() => ResizeGlyph(-1, 0, 0, 0),
			() => ResizeGlyph(0, -1, 0, 0),
			() => ResizeGlyph(0, 0, -1, 0),
			() => ResizeGlyph(0, 0, 0, -1)
		);
		
		EditorGUILayout.EndHorizontal();
	}

	void DrawGlyphTool(
		string _Name,
		Action _Left   = null,
		Action _Right  = null,
		Action _Top    = null,
		Action _Bottom = null
	)
	{
		EditorGUILayout.BeginVertical(GUILayout.Width(64));
		
		Rect nameRect = GUILayoutUtility.GetRect(64, 16);
		
		Rect toolRect = GUILayoutUtility.GetRect(64, 48);
		
		EditorGUILayout.EndVertical();
		
		toolRect = toolRect.Fit(1);
		
		float size = Mathf.Min(
			toolRect.width / 3,
			toolRect.height / 3
		);
		
		Rect lRect = new Rect(
			toolRect.x,
			toolRect.y + size,
			size,
			size
		);
		Rect rRect = new Rect(
			toolRect.x + size * 2,
			toolRect.y + size,
			size,
			size
		);
		Rect tRect = new Rect(
			toolRect.x + size,
			toolRect.y,
			size,
			size
		);
		Rect bRect = new Rect(
			toolRect.x + size,
			toolRect.y + size * 2,
			size,
			size
		);
		
		GUIStyle nameStyle = new GUIStyle(EditorStyles.label);
		nameStyle.normal.textColor = Color.white;
		nameStyle.fixedWidth       = 64;
		nameStyle.fixedHeight      = 16;
		nameStyle.alignment        = TextAnchor.MiddleCenter;
		
		EditorGUI.DropShadowLabel(nameRect, _Name, nameStyle);
		EditorGUI.DrawRect(lRect, Color.black);
		EditorGUI.DrawRect(rRect, Color.black);
		EditorGUI.DrawRect(tRect, Color.black);
		EditorGUI.DrawRect(bRect, Color.black);
		
		if (GUI.Button(lRect, GUIContent.none, GUIStyle.none) && _Left != null)
			_Left();
		if (GUI.Button(rRect, GUIContent.none, GUIStyle.none) && _Right != null)
			_Right();
		if (GUI.Button(tRect, GUIContent.none, GUIStyle.none) && _Top != null)
			_Top();
		if (GUI.Button(bRect, GUIContent.none, GUIStyle.none) && _Bottom != null)
			_Bottom();
	}

	void DrawGlyphCanvas()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		SerializedProperty dataProperty   = glyphProperty.FindPropertyRelative("m_Glyph");
		SerializedProperty widthProperty  = glyphProperty.FindPropertyRelative("m_Width");
		SerializedProperty heightProperty = glyphProperty.FindPropertyRelative("m_Height");
		
		int width  = widthProperty.intValue;
		int height = heightProperty.intValue;
		
		float canvasAspect = (float)width / height;
		
		Rect canvasRect = GUILayoutUtility.GetRect(250, 250);
		
		canvasRect = canvasRect.Fit(canvasAspect, Alignment.MiddleLeft);
		
		float cellSize = Mathf.Min(
			canvasRect.width / width,
			canvasRect.height / height
		);
		
		GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
		labelStyle.padding   = new RectOffset();
		labelStyle.margin    = new RectOffset();
		labelStyle.overflow  = new RectOffset();
		labelStyle.alignment = TextAnchor.UpperLeft;
		labelStyle.fontSize  = 10;
		
		for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			int index = x + width * y;
			
			SerializedProperty cellProperty = dataProperty.GetArrayElementAtIndex(index);
			
			Rect cellRect = new Rect(
				canvasRect.x + cellSize * x,
				canvasRect.y + cellSize * y,
				cellSize,
				cellSize
			);
			
			bool cellValue = cellProperty.boolValue;
			
			Handles.DrawSolidRectangleWithOutline(
				cellRect,
				cellValue
					? Color.black
					: Color.white,
				Color.black
			);
			
			labelStyle.normal.textColor = cellValue ? Color.white : Color.black;
			
			GUI.contentColor = Color.white;
			
			switch (Event.current.type)
			{
				case EventType.MouseDown:
				{
					Vector2 position = Event.current.mousePosition;
					
					if (Event.current.button == 0 && cellRect.Contains(position))
					{
						m_CellValue = cellProperty.boolValue;
						cellProperty.boolValue = !m_CellValue;
						
						if (m_Preview != null)
						{
							m_Preview.SetPixel(x, height - y - 1, cellProperty.boolValue ? Color.black : Color.clear);
							m_Preview.Apply();
						}
					}
					
					break;
				}
				case EventType.MouseMove:
				case EventType.MouseDrag:
				{
					Vector2 position = Event.current.mousePosition;
					
					if (Event.current.button == 0 && cellRect.Contains(position))
					{
						cellProperty.boolValue = !m_CellValue;
						
						if (m_Preview != null)
						{
							m_Preview.SetPixel(x, height - y - 1, cellProperty.boolValue ? Color.black : Color.clear);
							m_Preview.Apply();
						}
					}
					
					break;
				}
			}
		}
	}

	void DrawGlyphControl()
	{
		EditorGUILayout.BeginHorizontal();
		
		GUILayout.Space(50);
		
		EditorGUILayout.BeginVertical();
		
		if (DuplicateShortcut())
			DuplicateGlyph();
		
		if (CopyShortcut())
			CopyGlyph();
		
		if (PasteShortcut())
			PasteGlyph();
		
		if (GUILayout.Button("Clear"))
			ClearGlyph();
		
		if (DeleteShortcut())
			RemoveGlyph();
		
		EditorGUILayout.EndVertical();
		
		GUILayout.Space(50);
		
		EditorGUILayout.EndHorizontal();
	}

	void SortGlyphs()
	{
		BitmapFont font = target as BitmapFont;
		
		if (font == null)
			return;
		
		serializedObject.ApplyModifiedProperties();
		
		SerializedProperty glyphsProperty = serializedObject.FindProperty("m_Glyphs");
		string name = null;
		if (m_SelectedGlyph >= 0 && m_SelectedGlyph < glyphsProperty.arraySize)
		{
			SerializedProperty glyphProperty = glyphsProperty.GetArrayElementAtIndex(m_SelectedGlyph);
			SerializedProperty nameProperty = glyphProperty.FindPropertyRelative("m_Name");
			name = nameProperty.stringValue;
		}
		
		font.Sort();
		
		serializedObject.Update();
		
		if (!string.IsNullOrEmpty(name))
		{
			for (int i = 0; i < glyphsProperty.arraySize; i++)
			{
				SerializedProperty glyphProperty = glyphsProperty.GetArrayElementAtIndex(i);
				SerializedProperty nameProperty  = glyphProperty.FindPropertyRelative("m_Name");
				
				if (name == nameProperty.stringValue)
				{
					m_SelectedGlyph = i;
					break;
				}
			}
		}
	}

	void DuplicateGlyph()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty != null)
			glyphProperty.DuplicateCommand();
	}

	void ClearGlyph()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		SerializedProperty nameProperty = glyphProperty.FindPropertyRelative("m_Name");
		
		string glyphName = nameProperty.stringValue;
		
		bool confirm = EditorUtility.DisplayDialog(
			$"Clear '{glyphName}' glyph",
			"Are you sure?",
			"Yes",
			"No"
		);
		
		if (!confirm)
			return;
		
		SerializedProperty dataProperty = glyphProperty.FindPropertyRelative("m_Glyph");
		
		for (int i = 0; i < dataProperty.arraySize; i++)
		{
			SerializedProperty cellProperty = dataProperty.GetArrayElementAtIndex(i);
			
			cellProperty.boolValue = false;
		}
	}

	bool DuplicateShortcut()
	{
		return Event.current.command && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D;
	}

	bool CopyShortcut()
	{
		return Event.current.command && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C;
	}

	bool PasteShortcut()
	{
		return Event.current.command && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V;
	}

	bool DeleteShortcut()
	{
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
			return true;
		
		if (Event.current.command && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Backspace)
			return true;
		
		return false;
	}

	void SelectGlyph(int _SelectedGlyph)
	{
		m_SelectedGlyph = m_SelectedGlyph != _SelectedGlyph ? _SelectedGlyph : -1;
		
		SerializedProperty glyphsProperty = serializedObject.FindProperty("m_Glyphs");
		
		if (m_SelectedGlyph < 0 || m_SelectedGlyph >= glyphsProperty.arraySize)
			return;
		
		SerializedProperty glyphProperty   = glyphsProperty.GetArrayElementAtIndex(m_SelectedGlyph);
		SerializedProperty aliasesProperty = glyphProperty.FindPropertyRelative("m_Aliases");
		SerializedProperty nameProperty    = glyphProperty.FindPropertyRelative("m_Name");
		
		m_Aliases = new ReorderableList(
			glyphProperty.serializedObject,
			aliasesProperty,
			true,
			true,
			true,
			true
		);
		m_Aliases.drawHeaderCallback = _Rect =>
		{
			Rect titleRect = new Rect(
				_Rect.x,
				_Rect.y,
				_Rect.width - 100,
				_Rect.height
			);
			
			Rect fillButtonRect = new Rect(
				_Rect.x + _Rect.width - 100,
				_Rect.y,
				50,
				_Rect.height - 1
			);
			
			Rect clearButtonRect = new Rect(
				_Rect.x + _Rect.width - 50,
				_Rect.y,
				50,
				_Rect.height - 1
			);
			
			EditorGUI.LabelField(titleRect, aliasesProperty.displayName, EditorStyles.whiteBoldLabel);
			
			if (GUI.Button(fillButtonRect, "FILL", EditorStyles.miniButtonLeft))
			{
				string name = nameProperty.stringValue;
				
				if (string.IsNullOrEmpty(name) || !char.IsLetter(name, 0))
					return;
				
				aliasesProperty.ClearArray();
				aliasesProperty.arraySize                             = 2;
				aliasesProperty.GetArrayElementAtIndex(0).stringValue = name[0].ToString().ToUpper();
				aliasesProperty.GetArrayElementAtIndex(1).stringValue = name[0].ToString().ToLower();
				aliasesProperty.serializedObject.ApplyModifiedProperties();
			}
			if (GUI.Button(clearButtonRect, "CLEAR", EditorStyles.miniButtonRight))
			{
				aliasesProperty.ClearArray();
				aliasesProperty.serializedObject.ApplyModifiedProperties();
			}
		};
		m_Aliases.drawNoneElementCallback = _Rect =>
		{
			EditorGUI.LabelField(_Rect, $"Press '+' to add alias for '{nameProperty.stringValue}'");
		};
		m_Aliases.drawElementCallback = (_Rect, _Index, _Active, _Focused) =>
		{
			Rect aliasRect = new Rect(
				_Rect.x,
				_Rect.y,
				50,
				_Rect.height
			);
			
			Rect codeRect = new Rect(
				_Rect.x + 50,
				_Rect.y,
				_Rect.width - 50,
				_Rect.height
			);
			
			SerializedProperty aliasProperty = aliasesProperty.GetArrayElementAtIndex(_Index);
			
			EditorGUI.PropertyField(
				new RectOffset(0, 0, 1, 1).Remove(aliasRect),
				aliasProperty,
				GUIContent.none
			);
			
			string alias = aliasProperty.stringValue;
			
			if (string.IsNullOrEmpty(alias))
				return;
			
			byte[] codes = Encoding.Unicode.GetBytes(alias);
			
			if (codes.Length == 0)
				return;
			
			EditorGUI.LabelField(
				new RectOffset(0, 0, 1, 1).Remove(codeRect),
				$"Unicode: {codes[0]}"
			);
		};
		
		m_Preview = CreateGlyphTexture(glyphProperty, Color.black, Color.clear);
	}

	void CopyGlyph()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		SerializedProperty dataProperty   = glyphProperty.FindPropertyRelative("m_Glyph");
		SerializedProperty widthProperty  = glyphProperty.FindPropertyRelative("m_Width");
		SerializedProperty heightProperty = glyphProperty.FindPropertyRelative("m_Height");
		
		StringBuilder glyphData = new StringBuilder();
		glyphData.Append("glyph:");
		glyphData.Append(widthProperty.intValue).Append(":");
		glyphData.Append(heightProperty.intValue).Append(":");
		
		for (int i = 0; i < dataProperty.arraySize; i++)
		{
			SerializedProperty cellProperty = dataProperty.GetArrayElementAtIndex(i);
			
			if (cellProperty.boolValue)
				glyphData.Append("1");
			else
				glyphData.Append("0");
		}
		
		EditorGUIUtility.systemCopyBuffer = glyphData.ToString();
	}

	void PasteGlyph()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		string[] glyphData = EditorGUIUtility.systemCopyBuffer.Split(
			new char[] { ':' },
			StringSplitOptions.RemoveEmptyEntries
		);
		
		if (glyphData.Length != 4)
			return;
		
		string header = glyphData[0];
		
		if (!header.Equals("glyph", StringComparison.OrdinalIgnoreCase))
			return;
		
		int width;
		if (!int.TryParse(glyphData[1], out width))
			return;
		
		int height;
		if (!int.TryParse(glyphData[2], out height))
			return;
		
		string glyph = glyphData[3];
		if (string.IsNullOrEmpty(glyph))
			return;
		
		SerializedProperty dataProperty   = glyphProperty.FindPropertyRelative("m_Glyph");
		SerializedProperty widthProperty  = glyphProperty.FindPropertyRelative("m_Width");
		SerializedProperty heightProperty = glyphProperty.FindPropertyRelative("m_Height");
		
		dataProperty.arraySize = glyph.Length;
		widthProperty.intValue  = width;
		heightProperty.intValue = height;
		
		for (int i = 0; i < dataProperty.arraySize; i++)
		{
			SerializedProperty cellProperty = dataProperty.GetArrayElementAtIndex(i);
			
			cellProperty.boolValue = glyph[i] == '1';
		}
	}

	void RemoveGlyph()
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		SerializedProperty nameProperty = glyphProperty.FindPropertyRelative("m_Name");
		
		string glyphName = nameProperty.stringValue;
		
		bool confirm = EditorUtility.DisplayDialog(
			$"Remove '{glyphName}' glyph",
			"Are you sure?",
			"Yes",
			"No"
		);
		
		if (!confirm)
			return;
		
		glyphProperty.DeleteCommand();
	}

	void MoveGlyph(int _X, int _Y)
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		SerializedProperty dataProperty   = glyphProperty.FindPropertyRelative("m_Glyph");
		SerializedProperty widthProperty  = glyphProperty.FindPropertyRelative("m_Width");
		SerializedProperty heightProperty = glyphProperty.FindPropertyRelative("m_Height");
		
		int    width  = widthProperty.intValue;
		int    height = heightProperty.intValue;
		byte[] glyph  = new byte[width * height];
		
		for (int x = 0; x < width; x++)
		for (int y = 0; y < height; y++)
		{
			if (x + _X < 0 || x + _X >= width || y + _Y < 0 || y + _Y >= height)
				continue;
			
			int source = x + y * width;
			int target = (x + _X) + (y + _Y) * width;
			
			SerializedProperty pixel = dataProperty.GetArrayElementAtIndex(source);
			
			glyph[target] = (byte)pixel.intValue;
		}
		
		for (int x = 0; x < width; x++)
		for (int y = 0; y < height; y++)
		{
			int index = x + y * width;
			
			SerializedProperty pixel = dataProperty.GetArrayElementAtIndex(index);
			
			pixel.intValue = glyph[index];
		}
	}

	void ResizeGlyph(int _L, int _R, int _T, int _B)
	{
		SerializedProperty glyphProperty = SelectedGlyph;
		
		if (glyphProperty == null)
			return;
		
		MoveGlyph(-_L, -_T);
		
		SerializedProperty dataProperty   = glyphProperty.FindPropertyRelative("m_Glyph");
		SerializedProperty widthProperty  = glyphProperty.FindPropertyRelative("m_Width");
		SerializedProperty heightProperty = glyphProperty.FindPropertyRelative("m_Height");
		
		int sourceWidth  = widthProperty.intValue;
		int sourceHeight = heightProperty.intValue;
		int targetWidth  = sourceWidth - _L - _R;
		int targetHeight = sourceHeight - _T - _B;
		
		int width  = Mathf.Min(sourceWidth, targetWidth);
		int height = Mathf.Min(sourceHeight, targetHeight);
		
		bool[] glyph = new bool[targetWidth * targetHeight];
		
		widthProperty.intValue  = targetWidth;
		heightProperty.intValue = targetHeight;
		
		for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			int source = x + y * sourceWidth;
			int target = x + y * targetWidth;
			
			SerializedProperty cellProperty = dataProperty.GetArrayElementAtIndex(source);
			
			glyph[target] = cellProperty.boolValue;
		}
		
		dataProperty.ClearArray();
		
		for (int i = 0; i < glyph.Length; i++)
		{
			dataProperty.InsertArrayElementAtIndex(i);
			
			SerializedProperty cellProperty = dataProperty.GetArrayElementAtIndex(i);
			
			cellProperty.boolValue = glyph[i];
		}
	}

	void Bake()
	{
		BitmapFont font = target as BitmapFont;
		
		if (font != null)
		{
			font.Sort();
			font.Reset();
		}
		
		serializedObject.Update();
		
		string path = AssetDatabase.GetAssetPath(font);
		
		AssetDatabase.LoadAllAssetsAtPath(path)
			.OfType<Texture2D>()
			.ToList()
			.ForEach(AssetDatabase.RemoveObjectFromAsset);
		
		SerializedProperty textureProperty = serializedObject.FindProperty("m_Texture");
		SerializedProperty glyphsProperty  = serializedObject.FindProperty("m_Glyphs");
		
		Texture2D atlas = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		
		atlas.name       = "atlas";
		atlas.filterMode = FilterMode.Point;
		atlas.wrapMode   = TextureWrapMode.Clamp;
		
		Texture2D[] glyphs = new Texture2D[glyphsProperty.arraySize];
		for (int i = 0; i < glyphsProperty.arraySize; i++)
		{
			SerializedProperty glyphProperty = glyphsProperty.GetArrayElementAtIndex(i);
			
			glyphs[i] = CreateGlyphTexture(glyphProperty);
		}
		
		AssetDatabase.AddObjectToAsset(atlas, target);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		textureProperty.objectReferenceValue = atlas;
		
		Rect[] uvs = atlas.PackTextures(glyphs, 1, 2048);
		for (int i = 0; i < glyphsProperty.arraySize; i++)
		{
			SerializedProperty glyphProperty = glyphsProperty.GetArrayElementAtIndex(i);
			SerializedProperty uvProperty    = glyphProperty.FindPropertyRelative("m_UV");
			
			uvProperty.rectValue = uvs[i];
		}
	}

	Texture2D CreateGlyphTexture(SerializedProperty _GlyphProperty)
	{
		return CreateGlyphTexture(_GlyphProperty, Color.white, Color.black);
	}

	Texture2D CreateGlyphTexture(SerializedProperty _GlyphProperty, Color _EnabledColor, Color _DisabledColor)
	{
		if (_GlyphProperty == null)
			return null;
		
		SerializedProperty dataProperty   = _GlyphProperty.FindPropertyRelative("m_Glyph");
		SerializedProperty widthProperty  = _GlyphProperty.FindPropertyRelative("m_Width");
		SerializedProperty heightProperty = _GlyphProperty.FindPropertyRelative("m_Height");
		
		int width  = widthProperty.intValue;
		int height = heightProperty.intValue;
		
		Texture2D glyph = new Texture2D(width, height, TextureFormat.ARGB32, false);
		
		glyph.name       = "glyph";
		glyph.filterMode = FilterMode.Point;
		glyph.wrapMode   = TextureWrapMode.Clamp;
		
		for (int y = 0; y < height; y++)
		for (int x = 0; x < width; x++)
		{
			int index = x + width * y;
			
			SerializedProperty cellProperty = dataProperty.GetArrayElementAtIndex(index);
			
			Color color = cellProperty.boolValue
				? _EnabledColor
				: _DisabledColor;
			
			glyph.SetPixel(x, height - y - 1, color);
		}
		
		glyph.Apply();
		
		return glyph;
	}
}
