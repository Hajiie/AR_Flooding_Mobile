
using System;
using UnityEngine;

namespace com.zibra.common.Utilities
{
    /// <summary>
    ///     Wrapper for Unity's JsonUtility that can handle arrays.
    /// </summary>
    static class ZibraJsonUtil
    {
        const string WRAPPER_PREFIX = "{\"data\":";
        const string WRAPPER_SUFFIX = "***REMOVED***";

        /// <summary>
        ///     Equivalent to JsonUtility.FromJson<T>(json), but can handle arrays.
        /// </summary>
        public static T FromJson<T>(string json)
        {
            if (typeof(T).IsArray)
            {
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(WRAPPER_PREFIX + json + WRAPPER_SUFFIX);
                return wrapper.data;
            ***REMOVED***
            else
            {
                return JsonUtility.FromJson<T>(json);
            ***REMOVED***
        ***REMOVED***

        /// <summary>
        ///     Equivalent to JsonUtility.ToJson(obj), but can handle arrays.
        /// </summary>
        public static string ToJson<T>(T obj)
        {
            if (typeof(T).IsArray)
            {
                Wrapper<T> wrapper = new Wrapper<T>();
                wrapper.data = obj;
                string json = JsonUtility.ToJson(wrapper);
                json = json.Substring(WRAPPER_PREFIX.Length, json.Length - WRAPPER_PREFIX.Length - WRAPPER_SUFFIX.Length);
                return json;
            ***REMOVED***
            else
            {
                return JsonUtility.ToJson(obj);
            ***REMOVED***
        ***REMOVED***

        [Serializable]
        struct Wrapper<T>
        {
            public T data;
        ***REMOVED***
    ***REMOVED***
***REMOVED***