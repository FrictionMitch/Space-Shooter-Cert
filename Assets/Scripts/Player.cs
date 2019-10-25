using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 7f, _turbo = 15f, _speedBoost = 30f;
    private float _currentSpeed = 10f;
    private bool _canTurbo = true;
    private bool _speedBoostActive = false;

    [SerializeField]
    private int _currentHealth = 500, _startHealth = 500, _lives = 3;

    [Header("Screen Constraints")]
    [SerializeField]
    private float _topOfScreen = 6f;
    [SerializeField]
    private float _bottomOfScreen = -4f;
    [SerializeField]
    private float _leftOfScreen = -11f, _rightOfScreen = 11f;

    [Header("Projectile")]
    [SerializeField]
    private GameObject _singleProjectile, _tripleShot;
    private bool isTripleShotActive = false;
    private GameObject ProjectileParent;

    [SerializeField]
    private float _secondsActive = 5f;

    [Tooltip("Vertical offset for projectile to fire from")]
    [SerializeField]
    private float _projectileOffset;

    [Tooltip("Delay between firing")]
    [SerializeField]
    private float _coolDown = 0.5f;
    private float _lastFire = 0;

    private SpawnManager _spawnManager;

    private bool isShieldActive = false;
    private GameObject _shield;

    private int _score;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private GameObject[] _damageObjects;

    [Header("SoundFX")]
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserSound;
    [SerializeField]
    private AudioClip _explosion;

    private Animator _cameraAnim;

    // Start is called before the first frame update
    void Start()
    {
        _cameraAnim = Camera.main.GetComponent<Animator>();
        transform.position = Vector3.zero;
        _currentSpeed = _speed;
        _shield = GameObject.FindGameObjectWithTag("Shield");
        _shield.GetComponent<Renderer>().enabled = false;
        _uiManager.UpdateLives(_lives);
        _score = 0;

        foreach(GameObject damageObject in _damageObjects)
        {
            damageObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetButtonDown("Fire1") && Time.time > _lastFire)
        {
            Fire();
            _lastFire = Time.time + _coolDown;
        }

    }

    private void CalculateMovement()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalMovement, verticalMovement, transform.position.z);

        transform.Translate(direction * _currentSpeed * Time.deltaTime);

        Vector3 newPosition = transform.position;

        // if player position.y > top of screen
        newPosition.y = Mathf.Clamp(newPosition.y, _bottomOfScreen, _topOfScreen);
        transform.position = newPosition;

        // if player position.x ... wrap
        if (transform.position.x <= _leftOfScreen || transform.position.x >= _rightOfScreen)
        {
            newPosition.x = -newPosition.x;
            transform.position = newPosition;
        }

        if (Input.GetKey(KeyCode.LeftShift)  && _canTurbo)
        {
            _currentSpeed = _turbo;
        }
        else if (!_speedBoostActive)
        {
            _currentSpeed = _speed;
        }
    }

    void Fire()
    {
        Vector3 startPosition = transform.position;
        startPosition.y += _projectileOffset;
        GameObject projectile;
        if (isTripleShotActive)
        {
            projectile = Instantiate(_tripleShot, transform.position, Quaternion.identity);
        }
        else
        {
            projectile = Instantiate(_singleProjectile, startPosition, Quaternion.identity);
        }

        // laser sound
        _audioSource.PlayOneShot(_laserSound);


        //check for projectileParent folder
        if (!GameObject.Find("ProjectileParent"))
        {
            ProjectileParent = new GameObject("ProjectileParent");
            projectile.transform.parent = ProjectileParent.transform;
        }
        else
        {
            projectile.transform.parent = ProjectileParent.transform;
        }
    }

    public void Damage(int amount)
    {
        if (isShieldActive)
        {
            isShieldActive = false;
            _shield.GetComponent<Renderer>().enabled = false;
            return;
        }

        _currentHealth -= amount;
        if(_currentHealth <= 0)
        {
            _lives--;
            CameraShake();
            ShowDamage();
            _uiManager.UpdateLives(_lives);
            _currentHealth = _startHealth;
            Debug.Log($"Lives: {_lives}");
            if(_lives <= 0)
            {
                AudioSource.PlayClipAtPoint(_explosion, Camera.main.transform.position);
                _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
                if (_spawnManager)
                {
                    _spawnManager.OnPlayerDeath();
                }
                Destroy(this.gameObject);
            }
        }
    }

    public void EnableTripleShot()
    {
        StartCoroutine(TripleShotRoutine());
    }

    public void EnableSpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }

    public void EnableShield()
    {
        isShieldActive = true;
        _shield.GetComponent<Renderer>().enabled = true;
    }

    IEnumerator TripleShotRoutine()
    {
        isTripleShotActive = true;
        yield return new WaitForSeconds(_secondsActive);
        isTripleShotActive = false;
    }

    IEnumerator SpeedBoostRoutine()
    {
        _speedBoostActive = true;
        _currentSpeed = _speedBoost;
        yield return new WaitForSeconds(_secondsActive);
        _speedBoostActive = false;
        _currentSpeed = _speed;
    }

    public void AddScore(int points)
    {
        _score += points;
    }

    public int GetScore()
    {
        return _score;
    }

    public int GetLives()
    {
        return _lives;
    }

    private void ShowDamage()
    {
        if(_damageObjects.Length >= _lives && _lives > 0)
        {
            _damageObjects[_lives - 1].SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy Laser")
        {
            Damage(other.GetComponent<Laser>().GetLaserDamage());
        }
    }

    void CameraShake()
    {
        if (_cameraAnim)
        {
            _cameraAnim.SetTrigger("CameraShakeTrigger");
        }
    }
}
