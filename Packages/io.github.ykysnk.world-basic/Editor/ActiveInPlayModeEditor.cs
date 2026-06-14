using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(ActiveInPlayMode))]
internal class ActiveInPlayModeEditor : WorldBasicEditor
{
    protected override void OnWorldBasicInspectorGUI()
    {
        EditorGUILayout.HelpBox("label.active_in_play_mode.info".S(), MessageType.Info);
    }
}