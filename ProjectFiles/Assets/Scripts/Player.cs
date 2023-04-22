using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerNoiseMaster))]
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour 
{
	private float playerMovement;

    [SerializeField]
    private float walkMovement = 0f;

    [SerializeField]
    private float runMovement = 0f;

    private const float defaultFootStepTime = 0.5f;
    private float footStepTime = 0f;
    private float currentFootStepTime = 0f;

    float currentAcell;
    float currentStrafeAcell;

    [SerializeField]
    private float playerAcceleration;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float jumpCoolDown = 0.0f;
    float jumpCoolDownCounter = 0.0f;

    private bool bCanJump = true;

    [SerializeField]
    private float pGravity;

    [SerializeField] private AudioClip[] footStepSound = null;

	private Vector3 moveDirection = Vector3.zero;
    private AudioSource _soundSource = null;
    private CharacterController _playerController = null;
    private PlayerNoiseMaster _playerNoiseMaker = null;


    private void Awake()
    {
        _playerController = GetComponent<CharacterController>();
        _soundSource = GetComponent<AudioSource>();
        _playerNoiseMaker = GetComponent<PlayerNoiseMaster>();
    }

    void Start () 
    {
        playerMovement = walkMovement;
        jumpCoolDownCounter = jumpCoolDown;
    }

    void JumpTimer()
    {
        if (jumpCoolDownCounter > 0.0f)
        {
            jumpCoolDownCounter -= Time.deltaTime;
        }
        else if (jumpCoolDownCounter <= 0.0f)
        {
            jumpCoolDownCounter = jumpCoolDown;
            bCanJump = true;
            return;
        }
    }

    void Jumping()
    {
        moveDirection.y = jumpSpeed;
        bCanJump = false;
    }

    private void FootStepFunc(bool state)
    {
        if (state)
            footStepTime = defaultFootStepTime / 2;
        else
            footStepTime = defaultFootStepTime;

        if (currentFootStepTime >= footStepTime)
        {
            float vol = 0.5f;
            currentFootStepTime = 0f;

            if (state)
            {
                vol = 0.8f;
                _playerNoiseMaker.MakeNoise(PlayerNoises.Noise_RunningNoise);
            }
            else
                _playerNoiseMaker.MakeNoise(PlayerNoises.Noise_WalkingNoise);

            PlayAudio(GetRandomStepSound(), vol);
        }
        else
        {
            currentFootStepTime += Time.deltaTime;
        }
    }

    private AudioClip GetRandomStepSound()
    {
        int rand = Random.Range(0, footStepSound.Length);
        return footStepSound[rand];
    }

    private void PlayAudio(AudioClip clip, float vol = 1f)
    {
        if(_soundSource.isPlaying)
            _soundSource.Stop();

        _soundSource.clip = clip;
        _soundSource.volume = vol;
        _soundSource.Play();
    }

	// Update is called once per frame
	void Update () {

        if(Input.GetButtonDown("Sprint"))
        {
            playerMovement = runMovement;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerMovement = walkMovement;
        }

        if (_playerController.isGrounded) {

			moveDirection = new Vector3 (Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= playerMovement;

            if (Vector3.Magnitude(moveDirection) > 0)
                FootStepFunc(Input.GetButton("Sprint"));

            if (Input.GetButton("Jump") && bCanJump)
            {
                Jumping();
            }
            else if (!bCanJump)
            {
                JumpTimer();
            }
		}
        moveDirection.y -= pGravity * Time.deltaTime;
        _playerController.Move(moveDirection * Time.deltaTime);		
	}
}
