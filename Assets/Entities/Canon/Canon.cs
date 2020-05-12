﻿using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Canon : MonoBehaviour
{
    [SerializeField] ParticleSystem rechargeVFX;
    [SerializeField] Transform bombOrigin;
    [SerializeField] Transform pipe;
    [SerializeField] AudioClip bombFireSFX;
    [SerializeField] AudioClip superBombFireSFX;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        InputManager.Instance.OnBombRecharge.AddListener(HandleBombRecharge);
        InputManager.Instance.OnBombThrow.AddListener(HandleBombThrow);
    }

    void HandleBombThrow(Vector3 tapPosition)
    {
        pipe.LookAt(tapPosition);
        anim.SetTrigger("Shoot");
        AudioClip sfx = InputManager.Instance.isSuperBombActive ? superBombFireSFX : bombFireSFX;
        AudioManager.Instance.Play(sfx);
    }

    void HandleBombRecharge()
    {
        Instantiate(rechargeVFX, bombOrigin.position, bombOrigin.rotation);
    }
}