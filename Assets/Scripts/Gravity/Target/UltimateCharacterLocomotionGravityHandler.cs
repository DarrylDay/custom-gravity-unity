#if UltimateCharacterController
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.ThirdPersonController.Camera.ViewTypes;

namespace ProjectCloud
{
    [RequireComponent(typeof(UltimateCharacterLocomotion))]
    public class UltimateCharacterLocomotionGravityHandler : GravityTarget
    {
        private UltimateCharacterLocomotion _ultimateCharacterLocomotion;
        private CameraController _cameraController;
        private ThirdPerson _thirdPersonView;

        protected override void Awake()
        {
            base.Awake();

            _ultimateCharacterLocomotion = GetComponent<UltimateCharacterLocomotion>();
            _ultimateCharacterLocomotion.AlignToGravity = false;
            _ultimateCharacterLocomotion.SmoothGravityYawDelta = false;

            if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
            if (_cameraController != null && _cameraController.ActiveViewType is ThirdPerson thirdPerson) _thirdPersonView = thirdPerson;
        }

        protected override void HandleGravityResult(Vector3 resultVector)
        {
            if (_thirdPersonView == null && _cameraController != null)
                _thirdPersonView = (_cameraController.ActiveViewType as ThirdPerson);

            if (resultVector != Vector3.zero)
            {
                var currentUp = transform.up;
                var currentRight = transform.right;
                var newUpVector = resultVector * -1f;

                var quaternion = new Quaternion();
                quaternion.SetFromToRotation(transform.up, newUpVector.normalized);

                var newRotation = quaternion * transform.rotation;
                _ultimateCharacterLocomotion.SetRotation(newRotation, false); 

                _ultimateCharacterLocomotion.GravityDirection = resultVector.normalized;
                _ultimateCharacterLocomotion.GravityMagnitude = resultVector.magnitude;

                if (_thirdPersonView != null)
                {
                    _thirdPersonView.MinPitchLimit = -72;
                    _thirdPersonView.MaxPitchLimit = 72;

                    var pitchDelta = Vector3.Angle(currentUp, transform.up);

                    _thirdPersonView.ChangeViewType(true, _thirdPersonView.Pitch - pitchDelta, _thirdPersonView.Yaw, quaternion * _thirdPersonView.m_CharacterRotation);
                }
            }
            else
            {
                _ultimateCharacterLocomotion.GravityMagnitude = 0f;

                if (_thirdPersonView != null)
                {
                    _thirdPersonView.MinPitchLimit = -1000;
                    _thirdPersonView.MaxPitchLimit = 1000;
                }
            }


        }
    }
}
#endif