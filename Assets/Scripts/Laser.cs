using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int _laserDamage = 200;

    [SerializeField]
    private bool _canTrack = false;

    [SerializeField]
    private float _selfDestructTimer = 4f;

    private GameObject[] _enemies;

    private GameObject _player;

    [SerializeField]
    private bool _isEnemyLaser = false;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //if (!_canTrack || _enemies[0]?.GetComponent<BoxCollider2D>()?.enabled == false)
        if (!_canTrack)
        {
            if(gameObject.tag == "Enemy Laser")
            {
                transform.Translate(Vector3.up * Time.deltaTime * _speed);
            }
            if(gameObject.tag == "Laser")
            {
                transform.Translate(Vector3.down * Time.deltaTime * _speed);
            }
        }
        else
        {
            if (_isEnemyLaser)
            {
                RotateTowards(_player, 90);
                transform.Translate(Vector3.down * Time.deltaTime * (_speed/ 5));
            }
            else
            {
                if(_enemies == null)
                {
                    transform.Translate(Vector3.up * Time.deltaTime * _speed);
                }
                else
                {
                    if (_enemies.Length > 0)
                    {
                        RotateTowards(_enemies[0], 90);
                        transform.position = Vector3.MoveTowards(this.transform.position, _enemies[0].transform.position, _speed * Time.deltaTime);
                    }
                    else
                    {
                        transform.Translate(Vector3.up * Time.deltaTime * _speed);
                    }
                }
            }
        }
        SelfDestruct();
    }

    void SelfDestruct()
    {
        if(transform.position.y >= 8)
        {
            if (transform.parent)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject, _selfDestructTimer);
        }
    }

    void RotateTowards(GameObject target, float angleOffset)
    {
        if(_enemies != null && target != null)
        {
            //Vector3 distance = _enemies[0].transform.position - this.transform.position;
            Vector3 distance = target.transform.position - this.transform.position;
            float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - angleOffset, new Vector3(0, 0, 1));
            transform.rotation = rotation;
        }
    }

    public int GetLaserDamage()
    {
        return _laserDamage;
    }
}
