using System.Collections.Generic;
using Custom;
using Project.Scripts;
using Project.Scripts.Character;
using Project.Scripts.Enums;
using UnityEngine;
using UnityEngine.Serialization;

public class HandController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject handGrab;
    [SerializeField] private GameObject handStraight;
    [SerializeField] private LineRenderer handLine;
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject endPoint;
    [SerializeField] private float offsetEndPointRot;
    [SerializeField] private float offsetStartPointRot;
    [SerializeField] private float offsetHandPointRot; 
    [SerializeField]private int numberOfPoints = 50;
    [SerializeField]private float sagFactor = 0.5f; 
    [SerializeField]private Camera cam;
    private void Awake()
    {
        InputManager.playerInput.UI.Grab.performed += e => {if(ModeManager.Instance.nowMode==Mode.EditMode) handGrab.SetActive(true); handStraight.SetActive(false); };
        InputManager.playerInput.UI.UnGrab.performed += e => {if(ModeManager.Instance.nowMode==Mode.EditMode)handGrab.SetActive(false); handStraight.SetActive(true); };
        InputManager.playerInput.Enable();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (ModeManager.Instance.nowMode == Mode.EditMode)
        {
            handLine.gameObject.SetActive(true);
            startPoint.SetActive(true);
            endPoint.SetActive(true);
            startPoint.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, startPoint.transform.position.z);
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = startPoint.transform.position.z - cam.transform.position.z; 
            Vector3 endPointPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
            endPoint.transform.position =new Vector3(endPointPosition.x, endPointPosition.y, endPoint.transform.position.z); ;
            UpdateLineRenderer();
            UpdateRotations();
        }
        else
        {
            handLine.gameObject.SetActive(false);
            startPoint.SetActive(false);
            endPoint.SetActive(false);
        }
    }

    private void UpdateLineRenderer()
    {
        DrawSaggingLine(startPoint.transform.position, endPoint.transform.position);

    }
    void DrawSaggingLine(Vector3 p1, Vector3 p2)
    {
        handLine.positionCount = numberOfPoints;

        float distance = Vector3.Distance(p1, p2);
        float sagDepth = distance * sagFactor;

        for (int i = 0; i < numberOfPoints; i++)
        {
            float t = i / (float)(numberOfPoints - 1); // Прогресс от 0 до 1

            Vector3 currentPoint = Vector3.Lerp(p1, p2, t);
            
            Vector3 direction = (p2 - p1).normalized;
            Vector3 perpendicularDirection = Vector3.Cross(direction, Vector3.forward);
            /*if (cam.orthographic)
            {
                perpendicularDirection = new Vector3(-direction.y, direction.x, 0).normalized;
            }*/

            float parabolaOffset = -4 * sagDepth * t * (1 - t);
            currentPoint += perpendicularDirection * parabolaOffset;

            handLine.SetPosition(i, currentPoint);
        }
    }
    private void UpdateRotations()
    {

        endPoint.transform.rotation = Helper.RotateTo(startPoint.transform.position, endPoint.transform.position,new Vector3(0,0,offsetEndPointRot));
        startPoint.transform.rotation = Helper.RotateTo(endPoint.transform.position, startPoint.transform.position,new Vector3(0,0,offsetStartPointRot));
        handLine.transform.rotation = Helper.RotateTo(endPoint.transform.position, handLine.transform.position,new Vector3(0,0,offsetHandPointRot));
    }
}
