using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives;

public class VerminMovement : MonoBehaviour
{
    // [Header("References")]

    // [SerializeField]
    // private Transform vermin;

    // [SerializeField]
    // private Animator animator;

    // [Header("Stats")]

    // [SerializeField]
    // private float walkSpeed;

    // [SerializeField]
    // private float rotationSpeed;



    // [Header("Wandering parameters")]

    // [SerializeField]
    public List<Transform> pathPoints = new();

//     [SerializeField]
//     private Transform actualPoint;

//     private bool isDead = false;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//  void Start()
// {
//     if (pathPoints != null && pathPoints.Count > 0)
//     {
//         int randomNum = Random.Range(0, pathPoints.Count);
//         actualPoint = pathPoints[randomNum];
//     }
// }

//   public float switchDistance = 1.5f; 

//     void Update()
//     {
//         if (vermin == null || isDead) return;

//         if (Vector3.Distance(vermin.position, actualPoint.position) <= switchDistance)
//         {
//             int randomNum = Random.Range(0, pathPoints.Count);
//             actualPoint = pathPoints[randomNum];
//         }

//         Vector3 directionToTarget = actualPoint.position - vermin.position;
//         directionToTarget.y = 0; // On garde ça plat

//         if (directionToTarget != Vector3.zero)
//         {
//             Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//             vermin.rotation = Quaternion.Slerp(vermin.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//         }
//         vermin.position += vermin.forward * walkSpeed * Time.deltaTime;
        
//     }

//     public void VerminEnter(Transform obj, float speed)
//     {
//         vermin = obj;
//         animator = vermin.GetComponentInChildren<Animator>();
//         vermin.transform.rotation = new Quaternion(0, vermin.transform.rotation.y, 0, 0);
//         animator.SetBool("Walk", true); 
//         walkSpeed = speed;
//         isDead = false;
//     }

//     public void VerminExit()
//     {
//         vermin = null;
//         if(animator != null)
//         {
//             animator.SetBool("Walk", false);   
//         } 
//         animator = null;
//     }
//     public void KillVermin()
//     {
//         if (isDead) return;

//         isDead = true;
        
//         if (animator != null)
//         {
//             animator.SetTrigger("Death");
//             StartCoroutine(WaitForDeathAnimation());
//         }
//         else
//         {
//             DestroyVerminObject();
//         }
//     }

//     private IEnumerator WaitForDeathAnimation()
//     {
//         yield return null; 

//         //float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
//         yield return new WaitForSeconds(0.5f);

//         DestroyVerminObject();
//     }

//     private void DestroyVerminObject()
//     {
//         if (vermin != null)
//         {
//             Destroy(vermin.gameObject);
//             vermin = null;
//             animator = null;
//         }
//         isDead = false;
//     }
}
