using UnityEngine;

[ExecuteInEditMode]
public class ProceduralRingMesh : MonoBehaviour
{
    public float innerRadius = 0.35f;
    public float outerRadius = 0.5f;
    public int segments = 64;

    void Start()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.PI * 2 * i / segments;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            vertices[i * 2]     = new Vector3(cos * innerRadius, 0, sin * innerRadius);
            vertices[i * 2 + 1] = new Vector3(cos * outerRadius, 0, sin * outerRadius);

            uv[i * 2]     = new Vector2(i / (float)segments, 0);
            uv[i * 2 + 1] = new Vector2(i / (float)segments, 1);

            int ti = i * 6;
            int vi = i * 2;

            triangles[ti] = vi;
            triangles[ti + 1] = (vi + 2) % (segments * 2);
            triangles[ti + 2] = vi + 1;

            triangles[ti + 3] = vi + 1;
            triangles[ti + 4] = (vi + 2) % (segments * 2);
            triangles[ti + 5] = (vi + 3) % (segments * 2);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
