using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UITriggerMessage : MonoBehaviour
{
    public ShowMessageItems messageData;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (messageData != null)
            {
                // Pass message + position of this object
                MessageUIManager.Instance.ShowMessage(messageData, transform.position);
            }
        }
    }
}
