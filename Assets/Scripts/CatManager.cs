using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatManager : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    [SerializeField] private PlayerController playerController;

    private Transform targetAnimal = null;

    // Update is called once per frame
    void Update()
    {
        if (targetAnimal != null)
        {
            agent.SetDestination(targetAnimal.position);
        }
        else
        {
            agent.SetDestination(target.position);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            ResetTarget(other.transform);
            playerController.AddAnimal();
            Destroy(other.gameObject);
        }
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
