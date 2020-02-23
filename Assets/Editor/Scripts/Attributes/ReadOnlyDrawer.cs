using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect _Rect, SerializedProperty _Property, GUIContent _Label)
	{
		GUI.enabled = false;
		
		EditorGUI.PropertyField(_Rect, _Property, _Label);
		
		GUI.enabled = false;
	}
}