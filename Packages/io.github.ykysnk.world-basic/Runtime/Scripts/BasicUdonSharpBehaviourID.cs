using System;
using System.Linq;
using io.github.ykysnk.utils;
using io.github.ykysnk.utils.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using Object = UnityEngine.Object;

namespace io.github.ykysnk.WorldBasic
{
    // Refs: https://www.reddit.com/r/Unity3D/comments/fdc2on/easily_generate_unique_ids_for_your_game_objects/
    [DisallowMultipleComponent]
    [PublicAPI]
    public sealed class BasicUdonSharpBehaviourID : BasicEditorComponent
    {
        public delegate void IDChangedHandler(string newID);

        [SerializeField] private UniqueID id;

        public string ID
        {
            private set
            {
                id.value = value;
                OnIDChanged?.Invoke(value);
            }
            get => id.value;
        }

        private void OnValidate() => UpdateOrResetID();
        public event IDChangedHandler? OnIDChanged;

        private static void Log(object message) => Debug.Log($"[{nameof(BasicUdonSharpBehaviourID)}] {message}");

        private static void Log(object message, Object context) =>
            Debug.Log($"[{nameof(BasicUdonSharpBehaviourID)}] {message}", context);

        [ContextMenu("Force reset ID")]
        public void ResetId()
        {
            if (!Utilities.IsValid(gameObject.scene) || Utils.IsInPrefab()) return;
            ID = Guid.NewGuid().ToString();
            Log("Setting new ID on object: " + gameObject.FullName(), gameObject);
        }

        // Need to check for duplicates when copying a gameobject/component
        public static bool IsUnique(string id) =>
            Resources.FindObjectsOfTypeAll<BasicUdonSharpBehaviourID>().Count(x => x.ID == id) == 1;

        private void UpdateOrResetID()
        {
            if (!Utilities.IsValid(gameObject.scene) || Utils.IsInPrefab()) return;
            if (string.IsNullOrEmpty(ID) || !IsUnique(ID))
                ResetId();
        }

        [Serializable]
        private struct UniqueID
        {
            public string value;
        }

        [CustomPropertyDrawer(typeof(UniqueID))]
        private class UniqueIdDrawer : PropertyDrawer
        {
            private const float ButtonWidth = 120;
            private const float Padding = 2;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                GUI.enabled = false;
                var valueRect = position;
                valueRect.width -= ButtonWidth + Padding;
                var idProperty = property.FindPropertyRelative("value");
                EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

                GUI.enabled = true;

                var buttonRect = position;
                buttonRect.x += position.width - ButtonWidth;
                buttonRect.width = ButtonWidth;

                if (GUI.Button(buttonRect, "Copy to clipboard"))
                    EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;

                EditorGUI.EndProperty();
            }
        }
    }
}