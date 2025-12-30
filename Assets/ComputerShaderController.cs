using UnityEngine;

public class ComputeShaderController : MonoBehaviour
{
    public ComputeShader computeShader;

    const int VIRTUAL_WIDTH = 200000;
    const int VIRTUAL_HEIGHT = 200000;

    const int TILE_SIZE = 1024;
    const int THREADS_X = 8;
    const int THREADS_Y = 8;

    const int MAX_GROUPS = 65535;

    RenderTexture tileTexture;
    int kernel;

    void Start()
    {
        kernel = computeShader.FindKernel("CSMain");

        tileTexture = new RenderTexture(
            TILE_SIZE,
            TILE_SIZE,
            0,
            RenderTextureFormat.RFloat
        );
        tileTexture.enableRandomWrite = true;
        tileTexture.Create();

        computeShader.SetTexture(kernel, "Result", tileTexture);
        computeShader.SetInt("_VirtualWidth", VIRTUAL_WIDTH);
        computeShader.SetInt("_VirtualHeight", VIRTUAL_HEIGHT);

        ProcessTiles();
    }

    void ProcessTiles()
    {
        int tilesX = Mathf.CeilToInt(VIRTUAL_WIDTH / (float)TILE_SIZE);
        int tilesY = Mathf.CeilToInt(VIRTUAL_HEIGHT / (float)TILE_SIZE);

        int groupsXTotal = TILE_SIZE / THREADS_X;
        int groupsYTotal = TILE_SIZE / THREADS_Y;

        for (int ty = 0; ty < tilesY; ty++)
        {
            for (int tx = 0; tx < tilesX; tx++)
            {
                int baseOffsetX = tx * TILE_SIZE;
                int baseOffsetY = ty * TILE_SIZE;

                // 🔒 DISPATCH CHUNKING — IMPOSSIBLE TO EXCEED LIMIT
                for (int gx = 0; gx < groupsXTotal; gx += MAX_GROUPS)
                {
                    for (int gy = 0; gy < groupsYTotal; gy += MAX_GROUPS)
                    {
                        int dispatchX = Mathf.Min(MAX_GROUPS, groupsXTotal - gx);
                        int dispatchY = Mathf.Min(MAX_GROUPS, groupsYTotal - gy);

                        computeShader.SetInt(
                            "_GlobalOffsetX",
                            baseOffsetX + gx * THREADS_X
                        );
                        computeShader.SetInt(
                            "_GlobalOffsetY",
                            baseOffsetY + gy * THREADS_Y
                        );

                        computeShader.Dispatch(kernel, dispatchX, dispatchY, 1);
                    }
                }
            }
        }
    }
}
