using System;
using UnityEditor;

namespace io.github.ykysnk.WorldBasic.Editor;

[CustomEditor(typeof(BasicUdonSharpBehaviourHelper))]
[CanEditMultipleObjects]
[Obsolete("This class is no longer needed and will be removed in a future release.")]
internal class BasicUdonSharpBehaviourHelperEditor : WorldBasicEditor
{
    protected override void OnEnable()
    {
    }

    protected override void OnWorldBasicInspectorGUI()
    {
    }
}