using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private int damageFromAttack = 1;
    [SerializeField] private float attackCoolDown = 1f;
    [SerializeField] private AnimationClip[] slashAnimaionArray = null;
    [SerializeField] private AnimationClip idelAnim = null, stabAnim = null, twirlAnim = null;
    [SerializeField] private AudioClip swing = null, personHit = null, wallHit = null;

    private float attackTimer = 0f;
    private float idelTimer = 0f;
    private bool canAttack = false;
    private BoxCollider hitTrigger = null;
    private Collider hitColl;
    private Animation animator = null;
    private AudioSource soundSource = null;
    private PlayerNoiseMaster noiseMaster = null;

    // Start is called before the first frame update
    void Awake()
    {
        hitTrigger = GetComponent<BoxCollider>();
        soundSource = transform.parent.GetComponent<AudioSource>();
        animator = GetComponent<Animation>();
        animator.clip = idelAnim;
        noiseMaster = transform.parent.transform.parent.transform.parent.GetComponent<PlayerNoiseMaster>();
    }

    void Start()
    {

    }

    private void PlayAnimation(AnimationClip clip)
    {
        if (animator.isPlaying)
            animator.Stop();

        animator.clip = clip;
        animator.AddClip(clip, "clip");
        animator.Play("clip");
    }

    private void PlaySound(AudioClip sound, float volume = 1f)
    {
        if (soundSource.isPlaying)
            soundSource.Stop();

        soundSource.volume = volume;
        soundSource.clip = sound;
        soundSource.Play();
    }

    private AnimationClip GetRandomSlashAnim()
    {
        int index = Random.Range(0, slashAnimaionArray.Length);
        return slashAnimaionArray[index];
    }

    private void AttackTimerFunc()
    {
        if (attackTimer < attackCoolDown)
        {
            attackTimer += Time.deltaTime;
        }
        else if (attackTimer > attackCoolDown)
        {
            canAttack = true;
            attackTimer = 0f;
        }
    }

    private void AttackFunc()
    {
        canAttack = false;
        PlayAnimation(GetRandomSlashAnim());
        PlaySound(swing);
        Vector3 hitCenter = hitTrigger.transform.position + (hitTrigger.transform.forward * -2f);
        int layerMask = 1 << 6;
        layerMask = ~layerMask;

        bool didHit = Physics.BoxCast(hitCenter, hitTrigger.transform.localScale, transform.forward, out RaycastHit hit, transform.rotation, 2f, layerMask);
        if(didHit)
        {

            if (hit.collider.gameObject.layer == 7)
            {
                I_Damagable damageCode = hit.collider.GetComponent<I_Damagable>();
                if (damageCode != null)
                    damageCode.TakenDamage(damageFromAttack);

                PlaySound(personHit);
                noiseMaster.MakeNoise(PlayerNoises.Noise_EnemyHit);
                return;
            }

            PlaySound(wallHit);
            noiseMaster.MakeNoise(PlayerNoises.Noise_KnifeWallHit);
        }else
            noiseMaster.MakeNoise(PlayerNoises.Noise_KnifeSwing);

    }

    private void StabFunc()
    {
        canAttack = false;
        PlayAnimation(stabAnim);
        PlaySound(swing);
        Vector3 hitCenter = hitTrigger.transform.position + (hitTrigger.transform.forward * -2f);
        int layerMask = 1 << 6;
        layerMask = ~layerMask;

        bool didHit = Physics.BoxCast(hitCenter, hitTrigger.transform.localScale, transform.forward, out RaycastHit hit, transform.rotation, 2f, layerMask);
        if (didHit)
        {

            if (hit.collider.gameObject.layer == 7)
            {
                I_Damagable damageCode = hit.collider.GetComponent<I_Damagable>();
                if (damageCode != null)
                {
                    if (IsBehindeEnemy(hit.collider.gameObject.transform))
                    {
                        damageCode.TakenDamage(damageFromAttack * 10);
                    }
                    else
                        damageCode.TakenDamage(damageFromAttack * 2);
                }


                PlaySound(personHit);
                return;
            }

            PlaySound(wallHit);
        }

    }

    private bool IsBehindeEnemy(Transform enemyPos)
    {

        Vector3 forwardDir = enemyPos.TransformDirection(Vector3.forward);
        Vector3 playerDor = (transform.position - enemyPos.position).normalized;
        float dot = Vector3.Dot(forwardDir, playerDor);


        if (dot < -0.85f)
            return true;
        else
            return false;
    }

    private void idelTimerFunc()
    {
        idelTimer += Time.deltaTime;

        if (idelTimer >= 60)
        {
            PlayAnimation(twirlAnim);
            idelTimer = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canAttack)
            AttackTimerFunc();

        if (Input.GetButtonDown("Fire1") && canAttack)
        {
            AttackFunc();
            idelTimer = 0;
        }
        else if (Input.GetButtonDown("Fire2") && canAttack)
        {
            StabFunc();
            idelTimer = 0;
        }

        if (canAttack && idelTimer < 60f)
            idelTimerFunc();
    }
}
