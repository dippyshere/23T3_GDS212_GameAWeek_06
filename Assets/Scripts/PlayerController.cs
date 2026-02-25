using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    static readonly int Run = Animator.StringToHash("Run");
    static readonly int Jump = Animator.StringToHash("Jump");
    static readonly int RunSpeedMult = Animator.StringToHash("RunSpeedMult");
    static readonly int Grounded = Animator.StringToHash("Grounded");

    [Header("Configuration")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float walkMultiplier = 1f;
    [SerializeField] private float runMultiplier = 1.3f;
    [SerializeField] private float runVisualMultiplier = 0.1f;
    [SerializeField] private float jumpForce = 15f;
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private TextMeshProUGUI animalCountText;
    [SerializeField] private CatManager catManager;

    private float groundCheckRadius = 0.3f;
    private float speed = 8;
    private bool isGrounded;
    private Rigidbody rigidBody;
    private Vector3 direction;
    private Vector3 moveDirection = Vector3.forward;
    private bool canMove = true;
    private int savedAnimals = 0;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;

    Camera _mainCamera;

    void Start()
    {
        rigidBody = transform.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateAnimalText();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        _mainCamera = Camera.main;
        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = Mathf.CeilToInt((float)Screen.currentResolution.refreshRateRatio.value);
        }
        else
        {
            Application.targetFrameRate = -1;
        }

        InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
        InputSystem.settings.SetInternalFeatureFlag("USE_READ_VALUE_CACHING", true);
    }

    void Update()
    {
        direction = moveAction.ReadValue<Vector2>();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        animator.SetBool(Grounded, isGrounded);

        if (jumpAction.WasPressedThisFrame() && isGrounded && canMove)
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool(Jump, true);
        }
        if (canMove)
        {
            visualTransform.rotation = Quaternion.Slerp(visualTransform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * 10f);
        }
    }

    void FixedUpdate()
    {
        bool isRunning = direction.magnitude > 0.1f;

        if (isRunning && canMove)
        {
            if (sprintAction.IsPressed())
            {
                speed = moveSpeed * runMultiplier;
                animator.SetBool(Run, true);
                //animator.SetBool("Walk", false);
            }
            else
            {
                speed = moveSpeed * walkMultiplier;
                animator.SetBool(Run, true);
                //animator.SetBool("Walk", true);
            }
            Vector3 viewDir = transform.position - _mainCamera.transform.position;
            viewDir.y = 0;
            orientation.forward = viewDir.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(viewDir);
            orientation.rotation = targetRotation;
            moveDirection = orientation.forward * direction.y + orientation.right * direction.x;
            rigidBody.AddForce(moveDirection.normalized * (speed * 10f), ForceMode.Force);
            
            //rigidBody.MovePosition(rigidBody.position + moveDirection * (speed * Time.fixedDeltaTime));

            animator.SetFloat(RunSpeedMult, direction.magnitude * rigidBody.linearVelocity.magnitude * runVisualMultiplier);
        }
        else
        {
            animator.SetBool(Run, false);
        }

        if (rigidBody.linearVelocity.y <= 0)
        {
            animator.SetBool(Jump, false);
        }
    }

    private void OnDrawGizmos()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    private void UpdateAnimalText()
    {
        animalCountText.text = "Saved Animals: " + savedAnimals.ToString("N0", CultureInfo.InvariantCulture) + "/9";
    }

    public void AddAnimal()
    {
        savedAnimals++;
        UpdateAnimalText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            AddAnimal();
            catManager.ResetTarget(other.transform);
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Animal2"))
        {
            catManager.SetTargetAnimal(other.transform);
        }
    }
}
