using io.github.ykysnk.Localization.Editor;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(InactiveInPlayMode))]
internal class InactiveInPlayModeEditor : WorldBasicEditor
{
    protected override void OnWorldBasicInspectorGUI()
    {
        EditorGUILayout.HelpBox("label.inactive_in_play_mode.info".L(LocalizationID), MessageType.Info);
    }
}