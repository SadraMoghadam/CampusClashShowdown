using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float runSpeed = 3;
    [SerializeField] private float rotateSpeed = 360;
    public KeyCode interactKey = KeyCode.F;
    private Vector3 _input;
    private Rigidbody _rb;
    private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        GatherInput();
        Look();
    }

    private void FixedUpdate() {
        Move();
    }

    private void GatherInput() {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        // ANIMATIONS
        // idle
        if (_input == Vector3.zero)
        {
            _animator.SetFloat(Speed, 0);
        }
        // Walk
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            _animator.SetFloat(Speed, 0.5f);
        }
        // run
        else
        {
            _animator.SetFloat(Speed, 1);
        }
    }

    private void Look() {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(Utilities.ToIso(_input), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime);
    }

    private void Move()
    {
        float actualSpeed = speed;
        // run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            actualSpeed = runSpeed;  
        }
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * actualSpeed * Time.deltaTime);
    }


}