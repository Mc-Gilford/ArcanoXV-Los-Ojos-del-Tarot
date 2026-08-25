using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    [SerializeField] private float volume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    
    void Start()
    {
        volume = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            audioSource.PlayOneShot(audioClip, volume);
            Destroy(gameObject,0.5f);
        }
    }
}
