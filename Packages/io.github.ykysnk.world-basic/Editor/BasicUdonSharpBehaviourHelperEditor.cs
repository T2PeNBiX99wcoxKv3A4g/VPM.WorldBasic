using io.github.ykysnk.utils.Editor;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(BasicUdonSharpBehaviourHelper))]
[CanEditMultipleObjects]
internal class BasicUdonSharpBehaviourHelperEditor : WorldBasicEditor
{
    private const string IDProp = "id";

    private SerializedProperty? _id;

    protected override void OnEnable()
    {
        _id = serializedObject.FindProperty(IDProp);
    }

    protected override void OnWorldBasicInspectorGUI()
    {
        EditorGUILayout.PropertyField(_id, "ID".Label());
    }
}