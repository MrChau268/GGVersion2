using UnityEngine;

public class PlayEffectStreakRun : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody playerRB;
    [SerializeField]
    protected ParticleSystem speedLines;
    [SerializeField]
    protected float fastThreshold = 5f;

    protected void Update()
    {
        float speed = playerRB.linearVelocity.magnitude;

        var emission = speedLines.emission;
        if (speed > fastThreshold)
        {
            emission.rateOverTime = 20;
        }
        else
        {
            emission.rateOverTime = 5;
        }
    }
}
