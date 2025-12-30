using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject highlightMesh;     // Ring or glow mesh
    public GameObject buttonCanvas;      // UI canvas to open on interaction
    public Camera playerCamera;          // Assign main camera

    private bool isPlayerNearby = false;

    void Start()
    {
        if (highlightMesh != null)
            highlightMesh.SetActive(false);

        if (buttonCanvas != null)
            buttonCanvas.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerNearby || playerCamera == null)
            return;

        FacePlayer();

        // Check for click while player is near
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    Debug.Log("NPC Clicked!");
                    if (buttonCanvas != null)
                        buttonCanvas.SetActive(true);
                }
            }
        }
    }

    void FacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (highlightMesh != null)
                highlightMesh.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (highlightMesh != null)
                highlightMesh.SetActive(false);

            if (buttonCanvas != null)
                buttonCanvas.SetActive(false);
        }
    }
}
