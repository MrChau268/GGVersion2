using UnityEngine;
using System.Collections;

public class CollisionSound : MonoBehaviour
{
    public AudioClip collisionSound;
    private AudioSource audioSource;
    private Rigidbody rb;
    private Collider col;

    // Track if the player is inside
    private bool playerInside = false;
    // Track if the sound has played for the current entry
    private bool hasPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        audioSource.playOnAwake = false;

        rb.isKinematic = true;
        rb.useGravity = false;
        col.isTrigger = true;

    }



    void Update()
    {
        // Check if player is inside trigger and pressed T
        if (playerInside && Input.GetKeyDown(KeyCode.T) && !hasPlayed)
        {
            PlayCollisionSound();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            hasPlayed = false; // reset for new entry
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            hasPlayed = false;
        }
    }

    private void PlayCollisionSound()
    {
        audioSource.PlayOneShot(collisionSound);

        // Activate physics
        rb.isKinematic = false;
        rb.useGravity = true;
        col.isTrigger = false;

        hasPlayed = true;

        Debug.Log("Sound played!");
    }
}
