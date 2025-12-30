using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera targetCamera; // assign in inspector

    void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 dir = transform.position - targetCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
