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

    private GameObject[] _enemies;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canTrack)
        {
            transform.Translate(Vector3.up * Time.deltaTime * _speed);
        }
        else
        {
            RotateTowards();
            transform.position = Vector3.MoveTowards(this.transform.position, _enemies[0].transform.position, _speed * Time.deltaTime);
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
    }

    void RotateTowards()
    {
        Vector3 distance = _enemies[0].transform.position - this.transform.position;
        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, new Vector3(0, 0, 1));
        transform.rotation = rotation;
    }

    public int GetLaserDamage()
    {
        return _laserDamage;
    }
}
