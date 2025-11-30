using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomPropertyDrawer(typeof(UniqueID))]
public class UniqueIDDrawer : PropertyDrawer
{
    private const float ButtonWidth = 120;
    private const float Padding = 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        GUI.enabled = false;

        var valueRect = position;
        valueRect.width -= ButtonWidth + Padding;
        var idProperty = property.FindPropertyRelative("value");
        EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

        GUI.enabled = true;

        var buttonRect = position;
        buttonRect.x += position.width - ButtonWidth;
        buttonRect.width = ButtonWidth;

        if (GUI.Button(buttonRect, "Copy to clipboard"))
            EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;

        EditorGUI.EndProperty();
    }
}