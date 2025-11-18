using io.github.ykysnk.utils.Editor;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(DisableTransformChange))]
public class DisableTransformChangeEditor : BasicEditor
{
    protected override void OnInspectorGUIDraw()
    {
        EditorGUILayout.HelpBox("The object transform change is disable with this component", MessageType.Info);
    }
}