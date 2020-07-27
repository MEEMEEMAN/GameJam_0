using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Perhaps
{
    public class UIKvpPerhaps : MonoBehaviour
    {
        static Dictionary<string, List<UIKvpPerhaps>> pairs = new Dictionary<string, List<UIKvpPerhaps>>();
        [SerializeField] string keyListing;
        [SerializeField] TextMeshProUGUI text;

        public static void SetValue(string key, string value)
        {
            if(pairs.ContainsKey(key))
            {
                foreach (var item in pairs[key])
                {
                    item.OnValueSet(key, value);
                }
            }
        }

        public virtual void Start()
        {
            if (string.IsNullOrEmpty(keyListing))
                return;

            if (!pairs.ContainsKey(keyListing))
                pairs.Add(keyListing, new List<UIKvpPerhaps>());

            pairs[keyListing].Add(this);
        }

        public virtual void OnValueSet(string key, string value)
        {
            text.text = value;
        }
    }

}