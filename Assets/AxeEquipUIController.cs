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
    [Header("Setting")]
    public float fadeDuration = 0.4f;
    public float interactDistance = 3f;
    public KeyCode equipKey = KeyCode.Alpha1;
    public KeyCode cutKey = KeyCode.E;
    private Renderer axeRendere;
    private bool isEquipped;

    protected void Start()
    {
        axeRendere = axeObject.GetComponentInChildren<Renderer>();
        axeObject.SetActive(false);
        SetAxeAlpha(0f);
    }

    protected void SetAxeAlpha(float alpha)
    {
        Color c = axeRendere.material.color;
        c.a = alpha;
        axeRendere.material.color = c;
    }

    protected void ToggleAxe()
    {
        isEquipped = !isEquipped;
        axeObject.SetActive(true);
        float targetAlpha = isEquipped ? 1f : 0f;
        axeRendere.material.DOFade(targetAlpha, fadeDuration).OnComplete(() =>
        {
            if (!isEquipped)
            {
                axeObject.SetActive(false);
            }
        });
    }


    protected void TryCutTree()
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

}
