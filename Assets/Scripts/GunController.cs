using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun currentGun;

    private float currentFireRate; //연사속도 

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update(){
        GunFireRateCalc();
        TryFire();
    }

    private void GunFireRateCalc()
    {
        if(currentFireRate > 0){
            currentFireRate -= Time.deltaTime;
        }
    }

    private void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 ){
            Fire();
        }
    }

    private void Fire()
    {
        currentFireRate = currentGun.fireRate;
        Shoot();
    }
    private void Shoot()
    {
        PlayeSE(currentGun.fire_sound);
        currentGun.muzzleFlash.Play();
        Debug.Log("총알 발사");
    }

    private void PlayeSE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
