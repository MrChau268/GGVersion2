using UnityEngine;

public class FlowerCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Flower flower = other.GetComponentInParent<Flower>();

        if (flower != null && flower.lightEffectPrefab != null && flower.currentEffect == null)
        {
            // Spawn effect and store it
            flower.currentEffect = Instantiate(
                flower.lightEffectPrefab,
                flower.transform.position,
                Quaternion.identity
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Flower flower = other.GetComponentInParent<Flower>();

        if (flower != null && flower.currentEffect != null)
        {
            Destroy(flower.currentEffect);   // remove glow when player leaves
            flower.currentEffect = null;
        }
    }
}
