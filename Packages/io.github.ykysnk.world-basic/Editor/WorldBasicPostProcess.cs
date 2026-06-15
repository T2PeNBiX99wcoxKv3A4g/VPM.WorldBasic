using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace io.github.ykysnk.WorldBasic.Editor;

public class WorldBasicPostProcess : IProcessSceneWithReport
{
    public int callbackOrder => -100;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        ActiveInPlayModeProcess();
        InactiveInPlayModeProcess();
    }

    private static void ActiveInPlayModeProcess()
    {
        var activeInPlayModes = Object.FindObjectsOfType<ActiveInPlayMode>(true) ?? new ActiveInPlayMode[]
        {
        };

        foreach (var activeInPlayMode in activeInPlayModes)
            activeInPlayMode.gameObject.SetActive(true);
    }

    private static void InactiveInPlayModeProcess()
    {
        var inactiveInPlayModes = Object.FindObjectsOfType<InactiveInPlayMode>(true) ?? new InactiveInPlayMode[]
        {
        };

        foreach (var inactiveInPlayMode in inactiveInPlayModes)
            inactiveInPlayMode.gameObject.SetActive(false);
    }
}