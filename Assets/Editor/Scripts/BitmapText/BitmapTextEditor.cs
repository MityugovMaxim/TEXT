using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BitmapText))]
public class BitmapTextEditor : Editor
{
	TextEditor TextEditor
	{
		get
		{
			Type type = typeof(EditorGUI);
			
			FieldInfo textEditor = type.GetField("activeEditor", BindingFlags.Static | BindingFlags.NonPublic);
			
			return textEditor != null ? textEditor.GetValue(null) as TextEditor : null;
		}
	}

	public static GUIStyle TextStyle
	{
		get
		{
			if (m_TextStyle == null)
			{
				m_TextStyle               = new GUIStyle(EditorStyles.textArea);
				m_TextStyle.wordWrap      = true;
				m_TextStyle.clipping      = TextClipping.Clip;
				m_TextStyle.stretchWidth  = true;
				m_TextStyle.stretchHeight = true;
				m_TextStyle.fixedWidth    = 0;
				m_TextStyle.fixedHeight   = 0;
			}
			return m_TextStyle;
		}
	}

	BitmapText BitmapText
	{
		get { return target as BitmapText; }
	}

	static GUIStyle m_TextStyle;

	ReorderableList m_Processors;

	public override void OnInspectorGUI()
	{
		serializedObject.UpdateIfRequiredOrScript();
		SerializedProperty property = serializedObject.GetIterator();
		for (bool enterChildren = true; property.NextVisible(enterChildren); enterChildren = false)
		{
			switch (property.propertyPath)
			{
				case "m_OnCullStateChanged":
				case "m_Material":
					continue;
			}
			EditorGUILayout.PropertyField(property, true);
		}
		
		DrawFont();
		
		DrawMaterial();
		
		DrawText();
		
		DrawAlignment();
		
		DrawCharSize();
		
		DrawCharSpacing();
		
		DrawLineSpacing();
		
		serializedObject.ApplyModifiedProperties();
	}

	void DrawFont()
	{
		SerializedProperty fontProperty = serializedObject.FindProperty("m_Font");
		
		EditorGUILayout.PropertyField(fontProperty, true);
	}

	void DrawMaterial()
	{
		SerializedProperty materialProperty = serializedObject.FindProperty("m_Material");
		
		EditorGUILayout.PropertyField(materialProperty, true);
	}

	void DrawText()
	{
		SerializedProperty textProperty = serializedObject.FindProperty("m_Text");
		
		textProperty.stringValue = EditorGUILayout.TextArea(
			textProperty.stringValue,
			TextStyle,
			GUILayout.MinHeight(100)
		);
	}

	void DrawAlignment()
	{
		SerializedProperty alignmentProperty = serializedObject.FindProperty("m_Alignment");
		
		EditorGUILayout.PropertyField(alignmentProperty, true);
	}

	void DrawCharSize()
	{
		SerializedProperty charSizeProperty = serializedObject.FindProperty("m_CharSize");
		
		charSizeProperty.floatValue = EditorGUILayout.FloatField(
			charSizeProperty.displayName,
			charSizeProperty.floatValue
		);
		
		charSizeProperty.floatValue = Mathf.Max(0, charSizeProperty.floatValue);
	}

	void DrawCharSpacing()
	{
		SerializedProperty charSpacingProperty = serializedObject.FindProperty("m_CharSpacing");
		
		charSpacingProperty.floatValue = EditorGUILayout.FloatField(
			charSpacingProperty.displayName,
			charSpacingProperty.floatValue
		);
	}

	void DrawLineSpacing()
	{
		SerializedProperty lineGapProperty = serializedObject.FindProperty("m_LineSpacing");
		
		lineGapProperty.floatValue = EditorGUILayout.FloatField(
			lineGapProperty.displayName,
			lineGapProperty.floatValue
		);
	}
}
