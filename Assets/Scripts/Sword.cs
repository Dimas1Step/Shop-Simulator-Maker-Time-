using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private int damageToEnemy;
    [SerializeField] public int cost;

    public int DamageToEnemy => damageToEnemy;
}
