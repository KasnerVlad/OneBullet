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
    private Transform startAnchor=>startPoint.transform;
    private Transform endAnchor => endPoint.transform;
    [SerializeField] private float offsetEndPointRot;
    [SerializeField] private float offsetStartPointRot;
    [SerializeField] private float offsetHandPointRot; 
    [SerializeField]private int numberOfPoints = 50;
    [SerializeField]private float sagFactor = 0.5f; 
    [SerializeField]private Camera cam;
    
    [Header("Dynamic Segment Calculation")]
    public float targetSegmentLength = 0.5f; 
    public int minSegmentCount = 5;     
    public int maxSegmentCount = 50;     

    [Header("Bridge Physics Parameters")]
    public int constraintIterations = 10; 
    public float damping = 0.99f;         
    public float gravityScale = 1.0f;     

    private List<BridgeNode> nodes = new List<BridgeNode>();
    private float currentBridgeLength;    
    private int actualSegmentCount;       
    private float actualSegmentLength;    
    private void Awake()
    {
        InputManager.playerInput.UI.Grab.performed += e => {if(ModeManager.Instance.nowMode==Mode.EditMode) handGrab.SetActive(true); handStraight.SetActive(false); };
        InputManager.playerInput.UI.UnGrab.performed += e => {if(ModeManager.Instance.nowMode==Mode.EditMode)handGrab.SetActive(false); handStraight.SetActive(true); };
        InputManager.playerInput.Enable();
        handLine.useWorldSpace = true;
    }

    private void Start()
    {
        InitializeNodes(false);
    }
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
            UpdateRotations();
            // Если якоря отсутствуют (например, были удалены), выходим
            if (startAnchor == null || endAnchor == null) return;

            float newBridgeLength = Vector3.Distance(startAnchor.position, endAnchor.position);
            int newActualSegmentCount = Mathf.Clamp(Mathf.RoundToInt(newBridgeLength / targetSegmentLength), minSegmentCount, maxSegmentCount);

            // Проверяем, изменилась ли дистанция или количество сегментов достаточно, чтобы переинициализировать мост
            if (Mathf.Abs(newBridgeLength - currentBridgeLength) > 0.1f || newActualSegmentCount != actualSegmentCount)
            {
                InitializeNodes(true); // Пересоздаем мост с новыми параметрами, сохраняя текущую форму
            }

            ApplyGravity();
            ApplyConstraints();
            UpdateLineRendererPositions();

        }
        else
        {
            handLine.gameObject.SetActive(false);
            startPoint.SetActive(false);
            endPoint.SetActive(false);
        }
    }
    private void UpdateRotations()
    {

        endPoint.transform.rotation = Helper.RotateTo(startPoint.transform.position, endPoint.transform.position,new Vector3(0,0,offsetEndPointRot));
        startPoint.transform.rotation = Helper.RotateTo(endPoint.transform.position, startPoint.transform.position,new Vector3(0,0,offsetStartPointRot));
        handLine.transform.rotation = Helper.RotateTo(endPoint.transform.position, handLine.transform.position,new Vector3(0,0,offsetHandPointRot));
    }


    private class BridgeNode
    {
        public Vector3 currentPosition;
        public Vector3 previousPosition;
        public bool isFixed;
    }

    #region HandPhisics
    void InitializeNodes(bool preserveShape)
    {
        if (startAnchor == null || endAnchor == null)
        {
            Debug.LogError("Start Anchor или End Anchor не назначены! Пожалуйста, перетяните объекты в Inspector.");
            enabled = false;
            return;
        }

        currentBridgeLength = Vector3.Distance(startAnchor.position, endAnchor.position);
        actualSegmentCount = Mathf.Clamp(Mathf.RoundToInt(currentBridgeLength / targetSegmentLength), minSegmentCount, maxSegmentCount);
        actualSegmentLength = currentBridgeLength / actualSegmentCount;

        handLine.positionCount = actualSegmentCount + 1;

        List<Vector3> oldNodePositions = new List<Vector3>();
        if (preserveShape && nodes.Count > 0)
        {
            foreach (var node in nodes)
            {
                oldNodePositions.Add(node.currentPosition);
            }
        }

        nodes.Clear(); 
        for (int i = 0; i <= actualSegmentCount; i++)
        {
            BridgeNode node = new BridgeNode();
            float t = (float)i / actualSegmentCount;

            if (preserveShape && oldNodePositions.Count > 1)
            {
                node.currentPosition = GetPositionOnOldCurve(t, oldNodePositions, startAnchor.position, endAnchor.position);
            }
            else
            {
                node.currentPosition = Vector3.Lerp(startAnchor.position, endAnchor.position, t);
            }

            node.previousPosition = node.currentPosition;

            if (i == 0)
            {
                node.isFixed = true;
            }
            else if (i == actualSegmentCount)
            {
                node.isFixed = true;
            }
            else
            {
                node.isFixed = false;
            }
            nodes.Add(node);
        }
    }

    Vector3 GetPositionOnOldCurve(float t, List<Vector3> oldPositions, Vector3 newStart, Vector3 newEnd)
    {

        if (oldPositions.Count < 2)
        {
            return Vector3.Lerp(newStart, newEnd, t);
        }

        float oldCurveLength = 0;
        for (int i = 0; i < oldPositions.Count - 1; i++)
        {
            oldCurveLength += Vector3.Distance(oldPositions[i], oldPositions[i + 1]);
        }

        float targetLengthOnOldCurve = t * oldCurveLength;
        float currentLength = 0;

        for (int i = 0; i < oldPositions.Count - 1; i++)
        {
            float segmentLen = Vector3.Distance(oldPositions[i], oldPositions[i + 1]);
            if (currentLength + segmentLen >= targetLengthOnOldCurve)
            {
                float segmentT = (targetLengthOnOldCurve - currentLength) / segmentLen;
                return Vector3.Lerp(oldPositions[i], oldPositions[i + 1], segmentT);
            }
            currentLength += segmentLen;
        }

        return oldPositions[oldPositions.Count - 1];
    }


    void ApplyGravity()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!nodes[i].isFixed)
            {
                Vector3 tempCurrentPos = nodes[i].currentPosition;
                nodes[i].currentPosition += (nodes[i].currentPosition - nodes[i].previousPosition) * damping + Physics.gravity * gravityScale * Time.fixedDeltaTime * Time.fixedDeltaTime;
                nodes[i].previousPosition = tempCurrentPos;
            }
        }
    }

    void ApplyConstraints()
    {
        for (int k = 0; k < constraintIterations; k++)
        {
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                BridgeNode node1 = nodes[i];
                BridgeNode node2 = nodes[i + 1];

                float currentDistance = Vector3.Distance(node1.currentPosition, node2.currentPosition);
                Vector3 direction = (node2.currentPosition - node1.currentPosition).normalized;
                Vector3 correction = direction * (currentDistance - actualSegmentLength);

                if (!node1.isFixed)
                {
                    node1.currentPosition += correction * 0.5f;
                }
                if (!node2.isFixed)
                {
                    node2.currentPosition -= correction * 0.5f;
                }
            }

            if (startAnchor != null)
                nodes[0].currentPosition = startAnchor.position;
            if (endAnchor != null)
                nodes[nodes.Count - 1].currentPosition = endAnchor.position;
        }
    }

    void UpdateLineRendererPositions()
    {
        // ... (без изменений) ...
        for (int i = 0; i < nodes.Count; i++)
        {
            handLine.SetPosition(i, nodes[i].currentPosition);
        }
    }
    #endregion
    /*void OnDrawGizmos()
    {
        // ... (без изменений) ...
        if (nodes != null && nodes.Count > 0 && startAnchor != null && endAnchor != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < nodes.Count; i++)
            {
                Gizmos.DrawSphere(nodes[i].currentPosition, 0.1f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startAnchor.position, 0.2f);
            Gizmos.DrawSphere(endAnchor.position, 0.2f);
        }
    }*/
}
