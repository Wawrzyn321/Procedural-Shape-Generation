using UnityEngine;
using System.Collections;

public class MouseDrag : MonoBehaviour {
    
    public bool attachToCenterOfMass = true;

    private MeshBase connectedMeshBase;
    private Rigidbody2D connectedBody;
    private LineRenderer C_LR;
    private SpringJoint2D C_SP2D;
    private float oldDrag;

    void Awake()
    {
        C_LR = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Rigidbody2D>())
                {
                    connectedBody = hit.collider.GetComponent<Rigidbody2D>();

                    oldDrag = connectedBody.drag;
                    connectedBody.drag = 10f;

                    C_SP2D = hit.collider.gameObject.AddComponent<SpringJoint2D>();
                    C_SP2D.autoConfigureDistance = false;
                    C_SP2D.distance = 0;
                    C_SP2D.dampingRatio = 1f;
                    C_SP2D.frequency = 5f;

                    connectedMeshBase = hit.collider.GetComponent<MeshBase>();

                    if (attachToCenterOfMass)
                    {
                        if (connectedMeshBase != null)
                        {
                            C_SP2D.anchor = connectedBody.transform.InverseTransformPoint(connectedMeshBase.GetCenter());
                        }
                        else
                        {
                            C_SP2D.anchor = connectedBody.transform.InverseTransformPoint(connectedBody.transform.position);
                        }
                    }
                    else
                    {
                        C_SP2D.anchor = connectedBody.transform.InverseTransformPoint(hit.point);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (connectedBody != null)
            {
                Destroy(connectedBody.gameObject.GetComponent<SpringJoint2D>());
                connectedBody.gravityScale = 1;
                connectedBody.drag = oldDrag;
                C_SP2D = null;
                connectedBody = null;
                C_LR.SetPosition(0, Vector3.zero);
                C_LR.SetPosition(1, Vector3.zero);
            }
        }
    }

    void FixedUpdate()
    {
        if (connectedBody)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            C_SP2D.connectedAnchor = pos;
            C_LR.SetPosition(0, C_SP2D.connectedAnchor);
            C_LR.SetPosition(1, connectedBody.transform.TransformPoint(C_SP2D.anchor));
        }
    }
}
