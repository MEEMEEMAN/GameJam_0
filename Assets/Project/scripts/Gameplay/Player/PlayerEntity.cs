using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perhaps;

namespace Game
{
    public class PlayerEntity : LivingEntity
    {
        public override void Update()
        {
            base.Update();

            Vector2 wasd = input.GetWASDVector();
            wasd.y = 0;

            motor.Move(wasd);

            if (input.GetKeyTap(KeyCode.Space))
                motor.Jump();
        }


    }

}