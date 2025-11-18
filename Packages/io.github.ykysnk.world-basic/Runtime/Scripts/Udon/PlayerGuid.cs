using System;
using System.Diagnostics.CodeAnalysis;
using io.github.ykysnk.CheatClientProtector;
using io.github.ykysnk.LogManager;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBasic.Udon
{
    [AddComponentMenu("yky/World Basic/Player Guid")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [DisallowMultipleComponent]
    [PublicAPI]
    public class PlayerGuid : CheatClientProtectorBehaviour
#if !COMPILER_UDONSHARP && UNITY_EDITOR
        , ILogManager
#endif
    {
        private const string GuidKey = "guid";
        public const string EmptyGuid = "00000000-0000-0000-0000-000000000000";
        private const string LogName = nameof(PlayerGuid);
        private const string LogNameColor = "#D771C0";
        public LogManager.LogManager logManager;
        [UdonSynced] private string _firstMasterGuid = EmptyGuid;

        public string LocalPlayerGuid => GetLocalPlayerGuid(RandomKeyPublic);
        public string FirstMasterGuid => GetFirstMasterGuid(RandomKeyPublic);

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public LogManager.LogManager LogManager
        {
            get => logManager;
            set => logManager = value;
        }
#endif

        public override void OnPlayerRestored(VRCPlayerApi player) => Init(player);

        private void Log(string message)
        {
            Debug.Log($"[<color={LogNameColor}>{LogName}</color>] {message}");
            logManager.Log(LogNameColor, LogName, message, logManager.RandomKeyPublic);
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning($"[<color={LogNameColor}>{LogName}</color>] {message}");
            logManager.LogWarning(LogNameColor, LogName, message, logManager.RandomKeyPublic);
        }

        private void LogError(string message)
        {
            Debug.LogError($"[<color={LogNameColor}>{LogName}</color>] {message}");
            logManager.LogError(LogNameColor, LogName, message, logManager.RandomKeyPublic);
        }

        private void Init(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player))
                player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            GetOrSetGuid(player);
            SetFirstMaster(player);
        }

        private void Init() => Init(Networking.LocalPlayer);

        private void SetFirstMaster(VRCPlayerApi player)
        {
            if (_firstMasterGuid != EmptyGuid || !player.isLocal || !player.isMaster) return;
            string firstMasterGuid;

            if (PlayerData.TryGetString(player, GuidKey, out var savedGuid))
                firstMasterGuid = savedGuid;
            else
            {
                firstMasterGuid = EmptyGuid;
                LogError("Local player guid is not found, set empty guid to first master guid");
            }

            _firstMasterGuid = firstMasterGuid;
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(player, gameObject);
            RequestSerialization();
        }

        public bool IsFirstMaster(VRCPlayerApi player, int key)
        {
            if (!IsPublicKeyCorrect(key)) return false;
            return _firstMasterGuid != EmptyGuid && GetPlayerGuid(player, RandomKeyPublic) == _firstMasterGuid;
        }

        public bool IsLocalPlayerAreFirstMaster(int key)
        {
            if (!IsPublicKeyCorrect(key)) return false;
            return _firstMasterGuid != EmptyGuid && GetLocalPlayerGuid(RandomKeyPublic) == _firstMasterGuid;
        }

        public string GetLocalPlayerGuid(int key)
        {
            if (!IsPublicKeyCorrect(key)) return EmptyGuid;
            Init();
            if (PlayerData.TryGetString(Networking.LocalPlayer, GuidKey, out var savedGuid))
                return savedGuid;
            LogWarning("Local player guid is not found!!!");
            return EmptyGuid;
        }

        public string GetPlayerGuid(VRCPlayerApi player, int key)
        {
            if (!IsPublicKeyCorrect(key)) return EmptyGuid;
            Init(player);
            if (PlayerData.TryGetString(player, GuidKey, out var savedGuid))
                return savedGuid;
            LogWarning($"Player ({player.displayName}) guid is not found!!!");
            return EmptyGuid;
        }

        public string GetFirstMasterGuid(int key)
        {
            if (!IsPublicKeyCorrect(key)) return EmptyGuid;
            Init();
            return _firstMasterGuid;
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        private void GetOrSetGuid(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            if (PlayerData.TryGetString(player, GuidKey, out var savedGuid))
                return;
            var guid = Guid.NewGuid().ToString();
            PlayerData.SetString(GuidKey, guid);
            Log($"Create new guid: {guid}");
        }
    }
}