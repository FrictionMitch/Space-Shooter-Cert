using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Enemy : MonoBehaviour
{
    private SpawnManager _spawnManager;

    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private float _rammingSpeed = 8f;
    private float _currentSpeed;

    [SerializeField]
    private float _topOfScreen = 7.4f, _bottomOfScreen = -5.4f;
    [SerializeField]
    private float _leftSideOfScreen = -9.5f, _rightSideOfScreen = 9.5f;

    [SerializeField]
    private int _points = 10;

    [Header("Damage")]
    [SerializeField]
    private int _shootDamage = 100;
    [SerializeField]
    private int _kamakazeDamage = 200;
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
    private float _lastFire; // cool down for normal rate of fire
    private float _lastPowerupFire; // cool down after shooting at powerup

    [Space (10)]
    [SerializeField]
    private bool _canDodge = false;

    [SerializeField]
    private bool _canShootBackwards = false;

    [SerializeField]
    private bool _isAggressive = false;
    [SerializeField]
    private float _rammingDistance = 5f;

    [SerializeField]
    private bool _canTargetPowerups = true;
    [SerializeField]
    private float _powerupVisibility = 10f;

    [SerializeField]
    private bool _canZigZag = true;

    [SerializeField]
    private bool _canRotateTowards = false;

    [SerializeField]
    private bool _enableShields = false;
    [SerializeField]
    private GameObject _enemyShield;

    [SerializeField]
    private int _minZigAmount = 1, _maxZigAmount = 5;
    private int _zigAmount;

    [field: Tooltip("How far incoming projectile can be tracked")]
    [field: SerializeField]
    public GameObject _rearLaserPosition { get; private set; }

    [SerializeField]
    private bool _isBoss = false;
    [SerializeField]
    private int _bossHealth = 1;
    [SerializeField]
    private float _bossFireDelay = 1f;
    [SerializeField]
    private GameObject _bossExplosion;

    private bool _isAlive = true;




    // Start is called before the first frame update
    void Start()
    {
        _isAlive = true;
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _currentSpeed = _speed;
        _enableShields = Random.value > 0.5f; // randomizes if shield is enabled or not
        _enemyShield?.SetActive(_enableShields);
        _zigAmount = Random.Range(_minZigAmount, _maxZigAmount);
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("Player")?.GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _lastFire = 0;
        _lastPowerupFire = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBoss)
        {
            _enemyShield.SetActive(_enableShields);
            EnemyMovement();
            if (_isAlive)
            {
                EnemyFire(_fireDelay);
            }
            DodgeProjectiles();
            ShootBackwards();
            RotateTowardsPlayer();
        }
        else
        {
            BossDeath();
            EnemyFire(_bossFireDelay);
        }
    }

    void FixedUpdate()
    {
        if (!_isBoss)
        {
            TargetPowerup();
            RamPlayer();
        }
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

    private void EnemyFire( float delay)
    {
        if(Time.time > _lastFire)
        {
            _lastFire = Time.time + delay;
            GameObject enemyFire = Instantiate(_enemyLaser, transform.position, Quaternion.identity);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isBoss)
        {
            switch (other.tag){
                case "Laser":
                    
                    if (_enableShields)
                    {
                        _enableShields = false;
                        Destroy(other.gameObject);
                        return;
                    }
                    if (_player)
                    {
                        _player.AddScore(_points);
                    }
                    Destroy(other.gameObject);
                    EnemyDeath();
                    break;

                case "Player":
                    if (_enableShields)
                    {
                        _enableShields = false;
                        _player.Damage(_kamakazeDamage);
                        return;
                    }
                    if (_player)
                    {
                        _player.Damage(_kamakazeDamage);
                    }
                    EnemyDeath();
                    break;
            }
        }
        else
        {
            Destroy(other.gameObject);
        }
    }



    private void EnemyDeath()
    {
        _isAlive = false;
        _spawnManager.SpawnedEnemyDestroyed();
        _anim.SetTrigger("EnemyDeathTrigger");
        Destroy(this.gameObject, _deathDelay);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _currentSpeed = _speed / 3;
        _audioSource.PlayOneShot(_explosion);
    }

    private void ZigZag(Vector3 motion)
    {
        if (_canZigZag)
        {
            // use sine to oscillate movement
            motion.x = Mathf.Sin(Time.time * _zigAmount);
            transform.Translate(new Vector3(motion.x, -1, 0) * Time.deltaTime * _currentSpeed);
        }
        else
        {
            transform.Translate(Vector3.down * Time.deltaTime * _currentSpeed, Space.World);
        }
    }

    private void TargetPowerup()
    {
        //Cast a ray straight down.
        Vector3 yOffset = new Vector3(0, 1, 0);

        RaycastHit2D hit = Physics2D.Raycast((transform.position - yOffset), -Vector2.up, _powerupVisibility);
        //Debug.DrawRay((transform.position - yOffset), -Vector2.up * _powerupVisibility, Color.green);
        if (hit)
        {
            if (hit.collider.tag == "Powerup")
            {
                //print($"Shooting {hit.collider.name}");
                if (Time.time > _lastPowerupFire)
                {
                    _lastPowerupFire = Time.time + _fireDelay;
                    Instantiate(_enemyLaser, transform.position, Quaternion.identity);
                }
                //Instantiate(_enemyLaser, transform.position, Quaternion.identity);
            }
        }
    }

    private void RamPlayer()
    {
        if (_player)
        {
            if(transform.position.y > _player.transform.position.y)
            {

                Vector3 yOffset = new Vector3(0, 1, 0);

                RaycastHit2D hitPlayer = Physics2D.Raycast((transform.position - yOffset), -Vector2.up, _rammingDistance);
                //Debug.DrawRay((transform.position - yOffset), -Vector2.up * _rammingDistance, Color.green);
                if (hitPlayer)
                {
                    if(hitPlayer.collider.tag == "Player")
                    {
                        if (_isAggressive)
                        {
                            _canZigZag = false;
                            _currentSpeed = _rammingSpeed;
                        }
                
                    }
                }
            }
            else
            {
                _currentSpeed = _speed;
            }
        }
    }

    private void DodgeProjectiles()
    {
        if (_canDodge)
        {
            Vector2 laserRadar = new Vector2(2f, 6f);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, laserRadar, 0, Vector2.down, 10, 9);
            if(hit.collider?.tag == "Laser")
            {
                int leftRightModifier = 1;
                if(hit.transform.position.x > transform.position.x) // is the laser left or right of the enemy
                {
                    leftRightModifier = -1;
                }
                else
                {
                    leftRightModifier = 1;
                }
                StartCoroutine(DodgeProjectileRoutine(0.5f, leftRightModifier));
            }
        }
    }

    IEnumerator DodgeProjectileRoutine(float dodgeTime, int horizMovement)
    {
        {
            float dodgeAmount = 2f;
            float dodgeSpeed = 150f;
            Vector3 dodgePosition = transform.position;
            dodgePosition.x += (dodgeAmount * horizMovement);
            transform.position = Vector3.MoveTowards(transform.position, dodgePosition, dodgeSpeed * Time.deltaTime);
            yield return new WaitForSeconds(dodgeTime);
        }
    }

    private void ShootBackwards()
    {
        if (_canShootBackwards)
        {
            Vector2 rearRadar = new Vector2(2f, 6f);
            //RaycastHit2D hit = Physics2D.BoxCast(transform.position, rearRadar, 0, Vector2.up, 10, 9);
            RaycastHit2D playerHit = Physics2D.Raycast(_rearLaserPosition.transform.position, Vector2.up, 6, 1<<9);
            //Debug.DrawRay((_rearLaserPosition.transform.position), Vector2.up * 6, Color.green);

            if (playerHit)
            {
                if (playerHit.collider?.tag == "Player")
                {
                    StartCoroutine(RearFireRoutine());
                }
            }
        }
    }

    IEnumerator RearFireRoutine()
    {
        _canShootBackwards = false;
        GameObject rearLaser = Instantiate(_enemyLaser, _rearLaserPosition.transform.position, Quaternion.identity);
        rearLaser.transform.Rotate(0, 0, 180);
        yield return new WaitForSeconds(0.5f);
        _canShootBackwards = true;
    }

    private void RotateTowardsPlayer()
    {
        if (_player)
        {
            if (_canRotateTowards)
            {
                // distance
                Vector3 distance = _player.transform.position - this.transform.position;

                // angle
                float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;

                // quaternion
                Quaternion rotation = Quaternion.AngleAxis(angle + 90, new Vector3(0, 0, 1));

                // rotate
                transform.rotation = rotation;
            }
        }
    }

    public void DamageBoss()
    {
        _bossHealth--;
    }

    private void BossDeath()
    {
        if(_bossHealth <= 0)
        {
            Destroy(this.gameObject, 3f);
            AudioSource.PlayClipAtPoint(_explosion, Camera.main.transform.position);
            Instantiate(_bossExplosion, transform.position, Quaternion.identity);
        }
    }

    private void BossShoot()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("BossStrafe_anim"))
        {
            print("fuck off");
        }
    }

}
