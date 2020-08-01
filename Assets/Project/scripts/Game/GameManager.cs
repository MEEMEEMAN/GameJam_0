using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perhaps
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }
        [SerializeField] GameConstants constants;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Initialize()
        {
            constants.Init();
        }

        private void Start()
        {
            PlayerEntity player = FindObjectOfType<PlayerEntity>();
            MainCamera.instance.SetFollowTarget(player);
        }
    }

}