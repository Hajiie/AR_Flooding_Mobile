using UnityEngine;
using UnityEditor;
using com.zibra.common.Editor.SDFObjects;
using com.zibra.common.SDFObjects;
using com.zibra.common;
using com.zibra.common.Analytics;
using com.zibra.common.Editor.Licensing;
using com.zibra.common.Editor;

namespace com.zibra.common.Editor.SDFObjects
{
    [CustomEditor(typeof(SkinnedMeshSDF))]
    [CanEditMultipleObjects]
    internal class SkinnedMeshSDFEditor : UnityEditor.Editor
    {
        private static SkinnedMeshSDFEditor EditorInstance;

        private SkinnedMeshSDF[] SkinnedSDFs;

        private SerializedProperty BoneSDFList;
        private SerializedProperty SurfaceDistance;

        [MenuItem(Effects.BaseMenuBarPath + "Generate all Skinned Mesh SDFs in the Scene", false, 501)]
        private static void GenerateAllSDFs()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Neural colliders can only be generated in edit mode.");
                return;
            ***REMOVED***

            if (!GenerationManager.Instance.IsGenerationAvailable())
            {
                Debug.LogWarning("Skinned Mesh SDF Generation requires license verification.");
                Debug.LogWarning(GenerationManager.Instance.GetErrorMessage());
                return;
            ***REMOVED***

            // Find all neural colliders in the scene
            SkinnedMeshSDF[] skinnedMeshSDFs = FindObjectsByType<SkinnedMeshSDF>(FindObjectsSortMode.None);

            if (skinnedMeshSDFs.Length == 0)
            {
                Debug.LogWarning("No skinned mesh colliders found in the scene.");
                return;
            ***REMOVED***

            // Find all corresponding game objects
            GameObject[] skinnedMeshSDFsGameObjects = new GameObject[skinnedMeshSDFs.Length];
            for (int i = 0; i < skinnedMeshSDFs.Length; i++)
            {
                skinnedMeshSDFsGameObjects[i] = skinnedMeshSDFs[i].gameObject;
            ***REMOVED***
            // Set selection to that game objects so user can see generation progress
            Selection.objects = skinnedMeshSDFsGameObjects;

            // Add all colliders to the generation queue
            foreach (var skinnedMeshSDF in skinnedMeshSDFs)
            {
                if (!skinnedMeshSDF.HasRepresentation())
                {
                    GenerationQueue.AddToQueue(skinnedMeshSDF);
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        private void OnEnable()
        {
            EditorInstance = this;

            SkinnedSDFs = new SkinnedMeshSDF[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                SkinnedSDFs[i] = targets[i] as SkinnedMeshSDF;
            ***REMOVED***

            serializedObject.Update();
            BoneSDFList = serializedObject.FindProperty("BoneSDFList");
            SurfaceDistance = serializedObject.FindProperty("SurfaceDistance");
            serializedObject.ApplyModifiedProperties();
        ***REMOVED***

        private void OnDisable()
        {
            if (EditorInstance == this)
            {
                EditorInstance = null;
            ***REMOVED***
        ***REMOVED***
        private void GenerateSDFs(bool regenerate = false)
        {
            foreach (var instance in SkinnedSDFs)
            {
                if (!instance.HasRepresentation() || regenerate)
                {
                    GenerationQueue.AddToQueue(instance);
                ***REMOVED***
            ***REMOVED***
        ***REMOVED***

        public void Update()
        {
            if (GenerationQueue.GetQueueLength() > 0)
                EditorInstance.Repaint();
        ***REMOVED***

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GenerationGUI();

            if (SkinnedSDFs.Length == 1)
                EditorGUILayout.PropertyField(BoneSDFList);
            EditorGUILayout.PropertyField(SurfaceDistance);

            serializedObject.ApplyModifiedProperties();
        ***REMOVED***


        private void GenerationGUI()
        {
            bool isGenerationAvailable = GenerationAvailabilityGUI();

            EditorGUI.BeginDisabledGroup(!isGenerationAvailable);
            int toGenerateCount = 0;
            int toRegenerateCount = 0;

            foreach (var instance in SkinnedSDFs)
            {
                if (!GenerationQueue.Contains(instance))
                {
                    if (instance.HasRepresentation())
                    {
                        toRegenerateCount++;
                    ***REMOVED***
                    else
                    {
                        toGenerateCount++;
                    ***REMOVED***
                ***REMOVED***
            ***REMOVED***

            int inQueueCount = SkinnedSDFs.Length - toGenerateCount - toRegenerateCount;
            int fullQueueLength = GenerationQueue.GetQueueLength();
            if (fullQueueLength > 0)
            {
                if (fullQueueLength != inQueueCount)
                {
                    if (inQueueCount == 0)
                    {
                        GUILayout.Label($"Generating other SDFs. {fullQueueLength***REMOVED*** left in total.");
                    ***REMOVED***
                    else
                    {
                        GUILayout.Label(
                            $"Generating SDFs. {inQueueCount***REMOVED*** left out of selected SDFs. {fullQueueLength***REMOVED*** SDFs left in total.");
                    ***REMOVED***
                ***REMOVED***
                else
                {
                    GUILayout.Label(SkinnedSDFs.Length > 1 ? $"Generating SDFs. {inQueueCount***REMOVED*** left."
                                                          : "Generating SDF.");
                ***REMOVED***
                if (GUILayout.Button("Abort"))
                {
                    GenerationQueue.Abort();
                ***REMOVED***
            ***REMOVED***

            if (toGenerateCount > 0)
            {
                EditorGUILayout.HelpBox(SkinnedSDFs.Length > 1 ? $"{toGenerateCount***REMOVED*** SDFs don't have representation."
                                                              : "SDF doesn't have representation.",
                                        MessageType.Error);
                if (GUILayout.Button(SkinnedSDFs.Length > 1 ? "Generate SDFs" : "Generate SDF"))
                {
                    GenerateSDFs();
                ***REMOVED***
            ***REMOVED***

            if (toRegenerateCount > 0)
            {
                GUILayout.Label(SkinnedSDFs.Length > 1 ? $"{toRegenerateCount***REMOVED*** SDFs already generated."
                                                      : "SDF already generated.");
                if (GUILayout.Button(SkinnedSDFs.Length > 1 ? "Regenerate all selected SDFs" : "Regenerate SDF"))
                {
                    GenerateSDFs(true);
                ***REMOVED***
            ***REMOVED***

            EditorGUI.EndDisabledGroup();
        ***REMOVED***

        private static bool GenerationAvailabilityGUI()
        {
            if (EditorApplication.isPlaying)
            {
                GUILayout.Label("Generation is disabled during playmode");
                GUILayout.Space(20);

                return false;
            ***REMOVED***
            else if (!GenerationManager.Instance.IsGenerationAvailable())
            {
                GUILayout.Label("Skinned Mesh SDF Generation requires license verification.\n" +
                                GenerationManager.Instance.GetErrorMessage());

                if (GenerationManager.Instance.NeedActivation())
                {
                    if (GUILayout.Button("Activate license"))
                    {
                        ZibraEffectsOnboarding.ShowWindow("neuralSDF_generation");
                    ***REMOVED***
                ***REMOVED***

                GUILayout.Space(20);

                return false;
            ***REMOVED***
            else
            {
                return true;
            ***REMOVED***
        ***REMOVED***
    ***REMOVED***
***REMOVED***
