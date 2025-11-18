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
    /// <summary>
    ///     Represents a behavior that assigns and manages unique GUIDs (Globally Unique Identifiers)
    ///     for players within the context of a Unity UdonSharp-based environment. This class provides
    ///     mechanisms for identifying, validating, and retrieving player-specific GUIDs and supports
    ///     detecting the first master in the system.
    /// </summary>
    /// <remarks>
    ///     This class extends <see cref="CheatClientProtectorBehaviour" /> and utilizes functionality from
    ///     the UdonSharp scripting model to integrate with UdonBehaviour features in VRChat. It is
    ///     designed to ensure data consistency and integrity in multiplayer environments.
    /// </remarks>
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

        /// <summary>
        ///     Gets the GUID (Globally Unique Identifier) associated with the local player.
        /// </summary>
        /// <remarks>
        ///     The <c>LocalPlayerGuid</c> property provides a unique identifier for the local player in the
        ///     current multiplayer session. It is generated based on internal logic within the
        ///     <see cref="PlayerGuid" /> component, ensuring that the ID remains consistent and unique across
        ///     players during a session.
        /// </remarks>
        /// <returns>
        ///     A <see cref="string" /> representing the local player's unique GUID.
        /// </returns>
        /// <seealso cref="GetLocalPlayerGuid" />
        public string LocalPlayerGuid => GetLocalPlayerGuid(RandomKey);

        /// <summary>
        ///     Gets the GUID (Globally Unique Identifier) of the first master player in the system.
        /// </summary>
        /// <remarks>
        ///     The <c>FirstMasterGuid</c> property returns a unique identifier representing the
        ///     player who was designated as the first master in the multiplayer session. This value
        ///     is determined based on the internal logic of the <see cref="PlayerGuid" /> class, which
        ///     ensures consistency in identifying the first master across the session.
        /// </remarks>
        /// <returns>
        ///     A <see cref="string" /> representing the unique GUID of the first master player.
        /// </returns>
        /// <seealso cref="GetFirstMasterGuid" />
        public string FirstMasterGuid => GetFirstMasterGuid(RandomKey);

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
            logManager.Log(LogNameColor, LogName, message, logManager.RandomKey);
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning($"[<color={LogNameColor}>{LogName}</color>] {message}");
            logManager.LogWarning(LogNameColor, LogName, message, logManager.RandomKey);
        }

        private void LogError(string message)
        {
            Debug.LogError($"[<color={LogNameColor}>{LogName}</color>] {message}");
            logManager.LogError(LogNameColor, LogName, message, logManager.RandomKey);
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

        /// <summary>
        ///     Determines whether the specified player is the first master in the instance.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="key">The security key to validate the operation.</param>
        /// <returns>True if the specified player is the first master; otherwise, false.</returns>
        public bool IsFirstMaster(VRCPlayerApi player, int key)
        {
            if (!IsKeyCorrect(key)) return false;
            return _firstMasterGuid != EmptyGuid && GetPlayerGuid(player, RandomKey) == _firstMasterGuid;
        }

        /// <summary>
        ///     Determines whether the local player is the first master in the instance.
        /// </summary>
        /// <param name="key">The security key used to validate the operation.</param>
        /// <returns>True if the local player is the first master; otherwise, false.</returns>
        public bool IsLocalPlayerAreFirstMaster(int key)
        {
            if (!IsKeyCorrect(key)) return false;
            return _firstMasterGuid != EmptyGuid && GetLocalPlayerGuid(RandomKey) == _firstMasterGuid;
        }

        /// <summary>
        ///     Retrieves the GUID of the local player if the provided security key is valid.
        /// </summary>
        /// <param name="key">The security key used to validate the operation.</param>
        /// <returns>The GUID of the local player if the key is correct; otherwise, returns the value of <c>EmptyGuid</c>.</returns>
        public string GetLocalPlayerGuid(int key)
        {
            if (!IsKeyCorrect(key)) return EmptyGuid;
            Init();
            if (PlayerData.TryGetString(Networking.LocalPlayer, GuidKey, out var savedGuid))
                return savedGuid;
            LogWarning("Local player guid is not found!!!");
            return EmptyGuid;
        }

        /// <summary>
        ///     Retrieves the GUID associated with a specified player.
        /// </summary>
        /// <param name="player">The player whose GUID is to be retrieved.</param>
        /// <param name="key">The security key required to perform the operation.</param>
        /// <returns>The GUID of the specified player if accessible and valid; otherwise, a default empty GUID.</returns>
        public string GetPlayerGuid(VRCPlayerApi player, int key)
        {
            if (!IsKeyCorrect(key)) return EmptyGuid;
            Init(player);
            if (PlayerData.TryGetString(player, GuidKey, out var savedGuid))
                return savedGuid;
            LogWarning($"Player ({player.displayName}) guid is not found!!!");
            return EmptyGuid;
        }

        /// <summary>
        ///     Retrieves the GUID of the first player designated as the master in the instance.
        /// </summary>
        /// <param name="key">The security key used to validate the operation.</param>
        /// <returns>The GUID of the first master player if the key is valid; otherwise, an empty GUID.</returns>
        public string GetFirstMasterGuid(int key)
        {
            if (!IsKeyCorrect(key)) return EmptyGuid;
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