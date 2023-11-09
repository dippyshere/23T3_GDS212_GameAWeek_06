using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatManager : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.ResetTrigger("Walk");
            animator.SetTrigger("Idle");
        }
        else
        {
            animator.SetTrigger("Walk");
            animator.ResetTrigger("Idle");
        }
    }
}
