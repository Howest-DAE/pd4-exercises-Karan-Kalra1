using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private CharacterController _controller;
    [SerializeField]
    private InputActionReference _moveInput;
    [SerializeField]
    private float _moveSpeed = 3f;

    [SerializeField]
    private float _verticalSpeed = 5f;

    [SerializeField]
    private float _rotationSpeed = 3f;

    private Vector3 _moveDirection;

    private void Start()
    {
        if (!IsOwner) return;
        _moveInput.action.Enable();

        
    }
    void FixedUpdate()
    {

        if (IsOwner) //Input is handled on the local client
        {
            Vector2 input = _moveInput.action.ReadValue<Vector2>();
            if (input.sqrMagnitude > 0f)
            {
                MovePlayerRpc(input); //Invoke RPC to send input to the Server
            }
        }


        if (IsServer)
        {
            //apply gravity
            _verticalSpeed += Physics.gravity.y * Time.fixedDeltaTime;
            _controller.Move(Vector3.up * _verticalSpeed * Time.fixedDeltaTime);
            if (_controller.isGrounded)
            {
                _verticalSpeed = 0f;
            }

            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            _rotationSpeed * Time.fixedDeltaTime);

        }


    }





    //Movement executed on the server
    [Rpc(SendTo.Server)]
    void MovePlayerRpc(Vector2 input)
    {
        //TIP: to avoid cheating, clamp the input
        Mathf.Clamp(input.x, -1f, 1f);
        Mathf.Clamp(input.y, -1f, 1f);

        Vector3 moveVelocity = new Vector3(input.x, 0f, input.y) * _moveSpeed;
        _controller.Move(moveVelocity * Time.fixedDeltaTime);
        _moveDirection = moveVelocity.normalized;

    }



}

