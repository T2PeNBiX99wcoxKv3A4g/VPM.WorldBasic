using io.github.ykysnk.utils.Editor;
using io.github.ykysnk.WorldBasic.Udon;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(PlayerGuid))]
[CanEditMultipleObjects]
public class PlayerGuidEditor : BasicEditor
{
    private const string LogNameColorProp = "logNameColor";

    private SerializedProperty? _logNameColor;

    protected override void OnEnable()
    {
        _logNameColor = serializedObject.FindProperty(LogNameColorProp);
    }

    protected override void OnInspectorGUIDraw()
    {
        var count = FindObjectsOfType<PlayerGuid>().Length;

        if (count > 1)
            EditorGUILayout.HelpBox($"More than one {nameof(PlayerGuid)} found in scene.", MessageType.Warning);

        EditorGUILayout.PropertyField(_logNameColor, Utils.Label("Log Name Color"));
        EditorGUILayout.HelpBox("This component will manage all players guid.", MessageType.Info, true);
    }
}