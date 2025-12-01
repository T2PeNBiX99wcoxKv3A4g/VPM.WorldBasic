using io.github.ykysnk.utils;
using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomPropertyDrawer(typeof(UniqueIDAttribute))]
public class UniqueIDAttributeDrawer : PropertyDrawer
{
    private const float ButtonWidth = 65;
    private const float Padding = 2;
    private const string InPrefabModeTooltip = "In prefab mode";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
            throw new($"{nameof(UniqueIDAttribute)} can only be used on string field");

        EditorGUI.BeginProperty(position, label, property);

        // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        GUI.enabled = false;
        var valueRect = position;
        valueRect.width -= ButtonWidth + Padding;
        EditorGUI.TextField(valueRect, Utils.IsInPrefab() ? InPrefabModeTooltip : property.stringValue);
        GUI.enabled = true;

        var buttonRect = position;
        buttonRect.x += position.width - ButtonWidth;
        buttonRect.width = ButtonWidth;

        if (GUI.Button(buttonRect, "Copy ID"))
            EditorGUIUtility.systemCopyBuffer = Utils.IsInPrefab() ? InPrefabModeTooltip : property.stringValue;

        EditorGUI.EndProperty();
    }
}