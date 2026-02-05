using UnityEngine;

public class GPopupTrigger : MonoBehaviour
{
    [SerializeField] protected GPopupUI popupPrefab;

    protected GPopupUI popupInstance;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (popupInstance == null)
        {
            popupInstance = Instantiate(popupPrefab);
        }

        var data = new GPopupMessage("Hello Player");
        popupInstance.OnOpen(data);
    }
}
