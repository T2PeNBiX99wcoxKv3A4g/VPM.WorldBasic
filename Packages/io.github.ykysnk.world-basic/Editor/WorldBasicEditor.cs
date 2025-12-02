using io.github.ykysnk.Localization.Editor;
using io.github.ykysnk.utils.Editor;
using JetBrains.Annotations;

namespace io.github.ykysnk.WorldBasic.Editor;

[PublicAPI]
internal abstract class WorldBasicEditor : BasicEditor
{
    internal const string LocalizationID = "io.github.ykysnk.world-basic";

    protected override void OnInspectorGUIDraw()
    {
        OnWorldBasicInspectorGUI();
        GlobalLocalization.SelectLanguageGUI(LocalizationID);
    }

    protected abstract void OnWorldBasicInspectorGUI();
}