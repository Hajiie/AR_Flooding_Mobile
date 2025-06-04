using com.zibra.liquid.Manipulators;
using UnityEditor;

namespace com.zibra.liquid.Editor.Solver
{
    internal class ZibraLiquidManipulatorEditor : UnityEditor.Editor
    {
        protected SerializedProperty CurrentInteractionMode;
        protected SerializedProperty ParticleSpecies;

        protected void TriggerRepaint()
        {
            Repaint();
        ***REMOVED***

        protected void OnEnable()
        {
            Manipulator manipulator = target as Manipulator;
            manipulator.OnChanged += TriggerRepaint;

            ParticleSpecies = serializedObject.FindProperty("ParticleSpecies");
            CurrentInteractionMode = serializedObject.FindProperty("CurrentInteractionMode");
        ***REMOVED***

        protected void OnDisable()
        {
            Manipulator manipulator = target as Manipulator;
            manipulator.OnChanged -= TriggerRepaint;
        ***REMOVED***
    ***REMOVED***
***REMOVED***
