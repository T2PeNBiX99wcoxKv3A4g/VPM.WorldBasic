using io.github.ykysnk.utils.Editor;
using UnityEditor;
using EditorUtils = io.github.ykysnk.utils.Editor.Utils;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(BasicUdonSharpBehaviourHelper))]
[CanEditMultipleObjects]
public class BasicUdonSharpBehaviourHelperEditor : BasicEditor
{
    private const string IDProp = "id";

    private SerializedProperty? _id;

    protected override void OnEnable()
    {
        _id = serializedObject.FindProperty(IDProp);
    }

    protected override void OnInspectorGUIDraw()
    {
        EditorGUILayout.PropertyField(_id, EditorUtils.Label("ID"));
    }
}