#if UNITY_PIPELINE_URP

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using com.zibra.liquid.Solver;
using com.zibra.liquid.Bridge;

namespace com.zibra.liquid
{
    /// <summary>
    ///     Component responsible for rendering liquid in case of URP.
    /// </summary>
    /// <remarks>
    ///     This is not used in case liquid's
    ///     <see cref="Solver::ZibraLiquid::CurrentRenderingMode">ZibraLiquid.CurrentRenderingMode</see>
    ///     is set to Unity Render.
    /// </remarks>
    public class LiquidURPRenderComponent : ScriptableRendererFeature
    {
#region Public Interface
        /// <summary>
        ///     URP specific liquid rendering settings.
        /// </summary>
        [System.Serializable]
        public class LiquidURPRenderSettings
        {
            /// <summary>
            ///     Globally defines whether liquid renders in URP.
            /// </summary>
            public bool IsEnabled = true;
            /// <summary>
            ///     Injection point where we will insert liquid rendering.
            /// </summary>
            /// <remarks>
            ///     In case of URP, this parameter will be used instead of
            ///     <see cref="Solver::ZibraLiquid::CurrentInjectionPoint">ZibraLiquid.CurrentInjectionPoint</see>.
            /// </remarks>
            public RenderPassEvent InjectionPoint = RenderPassEvent.AfterRenderingSkybox;
        ***REMOVED***

        /// <summary>
        ///     See <see cref="LiquidURPRenderSettings"/>.
        /// </summary>
        // Must be called exactly "settings" so Unity shows this as render feature settings in editor
        public LiquidURPRenderSettings settings = new LiquidURPRenderSettings();

