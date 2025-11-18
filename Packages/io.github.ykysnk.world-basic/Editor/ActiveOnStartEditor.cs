using io.github.ykysnk.utils.Editor;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(ActiveOnStart))]
public class ActiveOnStartEditor : BasicEditor
{
    protected override void OnInspectorGUIDraw()
    {
        EditorGUILayout.HelpBox("The object will be active on play mode", MessageType.Info);
    }
}