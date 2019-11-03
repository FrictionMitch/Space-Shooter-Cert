using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeakSpot : MonoBehaviour
{
    private Enemy _enemy;

    // Start is called before the first frame update
    void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser")
        {
            _enemy.DamageBoss();
        }
    }
}
