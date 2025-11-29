using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomPropertyDrawer(typeof(ColorHex))]
public class ColorHexDrawer : PropertyDrawer
{
    private Color _color;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var colorProperty = property.FindPropertyRelative("color");
        var colorHexProperty = property.FindPropertyRelative("hex");

        EditorGUI.BeginChangeCheck();

        _color = EditorGUI.ColorField(position, colorProperty.colorValue);

        if (EditorGUI.EndChangeCheck())
        {
            colorProperty.colorValue = _color;
            colorHexProperty.stringValue = $"#{ColorUtility.ToHtmlStringRGBA(_color)}";
        }

        EditorGUI.EndProperty();
    }
}