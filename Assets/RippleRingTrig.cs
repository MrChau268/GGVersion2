using UnityEngine;

public class RippleRing : MonoBehaviour
{
    public float speed = 1.5f;
    public float lifetime = 1.5f;

    Material mat;
    float age;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        age += Time.deltaTime;

        // grow ring
        transform.localScale += Vector3.one * speed * Time.deltaTime;

        // fade out
        float fade = 1f - (age / lifetime);
        mat.SetFloat("_Fade", fade);

        if (fade <= 0f)
            Destroy(gameObject);
    }
}
