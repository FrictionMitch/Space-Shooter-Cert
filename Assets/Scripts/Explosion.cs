using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float _destroyDelay = 2f;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(_explosionSound);
        Destroy(this.gameObject, _destroyDelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
