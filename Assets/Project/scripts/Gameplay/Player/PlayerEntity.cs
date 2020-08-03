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
            {
                motor.Jump();
            }

            if(weapons.isEquipped)
            {
                Ray ray = MainCamera.instance.cam.ScreenPointToRay(input.MouseScreenPosition);
                weapons.equipped.Aim(ray.origin);


                switch (weapons.equipped.GetFireMode())
                {
                    case WeaponFireMode.SINGLE:
                        if (input.GetMouseTap(0))
                        {
                            weapons.equipped.Shoot();
                        }
                        break;
                    case WeaponFireMode.FULL:
                        if (input.GetMouseHeld(0))
                        {
                            weapons.equipped.Shoot();
                        }
                        break;
                    default:
                        break;
                }
            }
            

        }


    }

}