using io.github.ykysnk.Localization.Editor;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(ActiveInPlayMode))]
internal class ActiveInPlayModeEditor : WorldBasicEditor
{
    protected override void OnWorldBasicInspectorGUI()
    {
        EditorGUILayout.HelpBox("label.active_in_play_mode.info".L(LocalizationID), MessageType.Info);
    }
}