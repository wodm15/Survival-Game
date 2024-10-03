using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun currentGun;

    private float currentFireRate; //연사속도 

    private bool isReload = false;

    private bool isFineSightMode = false;
    [SerializeField]
    private Vector3 originPos; //본래 포지션 값


    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update(){
        GunFireRateCalc();
        TryFire();
        TryReload();
        TryFineSight();
    }

    private void GunFireRateCalc()
    {
        if(currentFireRate > 0){
            currentFireRate -= Time.deltaTime;
        }
    }

    private void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload){
            Fire();
        }
    }

    private void Fire()
    {
        if(!isReload){
            if(currentGun.currentBulletCount > 0)
                Shoot();
            else{
                CancelFineSight();
                StartCoroutine(ReloadCorutine());
            }
        }
    }
    private void Shoot()
    {
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; // 발사 후 재계산
        PlayeSE(currentGun.fire_sound);
        currentGun.muzzleFlash.Play();
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutin());

        Debug.Log("총알 발사");
    }

    private void TryReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && isReload == false && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCorutine());
        }
    }

    private void PlayeSE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    IEnumerator ReloadCorutine()
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            yield return new WaitForSeconds(currentGun.reloadTime);

            //총알이 재장전 개수보다 많으면 재조정
            if(currentGun.carryBulletCount > currentGun.reloadBulletCount){
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;   
            }
            isReload = false;
        }
        else
        {
            Debug.Log("총알이 없음");
        }
    }
        private void TryFineSight()
        {
            if(Input.GetButtonDown("Fire2") && !isReload)
            {
                FineSight();
            }
        }
        public void CancelFineSight()
        {
            if(isFineSightMode)
            {
                FineSight();
            }
        }
        private void FineSight()
        {
            isFineSightMode = !isFineSightMode;
            currentGun.anim.SetBool("FineSightMode" , isFineSightMode);

            if(isFineSightMode)
            {
                StopAllCoroutines();
                StartCoroutine(FineSightActiveCoroutine());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(FineSightDeactiveCoroutine());
            }
        }

        IEnumerator FineSightActiveCoroutine()
        {
            while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
                yield return null;
            }
        }
        IEnumerator FineSightDeactiveCoroutine()
        {
            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
                yield return null;
            }
        }

    IEnumerator RetroActionCoroutin()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        if(!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos;

            //반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            //원위치

            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            //반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack , 0.4f);
                yield return null;
            }

            //원위치

            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }
}

