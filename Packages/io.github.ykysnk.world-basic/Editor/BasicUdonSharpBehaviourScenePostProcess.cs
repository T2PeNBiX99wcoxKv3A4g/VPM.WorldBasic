using io.github.ykysnk.WorldBasic.Udon;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

namespace io.github.ykysnk.WorldBasic.Editor;

public static class BasicUdonSharpBehaviourScenePostProcess
{
    private static BasicUdonSharpBehaviour[] _basicUdonSharpBehaviours =
    {
    };

    [PostProcessScene(-100)]
    public static void ScenePostProcess()
    {
        _basicUdonSharpBehaviours = Object.FindObjectsOfType<BasicUdonSharpBehaviour>(true);
        if (_basicUdonSharpBehaviours.Length < 1) return;
        SetPlayerGuid();
        UploadAllID();
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

    private static void UploadAllID()
    {
        foreach (var basicUdonSharpBehaviour in _basicUdonSharpBehaviours)
            basicUdonSharpBehaviour.UpdateID();
    }

    // private static void SetStatusText()
    // {
    //     var tempSyncStatusTexts = new Dictionary<StatusText, List<StatusText>>();
    //
    //     foreach (var statusText in _statusTexts)
    //     {
    //         if (!Utilities.IsValid(statusText.syncStatusText)) continue;
    //         var syncStatusText = statusText.syncStatusText;
    //         if (!tempSyncStatusTexts.ContainsKey(syncStatusText))
    //             tempSyncStatusTexts.Add(syncStatusText, new List<StatusText>());
    //         tempSyncStatusTexts[syncStatusText].Add(statusText);
    //     }
    //
    //     foreach (var statusTextPair in tempSyncStatusTexts)
    //         statusTextPair.Key.needSyncStatusTexts = statusTextPair.Value.ToArray();
    // }

    // private static void SetActiveOnStart()
    // {
    //     if (!Utilities.IsValid(_activeOnStarts))
    //     {
    //         LogWarning("_activeOnStarts is not a valid ActiveOnStart");
    //         return;
    //     }
    //
    //     foreach (var activeOnStart in _activeOnStarts)
    //         activeOnStart.gameObject.SetActive(true);
    // }
}