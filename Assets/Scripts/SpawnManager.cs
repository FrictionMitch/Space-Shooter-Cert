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

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(_secondsBetweenEnemySpawn);
            Vector3 spawnPosition = Vector3.zero;
            spawnPosition.y = _startPosition;
            spawnPosition.x = Random.Range(_leftSide, _rightSide);
            GameObject enemy = Instantiate(_enemy, spawnPosition, Quaternion.identity);
            if (!enemyContainer)
            {
                enemyContainer = new GameObject("Enemy Container");
            }
            enemyContainer.transform.parent = this.transform;
            enemy.transform.parent = enemyContainer.transform;
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
}
