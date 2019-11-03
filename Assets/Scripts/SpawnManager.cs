using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject[] PowerUps;

    [SerializeField]
    private float _secondsBetweenEnemySpawn = 5.0f, _minPowerup = 3f, _maxPowerup = 7f;

    [SerializeField]
    private float _startPosition = 7.4f, _topOfScreen = 8f;
    [SerializeField]
    private float _leftSide = -9.4f, _rightSide = 9.4f;

    private GameObject enemyContainer;

    private bool _stopSpawning = false;

    private UIManager uIManager;

    [SerializeField]
    private int _wave = 1;
    [SerializeField]
    private int _waveMultiplier = 10;

    private int _enemiesDestroyed = 0;

    private int _enemiesNeededForNextWave { get; set; }

    private int _totalEnemiesSpawned { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _stopSpawning = true;
        uIManager = GameObject.FindObjectOfType<UIManager>();
        ResetCounter();
        uIManager.SetWaveText($"Wave: {_wave}");
    }

    public void StartSpawning()
    {
        _stopSpawning = false;
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        uIManager.SetWaveText("");
    }

    // Update is called once per frame
    void Update()
    {
        _enemiesNeededForNextWave = _waveMultiplier * _wave;
        if (_stopSpawning)
        {
            
        }
        else
        {
            
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(_secondsBetweenEnemySpawn);
            Vector3 spawnPosition = Vector3.zero;
            spawnPosition.y = _startPosition;
            spawnPosition.x = Random.Range(_leftSide, _rightSide);
            if(_totalEnemiesSpawned < _enemiesNeededForNextWave){
                GameObject enemy = Instantiate(_enemy, spawnPosition, Quaternion.identity);
                _totalEnemiesSpawned++;
                if (!enemyContainer)
                {
                    enemyContainer = new GameObject("Enemy Container");
                }
                enemyContainer.transform.parent = this.transform;
                enemy.transform.parent = enemyContainer.transform;
            }
            else
            {
                _stopSpawning = true;
                if (_enemiesDestroyed >= _enemiesNeededForNextWave)
                {
                    StartCoroutine(NextWaveRoutine());
                    _stopSpawning = false;
                }
            }
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (!_stopSpawning)
        {
            int randomPowerup = Random.Range(0, PowerUps.Length);
            yield return new WaitForSeconds(Random.Range(_minPowerup, _maxPowerup));
            Vector3 tripleShotPosition = new Vector3(Random.Range(_leftSide, _rightSide), _topOfScreen, transform.position.z);
            if (PowerUps[randomPowerup])
            {
                GameObject powerup = Instantiate(PowerUps[randomPowerup], tripleShotPosition, Quaternion.identity);
            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    IEnumerator NextWaveRoutine()
    {
        _wave++;
        _enemiesDestroyed = 0;
        _totalEnemiesSpawned = 0;
        uIManager.SetWaveText($"Wave: {_wave}");
        yield return new WaitForSeconds(3f);
        _stopSpawning = false;
        uIManager.SetWaveText("");
    }

    public void SpawnedEnemyDestroyed()
    {
        _enemiesDestroyed++;
    }

    public void ResetCounter()
    {
        _stopSpawning = false;
        _wave = 1;
        _enemiesDestroyed = 0;
        _totalEnemiesSpawned = 0;
    }
}
