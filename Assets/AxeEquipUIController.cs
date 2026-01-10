using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

/*This Class is using for Job Scenes */
public class AxeEquipUIController : MonoBehaviour
{
    [Header("References")]
    public GameObject axeObject;
    public Camera playerCamera;

    [Header("Settings")]
    public float fadeDuration = 0.4f;
    public float interactDistance = 3f;
    public KeyCode equipKey = KeyCode.Alpha1;
    public KeyCode cutKey = KeyCode.E;

    private Renderer axeRenderer;
    private bool isEquipped;

    void Start()
    {
        axeRenderer = axeObject.GetComponentInChildren<Renderer>();
        axeObject.SetActive(false);

        SetAxeAlpha(0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(equipKey))
            ToggleAxe();

        if (isEquipped && Input.GetKeyDown(cutKey))
            TryCutTree();
    }

    void ToggleAxe()
    {
        isEquipped = !isEquipped;

        axeObject.SetActive(true);

        float targetAlpha = isEquipped ? 1f : 0f;

        axeRenderer.material
            .DOFade(targetAlpha, fadeDuration)
            .OnComplete(() =>
            {
                if (!isEquipped)
                    axeObject.SetActive(false);
            });
    }

    void TryCutTree()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Tree"))
            {
                Debug.Log("Tree cut: " + hit.collider.name);
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void SetAxeAlpha(float alpha)
    {
        Color c = axeRenderer.material.color;
        c.a = alpha;
        axeRenderer.material.color = c;
    }
}