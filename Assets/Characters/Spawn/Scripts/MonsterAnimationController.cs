using UnityEngine;

public class MonsterAnimationControl : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Monster_StartLoop");
        Invoke(nameof(PlayIdle), 8f); // After 8 seconds, switch to Idle
    }

    void PlayIdle()
    {
        animator.Play("Monster_Idle");
    }
}
