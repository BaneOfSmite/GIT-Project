﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
To Do For This Script:
- Animation
- Bug Test
 */
public class PlayerController : MonoBehaviour {
    public static PlayerController Instance;
    public float MovementSpeed, JumpSpeed;
    private Transform CameraTransform;
    private bool isRotating, isGrounded;
    private Animator anim;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        CameraTransform = Camera.main.transform;
        anim = GetComponent<Animator>();
    }

    void Update() {
        //print(GetComponent<Rigidbody>().velocity.y);
        if (Input.GetKey(KeyCode.Q)) {
            transform.Rotate(new Vector3(0, -1, 0));
        } else if (Input.GetKey(KeyCode.E)) {
            transform.Rotate(new Vector3(0, 1, 0));
        }

        transform.position += GetMoveVector().normalized * MovementSpeed * Time.deltaTime;
    }

    void FixedUpdate() {
        if (Input.GetAxisRaw("Jump") != 0 && isGrounded) {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0) * JumpSpeed, ForceMode.Impulse);
            anim.SetBool("Idle", false);
            anim.SetBool("Stopped", false);
        }
        anim.SetFloat("Jump", GetComponent<Rigidbody>().velocity.y);
    }
    private void OnCollisionExit(Collision col) {
        if (col.gameObject.tag == ("Floor")) {
            isGrounded = false;
        }
    }
    void OnCollisionEnter(Collision hit) {
        if (hit.gameObject.tag == ("Floor")) {
            isGrounded = true;
            anim.SetBool("Stopped", true);
        }
        if (hit.gameObject.GetComponent<ObjectiveRenderer>() != null) {
            if (hit.gameObject.name.Contains("Fire")) {
                if (GameManager.Instance.CurrentType == GameManager.PlayerFilter.Water) {
                    if (hit.gameObject.GetComponent<ObjectiveRenderer>().IsFilter) {
                        GameManager.Instance.Have.Add(GameManager.PlayerFilter.Fire);
                    } else {
                        GameManager.Instance.ObjectivesLeft--;
                    }
                    Destroy(hit.gameObject);
                }
            } else if (hit.gameObject.name.Contains("Air")) {
                if (GameManager.Instance.CurrentType == GameManager.PlayerFilter.Fire) {
                    if (hit.gameObject.GetComponent<ObjectiveRenderer>().IsFilter) {
                        GameManager.Instance.Have.Add(GameManager.PlayerFilter.Air);
                    } else {
                        GameManager.Instance.ObjectivesLeft--;
                    }
                    Destroy(hit.gameObject);
                }
            } else if (hit.gameObject.name.Contains("Earth")) {
                if (GameManager.Instance.CurrentType == GameManager.PlayerFilter.Air) {
                    if (hit.gameObject.GetComponent<ObjectiveRenderer>().IsFilter) {
                        GameManager.Instance.Have.Add(GameManager.PlayerFilter.Earth);
                    } else {
                        GameManager.Instance.ObjectivesLeft--;
                    }
                    Destroy(hit.gameObject);
                }
            } else if (hit.gameObject.name.Contains("Water")) {
                if (GameManager.Instance.CurrentType == GameManager.PlayerFilter.Earth) {
                    if (hit.gameObject.GetComponent<ObjectiveRenderer>().IsFilter) {
                        GameManager.Instance.Have.Add(GameManager.PlayerFilter.Water);
                    } else {
                        GameManager.Instance.ObjectivesLeft--;
                    }
                    Destroy(hit.gameObject);
                }
            }
        }
    }

    private Quaternion GetCameraTurn() {
        return Quaternion.AngleAxis(CameraTransform.rotation.eulerAngles.y, Vector3.up);
    }

    private Vector3 GetMoveVector() {
        Vector3 moveVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveVector = GetCameraTurn() * moveVector;
        return moveVector;
    }

    public void FinishIdleStart() {
        anim.SetBool("Idle", true);
        anim.SetBool("Stopped", true);
    }
}