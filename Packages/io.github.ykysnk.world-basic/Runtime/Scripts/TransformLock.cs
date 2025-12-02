using io.github.ykysnk.utils;
using io.github.ykysnk.utils.NonUdon;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic
{
    [DisallowMultipleComponent]
    [AddComponentMenu("yky/World Basic/Transform Lock")]
    [ExecuteInEditMode]
    public class TransformLock : BasicEditorComponent
    {
        [SerializeField] private Vector3 lockPosition = Vector3.zero;
        [SerializeField] private Vector3 lockRotation = Vector3.zero;
        [SerializeField] private Vector3 lockScale = Vector3.one;
        [SerializeField] private BooleanVector3 isLockPosition = BooleanVector3.True;
        [SerializeField] private BooleanVector3 isLockRotation = BooleanVector3.True;
        [SerializeField] private BooleanVector3 isLockScale = BooleanVector3.True;

        private void Update()
        {
            if (!gameObject.scene.IsValid() || Utils.IsInPrefab() || Utils.IsPlaying()) return;

            var oldPosition = transform.localPosition;
            var newPosition = oldPosition;

            if (isLockPosition.x) newPosition.x = lockPosition.x;
            if (isLockPosition.y) newPosition.y = lockPosition.y;
            if (isLockPosition.z) newPosition.z = lockPosition.z;

            if (newPosition != oldPosition) transform.localPosition = newPosition;

            var oldRotation = transform.localEulerAngles;
            var newRotation = oldRotation;

            if (isLockRotation.x) newRotation.x = lockRotation.x;
            if (isLockRotation.y) newRotation.y = lockRotation.y;
            if (isLockRotation.z) newRotation.z = lockRotation.z;

            if (newRotation != oldRotation) transform.localEulerAngles = newRotation;

            var oldScale = transform.localScale;
            var newScale = oldScale;

            if (isLockScale.x) newScale.x = lockScale.x;
            if (isLockScale.y) newScale.y = lockScale.y;
            if (isLockScale.z) newScale.z = lockScale.z;

            if (newScale != oldScale) transform.localScale = newScale;
        }
    }
}