using UnityEngine;

/// <summary>
/// Sample script of mouse physics-based shape moving.
/// </summary>
public class PSG_MouseDrag : MonoBehaviour {
    
    public bool attachToCenterOfMass = true;

    private Rigidbody2D connectedBody;
    private float oldDrag;

    //components
    private LineRenderer C_LR;
    private SpringJoint2D C_SP2D;

    void Awake()
    {
        C_LR = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //raycast from mouse pointer
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                //if we hit something...
                if (hit.collider.GetComponent<Rigidbody2D>())
                {
                    //...and it has Rigidbody2D

                    //assign hit object's Rigidbody to {connectedBody}
                    connectedBody = hit.collider.GetComponent<Rigidbody2D>();

                    //save its drag
                    oldDrag = connectedBody.drag;
                    connectedBody.drag = 10f;

                    //attach SpringJoint to hit collider
                    C_SP2D = hit.collider.gameObject.AddComponent<SpringJoint2D>();
                    C_SP2D.autoConfigureDistance = false;
                    C_SP2D.distance = 0;
                    C_SP2D.dampingRatio = 1f;
                    C_SP2D.frequency = 5f;
                    
                    //choose the place to anchor spring joint
                    if (attachToCenterOfMass)
                    {
                        MeshBase connectedMeshBase = hit.collider.GetComponent<MeshBase>();
                        //check if selected Rigidbody has MeshBase derived component
                        if (connectedMeshBase != null)
                        {
                            //GetCenter overrides object origin
                            C_SP2D.anchor = connectedBody.transform.InverseTransformPoint(connectedMeshBase.GetCenter());
                        }
                        else
                        {
                            //set to generated center of mass
                            C_SP2D.anchor = connectedBody.transform.InverseTransformPoint(connectedBody.transform.position);
                        }
                    }
                    else
                    {
                        //set anchor to point of hit
                        C_SP2D.anchor = connectedBody.transform.InverseTransformPoint(hit.point);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (connectedBody != null)
            {
                //remove joint from body
                Destroy(C_SP2D);
                connectedBody.gravityScale = 1;
                connectedBody.drag = oldDrag;
                C_SP2D = null;
                connectedBody = null;
                SetLineRendererPositions(Vector3.zero, Vector3.zero);
            }
        }
    }

    void FixedUpdate()
    {
        if (connectedBody)
        {
            //drag connected body
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            C_SP2D.connectedAnchor = pos;
            SetLineRendererPositions(C_SP2D.connectedAnchor,
                connectedBody.transform.TransformPoint(C_SP2D.anchor));
        }
    }

    //set points of LineRenderer
    private void SetLineRendererPositions(Vector3 pos1, Vector3 pos2)
    {
        C_LR.SetPosition(0, pos1);
        C_LR.SetPosition(1, pos2);
    }

}
