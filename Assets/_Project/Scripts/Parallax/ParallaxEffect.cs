using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float xParallaxValue;
    [SerializeField] private float yParallaxValue;

    private Camera mainCamera;
    private Vector3 deltaMovement;
    private Vector3 lastCameraPosition;
    private float spriteLength;

    private void Start()
    {
        mainCamera = Camera.main;
        lastCameraPosition = mainCamera.transform.position;
        spriteLength = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        deltaMovement = mainCamera.transform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * xParallaxValue, deltaMovement.y * yParallaxValue);
        lastCameraPosition = mainCamera.transform.position;

        if (mainCamera.transform.position.x - transform.position.x >= spriteLength)
        {
            transform.position = new(mainCamera.transform.position.x + spriteLength, transform.position.y);
        }
        else if (transform.position.x - mainCamera.transform.position.x >= spriteLength)
        {
            transform.position = new(mainCamera.transform.position.x - spriteLength, transform.position.y);
        }
    }
}