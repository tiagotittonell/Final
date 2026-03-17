using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float time = 0.2f;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            StartCoroutine(DestroyAfterAnimation(animator));
        else
            Destroy(gameObject, time);
    }

    private IEnumerator DestroyAfterAnimation(Animator animator)
    {
        yield return null; 
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, length);
    }
}