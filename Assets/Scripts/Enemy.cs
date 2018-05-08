using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public enum EnemyState { Idle, Moving };
    [HideInInspector] public EnemyState enemyState;

    // COMPONENTS
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform associatedTransform;


    void Start()
    {
        animator = GetComponent<Animator>();
        associatedTransform = GetComponent<Transform>();
    }
}

