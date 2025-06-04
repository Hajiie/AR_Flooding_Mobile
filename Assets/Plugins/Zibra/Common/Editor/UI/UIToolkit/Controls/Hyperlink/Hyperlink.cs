using com.zibra.common.Foundation.Editor;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.zibra.common.Foundation.UIElements
{
    internal class Hyperlink : BindableElement
    {
        [UsedImplicitly]
        internal new class UxmlFactory : UxmlFactory<Hyperlink, UxmlTraits>
        {
        ***REMOVED***

        internal new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription m_Link = new UxmlStringAttributeDescription { name = "link" ***REMOVED***;

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((Hyperlink)ve).Link = m_Link.GetValueFromBag(bag, cc);
            ***REMOVED***
        ***REMOVED***

        internal const string USSClassName = "zibraai-hyperlink-block";
        internal const string HeaderUssClassName = USSClassName + "__header";
        internal const string ContentUssClassName = USSClassName + "__content";

        private string m_Link;
        private readonly VisualElement m_Container;

        public override VisualElement contentContainer => m_Container;

        internal string Link
        {
            get => m_Link;
            set => m_Link = value;
        ***REMOVED***

        public Hyperlink()
        {
            AddToClassList(USSClassName);

            var button = new Button() {
                name = "header",
            ***REMOVED***;
            button.clicked += () =>
            { Application.OpenURL(m_Link); ***REMOVED***;
            button.AddToClassList(HeaderUssClassName);

            m_Container = new VisualElement() {
                name = "content",
            ***REMOVED***;
            m_Container.AddToClassList(ContentUssClassName);

            button.Add(m_Container);

            hierarchy.Add(button);

            UIToolkitEditorUtility.ApplyStyleForInternalControl(this, nameof(Hyperlink));
        ***REMOVED***
    ***REMOVED***
***REMOVED***
