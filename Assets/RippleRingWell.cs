using UnityEngine;

public class RippleSpawner : MonoBehaviour
{
    public Camera cam;
    public GameObject ripplePrefab;
    public LayerMask waterMask;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, waterMask))
            {
                Vector3 pos = hit.point;

                // spawn ripple at water surface
                Instantiate(ripplePrefab, pos + Vector3.up * 0.01f, Quaternion.Euler(90,0,0));
            }
        }
    }
}
