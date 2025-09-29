using UnityEngine;
using System.Collections;
using System;
//C 2025 Daniel Snapir alias Baltazar Benoni
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float dashForce = 5f;
    [SerializeField] float dashRechargeTimer = 5f;
    [SerializeField] int dashAmount = 2;
    int dashCount = 0;
    float speed;
    [SerializeField] float eggHindrance = 0.35f;
    [SerializeField] float sensitivity = 1.5f;
    EggManager eggManager;
    float xInput;
    float zInput;
    bool idle;
    bool isGrounded;
    Rigidbody pBody;

    Animator animator;
    [SerializeField] ParticleSystem dashParticles;

    void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        dashParticles = dashParticles.GetComponent<ParticleSystem>();
        eggManager = GetComponent<EggManager>();
        pBody = GetComponent<Rigidbody>();
        InitSpeed();
        Actions.EggPickUp += ChangeSpeed;
        Actions.EggDrop += ChangeSpeed;
        Actions.UpdateChickenSpeed += ChangeSpeed;
    }
    void OnDisable()
    {
        Actions.EggPickUp -= ChangeSpeed;
        Actions.EggDrop -= ChangeSpeed;
        Actions.UpdateChickenSpeed -= ChangeSpeed;
    }
    void Start()
    {
    }
    void Update()
    {
        Move();
        isGrounded = GroundCheck();
        Dash();
        eggManager.playerMoving = pBody.linearVelocity.magnitude > 0.1f;
    }
    float GetInput()
    {
        zInput = Input.GetAxis("Vertical");
        eggManager.playerMoveInput = zInput;
        if (Mathf.Abs(zInput) < 0.3f)
        {
            idle = true; animator.SetBool("idle", true);
        } // anim idle checking
        else
        {
            idle = false; animator.SetBool("idle", false);
        }
        return zInput;
    }
    void Move()
    {
        float moveInput = GetInput();
        Vector3 movement = transform.forward * moveInput * speed;

        if (!idle) animator.SetFloat("blend", moveInput); // anim move

        eggManager.PlayerMovement = movement;
        transform.position += movement * Time.deltaTime; 
        RotatePlayer();
    }
    void RotatePlayer()
    {
        xInput = Input.GetAxis("Horizontal");

        if (idle) animator.SetFloat("blend", xInput); // anim rotate
        float deltaTimeFactor = Time.deltaTime / 0.01f;
        transform.RotateAround(transform.position, Vector3.up, xInput * sensitivity * deltaTimeFactor);
    }
    void ChangeSpeed()
    {
        speed = maxSpeed -  eggHindrance * eggManager.EggCount;
        eggManager.PlayerSpeed = speed;
    }
    void InitSpeed()
    {
        speed = maxSpeed;
        eggManager.PlayerSpeed = speed;
    }
    void Dash()
    {
        bool dash = Input.GetKeyDown(KeyCode.Space);
        if(transform.rotation.x != 0 || transform.rotation.z != 0)
        {
            FixRotation();
            Debug.Log("Rotation correction executed!");
        }
        if(!dash)
        {
            return;
        }
        if(dashCount > dashAmount)
        {
            Actions.ChangeAudio(1, "dash");
            Debug.Log("Dash has to recharge!!!");
            return;
        }
        Vector3 dashDirection = transform.forward + Vector3.up * 0.5f;
        if(isGrounded)
        {
            pBody.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            Actions.ChangeAudio(1, "dash");
            dashCount++;

            animator.SetTrigger("dash"); // anim dash
            dashParticles.Play(); // dashing particles

            if (dashCount == dashAmount)
            {
                StartCoroutine(DashRecharge());
            }
            if(Actions.Dash != null)
            {
                Actions.Dash();
            }
        }
    }
    IEnumerator DashRecharge()
    {
        yield return new WaitForSeconds(dashRechargeTimer);
        dashCount = 0;
    }
    bool GroundCheck()
    {
        bool grounded = this.transform.position.y <= 0.75f;
        Debug.Log("GROUND STATUS " + grounded);
        return grounded;
    }
    void FixRotation()
    {
        transform.Rotate(-transform.eulerAngles.x, 0 ,-transform.eulerAngles.z);
    }
}
