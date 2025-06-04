using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using com.zibra.common.Foundation.Editor;
using com.zibra.common.Foundation.UIElements;

namespace com.zibra.common.Plugins.Editor
{
    /// <summary>
    ///     Base class for Plugin Settings Window
    /// </summary>
    /// <typeparam name="TWindow"></typeparam>
    internal abstract class PackageSettingsWindow<TWindow> : EditorWindow
        where TWindow : EditorWindow
    {
        internal abstract IPackageInfo GetPackageInfo();
        protected abstract void OnWindowEnable(VisualElement root);
        internal ButtonStrip m_TabsButtons;

        protected ScrollView m_TabsContainer;
        protected readonly Dictionary<string, VisualElement> m_Tabs = new Dictionary<string, VisualElement>();

        private readonly string m_WindowUIFilesRootPath = $"{ZibraAIPackage.UIToolkitPath***REMOVED***/SettingsWindow";

        private void OnEnable()
        {
            var root = rootVisualElement;
            UIToolkitEditorUtility.CloneTreeAndApplyStyle(root, $"{m_WindowUIFilesRootPath***REMOVED***/PackageSettingsWindow");
            
            m_TabsContainer = root.Q<ScrollView>("tabs-container");

            var packageInfo = GetPackageInfo();
            root.Q<Label>("display-name").text = packageInfo.displayName;
            root.Q<Label>("description").text = packageInfo.description;
            root.Q<Label>("version").text = $"Version: {packageInfo.version***REMOVED*** {packageInfo.distributionType***REMOVED***";

            m_TabsButtons = root.Q<ButtonStrip>();
            m_TabsButtons.CleanUp();
            m_TabsButtons.OnButtonClick += ActivateTab;

            OnWindowEnable(root);
            ActivateTab();
        ***REMOVED***

        private void ActivateTab()
        {
            if (string.IsNullOrEmpty(m_TabsButtons.Value))
                return;

            foreach (var tab in m_Tabs)
                tab.Value.RemoveFromHierarchy();

            m_TabsContainer.Add(m_Tabs[m_TabsButtons.Value]);
        ***REMOVED***

        /// <summary>
        /// Add tab to the window top bar.
        /// </summary>
        /// <param name="label">Tab label.</param>
        /// <param name="content">Tab content.</param>
        /// <exception cref="ArgumentException">Will throw tab with the same label was already added.</exception>
        protected void AddTab(string label, VisualElement content)
        {
            if (!m_Tabs.ContainsKey(label))
            {
                m_TabsButtons.AddChoice(label, label);
                m_Tabs.Add(label, content);
                content.viewDataKey = label;
            ***REMOVED***
            else
            {
                throw new ArgumentException($"Tab '{label***REMOVED***' already added", nameof(label));
            ***REMOVED***
        ***REMOVED***

        /// <summary>
        /// Method will show and doc window next to the Inspector Window.
        /// </summary>
        /// <param name="windowTitle">Window Title.</param>
        /// <param name="icon">Window Icon.</param>
        /// <returns>
        /// Returns the first EditorWindow which is currently on the screen.
        /// If there is none, creates and shows new window and returns the instance of it.
        /// </returns>
        public static TWindow ShowTowardsInspector(string windowTitle, Texture icon)
        {
            var inspectorType = Type.GetType("UnityEditor.InspectorWindow, UnityEditor.dll");
            var window = GetWindow<TWindow>(inspectorType);
            window.Show();

            window.titleContent = new GUIContent(windowTitle, icon);
            window.minSize = new Vector2(350, 100);

            return window;
        ***REMOVED***
    ***REMOVED***
***REMOVED***
