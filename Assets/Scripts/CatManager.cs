using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatManager : MonoBehaviour
{
    static readonly int Walk = Animator.StringToHash("Walk");
    static readonly int Idle = Animator.StringToHash("Idle");
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    [SerializeField] private PlayerController playerController;

    private Transform targetAnimal = null;

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(targetAnimal ? targetAnimal.position : target.position);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.ResetTrigger(Walk);
            animator.SetTrigger(Idle);
        }
        else
        {
            animator.SetTrigger(Walk);
            animator.ResetTrigger(Idle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Animal"))
        {
            return;
        }

        ResetTarget(other.transform);
        playerController.AddAnimal();
        Destroy(other.gameObject);
    }

    public void SetTargetAnimal(Transform animal)
    {
        targetAnimal = animal;
    }

    public void ResetTarget(Transform target)
    {
        if (targetAnimal == target)
        {
            targetAnimal = null;
        }
    }
}
