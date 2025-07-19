using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Custom;
using static Models;
using Player.Character;
using Project.Scripts;
using Project.Scripts.Character;
using Project.Scripts.Enums;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    private CharacterController characterController;
    private Vector2 movement;
    private Vector3 currentSpeed;
    [SerializeField] private Vector2 view;
    [Header("References")]
    [SerializeField] private Transform gunHolder;
    [Space(10),Header("Settings")]
    [SerializeField]private PlayerSettings playerSettings;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private Camera cam;
    [SerializeField]private Vector3 camOffset;
    [SerializeField]private float rotSmoothTime;
    private bool isSprinting;
    private bool isJumping;
    private ILook look;
    private IMove move;/*
    private IStanceCalculate stanceCalculation;*/
    private bool canLook = true;
    private bool canMove = true;
    private bool isJumprework;
    private void Awake()
    {
        InputManager.playerInput.Character.Enable();
        InputManager.playerInput.Character.Movement.performed += e =>
        {
            if (canMove)
            {
                movement = e.ReadValue<Vector2>();
                if (movement.y >0)
                {
                    isJumping = true;
                }
                else if(!isJumprework)
                {
                    isJumping = false;
                }
            }

        };
        InputManager.playerInput.Character.Jump.performed += e =>
        {
            if (canMove)
            {            
                isJumping = true;
                isJumprework = true;
            }

        };
        InputManager.playerInput.Character.JumpUp.performed += e =>
        {
            if (canMove)
            {            
                isJumping = false;
                isJumprework = false;
            }
        };
        InputManager.playerInput.Character.Sprint.performed += e => isSprinting = true;
        InputManager.playerInput.Character.SprintUp.performed += e => isSprinting = false;
        characterController = GetComponent<CharacterController>();/*
        stanceCalculation = new StanceCalculation(_stand, _crouch, _prone, cameraHolder, characterController, CanStand);*/
        InputManager.playerInput.Character.View.performed += e =>
        {
            if(canLook&&ModeManager.Instance.nowMode==Mode.ShootMode) view = e.ReadValue<Vector2>();
        };
        InputManager.playerInput.Enable();
        look = new Look(gunHolder, cam, camOffset);
        move = new Move(playerSettings.jumpSettings,playerSettings.movementSettings, transform, characterController);
        
    }
    /*private void ChangeCursorState(bool state)
    {
        Cursor.visible = state;
        Cursor.lockState = !state ? CursorLockMode.Locked : CursorLockMode.None;
    }*/
    // Update is called once per frame
    void FixedUpdate()
    {
        move.CalculateMovement(isSprinting, movement, ref currentSpeed); 
        if(canLook&&ModeManager.Instance.nowMode==Mode.ShootMode){ look.CalculateView(view, rotSmoothTime);}
        move.CalculateJump(isJumping);/*
        stanceCalculation.CalculateStance(currentPlayerStance, playerStanceSmoothing);*/
    }
    /*private bool CanStand(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x,
            feetTransform.position.y+stanceCheckErrorMargin+characterController.radius,
            feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x,
            feetTransform.position.y-stanceCheckErrorMargin-characterController.radius+stanceCheckHeight,
            feetTransform.position.z);
        
        return Physics.CheckCapsule(start, end,characterController.radius ,groundLayer);
    }*/
    public void CanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void CanLook(bool canLook)
    {
        this.canLook = canLook;
    }
}
