using System;
using io.github.ykysnk.CheatClientProtector;
using io.github.ykysnk.LogManager;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBasic.Udon
{
#if !COMPILER_UDONSHARP && UNITY_EDITOR
    public partial class PlayerGuid : ILogManager
    {
        public LogManager.LogManager LogManager
        {
            get => logManager;
            set => logManager = value;
        }
    }
#endif

    /// <summary>
    ///     Represents a behavior that assigns and manages unique GUIDs (Globally Unique Identifiers)
    ///     for players within the context of a Unity UdonSharp-based environment. This class provides
    ///     mechanisms for identifying, validating, and retrieving player-specific GUIDs and supports
    ///     detecting the first master in the system.
    /// </summary>
    /// <remarks>
    ///     This class extends <see cref="CheatClientProtectorBehaviour" /> and uses functionality from
    ///     the UdonSharp scripting model to integrate with UdonBehaviour features in VRChat. It is
    ///     designed to ensure data consistency and integrity in multiplayer environments.
    /// </remarks>
    [AddComponentMenu("yky/World Basic/Player Guid")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [DisallowMultipleComponent]
    [PublicAPI]
    public partial class PlayerGuid : CheatClientProtectorBehaviour
    {
        private const string GuidKey = "guid";
        public const string EmptyGuid = "00000000-0000-0000-0000-000000000000";
        private const string LogName = nameof(PlayerGuid);
        public LogManager.LogManager logManager;

        [SerializeField] [ColorHex] private string logNameColor = "#D771C0";
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

        public override void OnPlayerRestored(VRCPlayerApi player) => Init(player, RandomKey);

        private void Log([NotNull] string message)
        {
            Debug.Log($"[<color={logNameColor}>{LogName}</color>] {message}");
            logManager.Log(logNameColor, LogName, message, logManager.RandomKey);
        }

        private void LogWarning([NotNull] string message)
        {
            Debug.LogWarning($"[<color={logNameColor}>{LogName}</color>] {message}");
            logManager.LogWarning(logNameColor, LogName, message, logManager.RandomKey);
        }

        private void LogError([NotNull] string message)
        {
            Debug.LogError($"[<color={logNameColor}>{LogName}</color>] {message}");
            logManager.LogError(logNameColor, LogName, message, logManager.RandomKey);
        }

        /// <summary>
        ///     Initializes the necessary data and configuration for the local player.
        ///     Only the local player can be initialized.
        ///     This method is use for <see cref="OnPlayerRestored" /> event.
        ///     Use when other udon sharp behaviors restore player data faster than this behavior.
        /// </summary>
        /// <param name="player">The player for whom initialization will be executed.</param>
        /// <param name="key">The security key used to validate the operation.</param>
        /// <seealso cref="Init(int)" />
        public void Init([NotNull] VRCPlayerApi player, int key)
        {
            if (!IsKeyCorrect(key)) return;
            if (!Utilities.IsValid(player))
                player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player) || !player.isLocal) return;
            GetOrSetGuid(player);
            SetFirstMaster(player);
        }

        /// <summary>
        ///     Initializes the necessary data and configuration for the local player.
        ///     Only the local player can be initialized.
        ///     This method is use for <see cref="OnPlayerRestored" /> event.
        ///     Use when other udon sharp behaviors restore player data faster than this behavior.
        /// </summary>
        /// <param name="key">The security key used to validate the operation.</param>
        /// <seealso cref="Init(VRCPlayerApi, int)" />
        public void Init(int key)
        {
            if (!IsKeyCorrect(key)) return;
            Init(Networking.LocalPlayer, RandomKey);
        }

        private void SetFirstMaster([NotNull] VRCPlayerApi player)
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
        public bool IsFirstMaster([NotNull] VRCPlayerApi player, int key)
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
        [NotNull]
        public string GetLocalPlayerGuid(int key)
        {
            if (!IsKeyCorrect(key)) return EmptyGuid;
            Init(RandomKey);
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
        [NotNull]
        public string GetPlayerGuid([NotNull] VRCPlayerApi player, int key)
        {
            if (!IsKeyCorrect(key)) return EmptyGuid;
            Init(player, RandomKey);
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
        [NotNull]
        public string GetFirstMasterGuid(int key)
        {
            if (!IsKeyCorrect(key)) return EmptyGuid;
            Init(RandomKey);
            return _firstMasterGuid;
        }

        private void GetOrSetGuid([NotNull] VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            // ReSharper disable once UnusedVariable
            if (PlayerData.TryGetString(player, GuidKey, out var savedGuid))
                return;
            var guid = Guid.NewGuid().ToString();
            PlayerData.SetString(GuidKey, guid);
            Log($"Create new guid: {guid}");
        }
    }
}