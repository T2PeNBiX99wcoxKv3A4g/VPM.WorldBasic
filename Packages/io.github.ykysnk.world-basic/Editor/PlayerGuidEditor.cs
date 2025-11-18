using io.github.ykysnk.utils.Editor;
using io.github.ykysnk.WorldBasic.Udon;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(PlayerGuid))]
public class PlayerGuidEditor : BasicEditor
{
    protected override void OnInspectorGUIDraw()
    {
        EditorGUILayout.HelpBox("This component will manage all players guid.", MessageType.Info, true);
    }
}