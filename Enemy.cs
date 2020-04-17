using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private AudioClip _explosionAudioClip;
    [SerializeField] private GameObject _laserPrefab;
    private bool _inExplodingSequence = false;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("The animator is NULL!");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The enemy audio source is NULL!");
        }
        else
        {
            _audioSource.clip = _explosionAudioClip;
        }

        StartCoroutine(EnemyFire());
    }
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y <= -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if(player != null)
            {
                if (!_inExplodingSequence)
                {
                    player.Damage();
                }
            }
            else
            {
                Debug.LogError("Player is null!");
            }
            
            _audioSource.Play();
            _inExplodingSequence = true;
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(this.gameObject, 2.8f);
        } 
        
        if(other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if(_player != null)
            {
                _player.AddScore(10);
            }

            _audioSource.Play();
            _inExplodingSequence = true;
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }

    IEnumerator EnemyFire()
    {
        yield return new WaitForSeconds(Random.Range(3f, 5f));
        if (_inExplodingSequence)
        {
            GameObject laser = Instantiate(_laserPrefab, (transform.position + new Vector3(0, -2.8f, 0)),
                Quaternion.identity);
            laser.GetComponent<Laser>().ChangeSpeed(-8f);
            laser.GetComponent<Laser>().ChangeEnemy(true);
        }
    }
}
