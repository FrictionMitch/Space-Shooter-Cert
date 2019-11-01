using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    [SerializeField]
    private float _topOfScreen = 7.4f, _bottomOfScreen = -5.4f;
    [SerializeField]
    private float _leftSideOfScreen = -9.5f, _rightSideOfScreen = 9.5f;

    [SerializeField]
    private int _points = 10;

    [Header("Damage")]
    [SerializeField]
    private int _shootDamage = 100, _kamakazeDamage = 200;
    [SerializeField]
    private float _deathDelay = 2f;

    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _explosion;

    [SerializeField]
    private GameObject _enemyLaser;
    [SerializeField]
    private float _fireDelay = 3f;
    private float _lastFire;

    [SerializeField]
    private bool _canDodge = false;

    [SerializeField]
    private bool _canShootBackwards = false;

    [SerializeField]
    private bool _canTargetPowerups = false;

    [SerializeField]
    private bool _canZigZag = true;

    [SerializeField]
    private int _minZigAmount = 1, _maxZigAmount = 5;
    private int _zigAmount;

    // Start is called before the first frame update
    void Start()
    {
        _zigAmount = Random.Range(_minZigAmount, _maxZigAmount);
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (!_player)
        {
            Debug.Log("Player is null");
        }
        _anim = GetComponent<Animator>();
        _lastFire = 0;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
        EnemyFire();
    }

    private void EnemyMovement()
    {
        Vector3 enemyPosition = this.transform.position;

        ZigZag(enemyPosition);

        if(transform.position.x < _leftSideOfScreen)
        {
            enemyPosition.x = _rightSideOfScreen;
            transform.position = enemyPosition;
        }

        if(transform.position.x > _rightSideOfScreen)
        {
            enemyPosition.x = _leftSideOfScreen;
            transform.position = enemyPosition;
        }

        if (transform.position.y <= _bottomOfScreen)
        {
            enemyPosition.y = _topOfScreen;
            enemyPosition.x = Random.Range(_leftSideOfScreen, _rightSideOfScreen);
            transform.position = enemyPosition;
        }
    }

    private void EnemyFire()
    {
        if(Time.time > _lastFire)
        {
            _lastFire = Time.time + _fireDelay;
            Instantiate(_enemyLaser, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag){
            case "Laser":
                
                if (_player)
                {
                    _player.AddScore(_points);
                }
                Destroy(other.gameObject);
                EnemyDeath();
                break;
            case "Player":
                if (_player)
                {
                    _player.Damage(_kamakazeDamage);
                }
                EnemyDeath();
                break;
                
        }
    }

    private void EnemyDeath()
    {
        _anim.SetTrigger("EnemyDeathTrigger");
        Destroy(this.gameObject, _deathDelay);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _speed = _speed / 3;
        _audioSource.PlayOneShot(_explosion);
    }

    private void ZigZag(Vector3 motion)
    {
        if (_canZigZag)
        {
            // use sine to oscillate movement
            motion.x = Mathf.Sin(Time.time * _zigAmount);
            transform.Translate(new Vector3(motion.x, -1, 0) * Time.deltaTime * _speed);
        }
        else
        {
            transform.Translate(Vector3.down * Time.deltaTime * _speed);
        }
    }

}
