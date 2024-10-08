using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CriticalPopUp : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Color textColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();    
        textColor = textMesh.color;
    }

    private float disappearTimer = 0.2f;
    private void Update()
    {
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0) {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textMesh.color.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
