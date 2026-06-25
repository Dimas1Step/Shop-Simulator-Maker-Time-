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
    public int crystal_count = 100;
    public int cost;
    private bool isPaused = false;

    private float verticalSpeed = 0f;
    private float mouseX;
    private float mouseY;
    private float currentAngleX;

    private CharacterController controller;
    private Animator animator;

    string EQUIPE_NOT_SELECTED_TEXT = "";
    GameObject currentEquipedItem;

    [SerializeField] private Camera goCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private string targetTag = "Shop";
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text HP_Text;
    [SerializeField] TMP_Text Crystal_Text;
    [SerializeField] private GameObject Die_Panel;
    [SerializeField] private GameObject Pause_Panel;
    [SerializeField] public GameObject[] equipebleItems;
    [SerializeField] GameObject buyPanel;
    [SerializeField] Transform center;

    public List<Sword> ownedItems;

    ShopItem current;


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
        CheckShopItem();
        SwitchWeapon();

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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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
            if (hit.collider.CompareTag("Shop"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene("Shop");
                }
            }

            if (hit.collider.CompareTag("FromShop"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene("Home");
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
        if (HP <= 0)
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
        crystal_count = PlayerPrefs.GetInt($"Crystals_{slot}", 100);

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
            Time.timeScale = 1f;
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

    private void EquipItem(string toolName)
    {
        foreach (GameObject tool in equipebleItems)
        {
            if (tool.name == toolName)
            {
                tool.SetActive(true);
                currentEquipedItem = tool;
                toolName = EQUIPE_NOT_SELECTED_TEXT;
            }
            else
            {
                tool.SetActive(false);
            }
        }
    }

    public void BuyWeapon()
    {
        if (crystal_count >= current.cost)
        {
            for (int i = 0; i < Inventory.instance.ownedItems.Count; i++)
            {
                if (Inventory.instance.ownedItems[i] == current.item.name)
                {
                    return;
                }
            }
            crystal_count -= current.cost;
            Inventory.instance.ownedItems.Add(current.item.name);
        }
    }

    void CheckShopItem()
    {
        current = null;

        Collider[] hits = Physics.OverlapSphere(center.position, 1f);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<ShopItem>(out ShopItem data))
            {
                current = data;
                break;
            }
        }

        buyPanel.SetActive(current != null);
        Cursor.lockState = current != null ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = current != null;

        if (current != null)
        {
            buyPanel.transform.Find("Cost_Text").GetComponent<TMP_Text>().text = "Cost:" + current.cost.ToString();
            buyPanel.transform.Find("Name_Text").GetComponent<TMP_Text>().text = "Name:" + current.name; 
        }
    }

    void SwitchWeapon()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                Debug.Log("Натиснута цифра: " + i);
                foreach (GameObject item in equipebleItems)
                {
                    item.SetActive(false);
                }

                if (i > Inventory.instance.ownedItems.Count) return;
                for (int j = 0; j < equipebleItems.Length; j++)
                {
                    if (equipebleItems[j].name == Inventory.instance.ownedItems[i - 1])
                    {
                        equipebleItems[j].SetActive(true);
                    }
                }


            }
        }
    }
}