using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerminTrigger : MonoBehaviour
{

     [Header("References")]

    [SerializeField]
    private Animator animator;

    [Header("Stats")]

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float rotationSpeed;



    [Header("Wandering parameters")]

    [SerializeField]
    private List<Transform> pathPoints = new();

    [SerializeField]
    private Transform actualPoint;

    private bool isDead = false;


    public float switchDistance = 1.5f; 

    private void Start() 
    {
        animator = this.GetComponentInChildren<Animator>();  
    }
    void Update()
    {
        if (isDead) return;
        if(pathPoints.Count == 0) return;
        if(actualPoint == null)
        {
            actualPoint = pathPoints[Random.Range(0, pathPoints.Count)];
        }

        if (Vector3.Distance(this.transform.position, actualPoint.position) <= switchDistance)
        {
            int randomNum = Random.Range(0, pathPoints.Count);
            actualPoint = pathPoints[randomNum];
        }

        Vector3 directionToTarget = actualPoint.position - this.transform.position;
        directionToTarget.y = 0; // On garde ça plat

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        this.transform.position += this.transform.forward * walkSpeed * Time.deltaTime;
        
    }

    public void KillVermin()
    {
        if (isDead) return;

        isDead = true;
        
        if (animator != null)
        {
            animator.SetTrigger("Death");
            StartCoroutine(WaitForDeathAnimation());
        }
        else
        {
            DestroyVerminObject();
        }
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return null; 
        yield return new WaitForSeconds(0.5f);

        DestroyVerminObject();
    }

    private void DestroyVerminObject()
    {
        Destroy(this.gameObject);
        isDead = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SimiliNavMesh")
        {
            isDead = false;
            pathPoints = other.gameObject.GetComponent<VerminMovement>().pathPoints;
            this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, 0);
            if(animator != null)
                animator.SetBool("Walk", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "SimiliNavMesh")
        {
            isDead = true;
            if(animator != null)
            {
                animator.SetBool("Walk", false);   
            }
        }
    }
    public void HitByWeapon()
    {
        KillVermin();
    }
}
