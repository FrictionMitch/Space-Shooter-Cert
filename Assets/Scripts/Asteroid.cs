using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private float _rotationSpeed = 3f;
    private float _randomRotationSpeed;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private float _destroyDelay = 2f;

    private SpawnManager _spawnManager;


    // Start is called before the first frame update
    void Start()
    {
        _randomRotationSpeed = Random.Range(-_rotationSpeed, _rotationSpeed);
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // rotate around the z axis
        transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * _randomRotationSpeed);
        //RotateTowards();
    }

    void Explode()
    {
        Instantiate(_explosion, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Explode();
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, _destroyDelay);
        }
    }

    void RotateTowards()
    {
        Vector3 distance = _player.transform.position - this.transform.position;

        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));

        transform.rotation = rotation;
    }
}
