using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float distToGround = 0.2f;
    [SerializeField] LayerMask groundMask;

    [Header("Movement")]
    [SerializeField] float normalStepOffset = 0.3f;
    [SerializeField] [Range(1f, 10f)] float normalSpeed = 3f;
    [SerializeField] [Range(4f, 15f)] float sprintSpeed = 6f;
    [SerializeField] [Range(0.1f, 5f)] float speedRateChange = 1f;
    [SerializeField] [Range(0.1f, 1f)] float turnSmoothTime = 0.1f;

    [Header("Jump")]
    [SerializeField] [Range(1f, 5f)] float jumpHeight = 5f;
    [SerializeField] [Range(0.5f, 3f)] float delayBetweenJump = 1f;
    [SerializeField] float gravity = -9.81f;

    [Header("Weapon")]
    [SerializeField] Weapon weapon;


    float _currentSpeed;
    float _maxSpeed;

    bool isGrounded;
    bool isAim;
    bool isWeapon;
    bool isJumping = true;
    bool isMove = false;
    public bool IsMove
    {
        get { return isMove; }
        set { isMove = value; }
    }

    Vector3 velocity;
    PlatformMovement platformMovement;

    Transform currentCamera;
    float turnSmoothVelosity;
    CharacterController controller;
    Animator animator;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentCamera = FindObjectOfType<CameraController>().gameObject.transform;
        animator = GetComponentInChildren<Animator>();

        _maxSpeed = normalSpeed;
    }

    private void Start()
    {
        EventsManager.instance.onChangeStateTrigger += ChangeStateTrigger;
    }

    private void OnEnable()
    {
        EventsManager.instance.onChangeStateTrigger -= ChangeStateTrigger;
    }

    private void ChangeStateTrigger(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Play:
                isMove = true;
                break;

            default:
                break;
        }
    }

    void Update()
    {
        Move();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Sprint"))
        {
            _maxSpeed = sprintSpeed;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            _maxSpeed = normalSpeed;
        }

        if (isGrounded && isMove && isWeapon)
        {
            if (Input.GetButtonDown("Aim"))
            {
                isAim = true;
                animator.SetBool("aim", isAim);
            }
            else if (Input.GetButtonUp("Aim"))
            {
                isAim = false;
                animator.SetBool("aim", isAim);
            }
        }

        if (Input.GetButtonDown("Fire") && isAim)
        {
            weapon.Shot(transform);
        }
    }

    public void Move()
    {
        if (isMove)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, distToGround, groundMask);
            animator.SetBool("isFalling", !isGrounded);

            if (!isGrounded) animator.SetFloat("falling", velocity.y);
            else if (velocity.y < -9.5f) StartCoroutine(WaitToMove());

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

            if (direction.x != 0 || direction.z != 0) _currentSpeed += Time.deltaTime * speedRateChange;
            else _currentSpeed = 0;
            _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

            if (!isAim && direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + currentCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelosity, turnSmoothTime);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                animator.SetFloat("running", _currentSpeed);
                controller.Move(moveDir.normalized * _currentSpeed * Time.deltaTime);
            }
            else animator.SetFloat("running", 0f);

            if (isAim)
            {
                float targetAngle = Mathf.Atan2(0, 1) * Mathf.Rad2Deg + currentCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelosity, turnSmoothTime);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }


            controller.Move(velocity * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
        }

        if (platformMovement != null) controller.Move(platformMovement.Direction * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded && isJumping)
        {
            animator.SetBool("isJumping", true);
            isJumping = false;
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            isGrounded = false;

            EventsManager.instance.JumpTrigger();

            StartCoroutine(WaitToJump());
        }
    }

    public bool PickingUp()
    {
        if (isGrounded)
        {
            StartCoroutine(WaitToTake());
            return true;
        }
        else return false;
    }

    IEnumerator WaitToMove()
    {
        isMove = false;
        yield return new WaitForSeconds(1.7f);
        isMove = true;
    }

    IEnumerator WaitToTake()
    {
        isMove = false;
        animator.SetTrigger("pickingup");
        yield return new WaitForSeconds(2.2f);
        isMove = true;
        isWeapon = true;
    }

    IEnumerator WaitToJump()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isJumping", false);
        yield return new WaitForSeconds(delayBetweenJump-0.2f);
        isJumping = true;
    }

    public void Interaction()
    {
        Transform cam = GetComponentInChildren<Camera>().transform;
        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2f))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            //if (hit.collider.gameObject.CompareTag("Console"))
            //{

            //}
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            platformMovement = other.GetComponentInParent<PlatformMovement>();
            controller.stepOffset = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            platformMovement = null;
            controller.stepOffset = normalStepOffset;
        }
    }

    public void Win()
    {

        isWeapon = false;
        animator.SetTrigger("win");
        isMove = false;
    }
}
