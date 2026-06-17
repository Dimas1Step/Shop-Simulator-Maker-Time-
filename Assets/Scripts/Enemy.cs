using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int health;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider healthSlider;

    private bool isDead = false;

    private NavMeshAgent agent;
    private Transform target;
    private Animator animator;

    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 10f;

    [SerializeField] private GameObject[] drop;
    [SerializeField] private int dropCount = 2;
    [SerializeField] private float dropRadius = 1.5f;

    private float nextAttackTime = 0f;

    public void AttackPlayer()
    {
        if(Vector3.Distance(transform.position, target.position) < 2f)
        {
            target.GetComponent<PlayerController>().HP -= 10;
        }
    }

    private void Start()
    {
        health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        UpdateHealthUI();

        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            target = player.transform;
        }
    }

    private void Update()
    {
        if (isDead || target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackDistance)
        {
            agent.destination = transform.position;
            animator.SetBool("IsWalking", false);

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else if (distance <= chaseDistance)
        {
            agent.destination = target.position;
            animator.SetBool("IsWalking", true);
        }
        else
        {
            agent.destination = transform.position;
            animator.SetBool("IsWalking", false);
        }
    }

    private void Attack()
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        Debug.Log("Enemy attacks!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("weapon"))
        {
            health -= 10;

            UpdateHealthUI();

            Debug.Log("Enemy hit! HP: " + health);

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = health + " / " + maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    private void Die()
    {
        isDead = true;

        animator.SetBool("IsWalking", false);
        animator.SetTrigger("Die");

        agent.enabled = false;

        SpawnDrops();

        Destroy(gameObject, 3f);
    }

    private void SpawnDrops()
    {
        if (drop == null || drop.Length == 0)
            return;

        for (int i = 0; i < dropCount; i++)
        {
            GameObject prefab = drop[Random.Range(0, drop.Length)];

            Vector3 randomOffset = Random.insideUnitSphere * dropRadius;
            randomOffset.y = 0;

            Vector3 spawnPos = transform.position + randomOffset;
            Debug.Log("Spawning drop at: " + spawnPos);

            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}