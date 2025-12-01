using io.github.ykysnk.WorldBasic.Udon;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace io.github.ykysnk.WorldBasic.Editor;

public class WorldBasicPostProcess : IProcessSceneWithReport
{
    private static BasicUdonSharpBehaviour[] _basicUdonSharpBehaviours =
    {
    };

    public int callbackOrder => -100;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        _basicUdonSharpBehaviours = Object.FindObjectsOfType<BasicUdonSharpBehaviour>(true);
        if (_basicUdonSharpBehaviours.Length < 1) return;
        SetPlayerGuid();
        SetActiveOnStart();
    }

    private static void SetPlayerGuid()
    {
        var playerGuids = Object.FindObjectsOfType<PlayerGuid>();

        switch (playerGuids.Length)
        {
            case < 1:
                throw new("No PlayerGuid found in scene.");
            case > 1:
                throw new("More than one PlayerGuid found in scene.");
        }

        foreach (var basicUdonSharpBehaviour in _basicUdonSharpBehaviours)
            basicUdonSharpBehaviour.playerGuid = playerGuids[0];
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