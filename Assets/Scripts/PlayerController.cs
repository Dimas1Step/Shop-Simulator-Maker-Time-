using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private float gravityScale = 9.8f;
    private float speedScale = 5f;
    private float runScale = 10f;
    private float jumpForce = 2f;
    private float turnSpeed = 2f;
    public int HP = 100;
    public int crystal_count = 0;

    private float verticalSpeed = 0f;
    private float mouseX;
    private float mouseY;
    private float currentAngleX;

    private CharacterController controller;
    private Animator animator;

    [SerializeField] private Camera goCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private string targetTag = "Shop";
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text HP_Text;
    [SerializeField] TMP_Text Crystal_Text;

    private void Start()
    {
        slider.maxValue = HP;
            slider.value = HP;
    }

    private void LateUpdate()
    {
        slider.value = HP;
        HP_Text.text = "HP:" + HP;
        Crystal_Text.text = "crystal:" + crystal_count;
    }

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        RotateCharacter();
        MoveCharacter();
        ShopTeleport();
        Die();

        animator.SetBool("IsGrounded", controller.isGrounded);

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void RotateCharacter()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(0f, mouseX * turnSpeed, 0f);

        currentAngleX -= mouseY * turnSpeed;
        currentAngleX = Mathf.Clamp(currentAngleX, -60f, 60f);

        goCamera.transform.localRotation =
            Quaternion.Euler(currentAngleX, 0f, 0f);
    }

    private void MoveCharacter()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return;
        }
        float speed = speedScale;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        if (isRunning)
            speed = runScale;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z);
        move = Vector3.ClampMagnitude(move, 1f);

        float animatorSpeed = 0f;

        if (move.magnitude < 0.1f)
        {
            animatorSpeed = 0f;
        }
        else if (!isRunning)
        {
            animatorSpeed = 5f;
        }
        else
        {
            animatorSpeed = 10f;
        }

        animator.SetFloat("Speed", animatorSpeed);

        if (controller.isGrounded)
        {
            verticalSpeed = 0f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalSpeed = jumpForce;
                animator.SetBool("IsGrounded", false);
                animator.SetTrigger("Jump");
            }
        }

        verticalSpeed -= gravityScale * Time.deltaTime;
        move.y = verticalSpeed;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void ShopTeleport()
    {
        Ray ray = new Ray(goCamera.transform.position, goCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene("Shop");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teleport"))
        {
            SceneManager.LoadScene("Forest");
        }

        if (other.CompareTag("crystal"))
        {
            crystal_count += 1;
            Destroy.other;
        }
    }

    private void Die()
    {
        if(HP <= 0)
        {
            animator.SetTrigger("Die");
            animator.SetBool("IsGrounded", true);
            animator.SetFloat("Speed", 0f);
            this.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}