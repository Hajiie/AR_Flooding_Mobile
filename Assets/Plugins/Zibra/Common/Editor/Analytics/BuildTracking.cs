
using com.zibra.common.Editor;
using com.zibra.common.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.UIElements;

namespace com.zibra.common.Analytics
{
    internal class BuildTracking : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; ***REMOVED*** ***REMOVED***
        public void OnPostprocessBuild(BuildReport report)
        {
            Dictionary<string, object> eventProperties = new Dictionary<string, object>
            {
                { "Render_pipeline", RenderPipelineDetector.GetRenderPipelineType().ToString() ***REMOVED***,
                { "Built_platform", GetPlatformName(report.summary.platform) ***REMOVED***,
                { "AppleARKit", PackageTracker.IsPackageInstalled("com.unity.xr.arkit") ***REMOVED***,
                { "GoogleARCore", PackageTracker.IsPackageInstalled("com.unity.xr.arcore") ***REMOVED***,
                { "MagicLeap", PackageTracker.IsPackageInstalled("com.unity.xr.magicleap") ***REMOVED***,
                { "Oculus", PackageTracker.IsPackageInstalled("com.unity.xr.oculus") ***REMOVED***,
                { "OpenXR", PackageTracker.IsPackageInstalled("com.unity.xr.openxr") ***REMOVED***
            ***REMOVED***;

#if ZIBRA_EFFECTS_OTP_VERSION
            if (PluginManager.IsAvailable(PluginManager.Effect.Liquid))
            {
                AnalyticsManager.GetInstance(PluginManager.Effect.Liquid).TrackEvent(new AnalyticsManager.AnalyticsEvent
                {
                    EventType = "Liquid_built",
                    Properties = eventProperties
                ***REMOVED***);
            ***REMOVED***
            if (PluginManager.IsAvailable(PluginManager.Effect.Smoke))
            {
                AnalyticsManager.GetInstance(PluginManager.Effect.Smoke).TrackEvent(new AnalyticsManager.AnalyticsEvent
                {
                    EventType = "SF_built",
                    Properties = eventProperties
                ***REMOVED***);
            ***REMOVED***
#else
            AnalyticsManager.GetInstance("effects").TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Effects_built",
                Properties = eventProperties
            ***REMOVED***);
#endif
        ***REMOVED***

        string GetPlatformName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                    return "MacOS";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.iOS:
                    return "IOS";
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.WSAPlayer:
                    return "UWP";
                case BuildTarget.StandaloneLinux64:
                    return "Linux";
                default:
                    return "Unknown";
            ***REMOVED***
        ***REMOVED***
    ***REMOVED***
***REMOVED***
