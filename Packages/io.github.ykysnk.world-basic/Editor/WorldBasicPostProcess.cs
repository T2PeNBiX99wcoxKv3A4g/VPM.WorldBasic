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
        SetPlayerGuid();
        SetActiveOnStart();
    }

    private static void SetPlayerGuid()
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

    private static void SetActiveOnStart()
    {
        var activeOnStarts = Object.FindObjectsOfType<ActiveOnStart>(true) ?? new ActiveOnStart[]
        {
        };

        foreach (var activeOnStart in activeOnStarts)
            activeOnStart.gameObject.SetActive(true);
    }
}