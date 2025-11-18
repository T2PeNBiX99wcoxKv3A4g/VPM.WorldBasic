using io.github.ykysnk.utils;
using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic
{
    [AddComponentMenu("yky/World Basic/Disable Transform Change")]
    public class DisableTransformChange : BasicEditorComponent
    {
        private void OnValidate()
        {
            ResetTransform();
            ObjectChangeEvents.changesPublished -= OnChangesPublished;
            ObjectChangeEvents.changesPublished += OnChangesPublished;
        }

        public override void OnInspectorGUI() => ResetTransform();

        private void OnChangesPublished(ref ObjectChangeEventStream stream) => ResetTransform();

        private void ResetTransform()
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }
}