using System;

namespace com.zibra.common.DataStructures
{
// C# doesn't know we use it with JSON deserialization
#pragma warning disable 0649
    [Serializable]
    internal class ObjectTransform
    {
        public string Q;
        public string T;
        public string S;
    ***REMOVED***

    [Serializable]
    internal class VoxelRepresentation
    {
        public string embeds;
        public string sd_grid;
        public ObjectTransform transform;
    ***REMOVED***

    [Serializable]
    internal class SkinnedVoxelRepresentation
    {
        public VoxelRepresentation[] meshes_data;
    ***REMOVED***
#pragma warning restore 0649
***REMOVED***