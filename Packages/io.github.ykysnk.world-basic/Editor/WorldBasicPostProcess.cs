using System.Linq;
using io.github.ykysnk.WorldBasic.Udon;
using UdonSharp;
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
        PlayerGuidProcess();
        ActiveInPlayModeProcess();
    }

    private static void PlayerGuidProcess()
    {
        var playerGuids = Object.FindObjectsOfType<PlayerGuid>();

        foreach (var playerGuid in Object.FindObjectsOfType<UdonSharpBehaviour>(true).OfType<IPlayerGuid>())
        {
            switch (playerGuids.Length)
            {
                case < 1:
                    throw new("No PlayerGuid found in scene.");
                case > 1:
                    throw new("More than one PlayerGuid found in scene.");
            }

            playerGuid.PlayerGuid = playerGuids[0];
        }
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