using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Perhaps
{
    /// <summary>
    /// The Perhaps Framework's event system.
    /// </summary>
    public static class PerhapsMessageDispatcher
    {
        static Dictionary<int, List<Delegate>> listeners = new Dictionary<int, List<Delegate>>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener">The function that gets called during an event.</param>
        /// <param name="sole">Clear listener list before adding.</param>
        public static void RegisterListener<T>(Action<T> listener, bool sole = false) where T : IPerhapsMessage
        {
            Type t = typeof(T);
            int hash = t.GetHashCode();
            if (!listeners.ContainsKey(hash))
            {
                listeners.Add(hash, new List<Delegate>());
            }

            if (sole)
                listeners[hash].Clear();

            listeners[hash].Add(listener);
        }


        public static void UnRegisterListener<T>(Action<T> listener) where T : IPerhapsMessage
        {
            Type t = typeof(T);
            int hash = t.GetHashCode();

            if (listeners.TryGetValue(hash, out List<Delegate> list))
            {
                list.Remove(listener);
            }
        }

        /// <summary>
        /// Dispatch a message for all listeners to hear.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public static void Dispatch<T>(T message) where T : IPerhapsMessage
        {
            Type t = typeof(T);
            int hash = t.GetHashCode();

            if (listeners.TryGetValue(hash, out List<Delegate> list))
            {
                object[] msgObj = new object[] { message };
                for (int i = 0; i < list.Count; i++)
                {
                    Delegate d = list[i];
                    if (d == null || d.Target == null || (d.Target is UnityEngine.Object o && o == null))
                    {
                        list.RemoveAt(i);
                        i--;
                        continue;
                    }

                    d.Method.Invoke(d.Target, msgObj);
                }
            }
        }
    }

}