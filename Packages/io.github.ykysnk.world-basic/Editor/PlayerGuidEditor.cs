using io.github.ykysnk.Localization.Editor;
using io.github.ykysnk.WorldBasic.Udon;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(PlayerGuid))]
[CanEditMultipleObjects]
internal class PlayerGuidEditor : WorldBasicEditor
{
    private const string LogNameColorProp = "logNameColor";

    private SerializedProperty? _logNameColor;

    protected override void OnEnable()
    {
        _logNameColor = serializedObject.FindProperty(LogNameColorProp);
    }

    protected override void OnWorldBasicInspectorGUI()
    {
        var count = FindObjectsOfType<PlayerGuid>().Length;

        if (count > 1)
            EditorGUILayout.HelpBox("label.player_guid.warning".L(LocalizationID), MessageType.Warning);

        EditorGUILayout.PropertyField(_logNameColor, "label.player_guid.log_name_color".G(LocalizationID));
        EditorGUILayout.HelpBox("label.player_guid.info".L(LocalizationID), MessageType.Info, true);
    }
}