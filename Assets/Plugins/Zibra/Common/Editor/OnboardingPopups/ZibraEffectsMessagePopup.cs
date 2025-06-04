using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using com.zibra.common.Editor.SDFObjects;

namespace com.zibra.common.Editor
{
    internal class ZibraEffectsMessagePopup : EditorWindow
    {
        const string UXML_GUID = "7e8488b56556d8c48a38cd92f80fbd6a";
        public static GUIContent WindowTitle => new GUIContent("Zibra Effects License Message");

        private string Url = "";

        private static void ShowWindow()
        {
            ZibraEffectsMessagePopup window = (ZibraEffectsMessagePopup)GetWindow(typeof(ZibraEffectsMessagePopup));
            window.titleContent = WindowTitle;
            window.Show();
        ***REMOVED***

        private void OnEnable()
        {
            var root = rootVisualElement;
            root.Clear();

            int width = 456;
            int height = 442;

            minSize = maxSize = new Vector2(width, height);

            var uxmlAssetPath = AssetDatabase.GUIDToAssetPath(UXML_GUID);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlAssetPath);
            visualTree.CloneTree(root);

            root.Q<Button>("Button").clicked += ButtonClick;
        ***REMOVED***

        private void ButtonClick()
        {
            Application.OpenURL(Url);
        ***REMOVED***

        static public void OpenMessagePopup(string headerText, string bodyText, string url, string buttonText)
        {
            ZibraEffectsMessagePopup popup = CreateInstance<ZibraEffectsMessagePopup>();
            popup.OpenMessagePopupInternal(headerText, bodyText, url, buttonText);
        ***REMOVED***

        private void OpenMessagePopupInternal(string headerText, string bodyText, string url, string buttonText)
        {
            var root = rootVisualElement;

            root.Q<Label>("TextTitle").text = headerText;
            root.Q<Label>("TextMessage").text = bodyText;
            if (buttonText == "")
            {
                root.Q<Button>("Button").style.display = DisplayStyle.None;
            ***REMOVED***
            else
            {
                root.Q<Button>("Button").text = buttonText;
            ***REMOVED***

            Url = url;
            ShowWindow();
        ***REMOVED***
    ***REMOVED***
***REMOVED***
