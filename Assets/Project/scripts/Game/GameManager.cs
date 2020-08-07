using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
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

            Physics2D.IgnoreLayerCollision(11 /*Entity*/, 10 /*Rope*/, true);
            Physics2D.IgnoreLayerCollision(10 /*Rope*/, 10 /*Rope*/, true);
        }

        private void Start()
        {
            PlayerEntity player = FindObjectOfType<PlayerEntity>();
            MainCamera.instance.SetFollowTarget(player);
        }
    }

}