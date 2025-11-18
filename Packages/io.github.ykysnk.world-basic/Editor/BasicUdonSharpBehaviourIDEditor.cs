using io.github.ykysnk.utils.Editor;
using UnityEditor;
using EditorUtils = io.github.ykysnk.utils.Editor.Utils;
using Utils = io.github.ykysnk.utils.Utils;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(BasicUdonSharpBehaviourID))]
public class BasicUdonSharpBehaviourIDEditor : BasicEditor
{
    private const string IDProp = "id";

    private SerializedProperty? _id;

    protected override void OnEnable()
    {
        _id = serializedObject.FindProperty(IDProp);
    }

    protected override void OnInspectorGUIDraw()
    {
        if (!Utils.IsInPrefab())
            EditorGUILayout.PropertyField(_id, EditorUtils.Label("ID"));
        else
            EditorGUILayout.HelpBox("Will not show up in prefab!", MessageType.Warning, true);
    }
}