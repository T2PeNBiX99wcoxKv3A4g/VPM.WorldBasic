using System;
using System.Linq;
using io.github.ykysnk.utils;
using io.github.ykysnk.utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace io.github.ykysnk.WorldBasic
{
    // Refs: https://www.reddit.com/r/Unity3D/comments/fdc2on/easily_generate_unique_ids_for_your_game_objects/
    [DisallowMultipleComponent]
    [PublicAPI]
    public sealed class BasicUdonSharpBehaviourHelper : BasicEditorComponent
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

        private static void Log(object message) => Utils.Log(nameof(BasicUdonSharpBehaviourHelper), message);

        private static void Log(object message, Object context) =>
            Utils.Log(nameof(BasicUdonSharpBehaviourHelper), message, context);

        [ContextMenu("Force reset ID")]
        public void ResetId()
        {
            if (Utils.IsInPrefab()) return;
            ID = Guid.NewGuid().ToString();
            Log("Setting new ID on object: " + gameObject.FullName(), gameObject);
        }

        // Need to check for duplicates when copying a gameobject/component
        public static bool IsUnique(string id) =>
            FindObjectsOfType<BasicUdonSharpBehaviourHelper>(true).Count(x => x.ID == id) == 1;

        private void UpdateOrResetID()
        {
            if (Utils.IsInPrefab()) return;
            if (string.IsNullOrEmpty(ID) || !IsUnique(ID))
                ResetId();
        }
    }
}