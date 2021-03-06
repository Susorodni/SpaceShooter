﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 3.0f; 
    /*
        ID for powerups
        0 = Triple Shot
        1 = Speed
        2 = Shields
    */
    [SerializeField] private int _powerupID;
    [SerializeField] private AudioClip _powerupAudioClip;
    

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y <= -4.5f)
        {
            Destroy(this.gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_powerupAudioClip, transform.position);
            if(player != null)
            {
                switch(_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;

                    case 1:
                        player.SpeedBoostActive();
                        break;

                    case 2:
                        player.ShieldActive();
                        break;

                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
            else
            {
                Debug.LogError("Player object is null!");
            }
            Destroy(this.gameObject);
        }
    }
}
