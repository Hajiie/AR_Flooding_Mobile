#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using static UnityEngine.Networking.UnityWebRequest;
using static com.zibra.common.Editor.PluginManager;

namespace com.zibra.common.Editor
{
    /// <summary>
    ///     Class responsible for detecting installed plugins.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [InitializeOnLoad]
    public static class PluginManager
    {
#region Public Interface
        /// <summary>
        ///     Effect types.
        /// </summary>
        public enum Effect
        {
            Liquid = 0,
            Smoke = 1,
            ZibraVDB = 2,
            Count = 3
        ***REMOVED***

        /// <summary>
        ///     Checks whether specified effect is available in the project.
        /// </summary>
        public static bool IsAvailable(Effect effect)
        {
            if (effect >= Effect.Count)
            {
                return false;
            ***REMOVED***

            return AvailablePlugins[(int)effect] != null;
        ***REMOVED***

        /// <summary>
        ///     Returns count of available effects.
        /// </summary>
        public static int AvailableCount()
        {
            int result = 0;
            for (int i = 0; i < (int)Effect.Count; i++)
            {
                if (IsAvailable((Effect)i))
                {
                    ++result;
                ***REMOVED***
            ***REMOVED***

            return result;
        ***REMOVED***

        /// <summary>
        ///     Returns hardware ID used for plugin licensing.
        /// </summary>
        public static string GetHardwareID()
        {
            for (int i = 0; i < (int)Effect.Count; ++i)
            {
                Effect effect = (Effect)i;

                // TODO remove when making native plugin for ZibraVDB
                if (effect == Effect.ZibraVDB)
                {
                    return SystemInfo.deviceUniqueIdentifier;
                ***REMOVED***

                MethodInfo method = GetMethod(effect, "GetHardwareID");

                if (method == null)
                {
                    continue;
                ***REMOVED***

                object result = method.Invoke(null, null);

                IntPtr? ptrResult = result as IntPtr?;
                if (ptrResult == null)
                {
                    continue;
                ***REMOVED***

                string stringResult = Marshal.PtrToStringAnsi(ptrResult.Value);

                return stringResult;
            ***REMOVED***
            return "";
        ***REMOVED***
#endregion
#region Implementation details
        static Type[] AvailablePlugins = new Type[(int)Effect.Count];

        static PluginManager()
        {
            FixMacOSBundles();

            try
            {
                Assembly liquidAssembly = Assembly.Load("ZibraAI.ZibraEffects.Liquid.Bridge");
                AvailablePlugins[(int)Effect.Liquid] = liquidAssembly.GetType("com.zibra.liquid.Bridge.LiquidBridge");
            ***REMOVED***
            catch { ***REMOVED***
            try
            {
                Assembly smokeAssembly = Assembly.Load("ZibraAI.ZibraEffects.SmokeAndFire.Bridge");
                AvailablePlugins[(int)Effect.Smoke] = smokeAssembly.GetType("com.zibra.smoke_and_fire.Bridge.SmokeAndFireBridge");
            ***REMOVED***
            catch { ***REMOVED***
            try
            {
                Assembly vdbAssembly = Assembly.Load("ZibraAI.ZibraEffects.ZibraVDB");
                AvailablePlugins[(int)Effect.ZibraVDB] = vdbAssembly.GetType("com.zibra.vdb.ZibraVDB");
            ***REMOVED***
            catch { ***REMOVED***
        ***REMOVED***

        internal static string GetEffectName(Effect effect)
        {
            string result = Enum.GetName(typeof(Effect), effect).ToLower();
            if (effect == Effect.Liquid)
            {
                result = "liquids";
            ***REMOVED***
#if ZIBRA_EFFECTS_OTP_VERSION
            result += "_otp";
#endif
            return result;
        ***REMOVED***

        internal static Effect ParseEffectName(string name)
        {
#if ZIBRA_EFFECTS_OTP_VERSION
            if (!name.Contains("_otp"))
            {
                return Effect.Count;
            ***REMOVED***
            name = name.Replace("_otp", "");
#endif
            if (name == "liquids")
            {
                return Effect.Liquid;
            ***REMOVED***

            Effect result = Effect.Count;
            Enum.TryParse<Effect>(name, true, out result);
            return result;
        ***REMOVED***

        static private MethodInfo GetMethod(Effect effect, string methodName)
        {
            if (effect > Effect.Count)
            {
                return null;
            ***REMOVED***

            string finalMethodName = "";

            switch(effect)
            {
                case Effect.Liquid:
                    finalMethodName += "ZibraLiquid_";
                    break;
                case Effect.Smoke:
                    finalMethodName += "ZibraSmokeAndFire_";
                    break;
                default:
                    throw new Exception("Trying to call GetMethod with unknown effect");
            ***REMOVED***
            finalMethodName += methodName;

            return AvailablePlugins[(int)effect]?.GetMethod(finalMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        ***REMOVED***

        // Hack to make .bundle plugins work after import from .unitypackage
        // Deletes all .meta files inside .bundle plugins
        // If macOS Sonoma sees any .meta files inside plugin it will fail signature check
        // So we need to remove them before trying to load said .bundle
        static private void FixMacOSBundles()
        {
            bool needAssetDatabaseRefresh = false;
            string[] bundleGUIDs = { "9d66c504b4c3c499fb1cacd335311051", "d25c9220fa93842e3b3577e6014e8794", "f2d0aa902f86543378fb230ed30e178a", "789b505a3e8447f99bf855d586ee9991" ***REMOVED***;
            // Loops over all guids
            foreach (string guid in bundleGUIDs)
            {
                // Converts the guid to a path
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Do nothing if asset is not found
                if (path == "")
                {
                    continue;
                ***REMOVED***

                if(!System.IO.Directory.Exists(path))
                {
                    continue;
                ***REMOVED***

                // Deletes the asset
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                foreach (System.IO.FileInfo file in dir.GetFiles("*.meta", System.IO.SearchOption.AllDirectories))
                {
                    needAssetDatabaseRefresh = true;
                    file.Delete();
                ***REMOVED***
            ***REMOVED***

            if (needAssetDatabaseRefresh)
            { 
                AssetDatabase.Refresh();
            ***REMOVED***
        ***REMOVED***

        internal static Int32 LicensingGetRandomNumber(Effect effect)
        {
            // TODO remove when making native plugin for ZibraVDB
            if (effect == Effect.ZibraVDB)
                return 119249569;

            MethodInfo method = GetMethod(effect, "GetRandomNumber");

            if (method == null)
            {
                return 0;
            ***REMOVED***

            object result = method.Invoke(null, null);

            Int32? intResult = result as Int32?;
            if (intResult == null)
            {
                return 0;
            ***REMOVED***

            return intResult.Value;
        ***REMOVED***

        internal static void ValidateLicense(Effect effect, string serverResponse)
        {
            // TODO remove when making native plugin for ZibraVDB
            if (effect == Effect.ZibraVDB)
                return;

            MethodInfo method = GetMethod(effect, "ValidateLicense");

            if (method == null)
            {
                return;
            ***REMOVED***

            object[] parameters = { serverResponse, (Int32)serverResponse.Length ***REMOVED***;
            method.Invoke(null, parameters);
        ***REMOVED***
#endregion
    ***REMOVED***
***REMOVED***

#endif
