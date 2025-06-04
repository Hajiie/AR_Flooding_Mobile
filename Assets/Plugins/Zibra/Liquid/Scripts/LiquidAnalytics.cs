#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using com.zibra.liquid.Solver;
using com.zibra.common.Analytics;
using static com.zibra.common.Editor.PluginManager;
using com.zibra.common.SDFObjects;
using com.zibra.liquid.Manipulators;
using com.zibra.common.Utilities;

namespace com.zibra.liquid.Analytics
{
    [InitializeOnLoad]
    internal static class LiquidAnalytics
    {
#region Public Interface
        public static void SimulationCreated(ZibraLiquid liquid)
        {
            AnalyticsManagerInstance.TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Liquid_simulation_created",
                Properties = new Dictionary<string, object>
                    {
                        { "Liquid_simulation_id", liquid.SimulationGUID ***REMOVED***
                    ***REMOVED***
            ***REMOVED***);
        ***REMOVED***

        public static void SimulationStart(ZibraLiquid liquid)
        {
            PurchasedAssetRunEvent(liquid);
            SimulationdRun(liquid);
            EmitterRun(liquid);
            VoidRun(liquid);
            DetectorRun(liquid);
            ForceFieldRun(liquid);
        ***REMOVED***
#endregion
#region Implementation details

        private static void PurchasedAssetRunEvent(ZibraLiquid liquid)
        {
            List<string> presetNames = GetPresetNames(liquid);

            AnalyticsManagerInstance.TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Liquid_purchased_asset_run",
                Properties = new Dictionary<string, object>
                {
                    { "Liquid_simulation_id", liquid.SimulationGUID ***REMOVED***,
                    { "Presets_used", presetNames ***REMOVED***,
                    { "Build_platform", EditorUserBuildSettings.activeBuildTarget.ToString() ***REMOVED***,
                    { "AppleARKit", PackageTracker.IsPackageInstalled("com.unity.xr.arkit") ***REMOVED***,
                    { "GoogleARCore", PackageTracker.IsPackageInstalled("com.unity.xr.arcore") ***REMOVED***,
                    { "MagicLeap", PackageTracker.IsPackageInstalled("com.unity.xr.magicleap") ***REMOVED***,
                    { "Oculus", PackageTracker.IsPackageInstalled("com.unity.xr.oculus") ***REMOVED***,
                    { "OpenXR", PackageTracker.IsPackageInstalled("com.unity.xr.openxr") ***REMOVED***
                ***REMOVED***
            ***REMOVED***);
        ***REMOVED***

        private static void SimulationdRun(ZibraLiquid liquid)
        {
            List<string> presetNames = GetPresetNames(liquid);

            AnalyticsManagerInstance.TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Liquid_simulation_run",
                Properties = new Dictionary<string, object>
                {
                    { "Purchased_asset", (presetNames.Count > 0) ***REMOVED***,
                    { "Liquid_simulation_id", liquid.SimulationGUID ***REMOVED***,
                    { "Effective_voxel_count", liquid.GridSize.x * liquid.GridSize.y * liquid.GridSize.z ***REMOVED***,
                    { "Foam_used", liquid.MaterialParameters.EnableFoam ***REMOVED***,
                    { "Particle_species", (liquid.SolverParameters.AdditionalParticleSpecies.Count > 0) ***REMOVED***,
                    { "Emitter_count", CountManipulators(liquid, typeof(ZibraLiquidEmitter)) ***REMOVED***,
                    { "Void_count", CountManipulators(liquid, typeof(ZibraLiquidVoid)) ***REMOVED***,
                    { "Detector_count", CountManipulators(liquid, typeof(ZibraLiquidDetector)) ***REMOVED***,
                    { "Forcefield_count", CountManipulators(liquid, typeof(ZibraLiquidForceField)) ***REMOVED***,
                    { "Analytic_collider_count", CountCollidersWithSDFs(liquid, typeof(AnalyticSDF)) ***REMOVED***,
                    { "Neural_collider_count", CountCollidersWithSDFs(liquid, typeof(NeuralSDF)) ***REMOVED***,
                    { "Skinned_mesh_colider_count", CountCollidersWithSDFs(liquid, typeof(SkinnedMeshSDF)) ***REMOVED***,
                    { "Terrain_collider_count", CountCollidersWithSDFs(liquid, typeof(TerrainSDF)) ***REMOVED***,
                    { "Build_platform", EditorUserBuildSettings.activeBuildTarget.ToString() ***REMOVED***,
                    { "Render_pipeline", RenderPipelineDetector.GetRenderPipelineType().ToString() ***REMOVED***,
                    { "AppleARKit", PackageTracker.IsPackageInstalled("com.unity.xr.arkit") ***REMOVED***,
                    { "GoogleARCore", PackageTracker.IsPackageInstalled("com.unity.xr.arcore") ***REMOVED***,
                    { "MagicLeap", PackageTracker.IsPackageInstalled("com.unity.xr.magicleap") ***REMOVED***,
                    { "Oculus", PackageTracker.IsPackageInstalled("com.unity.xr.oculus") ***REMOVED***,
                    { "OpenXR", PackageTracker.IsPackageInstalled("com.unity.xr.openxr") ***REMOVED***
                ***REMOVED***
            ***REMOVED***);
        ***REMOVED***

        private static void EmitterRun(ZibraLiquid liquid)
        {
            int totalCount = CountManipulators(liquid, typeof(ZibraLiquidEmitter));
            if (totalCount == 0)
            {
                return;
            ***REMOVED***

            AnalyticsManagerInstance.TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Liquid_emitter_run",
                Properties = new Dictionary<string, object>
                {
                    { "Liquid_simulation_id", liquid.SimulationGUID ***REMOVED***,
                    { "SDF_analytic_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidEmitter), typeof(AnalyticSDF)) ***REMOVED***,
                    { "SDF_neural_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidEmitter), typeof(NeuralSDF)) ***REMOVED***,
                    { "SDF_skinned_mesh_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidEmitter), typeof(SkinnedMeshSDF)) ***REMOVED***,
                    { "Total_count", totalCount ***REMOVED***
                ***REMOVED***
            ***REMOVED***);
        ***REMOVED***

        private static void VoidRun(ZibraLiquid liquid)
        {
            int totalCount = CountManipulators(liquid, typeof(ZibraLiquidVoid));
            if (totalCount == 0)
            {
                return;
            ***REMOVED***

            AnalyticsManagerInstance.TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Liquid_void_run",
                Properties = new Dictionary<string, object>
                {
                    { "Liquid_simulation_id", liquid.SimulationGUID ***REMOVED***,
                    { "SDF_analytic_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidVoid), typeof(AnalyticSDF)) ***REMOVED***,
                    { "SDF_neural_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidVoid), typeof(NeuralSDF)) ***REMOVED***,
                    { "SDF_skinned_mesh_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidVoid), typeof(SkinnedMeshSDF)) ***REMOVED***,
                    { "Total_count", totalCount ***REMOVED***
                ***REMOVED***
            ***REMOVED***);
        ***REMOVED***

        private static void DetectorRun(ZibraLiquid liquid)
        {
            int totalCount = CountManipulators(liquid, typeof(ZibraLiquidDetector));
            if (totalCount == 0)
            {
                return;
            ***REMOVED***

            AnalyticsManagerInstance.TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Liquid_detector_run",
                Properties = new Dictionary<string, object>
                {
                    { "Liquid_simulation_id", liquid.SimulationGUID ***REMOVED***,
                    { "SDF_analytic_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidDetector), typeof(AnalyticSDF)) ***REMOVED***,
                    { "SDF_neural_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidDetector), typeof(NeuralSDF)) ***REMOVED***,
                    { "SDF_skinned_mesh_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidDetector), typeof(SkinnedMeshSDF)) ***REMOVED***,
                    { "Total_count", totalCount ***REMOVED***
                ***REMOVED***
            ***REMOVED***);
        ***REMOVED***

        private static void ForceFieldRun(ZibraLiquid liquid)
        {
            int totalCount = CountManipulators(liquid, typeof(ZibraLiquidForceField));
            if (totalCount == 0)
            {
                return;
            ***REMOVED***

            AnalyticsManagerInstance.TrackEvent(new AnalyticsManager.AnalyticsEvent
            {
                EventType = "Liquid_forcefield_run",
                Properties = new Dictionary<string, object>
                {
                    { "Liquid_simulation_id", liquid.SimulationGUID ***REMOVED***,
                    { "SDF_analytic_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidForceField), typeof(AnalyticSDF)) ***REMOVED***,
                    { "SDF_neural_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidForceField), typeof(NeuralSDF)) ***REMOVED***,
                    { "SDF_skinned_mesh_count", CountManipulatorsWithSDFs(liquid, typeof(ZibraLiquidForceField), typeof(SkinnedMeshSDF)) ***REMOVED***,
                    { "Total_count", totalCount ***REMOVED***
                ***REMOVED***
            ***REMOVED***);
        ***REMOVED***

        private static int CountManipulators(ZibraLiquid liquid, Type type)
        {
            int result = 0;
            foreach(var manipuilator in liquid.GetManipulatorList())
            {
                if (manipuilator != null && manipuilator.GetType() == type)
                {
                    result++;
                ***REMOVED***
            ***REMOVED***
            return result;
        ***REMOVED***

        private static int CountManipulatorsWithSDFs(ZibraLiquid liquid, Type manipType, Type SDFType)
        {
            int result = 0;
            foreach (var manipuilator in liquid.GetManipulatorList())
            {
                if (manipuilator == null)
                    continue;
                SDFObject sdf = manipuilator.GetComponent<SDFObject>();
                if (sdf != null && manipuilator.GetType() == manipType && sdf.GetType() == SDFType)
                {
                    result++;
                ***REMOVED***
            ***REMOVED***
            return result;
        ***REMOVED***
        private static int CountCollidersWithSDFs(ZibraLiquid liquid, Type SDFType)
        {
            int result = 0;
            foreach (var collider in liquid.GetColliderList())
            {
                if (collider == null)
                    continue;
                SDFObject sdf = collider.GetComponent<SDFObject>();
                if (sdf != null && sdf.GetType() == SDFType)
                {
                    result++;
                ***REMOVED***
            ***REMOVED***
            return result;
        ***REMOVED***

        private static List<string> GetPresetNames(ZibraLiquid liquid)
        {
            List<string> result = new List<string> { liquid.AdvancedRenderParameters.PresetName, liquid.SolverParameters.PresetName, liquid.MaterialParameters.PresetName ***REMOVED***;
            result.RemoveAll(s => s == "");
            return result;
        ***REMOVED***

        private static AnalyticsManager AnalyticsManagerInstance = AnalyticsManager.GetInstance(Effect.Liquid);
#endregion
    ***REMOVED***
***REMOVED***
#endif
