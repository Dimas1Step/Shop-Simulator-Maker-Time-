using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    private float gravityScale = 9.8f;
    private float speedScale = 5f;
    private float runScale = 10f;
    private float jumpForce = 2f;
    private float turnSpeed = 2f;
    public int HP = 100;
    public int crystal_count = 0;
    private bool isPaused = false;

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
    [SerializeField] private GameObject Die_Panel;
    [SerializeField] private GameObject Pause_Panel;

    private void Start()
    {
        LoadGame();
        slider.maxValue = HP;
            slider.value = HP;

        Die_Panel.SetActive(false);
        Pause_Panel.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

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

        if (other.CompareTag("GoHome"))
        {
            SceneManager.LoadScene("Home");
        }

        if (other.CompareTag("crystal"))
        {
            crystal_count += 1;
            SaveGame();
            Destroy(other.gameObject);
        }
    }

    private void Die()
    {
        if(HP <= 0)
        {
            animator.SetTrigger("Die");
            animator.SetBool("IsGrounded", true);
            animator.SetFloat("Speed", 0f);

            Die_Panel.SetActive(true);
            SaveGame();
            this.enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoHome()
    {
        HP = 100;
        crystal_count = crystal_count / 2;
        SaveGame();
        SceneManager.LoadScene("Home");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot", 1);

        PlayerPrefs.SetInt($"HP_{slot}", HP);
        PlayerPrefs.SetInt($"Crystals_{slot}", crystal_count);

        PlayerPrefs.SetString($"Time_{slot}",
            DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

        PlayerPrefs.Save();

        Debug.Log("Гру збережено");
    }

    public void LoadGame()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot", 1);

        HP = PlayerPrefs.GetInt($"HP_{slot}", 100);
        crystal_count = PlayerPrefs.GetInt($"Crystals_{slot}", 0);

        Debug.Log("Гру завантажено");
    }

    public void PauseGame()
    {
        isPaused = !isPaused;

        Pause_Panel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;

        Die_Panel.SetActive(false);
        Pause_Panel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}