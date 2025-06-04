using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System;
using System.Collections.Generic;
using com.zibra.common.Utilities;
using com.zibra.common.Editor.SDFObjects;
using com.zibra.common.Editor.Licensing;

namespace com.zibra.common.Editor.Menus
{
    internal static class Helpers
    {
        static string[] GetImportedRenderPipelines()
        {
            return new string[] {
                "BuiltInRP",
#if UNITY_PIPELINE_HDRP
                "HDRP",
#endif
#if UNITY_PIPELINE_URP
                "URP",
#endif
            ***REMOVED***;
        ***REMOVED***

        static List<string> GetWarnings()
        {
            List<string> result = new List<string>();
            var needed_components = new List<string> { "LiquidURPRenderComponent", "SmokeAndFireURPRenderComponent" ***REMOVED***;
            foreach (var component in needed_components)
            {
                if (RenderPipelineDetector.IsURPMissingRenderComponent(component))
                {
                    result.Add(component + " is missing!");
                ***REMOVED***
            ***REMOVED***
            return result;
        ***REMOVED***

        [Serializable]
        internal class EffectLicenseStatus
        {
            public string EffectName;
            public string LicenseStatus;
        ***REMOVED***

        static EffectLicenseStatus[] GetLicenseStatuses()
        {
            var result = new EffectLicenseStatus[(int)PluginManager.Effect.Count];

            for (int i = 0; i < (int)PluginManager.Effect.Count; ++i)
            {
                var effect = (PluginManager.Effect)i;
                result[i] = new EffectLicenseStatus();
                result[i].EffectName = effect.ToString();
                result[i].LicenseStatus = LicensingManager.Instance.GetStatus(effect).ToString();
            ***REMOVED***

            return result;
        ***REMOVED***

        [Serializable]
        class DiagInfo
        {
            public string Version;
            public string DistributionType;
            public bool LiquidAvailable;
            public bool SmokeAvailable;
            public string UnityVersion;
            public string RenderPipeline;
            public string[] ImportedRenderPipelines;
            public string OS;
            public string TargetPlatform;
            public string GPU;
            public string GPUFeatureLevel   ;
            public EffectLicenseStatus[] LicenseStatuses;
            public string KeyCount;
            public List<string> Warnings;

            public DiagInfo()
            {
                try
                {
                    Warnings = GetWarnings();
                ***REMOVED***
                    catch (Exception e)
                {
                    Warnings = new List<string>{"Failed to get Warnings: " + e.Message***REMOVED***;
                ***REMOVED***

                try
                {
                    Version = Effects.Version;
                ***REMOVED***
                catch (Exception e)
                {
                    Version = "Unknown";
                    Warnings.Add("Failed to get Version: " + e.Message);
                ***REMOVED***


                try
                {
                    DistributionType = Effects.DistributionType;
                ***REMOVED***
                catch (Exception e)
                {
                    DistributionType = "Unknown";
                    Warnings.Add("Failed to get DistributionType: " + e.Message);
                ***REMOVED***

                try
                {
                    LiquidAvailable = PluginManager.IsAvailable(PluginManager.Effect.Liquid);
                ***REMOVED***
                catch (Exception e)
                {
                    LiquidAvailable = false;
                    Warnings.Add("Failed to get LiquidAvailable: " + e.Message);
                ***REMOVED***

                try
                {
                    SmokeAvailable = PluginManager.IsAvailable(PluginManager.Effect.Smoke);
                ***REMOVED***
                catch (Exception e)
                {
                    SmokeAvailable = false;
                    Warnings.Add("Failed to get SmokeAvailable: " + e.Message);
                ***REMOVED***

                try
                {
                    UnityVersion = Application.unityVersion;
                ***REMOVED***
                catch (Exception e)
                {
                    UnityVersion = "Unknown";
                    Warnings.Add("Failed to get UnityVersion: " + e.Message);
                ***REMOVED***

                try
                {
                    RenderPipeline = "" + RenderPipelineDetector.GetRenderPipelineType();
                ***REMOVED***
                catch (Exception e)
                {
                    RenderPipeline = "Unknown";
                    Warnings.Add("Failed to get RenderPipeline: " + e.Message);
                ***REMOVED***

                try
                {
                    ImportedRenderPipelines = GetImportedRenderPipelines();
                ***REMOVED***
                catch (Exception e)
                {
                    ImportedRenderPipelines = null; // Assuming a default value of null.
                    Warnings.Add("Failed to get ImportedRenderPipelines: " + e.Message);
                ***REMOVED***

                try
                {
                    OS = SystemInfo.operatingSystem;
                ***REMOVED***
                catch (Exception e)
                {
                    OS = "Unknown";
                    Warnings.Add("Failed to get OS: " + e.Message);
                ***REMOVED***

                try
                {
                    TargetPlatform = "" + EditorUserBuildSettings.activeBuildTarget;
                ***REMOVED***
                catch (Exception e)
                {
                    TargetPlatform = "Unknown";
                    Warnings.Add("Failed to get TargetPlatform: " + e.Message);
                ***REMOVED***

                try
                {
                    GPU = SystemInfo.graphicsDeviceName;
                ***REMOVED***
                catch (Exception e)
                {
                    GPU = "Unknown";
                    Warnings.Add("Failed to get GPU: " + e.Message);
                ***REMOVED***

                try
                {
                    GPUFeatureLevel = SystemInfo.graphicsDeviceVersion;
                ***REMOVED***
                catch (Exception e)
                {
                    GPUFeatureLevel = "Unknown";
                    Warnings.Add("Failed to get GPUFeatureLevel: " + e.Message);
                ***REMOVED***

                try
                {
                    LicenseStatuses = GetLicenseStatuses();
                ***REMOVED***
                catch (Exception e)
                {
                    LicenseStatuses = null; // Assuming a default value of null.
                    Warnings.Add("Failed to get LicenseStatuses: " + e.Message);
                ***REMOVED***

                try
                {
                    KeyCount = LicensingManager.Instance.GetSavedLicenseKeys().Length.ToString();
                ***REMOVED***
                catch (Exception e)
                {
                    KeyCount = "0"; // Assuming a default value of "0".
                    Warnings.Add("Failed to get KeyCount: " + e.Message);
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***;

        [MenuItem(Effects.BaseMenuBarPath + "Copy Diagnostic Information to Clipboard", false, 10000)]
        public static void Copy()
        {
            DiagInfo info = new DiagInfo();
            string diagInfo = "```\nZibra Effects Diagnostic Information Begin\n";
            diagInfo += JsonUtility.ToJson(info, true) + "\n";
            diagInfo += "Zibra Effects Diagnostic Information End\n```\n";
            GUIUtility.systemCopyBuffer = diagInfo;
        ***REMOVED***

        public static string GetUniqueGameObjectName(string baseName)
        {
            // Get all game objects in the scene
            GameObject[] gameObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

            // Get the names of all game objects
            List<string> objectNames = new List<string>();
            foreach (GameObject gameObject in gameObjects)
            {
                objectNames.Add(gameObject.name);
            ***REMOVED***

            return ObjectNames.GetUniqueName(objectNames.ToArray(), baseName);
        ***REMOVED***
    ***REMOVED***
***REMOVED***