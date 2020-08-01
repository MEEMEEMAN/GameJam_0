using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    /// <summary>
    /// Represents a handle for a pooled Resource.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PooledResource<T>
    {
        protected static Dictionary<Type, Deque<object>> Pool = new Dictionary<Type, Deque<object>>();
        public static void Populate(params object[] resources)
        {
            Type t = typeof(T);

            if (!Pool.ContainsKey(t))
            {
                Pool.Add(t, new Deque<object>());
            }

            Pool[t].AddRange(resources);
        }

        public T Resource { get; protected set; }

        public void GetResource()
        {
            if (Pool.TryGetValue(typeof(T), out var collection))
            {
                Resource = (T)collection.RemoveBack();
            }
        }

        public void ReturnResource()
        {
            if(Resource != null)
            {
                Pool[typeof(T)].AddFront(Resource);
                Resource = default;
            }
        }
    }

}