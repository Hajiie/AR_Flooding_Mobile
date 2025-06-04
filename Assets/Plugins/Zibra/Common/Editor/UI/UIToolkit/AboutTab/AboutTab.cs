using UnityEditor;
using UnityEngine.UIElements;
using com.zibra.common.Foundation.UIElements;
using com.zibra.common.Editor.SDFObjects;
using com.zibra.common.Editor;
using System;
using com.zibra.common.Editor.Licensing;
using static com.zibra.common.Editor.PluginManager;
using UnityEngine;

#if UNITY_2019_4_OR_NEWER
namespace com.zibra.common.Plugins.Editor
{
    internal class AboutTab : BaseTab
    {
        Label LicenseStatus;
        Button ContantSupportButton;
        Button RemoveLicenseButton;

        public AboutTab() : base($"{ZibraAIPackage.UIToolkitPath***REMOVED***/AboutTab/AboutTab")
        {
            LicenseStatus = this.Q<Label>("licenseStatus");
            ContantSupportButton = this.Q<Button>("contactSupportButton");
            RemoveLicenseButton = this.Q<Button>("removeLicenseButton");

            ContantSupportButton.clicked += OnRemoveKeyBtnOnClickedHandler;
            RemoveLicenseButton.clicked += OnRemoveKeyBtnOnClickedHandler;

            LicensingManager.Instance.OnLicenseStatusUpdate += UpdateLicenseStatus;
            LicenseInfoQuery.Instance.OnLicenseInfoUpdate += UpdateLicenseStatus;
            UpdateLicenseStatus();
        ***REMOVED***

        ~AboutTab()
        {
            LicensingManager.Instance.OnLicenseStatusUpdate -= UpdateLicenseStatus;
            LicenseInfoQuery.Instance.OnLicenseInfoUpdate -= UpdateLicenseStatus;
        ***REMOVED***

        private void UpdateLicenseStatus()
        {
            LicenseStatus.text = "";
            bool isLicenseKeySaved = false;
            bool isLimitReached = false;
            for (int i = 0; i < (int)Effect.Count; ++i)
            {
                Effect effect = (Effect)i;
#if ZIBRA_EFFECTS_OTP_VERSION
                string effectName = effect.ToString();
#else
                string effectName = "Effects";
#endif

                if (!PluginManager.IsAvailable(effect))
                {
                    continue;
                ***REMOVED***

                LicenseStatus.text += "\n";

                string licenseKey = LicensingManager.Instance.GetSavedLicenseKey(effect);
                if (!string.IsNullOrEmpty(licenseKey))
                {
                    LicenseStatus.text += $"{effectName***REMOVED*** license key {licenseKey***REMOVED***\n";
                    isLicenseKeySaved = true;
                ***REMOVED***
                else
                {
                    LicenseStatus.text += $"{effectName***REMOVED*** no license key\n";
                    continue;
                ***REMOVED***

                if (LicensingManager.Instance.IsLicenseVerified(effect))
                {
                    LicenseStatus.text += $"{effectName***REMOVED*** license validated\n";
                ***REMOVED***
                else
                {
                    LicenseStatus.text += $"{effectName***REMOVED*** license is not validated\n";
                ***REMOVED***

                LicenseStatus.text += LicenseInfoQuery.Instance.GetInfo(effect);

                isLimitReached = isLimitReached || LicenseInfoQuery.Instance.IsHardwareLimitHit(effect);

#if !ZIBRA_EFFECTS_OTP_VERSION
                break;
#endif
            ***REMOVED***

            if (isLimitReached)
            {
                LicenseStatus.text += "\nContact support to remove license from computers.";
            ***REMOVED***
            ContantSupportButton.style.display = isLimitReached ? DisplayStyle.Flex : DisplayStyle.None;
            RemoveLicenseButton.style.display = isLicenseKeySaved ? DisplayStyle.Flex : DisplayStyle.None;
        ***REMOVED***
        private void OnContactSupportBtnOnClickedHandler()
        {
            Application.OpenURL("mailto:support@zibra.ai");
        ***REMOVED***

        private void OnRemoveKeyBtnOnClickedHandler()
        {
            LicensingManager.Instance.RemoveLicenses();
        ***REMOVED***
    ***REMOVED***
***REMOVED***
#endif