using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    private float _speedMultiplier = 2;
    private float _canFire = -1f;
    private SpawnManager _spawnManager;

    // Variables for powerups
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false; // not used. keeping it here anyways

    private bool _isShieldActive = false;

    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _leftEngine;
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private int _score;
    [SerializeField] private AudioClip _laserAudioClip;
    
    private AudioSource _audioSource;
    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
            
        if(_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if(_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source is NULL!");
        }
        else
        {
            _audioSource.clip = _laserAudioClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    /**
        Tells the player how to move and
        checks the different boundary rules and sets the position accordingly
    */
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        // Horizontal
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        // Vertical
        if(transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if(transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    /**
        Fires a laser gameObject
    */
    void FireLaser()
    {
         _canFire = Time.time + _fireRate;
         if(_isTripleShotActive)
         {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
         }
         else
         {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.2f, 0), Quaternion.identity);
         }
         
         _audioSource.Play();
    }

    /**
        Call to damage by the enemy to reduce one life
    */
    public void Damage()
    {
        if(_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        } 
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }
        _uiManager.UpdateLives(_lives);

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    /**
        Enables triple shot when collected by the player
    */
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
