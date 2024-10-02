using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    public float range;
    public float accurancy; //정확도
    public float fireRate; //연사 속도
    public float reloadTime; //재장전 속도
    public int damage;
    public int reloadBulletCount; //총알 재장전 개수.
    public int currentBulletCount; //현재 탄알집에 남아있는 총알의 개수
    public int maxBulletCount; // 최대 소유 가능 총알 개수
    public int carryBulletCount; //현재 소유하고 있는 총알 개수
    public float retroActionForce; // 반동 세기
    public float retroActionFineSightForce; // 정조준시 반동 세기
    public Vector3 fineSightOriginPos;  // 정조준시 위치
    public Animator anim;
    public ParticleSystem muzzleFlash;

    public AudioClip fire_sound;
}
