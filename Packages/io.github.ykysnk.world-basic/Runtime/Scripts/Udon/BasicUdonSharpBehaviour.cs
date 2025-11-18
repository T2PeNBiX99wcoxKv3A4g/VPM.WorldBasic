using io.github.ykysnk.CheatClientProtector;
using io.github.ykysnk.LogManager;
using io.github.ykysnk.utils;
using io.github.ykysnk.utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace io.github.ykysnk.WorldBasic.Udon
{
#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [RequireComponent(typeof(BasicUdonSharpBehaviourID))]
#endif
    [PublicAPI]
    public abstract class BasicUdonSharpBehaviour : CheatClientProtectorBehaviour
#if !COMPILER_UDONSHARP && UNITY_EDITOR
        , ILogManager
#endif
    {
        private const string IsTurnOnKey = "isTurnOn";
        private const string ModeKey = "mode";
        [SerializeField] [HideInInspector] private string id;
        [HideInInspector] public PlayerGuid playerGuid;
        [HideInInspector] public LogManager.LogManager logManager;
        [SerializeField] [HideInInspector] private string logName;

        public string ID => id;
        protected virtual bool LogShowFullName => true;
        protected virtual string LogNameColor => "#D771C0";

        protected string LocalPlayerGuid =>
            Utilities.IsValid(playerGuid)
                ? playerGuid.GetLocalPlayerGuid(playerGuid.RandomKey)
                : PlayerGuid.EmptyGuid;

        protected virtual void OnValidate()
        {
#if !COMPILER_UDONSHARP && UNITY_EDITOR
            logName = GetType().Name;
            var idComponent = GetComponent<BasicUdonSharpBehaviourID>();
            idComponent.OnIDChanged -= OnIDChanged;
            idComponent.OnIDChanged += OnIDChanged;
            if (Utilities.IsValid(gameObject.scene) && !Utils.IsInPrefab())
                UpdateID();
#endif
            OnChange();
        }

        protected virtual void OnChange()
        {
        }

        private string LogPrefix()
        {
            var msg = $"[<color={LogNameColor}>{logName}</color>] ";
            if (LogShowFullName)
                msg += $"({gameObject.FullName()}) ";
            return msg;
        }

        private string LogPrefix(GameObject obj) =>
            $"[<color={LogNameColor}>{logName}</color>] ({obj.FullName()}) ";

        private void AddLogToManager(object message, LogType logType)
        {
            if (!Utilities.IsValid(logManager)) return;

            var tempMsg = message.ToString();

            if (LogShowFullName)
                tempMsg = $"({gameObject.FullName()}) " + tempMsg;

            logManager.AddLog(LogNameColor, logName, tempMsg, logType, logManager.RandomKey);
        }

        private void AddLogToManager(GameObject obj, object message, LogType logType)
        {
            if (!Utilities.IsValid(logManager)) return;

            var tempMsg = message.ToString();

            if (LogShowFullName)
                tempMsg = $"({obj.FullName()}) " + tempMsg;

            logManager.AddLog(LogNameColor, logName, tempMsg, logType, logManager.RandomKey);
        }

        protected void Log(object message)
        {
            Debug.Log(LogPrefix() + message);
            AddLogToManager(message, LogType.Log);
        }

        protected void LogWarning(object message)
        {
            Debug.LogWarning(LogPrefix() + message);
            AddLogToManager(message, LogType.Warning);
        }

        protected void LogError(object message)
        {
            Debug.LogError(LogPrefix() + message);
            AddLogToManager(message, LogType.Error);
        }

        protected void Log(object message, Object context)
        {
            Debug.Log(LogPrefix() + message, context);
            AddLogToManager(message, LogType.Log);
        }

        protected void LogWarning(object message, Object context)
        {
            Debug.LogWarning(LogPrefix() + message, context);
            AddLogToManager(message, LogType.Warning);
        }

        protected void LogError(object message, Object context)
        {
            Debug.LogError(LogPrefix() + message, context);
            AddLogToManager(message, LogType.Error);
        }

        protected void Log(GameObject obj, object message)
        {
            Debug.Log(LogPrefix(obj) + message);
            AddLogToManager(message, LogType.Log);
        }

        protected void LogWarning(GameObject obj, object message)
        {
            Debug.LogWarning(LogPrefix(obj) + message);
            AddLogToManager(message, LogType.Warning);
        }

        protected void LogError(GameObject obj, object message)
        {
            Debug.LogError(LogPrefix(obj) + message);
            AddLogToManager(message, LogType.Error);
        }

        protected void Log(GameObject obj, object message, Object context)
        {
            Debug.Log(LogPrefix(obj) + message, context);
            AddLogToManager(obj, message, LogType.Log);
        }

        protected void LogWarning(GameObject obj, object message, Object context)
        {
            Debug.LogWarning(LogPrefix(obj) + message, context);
            AddLogToManager(obj, message, LogType.Warning);
        }

        protected void LogError(GameObject obj, object message, Object context)
        {
            Debug.LogError(LogPrefix(obj) + message, context);
            AddLogToManager(obj, message, LogType.Error);
        }

        protected string SaveKey(string key) => $"{ID}_{key}";
        protected string SaveIsTurnOn() => SaveKey(IsTurnOnKey);
        protected string SaveMode() => SaveKey(ModeKey);
        protected static string SaveKey(BasicUdonSharpBehaviour obj, string key) => $"{obj.ID}_{key}";
        protected static string SaveIsTurnOn(BasicUdonSharpBehaviour obj) => SaveKey(obj, IsTurnOnKey);
        protected static string SaveMode(BasicUdonSharpBehaviour obj) => SaveKey(obj, ModeKey);

        protected bool IsFirstMaster(VRCPlayerApi player)
        {
            if (Utilities.IsValid(playerGuid)) return playerGuid.IsFirstMaster(player, playerGuid.RandomKey);
            LogWarning("playerGuid has not been valid.");
            return player.isMaster;
        }

        protected virtual void Save(VRCPlayerApi player)
        {
        }

        protected void Save() => Save(Networking.LocalPlayer);

        protected virtual void Load(VRCPlayerApi player)
        {
        }

        protected void Load() => Load(Networking.LocalPlayer);

        // Refs: https://github.com/purabesan/ManualSyncUB/blob/main/sources/ManualSyncUB.cs
        /* 同期関連処理のまとめ */

        /// <summary>
        ///     オーナ権限取得
        /// </summary>
        /// <param name="target">取得対象オブジェクト</param>
        protected void GetOwner(GameObject target = null)
        {
            if (!Utilities.IsValid(target))
                target = gameObject;
            if (!Networking.IsOwner(target))
                Networking.SetOwner(Networking.LocalPlayer, target);
        }

        //同期変数受信時の処理
        public override void OnDeserialization() => AfterSynchronize(false);

        /// <summary>
        ///     同期化処理。オーナ権限の移動・同期指示側のAfterSynchronize実行を含む。
        /// </summary>
        protected void Synchronize()
        {
            GetOwner();
            RequestSerialization();
            AfterSynchronize(true);
        }

        /// <summary>
        ///     同期後の共通処理
        /// </summary>
        protected virtual void AfterSynchronize(bool isOwner)
        {
        }

        /// <summary>
        ///     グローバル処理実行。SendCustomNetworkEvent Allするだけ。
        /// </summary>
        /// <param name="eventName">実行したいpublic関数。nameof(関数名)にて指定</param>
        protected void ExecuteOnAll(string eventName) => SendCustomNetworkEvent(NetworkEventTarget.All, eventName);

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public LogManager.LogManager LogManager
        {
            get => logManager;
            set => logManager = value;
        }

        public void UpdateID()
        {
            var idComponent = GetComponent<BasicUdonSharpBehaviourID>();
            if (idComponent == null || idComponent.ID == id)
                return;
            id = idComponent.ID;
        }

        private void OnIDChanged(string newID) => id = newID;
#endif
    }
}