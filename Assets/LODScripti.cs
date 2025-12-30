using UnityEngine;

public class CreatureLODAnimation : MonoBehaviour
{
    public Animator animator;
    public LODGroup lodGroup;

    public float idleOnlyDistance = 15f;
    public float disableAnimationDistance = 30f;

    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(cam.position, transform.position);

        if (distance > disableAnimationDistance)
        {
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;
            animator.SetBool("IdleOnly", distance > idleOnlyDistance);
        }
    }
}
