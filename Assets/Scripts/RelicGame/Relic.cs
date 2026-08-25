using UnityEngine;

public class Relic : MonoBehaviour
{
    private RelicRoom relicRoomScript;

    private AudioSource audioSource;

    public AudioClip clueSound;

    [SerializeField] private float volume =1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        relicRoomScript = GameObject.Find("H8 La Reliquia").GetComponent<RelicRoom>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClue()
    {
        audioSource.PlayOneShot(clueSound, volume);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            relicRoomScript.DestroyRelic();
        }
    }
}
