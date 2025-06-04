using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using com.zibra.common.Editor.SDFObjects;
using com.zibra.common.Utilities;
using static com.zibra.common.Editor.PluginManager;
using com.zibra.common.Analytics;

namespace com.zibra.common.Editor.Licensing
{
    /// <summary>
    ///     Class responsible for managing licensing and allowing server communication.
    /// </summary>
    [InitializeOnLoad]
    public class LicensingManager
    {
#region Public Interface

        /// <summary>
        ///     Status of license validation.
        /// </summary>
        public enum Status
        {
            NotInitialized,
            OK,
            ValidationInProgress,
            NetworkError,
            ValidationError,
#if !ZIBRA_EFFECTS_OTP_VERSION
            NoMaintance,
            Expired,
#endif
            NoKey,
            NotInstalled,
        ***REMOVED***

        /// <summary>
        ///     Instance of licensing manager.
        /// </summary>
        public static LicensingManager Instance { get
            {
                if (_Instance == null)
                {
                    _Instance = new LicensingManager();
                ***REMOVED***
                return _Instance;
            ***REMOVED***
        ***REMOVED***

        /// <summary>
        ///     Checks whether specified string is correctly formated key.
        /// </summary>
        public static bool ValidateKeyFormat(string key)
        {
            return Regex.IsMatch(key, KEY_FORMAT_REGEX);
        ***REMOVED***

        /// <summary>
        ///     Saves license key intended for single effect and starts validation.
        /// </summary>
        public void ValidateLicense(string key, Effect effect)
        {
            ValidateLicense(key, new Effect[] { effect ***REMOVED***);
        ***REMOVED***

        /// <summary>
        ///     Saves license key intended for list of effects and starts validation.
        /// </summary>
        public void ValidateLicense(string key, Effect[] effects)
        {
            if (!ValidateKeyFormat(key))
            {
                throw new ArgumentException("License key has invalid format.");
            ***REMOVED***

            foreach (Effect effect in effects)
            {
                if (effect == Effect.Count)
                {
                    throw new ArgumentException("Count is not valid value for effect");
                ***REMOVED***
                if (IsLicenseVerified(effect))
                {
                    throw new ArgumentException("Effect " + effect + " already has verified license.");
                ***REMOVED***
            ***REMOVED***

            foreach (Effect effect in effects)
            {
                Statuses[(int)effect] = Status.NotInitialized;
                LicenseKeys[(int)effect] = key;
            ***REMOVED***

            SaveLicenseKeys();
            ValidateLicense();
        ***REMOVED***

        /// <summary>
        ///     Validates saved license keys.
        /// </summary>
        public void ValidateLicense()
        {
            ValidateEffectStatuses();

            List<Effect> toValidate = new List<Effect>();

            for (int i = 0; i < (int)Effect.Count; ++i)
            {
                Effect effect = (Effect)i;
                switch (Statuses[i])
                {
                    case Status.NotInitialized:
                    case Status.NetworkError:
                        toValidate.Add(effect);
                        break;
                ***REMOVED***
            ***REMOVED***

            if (toValidate.Count == 0)
            {
                return;
            ***REMOVED***

            SendValidationRequest(toValidate);
            SetStatus(toValidate, Status.ValidationInProgress);
        ***REMOVED***

        /// <summary>
        ///     Removes all saved license keys and resets license validation status.
        /// </summary>
        public void RemoveLicenses()
        {
            LicenseKeys = new string[(int)Effect.Count];
            Statuses = new Status[(int)Effect.Count];
            ServerErrors = new string[(int)Effect.Count];
            SaveLicenseKeys();
            SaveSessionState();
#if ZIBRA_EFFECTS_OTP_VERSION
            RegistrationManager.Reset();
#endif
            for (int i = 0; i < (int)Effect.Count; ++i)
            {
                Effect effect = (Effect)i;
                ActivationTracking.GetInstance(effect).SetAutomaticActivation(false);
            ***REMOVED***
            LicenseInfoQuery.Instance.Reset();
            OnLicenseStatusUpdate?.Invoke();
        ***REMOVED***

        /// <summary>
        ///     Returns list of saved license keys.
        /// </summary>
        public string[] GetSavedLicenseKeys()
        {
            List<string> filteredKeys = new List<string>();
            foreach (string key in LicenseKeys)
            {
                if (!string.IsNullOrEmpty(key) && !filteredKeys.Contains(key))
                {
                    filteredKeys.Add(key);
                ***REMOVED***
            ***REMOVED***
            return filteredKeys.ToArray();
        ***REMOVED***

        /// <summary>
        ///     Returns saved license key for specified effect.
        /// </summary>
        public string GetSavedLicenseKey(Effect effect)
        {
            return LicenseKeys[(int)effect];
        ***REMOVED***

        /// <summary>
        ///     Returns license validation status for specified effect.
        /// </summary>
        public Status GetStatus(Effect effect)
        {
            return Statuses[(int)effect];
        ***REMOVED***

        /// <summary>
        ///     Returns human readable error describing error of license validation for specified effect.
        /// </summary>
        public string GetErrorMessage(Effect effect)
        {
            Status status = Statuses[(int)effect];
            switch (status)
            {
                case Status.ValidationInProgress:
                    return "License key validation in progress. Please wait.";
                case Status.NetworkError:
                    return "Network error. Please ensure you are connected to the Internet and try again.";
                case Status.ValidationError:
                    return ServerErrors[(int)effect];
                case Status.NoKey:
                    return "Plugin is not registered.";
#if !ZIBRA_EFFECTS_OTP_VERSION
                case Status.NoMaintance:
                    return "License expired.";
#endif
                case Status.NotInstalled:
                    return "Specified effect is not installed.";
                default:
                    return "";

            ***REMOVED***
        ***REMOVED***

        /// <summary>
        ///     Checks whether license is verified for specified effect.
        /// </summary>
        public bool IsLicenseVerified(Effect effect)
        {
            Status status = Statuses[(int)effect];
            switch (status)
            {
                case Status.OK:
#if !ZIBRA_EFFECTS_OTP_VERSION
                case Status.NoMaintance:
#endif
                    return true;
                default:
                    return false;
            ***REMOVED***
        ***REMOVED***

        /// <summary>
        ///     Checks whether license still has maintenance.
        /// </summary>
        public bool HasMaintenance(Effect effect)
        {
            Status status = Statuses[(int)effect];
            switch (status)
            {
                case Status.OK:
                    return true;
                default:
                    return false;
            ***REMOVED***
        ***REMOVED***

        /// <summary>
        ///     Callback that is triggered when license status is updated.
        /// </summary>
        public Action OnLicenseStatusUpdate;

#endregion
#region Implementation Details
#if ZIBRA_EFFECTS_OTP_VERSION
        const string LICENSE_KEYS_PREF_KEY_OLD = "ZibraLiquidsLicenceKey";
        const string LICENSE_KEYS_PREF_KEY_OLD2 = "ZibraEffectsLicenceKeyOTP";
        const string LICENSE_KEYS_PREF_KEY = "ZibraEffectsLicenceKeyOTPV2";
#else
        const string LICENSE_KEYS_PREF_KEY_OLD = "ZibraEffectsLicenceKey";
        const string LICENSE_KEYS_PREF_KEY = "ZibraEffectsLicenceKeyV2";
#endif
        const string SESSION_STATE_KEY = "ZibraEffectsLicenceSessionState";

        private const string KEY_FORMAT_REGEX =
            "^(([0-9a-fA-F]{8***REMOVED***-[0-9a-fA-F]{4***REMOVED***-[0-9a-fA-F]{4***REMOVED***-[0-9a-fA-F]{4***REMOVED***-[0-9a-fA-F]{12***REMOVED***)|([0-9A-F]{8***REMOVED***-[0-9A-F]{8***REMOVED***-[0-9A-F]{8***REMOVED***-[0-9A-F]{8***REMOVED***))$";

        private const string VERSION_DATE = "2024.04.29";

        private const string LICENSE_VALIDATION_URL = "https://license.zibra.ai/api/licenseExpiration";

        string[] LicenseKeys = new string[(int)Effect.Count];
        Status[] Statuses = new Status[(int)Effect.Count];
        string[] ServerErrors = new string[(int)Effect.Count];

        struct LicenseSessionState
        {
            public Status[] Statuses;
            public string[] ServerErrors;
        ***REMOVED***

        // C# doesn't know we use it with JSON deserialization
#pragma warning disable 0649
        /// @cond SHOW_INTERNAL_JSON_FIELDS
        // Some classes needs to be public for JSON deserialization
        // But it is not intended to be used by end users
        [Serializable]
        public class RandomNumberDeclaration
        {
            public string product;
            public string number;
        ***REMOVED***

        [Serializable]
        class LicenseKeyRequest
        {
            public string api_version = "2023.05.17";
            public string[] license_keys;
            public RandomNumberDeclaration[] random_numbers;
            public string hardware_id;
            public string engine;
        ***REMOVED***

        [Serializable]
        class LicenseKeyResponse
        {
            public string license_info;
            public string signature;
        ***REMOVED***

        [Serializable]
        public class LicenseWarning
        {
            public string header_text;
            public string body_text;
            public string button_text;
            public string URL;
        ***REMOVED***

        [Serializable]
        class LicenseInfo
        {
            public string license_key;
            public string license;
            public string latest_version;
            public string random_number;
            public string hardware_id;
            public string engine;
            public string product;
            public string message;
            public LicenseWarning warning;
        ***REMOVED***

        [Serializable]
        class LicenseInfoWrapper
        {
            public LicenseInfo[] items;
        ***REMOVED***

        [Serializable]
        class ErrorInfo
        {
            public string license_info;
        ***REMOVED***

        /// @endcond
#pragma warning restore 0649

        static private LicensingManager _Instance;

        private LicensingManager()
        {
            RestoreLicenseKeys();
            RestoreSessionState();
            ValidateLicense();
        ***REMOVED***

        void SaveLicenseKeys()
        {
            string serializedList = ZibraJsonUtil.ToJson(LicenseKeys);
            EditorPrefs.SetString(LICENSE_KEYS_PREF_KEY, serializedList);
        ***REMOVED***

        void RestoreLicenseKeys()
        {
#if ZIBRA_EFFECTS_OTP_VERSION
            if (EditorPrefs.HasKey(LICENSE_KEYS_PREF_KEY_OLD))
            {
                string key = EditorPrefs.GetString(LICENSE_KEYS_PREF_KEY_OLD);
                LicenseKeys[(int)Effect.Liquid] = key;
                EditorPrefs.DeleteKey(LICENSE_KEYS_PREF_KEY_OLD);
            ***REMOVED***
            if (EditorPrefs.HasKey(LICENSE_KEYS_PREF_KEY_OLD2))
            {
                string key = EditorPrefs.GetString(LICENSE_KEYS_PREF_KEY_OLD2);
                LicenseKeys[(int)Effect.Liquid] = key;
                EditorPrefs.DeleteKey(LICENSE_KEYS_PREF_KEY_OLD2);
            ***REMOVED***
#else
            if (EditorPrefs.HasKey(LICENSE_KEYS_PREF_KEY_OLD))
            {
                string key = EditorPrefs.GetString(LICENSE_KEYS_PREF_KEY_OLD);
                LicenseKeys[(int)Effect.Liquid] = key;
                LicenseKeys[(int)Effect.Smoke] = key;
                EditorPrefs.DeleteKey(LICENSE_KEYS_PREF_KEY_OLD);
            ***REMOVED***
#endif
            if (EditorPrefs.HasKey(LICENSE_KEYS_PREF_KEY))
            {
                string serializedList = EditorPrefs.GetString(LICENSE_KEYS_PREF_KEY);
                LicenseKeys = ZibraJsonUtil.FromJson<string[]>(serializedList);
            ***REMOVED***

            if (LicenseKeys.Length != (int)Effect.Count)
            {
                var oldKeys = LicenseKeys;
                LicenseKeys = new string[(int)Effect.Count];
                for (int i = 0; i < Math.Min(oldKeys.Length, LicenseKeys.Length); i++)
                {
                    LicenseKeys[i] = oldKeys[i];
                ***REMOVED***
            ***REMOVED***

            for (int i = 0; i < (int)Effect.Count; ++i)
            {
                Effect effect = (Effect)i;
                if (!string.IsNullOrEmpty(GetSavedLicenseKey(effect)))
                {
                    ActivationTracking.GetInstance(effect).SetAutomaticActivation(true);
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        void SaveSessionState()
        {
            LicenseSessionState sessionState = new LicenseSessionState();
            sessionState.Statuses = Statuses;
            sessionState.ServerErrors = ServerErrors;
            string serializedState = ZibraJsonUtil.ToJson(sessionState);
            SessionState.SetString(SESSION_STATE_KEY, serializedState);
        ***REMOVED***

        void RestoreSessionState()
        {
            if (SessionState.GetString(SESSION_STATE_KEY, "") != "")
            {
                string serializedState = SessionState.GetString(SESSION_STATE_KEY, "");
                LicenseSessionState sessionState = ZibraJsonUtil.FromJson<LicenseSessionState>(serializedState);
                Statuses = sessionState.Statuses;
                ServerErrors = sessionState.ServerErrors;
            ***REMOVED***

            for (int i = 0; i < Statuses.Length; i++)
            {
                switch (Statuses[i])
                {
                    case Status.ValidationInProgress:
                    case Status.NotInstalled:
                        Statuses[i] = Status.NotInitialized;
                        break;
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        void ValidateEffectStatuses()
        {
            for (int i = 0; i < (int)Effect.Count; ++i)
            {
                Effect effect = (Effect)i;
                if (!PluginManager.IsAvailable(effect))
                {
                    Statuses[i] = Status.NotInstalled;
                ***REMOVED***
                if (string.IsNullOrEmpty(LicenseKeys[i]))
                {
                    Statuses[i] = Status.NoKey;
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        void SendValidationRequest(List<Effect> toValidate)
        {
            LicenseKeyRequest licenseKeyRequest = FillKeyRequest(toValidate);
            string requestData = ZibraJsonUtil.ToJson(licenseKeyRequest);

#if UNITY_2022_2_OR_NEWER
            UnityWebRequest request = UnityWebRequest.PostWwwForm(LICENSE_VALIDATION_URL, requestData);
#else
            UnityWebRequest request = UnityWebRequest.Post(LICENSE_VALIDATION_URL, requestData);
#endif
            UnityWebRequestAsyncOperation requestOperation = request.SendWebRequest();
            requestOperation.completed += (operation) =>
            {
                UnityWebRequestAsyncOperation requestOperation = operation as UnityWebRequestAsyncOperation;
                UnityWebRequest request = requestOperation.webRequest;
                RequestCompleted(toValidate, request);
            ***REMOVED***;
        ***REMOVED***

        private LicenseKeyRequest FillKeyRequest(List<Effect> toValidate)
        {
            LicenseKeyRequest licenseKeyRequest = new LicenseKeyRequest();
            licenseKeyRequest.license_keys = new string[toValidate.Count];
            licenseKeyRequest.random_numbers = new RandomNumberDeclaration[toValidate.Count];
            licenseKeyRequest.hardware_id = PluginManager.GetHardwareID();
            licenseKeyRequest.engine = "unity";

            // Fill licenseKeyRequest with data from toValidate
            for (int i = 0; i < toValidate.Count; ++i)
            {
                Effect effect = toValidate[i];
                licenseKeyRequest.license_keys[i] = LicenseKeys[(int)effect];
                licenseKeyRequest.random_numbers[i] = new RandomNumberDeclaration();
                licenseKeyRequest.random_numbers[i].product = PluginManager.GetEffectName(effect);
                licenseKeyRequest.random_numbers[i].number = PluginManager.LicensingGetRandomNumber(effect).ToString();
            ***REMOVED***

            return licenseKeyRequest;
        ***REMOVED***

        void SetStatus(List<Effect> effects, Status status)
        {
            for (int i = 0; i < effects.Count; ++i)
            {
                Effect effect = effects[i];
                Statuses[(int)effect] = status;
            ***REMOVED***
            SaveSessionState();
            OnLicenseStatusUpdate?.Invoke();
        ***REMOVED***

        void SetStatus(Effect effect, Status status)
        {
            Statuses[(int)effect] = status;
            SaveSessionState();
            OnLicenseStatusUpdate?.Invoke();
        ***REMOVED***

        void RequestCompleted(List<Effect> toValidate, UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                ProcessServerResponse(toValidate, request.downloadHandler.text);
            ***REMOVED***
            else
            {
                SetStatus(toValidate, Status.NetworkError);
                Debug.LogError("Zibra License Key validation error: " + request.error + "\n" +
                    request.downloadHandler.text);
            ***REMOVED***
            request.Dispose();
        ***REMOVED***

        void ProcessServerResponse(List<Effect> toValidate, string response)
        {
            LicenseKeyResponse parsedResponse = ZibraJsonUtil.FromJson<LicenseKeyResponse>(response);
            if (parsedResponse.signature == null || parsedResponse.license_info == null)
            {
                SetStatus(toValidate, Status.NetworkError);
                return;
            ***REMOVED***

            if (CheckResponseForError(toValidate, parsedResponse))
            {
                return;
            ***REMOVED***

            LicenseInfo[] licenseInfos = null;
            if (!DeserializeLicenseInfo(toValidate, parsedResponse, ref licenseInfos))
            {
                return;
            ***REMOVED***

            foreach (LicenseInfo licenseInfo in licenseInfos)
            {
                ParseLicenseInfo(toValidate, licenseInfo, response);
            ***REMOVED***

            VSPAttribution(toValidate);

            for (int i = 0; i < (int)Effect.Count; ++i)
            {
                Effect effect = (Effect)i;
                if (IsLicenseVerified(effect))
                {
                    ActivationTracking.GetInstance(effect).TrackActivation();
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        private void VSPAttribution(List<Effect> toValidate)
        {
#if !ZIBRA_EFFECTS_OTP_VERSION
            bool isActivated = false;
            foreach (Effect effect in toValidate)
            {
                if (IsLicenseVerified(effect))
                {
                    isActivated = true;
                    break;
                ***REMOVED***
            ***REMOVED***

            if (!isActivated)
            {
                return;
            ***REMOVED***

            string licenseKeys = string.Join(',', GetSavedLicenseKeys());
            if (!string.IsNullOrEmpty(licenseKeys))
            {
                UnityEditor.VSAttribution.ZibraAI.VSAttribution.SendAttributionEvent("ZibraEffects_Login", "ZibraAI", licenseKeys);
            ***REMOVED***
#endif
        ***REMOVED***

        void ParseLicenseInfo(List<Effect> toValidate, LicenseInfo licenseInfo, string response)
        {
            Effect effect = PluginManager.ParseEffectName(licenseInfo.product);

            if (!toValidate.Contains(effect))
            {
                return;
            ***REMOVED***

            // Unity's JsonUtility may create empty, non-null licenseInfo.warning
            // Need to check whether we have at least some data in licenseInfo.warning
            if (licenseInfo.warning != null && licenseInfo.warning.header_text != null)
            {
                LicenseWarning warning = licenseInfo.warning;
                Debug.LogWarning(warning.header_text + "\n" + warning.body_text);
                ZibraEffectsMessagePopup.OpenMessagePopup(warning.header_text, warning.body_text, warning.URL, warning.button_text);
            ***REMOVED***

            switch (licenseInfo.license)
            {
                case "ok":
                    SetStatus(effect, Status.OK);
                    break;
#if !ZIBRA_EFFECTS_OTP_VERSION
                case "old_version_only":
                    int comparison = String.Compare(licenseInfo.latest_version, VERSION_DATE, StringComparison.Ordinal);
                    if (comparison < 0)
                    {
                        SetStatus(effect, Status.Expired);
                    ***REMOVED***
                    else
                    {
                        SetStatus(effect, Status.NoMaintance);
                    ***REMOVED***
                    break;
                case "expired":
                    SetStatus(effect, Status.Expired);
                    break;
#endif
                default:
                    SetStatus(effect, Status.ValidationError);
                    break;
            ***REMOVED***

            if (IsLicenseVerified(effect))
            {
                PluginManager.ValidateLicense(effect, response);
            ***REMOVED***
        ***REMOVED***

        bool DeserializeLicenseInfo(List<Effect> toValidate, LicenseKeyResponse parsedResponse, ref LicenseInfo[] licenseInfos)
        {
            try
            {
                licenseInfos = ZibraJsonUtil.FromJson<LicenseInfo[]>(parsedResponse.license_info);
            ***REMOVED***
            catch (Exception)
            {
                for (int i = 0; i < toValidate.Count; ++i)
                {
                    Effect effect = toValidate[i];
                    ServerErrors[(int)effect] = "Invalid Key.";
                ***REMOVED***
                SetStatus(toValidate, Status.ValidationError);
                return false;
            ***REMOVED***
            return true;
        ***REMOVED***

        bool CheckResponseForError(List<Effect> toValidate, LicenseKeyResponse parsedResponse)
        {
            try
            {
                ErrorInfo errorInfo = ZibraJsonUtil.FromJson<ErrorInfo>(parsedResponse.license_info);
                if (errorInfo != null)
                {
                    for (int i = 0; i < toValidate.Count; ++i)
                    {
                        Effect effect = toValidate[i];
                        ServerErrors[(int)effect] = errorInfo.license_info;
                    ***REMOVED***
                    SetStatus(toValidate, Status.ValidationError);
                    return true;
                ***REMOVED***
            ***REMOVED***
            catch (Exception)
            {
                // No errors reported
            ***REMOVED***
            return false;
        ***REMOVED***

#endregion
    ***REMOVED***
***REMOVED***
