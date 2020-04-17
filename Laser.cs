using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    private bool _isEnemy = false;

    
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if(transform.position.y >= 8f)
        {
            if(transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void ChangeSpeed(float speed)
    {
        _speed = speed;
    }

    public void ChangeEnemy(bool type)
    {
        _isEnemy = type;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemy)
        {
            other.GetComponent<Player>().Damage();
            Destroy(gameObject);
        }
    }
}
