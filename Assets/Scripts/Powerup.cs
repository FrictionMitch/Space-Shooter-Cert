using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private float _bottomOfScreen = -6f;
    [SerializeField]
    private int _powerupID = 0;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _powerUpSound;


    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);

        if (transform.position.y <= _bottomOfScreen)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player)
            {
                AudioSource.PlayClipAtPoint(_powerUpSound, Camera.main.transform.position);

                switch (_powerupID)
                {
                    case 0:
                        player.EnableTripleShot();
                        break;
                    case 1:
                        player.EnableSpeedBoost();
                        break;
                    case 2:
                        player.EnableShield();
                        break;

                }
            }
            Destroy(this.gameObject, 0.01f);
        }
        
    }
}
