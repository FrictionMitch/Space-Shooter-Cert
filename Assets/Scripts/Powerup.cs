using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private float _magnetSpeed = 6f;
    [SerializeField]
    private float _bottomOfScreen = -6f;
    [SerializeField]
    private int _powerupID = 0;

    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _powerUpSound;

    private Player _player;

    private bool _wasCalled = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _wasCalled = true;
        }
        if (!_wasCalled)
        {
            Movement();
        }
        else
        {
            MoveToPlayer();
        }

        
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
            //Player player = other.GetComponent<Player>();
            if (_player)
            {
                AudioSource.PlayClipAtPoint(_powerUpSound, Camera.main.transform.position);

                switch (_powerupID)
                {
                    case 0:
                        _player.EnableTripleShot();
                        _player.FillAmmo();
                        break;
                    case 1:
                        _player.EnableSpeedBoost();
                        break;
                    case 2:
                        _player.EnableShield();
                        break;
                    case 3:
                        _player.FillAmmo();
                        break;
                    case 4:
                        // add health
                        _player.AddLife();
                        break;
                    case 5:
                        // enable another powerup
                        _player.EnableMinis();
                        break;
                    case 6:
                        // invert controls (negative powerup)
                        _player.InvertControls();
                        break;
                    case 7:
                        // heat seaker
                        _player.EnableHeatSeeker();
                        break;
                }
            }
            Destroy(this.gameObject, 0.01f);
        }
        
    }

    private void MoveToPlayer()
    {
        //_speed = _magnetSpeed;
        float attractRate = _magnetSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, attractRate);
    }
}
