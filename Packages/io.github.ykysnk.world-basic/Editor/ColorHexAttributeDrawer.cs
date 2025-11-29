using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomPropertyDrawer(typeof(ColorHexAttribute))]
public class ColorHexAttributeDrawer : PropertyDrawer
{
    private Color _color;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
            throw new($"{nameof(ColorHexAttribute)} can only be used on string field");

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var hexValue = property.stringValue;

        _color = ColorUtility.TryParseHtmlString(hexValue, out var color) ? color : Color.white;

        EditorGUI.BeginChangeCheck();

        _color = EditorGUI.ColorField(position, _color);

        if (EditorGUI.EndChangeCheck())
            property.stringValue = $"#{ColorUtility.ToHtmlStringRGBA(_color)}";

        EditorGUI.EndProperty();
    }
}