        /// <summary>
        ///     Creates URP ScriptableRendererFeature.
        /// </summary>
        public override void Create()
        {
            handleSystem.Initialize(0, 0);
        ***REMOVED***

        /// <summary>
        ///     Adds scriptable render passes.
        /// </summary>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!settings.IsEnabled)
            {
                return;
            ***REMOVED***

            if (renderingData.cameraData.cameraType != CameraType.Game &&
                renderingData.cameraData.cameraType != CameraType.SceneView)
            {
                return;
            ***REMOVED***

            Camera camera = renderingData.cameraData.camera;
            camera.depthTextureMode = DepthTextureMode.Depth;

            int liquidsToRenderCount = 0;
            int backgroundsToCopyCount = 0;
            int liquidsToVisualizeSDFCount = 0;
            int liquidsToUpscaleCount = 0;

            foreach (var liquid in ZibraLiquid.AllFluids)
            {
                if (liquid != null && liquid.Initialized)
                {
                    liquidsToRenderCount++;
                    if (liquid.EnableDownscale)
                    {
                        liquidsToUpscaleCount++;
                    ***REMOVED***
                    if (liquid.IsBackgroundCopyNeeded(camera))
                    {
                        backgroundsToCopyCount++;
                    ***REMOVED***
                    if (liquid.VisualizeSceneSDF)
                    {
                        liquidsToVisualizeSDFCount++;
                    ***REMOVED***
                ***REMOVED***
            ***REMOVED***

            if (copyPasses == null || copyPasses.Length != backgroundsToCopyCount)
            {
                copyPasses = new CopyBackgroundURPRenderPass[backgroundsToCopyCount];
                for (int i = 0; i < backgroundsToCopyCount; ++i)
                {
                    copyPasses[i] = new CopyBackgroundURPRenderPass(settings.InjectionPoint);
                ***REMOVED***
            ***REMOVED***

            if (liquidNativePasses == null || liquidNativePasses.Length != liquidsToRenderCount)
            {
                liquidNativePasses = new LiquidNativeRenderPass[liquidsToRenderCount];
                for (int i = 0; i < liquidsToRenderCount; ++i)
                {
                    liquidNativePasses[i] = new LiquidNativeRenderPass(settings.InjectionPoint);
                ***REMOVED***
            ***REMOVED***

            if (liquidURPPasses == null || liquidURPPasses.Length != liquidsToRenderCount)
            {
                liquidURPPasses = new LiquidURPRenderPass[liquidsToRenderCount];
                for (int i = 0; i < liquidsToRenderCount; ++i)
                {
                    liquidURPPasses[i] = new LiquidURPRenderPass(settings.InjectionPoint);
                ***REMOVED***
            ***REMOVED***

            if (liquidVisualizeSDFPasses == null || liquidVisualizeSDFPasses.Length != liquidsToVisualizeSDFCount)
            {
                liquidVisualizeSDFPasses = new LiquidVisualizeSDFPass[liquidsToVisualizeSDFCount];
                for (int i = 0; i < liquidsToVisualizeSDFCount; ++i)
                {
                    liquidVisualizeSDFPasses[i] = new LiquidVisualizeSDFPass(settings.InjectionPoint);
                ***REMOVED***
            ***REMOVED***

            if (upscalePasses == null || upscalePasses.Length != liquidsToUpscaleCount)
            {
                upscalePasses = new LiquidUpscaleURPRenderPass[liquidsToUpscaleCount];
                for (int i = 0; i < liquidsToUpscaleCount; ++i)
                {
                    upscalePasses[i] = new LiquidUpscaleURPRenderPass(settings.InjectionPoint);
                ***REMOVED***
            ***REMOVED***

            int currentCopyPass = 0;
            int currentLiquidPass = 0;
            int currentVisualizeSDFPass = 0;
            int currentUpscalePass = 0;

            foreach (var liquid in ZibraLiquid.AllFluids)
            {
                if (liquid != null && liquid.IsRenderingEnabled() &&
                    ((camera.cullingMask & (1 << liquid.gameObject.layer)) != 0))
                {
                    if (liquid.IsBackgroundCopyNeeded(camera))
                    {
                        copyPasses[currentCopyPass].liquid = liquid;
                        copyPasses[currentCopyPass].handleSystem = handleSystem;

                        copyPasses[currentCopyPass].ConfigureInput(ScriptableRenderPassInput.Color);
                        copyPasses[currentCopyPass].renderPassEvent = settings.InjectionPoint;

                        renderer.EnqueuePass(copyPasses[currentCopyPass]);
                        currentCopyPass++;
                    ***REMOVED***

                    liquidNativePasses[currentLiquidPass].liquid = liquid;
                    liquidNativePasses[currentLiquidPass].handleSystem = handleSystem;
                    liquidNativePasses[currentLiquidPass].renderPassEvent = settings.InjectionPoint;
                    renderer.EnqueuePass(liquidNativePasses[currentLiquidPass]);

                    liquidURPPasses[currentLiquidPass].liquid = liquid;
                    liquidURPPasses[currentLiquidPass].handleSystem = handleSystem;
                    liquidURPPasses[currentLiquidPass].ConfigureInput(ScriptableRenderPassInput.Color |
                                                                      ScriptableRenderPassInput.Depth);

                    liquidURPPasses[currentLiquidPass].renderPassEvent = settings.InjectionPoint;

                    renderer.EnqueuePass(liquidURPPasses[currentLiquidPass]);
                    currentLiquidPass++;

                    if (liquid.VisualizeSceneSDF)
                    {
                        liquidVisualizeSDFPasses[currentVisualizeSDFPass].liquid = liquid;
                        liquidVisualizeSDFPasses[currentVisualizeSDFPass].handleSystem = handleSystem;
                        liquidVisualizeSDFPasses[currentVisualizeSDFPass].ConfigureInput(
                            ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
                        liquidVisualizeSDFPasses[currentVisualizeSDFPass].renderPassEvent = settings.InjectionPoint;
                        renderer.EnqueuePass(liquidVisualizeSDFPasses[currentVisualizeSDFPass]);
                        currentVisualizeSDFPass++;
                    ***REMOVED***

                    if (liquid.EnableDownscale)
                    {
                        upscalePasses[currentUpscalePass].liquid = liquid;
                        upscalePasses[currentUpscalePass].handleSystem = handleSystem;

                        upscalePasses[currentUpscalePass].renderPassEvent = settings.InjectionPoint;

                        renderer.EnqueuePass(upscalePasses[currentUpscalePass]);
                        currentUpscalePass++;
                    ***REMOVED***
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***
#endregion
#region Implementation details
        private class CopyBackgroundURPRenderPass : ScriptableRenderPass
        {
            public ZibraLiquid liquid;
            public RTHandleSystem handleSystem;

            RTHandle cameraColorTarget;

            public CopyBackgroundURPRenderPass(RenderPassEvent injectionPoint)
            {
                renderPassEvent = injectionPoint;
            ***REMOVED***

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
#if UNITY_PIPELINE_URP_13_1_OR_HIGHER
                cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
#else
                cameraColorTarget = handleSystem.Alloc(renderingData.cameraData.renderer.cameraColorTarget);
#endif
            ***REMOVED***

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Camera camera = renderingData.cameraData.camera;

                CommandBuffer cmd = CommandBufferPool.Get("ZibraLiquid.Render");

                if (liquid.CameraResourcesMap.ContainsKey(camera))
                {
                    RTHandle background = handleSystem.Alloc(liquid.CameraResourcesMap[camera].Background);
                    Blit(cmd, cameraColorTarget, background);
                ***REMOVED***

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            ***REMOVED***
        ***REMOVED***

        private class LiquidNativeRenderPass : ScriptableRenderPass
        {
            public ZibraLiquid liquid;
            public RTHandleSystem handleSystem;

            public LiquidNativeRenderPass(RenderPassEvent injectionPoint)
            {
                renderPassEvent = injectionPoint;
            ***REMOVED***

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Camera camera = renderingData.cameraData.camera;
                camera.depthTextureMode = DepthTextureMode.Depth;
                CommandBuffer cmd = CommandBufferPool.Get("ZibraLiquid.Render");

                liquid.RenderCallBack(renderingData.cameraData.camera, renderingData.cameraData.renderScale);

                // set initial parameters in the native plugin
                LiquidBridge.SubmitInstanceEvent(cmd, liquid.CurrentInstanceID, LiquidBridge.EventID.SetCameraParams,
                                                 liquid.CamNativeParams[camera]);

                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan)
                {
                    cmd.SetRenderTarget(liquid.Color0, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                                        liquid.Depth, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
                    cmd.ClearRenderTarget(true, true, Color.clear);
                ***REMOVED***

                liquid.RenderLiquidNative(cmd, renderingData.cameraData.camera);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            ***REMOVED***
        ***REMOVED***

        private class LiquidURPRenderPass : ScriptableRenderPass
        {
            public ZibraLiquid liquid;
            public RTHandleSystem handleSystem;

            RTHandle cameraColorTarget;

            static int UpscaleColorTextureID = Shader.PropertyToID("ZibraLiquid_LiquidTempColorTexture");
            RTHandle UpscaleColorTexture;

            public LiquidURPRenderPass(RenderPassEvent injectionPoint)
            {
                renderPassEvent = injectionPoint;
            ***REMOVED***

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
#if UNITY_PIPELINE_URP_13_1_OR_HIGHER
                cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
#else
                cameraColorTarget = handleSystem.Alloc(renderingData.cameraData.renderer.cameraColorTarget);
#endif
            ***REMOVED***

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (liquid.EnableDownscale)
                {
                    RenderTextureDescriptor descriptor = cameraTextureDescriptor;

                    Vector2Int dimensions = new Vector2Int(descriptor.width, descriptor.height);
                    dimensions = liquid.ApplyDownscaleFactor(dimensions);
                    descriptor.width = dimensions.x;
                    descriptor.height = dimensions.y;

                    descriptor.msaaSamples = 1;

                    descriptor.colorFormat = RenderTextureFormat.ARGBHalf;
                    descriptor.depthBufferBits = 0;

                    cmd.GetTemporaryRT(UpscaleColorTextureID, descriptor, FilterMode.Bilinear);

                    UpscaleColorTexture = handleSystem.Alloc(new RenderTargetIdentifier(UpscaleColorTextureID));
                    ConfigureTarget(UpscaleColorTexture);
                    ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
                ***REMOVED***
                else
                {
                    ConfigureTarget(cameraColorTarget);
                    // ConfigureClear seems to be persistent, so need to reset it
                    ConfigureClear(ClearFlag.None, new Color(0, 0, 0, 0));
                ***REMOVED***
            ***REMOVED***

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Camera camera = renderingData.cameraData.camera;
                camera.depthTextureMode = DepthTextureMode.Depth;
                CommandBuffer cmd = CommandBufferPool.Get("ZibraLiquid.Render");

                if (!liquid.EnableDownscale)
                {
                    cmd.SetRenderTarget(cameraColorTarget);
                ***REMOVED***

                liquid.RenderLiquidMain(cmd, camera);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            ***REMOVED***

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                if (liquid.EnableDownscale)
                {
                    cmd.ReleaseTemporaryRT(UpscaleColorTextureID);
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        private class LiquidVisualizeSDFPass : ScriptableRenderPass
        {
            public ZibraLiquid liquid;
            public RTHandleSystem handleSystem;

            RTHandle cameraColorTarget;

            static int UpscaleColorTextureID = Shader.PropertyToID("ZibraLiquid_LiquidTempColorTexture");
            RTHandle UpscaleColorTexture;

            public LiquidVisualizeSDFPass(RenderPassEvent injectionPoint)
            {
                renderPassEvent = injectionPoint;
            ***REMOVED***

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
#if UNITY_PIPELINE_URP_13_1_OR_HIGHER
                cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
#else
                cameraColorTarget = handleSystem.Alloc(renderingData.cameraData.renderer.cameraColorTarget);
#endif
            ***REMOVED***

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (liquid.EnableDownscale)
                {
                    UpscaleColorTexture = handleSystem.Alloc(new RenderTargetIdentifier(UpscaleColorTextureID));
                    ConfigureTarget(UpscaleColorTexture);
                ***REMOVED***
                else
                {
                    ConfigureTarget(cameraColorTarget);
                ***REMOVED***
            ***REMOVED***

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Camera camera = renderingData.cameraData.camera;
                camera.depthTextureMode = DepthTextureMode.Depth;
                CommandBuffer cmd = CommandBufferPool.Get("ZibraLiquid.Render");

                LiquidBridge.SubmitInstanceEvent(cmd, liquid.CurrentInstanceID, LiquidBridge.EventID.RenderSDF);

                if (liquid.EnableDownscale)
                {
                    cmd.SetRenderTarget(UpscaleColorTexture);
                ***REMOVED***
                else
                {
                    cmd.SetRenderTarget(cameraColorTarget);
                ***REMOVED***

                liquid.RenderSDFVisualization(cmd, camera);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            ***REMOVED***
        ***REMOVED***

        private class LiquidUpscaleURPRenderPass : ScriptableRenderPass
        {
            public ZibraLiquid liquid;
            public RTHandleSystem handleSystem;

            RTHandle cameraColorTarget;

            static int UpscaleColorTextureID = Shader.PropertyToID("ZibraLiquid_LiquidTempColorTexture");
            RTHandle UpscaleColorTexture;

            public LiquidUpscaleURPRenderPass(RenderPassEvent injectionPoint)
            {
                renderPassEvent = injectionPoint;
            ***REMOVED***

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
#if UNITY_PIPELINE_URP_13_1_OR_HIGHER
                cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
#else
                cameraColorTarget = handleSystem.Alloc(renderingData.cameraData.renderer.cameraColorTarget);
#endif
            ***REMOVED***

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                UpscaleColorTexture = handleSystem.Alloc(new RenderTargetIdentifier(UpscaleColorTextureID));
                ConfigureTarget(cameraColorTarget);
            ***REMOVED***

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Camera camera = renderingData.cameraData.camera;
                camera.depthTextureMode = DepthTextureMode.Depth;
                CommandBuffer cmd = CommandBufferPool.Get("ZibraLiquid.Render");

                liquid.UpscaleLiquidDirect(cmd, camera, UpscaleColorTexture);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            ***REMOVED***
        ***REMOVED***

        RTHandleSystem handleSystem = new RTHandleSystem();
        // 1 pass per rendered liquid that requires background copy
        CopyBackgroundURPRenderPass[] copyPasses;
        // 1 pass per rendered liquid
        LiquidNativeRenderPass[] liquidNativePasses;
        // 1 pass per rendered liquid
        LiquidURPRenderPass[] liquidURPPasses;
        // 1 pass per rendered liquid with VisualizeSceneSDF enabled
        LiquidVisualizeSDFPass[] liquidVisualizeSDFPasses;
        // 1 pass per rendered liquid that have downscale enabled
        LiquidUpscaleURPRenderPass[] upscalePasses;
#endregion
    ***REMOVED***
***REMOVED***

#endif // UNITY_PIPELINE_URP
