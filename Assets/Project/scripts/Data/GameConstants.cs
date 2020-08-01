using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    [CreateAssetMenu(menuName ="Game/Constants")]
    public class GameConstants : ScriptableObject
    {
        public static GameConstants instance { get; private set; }

        public void Init()
        {
            instance = this;
        }


        public float groundedRaycastLength = 1f;
    }

}