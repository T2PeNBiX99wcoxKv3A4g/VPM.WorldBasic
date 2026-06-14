using io.github.ykysnk.Localization.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic.Editor;

[PublicAPI]
internal static class InternalLocalizationExtensions
{
    internal static LocalizationHelper Helper => new("io.github.ykysnk.world-basic");

    internal static string S(this string key) => Helper.S(key);
    internal static string Sf(this string key, params object?[] args) => Helper.Sf(key, args);
    internal static GUIContent G(this string key) => Helper.G(key);
    internal static GUIContent G(this SerializedProperty property) => Helper.G(property);
    internal static string S(this SerializedProperty property) => Helper.S(property);

    internal static void Register(this SerializedProperty property, UpdateHelper.Callback callback) =>
        Helper.UpdateRegister(property, callback);

    internal static void Register(this string localizeKey, UpdateHelper.Callback callback) =>
        Helper.UpdateRegister(localizeKey, callback);
}