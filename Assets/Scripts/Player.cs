﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 7f, _turbo = 15f, _speedBoost = 30f;
    private float _currentSpeed = 10f;
    private bool _speedBoostActive = false;
    public bool canTurbo = true;

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
    private GameObject _singleProjectile;
    [SerializeField]
    private GameObject _tripleShot;
    private bool isTripleShotActive = false;
    private bool isUltraShotActive = false;
    private bool isHeatSeekerActive = false;
    [SerializeField]
    private GameObject _heatSeeker;
    [SerializeField]
    private GameObject _minis;
    [SerializeField]
    private int _maxAmmo = 15;
    private int _currentAmmo;
    [SerializeField]
    private GameObject ProjectileParent;

    private List<GameObject> heatSeekerList;

    [SerializeField]
    private float _secondsActive = 5f;

    [Tooltip("Vertical offset for projectile to fire from")]
    [SerializeField]
    private float _projectileOffset;

    [Tooltip("Delay between firing")]
    [SerializeField]
    private float _coolDown = 0.5f;
    private float _lastFire = 0;

    [Header("Shield")]
    [SerializeField]
    [Tooltip("Number of hits shield can take before destroyed")]
    private int _shieldStrength = 3;
    [SerializeField]
    private GameObject _shield;
    public int _currentShieldStrength;
    private bool _isShieldActive = false;
    [SerializeField]
    Color[] colors;

    private int _score;

    [Space(20)]
    [SerializeField]
    private UIManager _uiManager;
    [Space(20)]

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
    private Animator _shieldAnim;

    private SpawnManager _spawnManager;

    [SerializeField]
    private bool isMini = false;

    private bool _invertControls = false;


    // Start is called before the first frame update
    void Start()
    {
        heatSeekerList = new List<GameObject>();
        _currentAmmo = _maxAmmo;
        _cameraAnim = Camera.main.GetComponent<Animator>();
        if (!isMini)
        {
            _minis.SetActive(false);
            
            transform.position = Vector3.zero;
            _uiManager.UpdateLives(_lives);
            _shield.SetActive(false);
            _score = 0;

        }
        _currentSpeed = _speed;
        _currentShieldStrength = _shieldStrength;
        _shieldAnim = _shield.GetComponent<Animator>();

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
            if (!isMini)
            {
                _currentAmmo--;
            }
            _lastFire = Time.time + _coolDown;
        }

    }

    private void CalculateMovement()
    {
        if (!isMini)
        {

            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(horizontalMovement, verticalMovement, transform.position.z);
            if (!_invertControls)
            {
                transform.Translate(direction * _currentSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(-direction * _currentSpeed * Time.deltaTime);
            }

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
        }

        if (!isMini)
        {
            if (Input.GetKey(KeyCode.LeftShift)  && canTurbo)
            {
                _currentSpeed = _turbo;
                _uiManager.EmptyThrusterBar();
            }
            else if (!_speedBoostActive)
            {
                _currentSpeed = _speed;
                _uiManager.FillThrusterBar();
            }
        }
    }

    void Fire()
    {
        if(_currentAmmo > 0)
        {
            Vector3 startPosition = transform.position;
            startPosition.y += _projectileOffset;
            GameObject projectile;

            // check ammo
            if (isTripleShotActive)
            {
                projectile = Instantiate(_tripleShot, transform.position, Quaternion.identity);
            }
            else if (isHeatSeekerActive)
            {
                projectile = Instantiate(_heatSeeker, transform.position, Quaternion.identity);
                heatSeekerList.Add(projectile);
            }
            else
            {
                projectile = Instantiate(_singleProjectile, startPosition, Quaternion.identity);
            }

            AudioSource.PlayClipAtPoint(_laserSound, Camera.main.transform.position);

            if (!isMini)
            {

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
        }
    }

    public void Damage(int amount)
    {
        if (!isMini)
        {
            if (_isShieldActive)
            {
                _currentShieldStrength--;
                switch (_currentShieldStrength)
                {
                    case 2:
                        _shield.GetComponent<SpriteRenderer>().color = Color.white;
                        break;
                    case 1:
                        _shield.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                    default:
                        _shield.GetComponent<SpriteRenderer>().color = Color.green;
                        break;

                }
                _shieldAnim.SetTrigger("HitTrigger");

                if (_currentShieldStrength <= 0)
                {
                    _isShieldActive = false;
                    _shield.SetActive(false);
                }
                return;
            }
            _currentHealth -= amount;

            if (_currentHealth <= 0)
            {
                _lives--;
                CameraShake();
                ShowDamage();
                _uiManager.UpdateLives(_lives);
                _currentHealth = _startHealth;
                if (_lives <= 0)
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
    }

    public void EnableTripleShot()
    {
        StartCoroutine(TripleShotRoutine());
    }

    public void EnableHeatSeeker()
    {
        StartCoroutine(HeatSeekerRoutine());
    }

    public void EnableSpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }

    public void EnableShield()
    {
        _isShieldActive = true;
        _currentShieldStrength = _shieldStrength;
        _shield.SetActive(true);
        _shield.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void EnableMinis()
    {
        if (!isMini)
        {
            FillAmmo();
            StartCoroutine(UltraShotRoutine());
        }
    }

    IEnumerator TripleShotRoutine()
    {
        isTripleShotActive = true;
        yield return new WaitForSeconds(_secondsActive);
        isTripleShotActive = false;
    }

    IEnumerator HeatSeekerRoutine()
    {
        isHeatSeekerActive = true;
        yield return new WaitForSeconds(_secondsActive);
        isHeatSeekerActive = false;
        foreach(GameObject heatseeker in heatSeekerList)
        {
            Destroy(heatseeker);
        }
    }

    // Secondary Powerup -- RARE
    IEnumerator UltraShotRoutine()
    {
        //isUltraShotActive = true;
        _minis.SetActive(true);
        yield return new WaitForSeconds(_secondsActive);
        _minis.SetActive(false);
        //isUltraShotActive = false;
    }

    // Negative Powerup
    IEnumerator InvertControlsRoutine()
    {
        _invertControls = true;
        yield return new WaitForSeconds(_secondsActive);
        _invertControls = false;
        
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
        if (!isMini)
        {
            if(_damageObjects.Length >= _lives && _lives > 0)
            {
                _damageObjects[_lives - 1].SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy Laser")
        {
            Damage(other.GetComponent<Laser>().GetLaserDamage());
            Destroy(other.gameObject);
        }
    }

    void CameraShake()
    {
        if (_cameraAnim)
        {
            _cameraAnim.SetTrigger("CameraShakeTrigger");
        }
    }

    public int GetAmmo()
    {
        if(_currentAmmo <= 0)
        {
            _currentAmmo = 0;
        }
        return _currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return _maxAmmo;
    }

    public void FillAmmo()
    {
        _currentAmmo = _maxAmmo;
    }

    public void AddLife()
    {
        // add life
        if(_lives < 3)
        {
            _damageObjects[_lives-1].SetActive(false);
            _lives++;
            _uiManager.UpdateLives(_lives);
        }
    }

    public void InvertControls()
    {
        StartCoroutine(InvertControlsRoutine());
    }

}
