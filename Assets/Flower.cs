using UnityEngine;

public class Flower : MonoBehaviour
{
    public GameObject lightEffectPrefab;
    
    [HideInInspector] 
    public GameObject currentEffect;  // store the spawned effect
}
