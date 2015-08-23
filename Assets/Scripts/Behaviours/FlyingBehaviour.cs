using UnityEngine;
using System.Collections;

namespace Monster.Behaviours {
    public class FlyingBehaviour : MonsterBehaviour {
        protected override void OnSwitchIn() {
		}
		
		protected override void UpdateRigidbody() {
            /*
            _Rigidbody.AddForce(
                    AvatarUtils.GetFlyingVector(_Controller) *
                    _Config.FLYING_Continuous_Acceleration,
                    _Config.FLYING_Continous_Acceleration_Force_Mode);
                    */

        }

        protected override void UpdateContinuousInput() {
            //float axis = ControllerUtils.LeftStickHorizontal(_Controller.PlayerNumber);

            /*
            if (axis != 0f) {
                float rotationDelta = axis * -1 *
                        _Config.FLYING_Rotation_Speed;

                _Rigidbody.MoveRotation(_Rigidbody.rotation + rotationDelta);
            }
            */

        }

        protected override void OnAction1Up() {
        }

        protected override void OnAction2Down() {
		}
    }
}
