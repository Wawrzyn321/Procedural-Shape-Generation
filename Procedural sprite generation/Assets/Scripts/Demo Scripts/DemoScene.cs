using UnityEngine;

/// <summary>
/// Demo scene to present all* PSG shapes.
/// * all but LineMesh, which is presented in {LineMeshDemo}.
/// </summary>
public class DemoScene : MonoBehaviour
{

    public Material material;
    public Texture roundTexture;
    public Texture otherTexture;
    public Texture squareTexture;
    public Texture uvGridTexture;

    void Start()
    {
        //add some shapes to playground
        AddQuadrangle(new Vector3(4, 0, 0));
        AddCircle(new Vector3(-4, 0, 0));
        AddTriangle(new Vector3(0, 3, 0));
        AddCake(new Vector3(0, -3, 0));
        AddRing(new Vector3(-4, -3, 0));
        AddPointedCircle(new Vector3(-4, 3, 0));
        AddRectangle(new Vector3(4, 3, 0));
        AddEllipse(new Vector3(4, -3, 0));
        AddStar(new Vector3(1, 1));
        AddGear(new Vector3(-1, 1));
        AddConvex(Vector3.zero);
        MeshBase.Join(GameObject.Find("Convex Mesh").GetComponent<MeshBase>(), GameObject.Find("Circle").GetComponent<MeshBase>());
        GameObject.Find("Convex Mesh").GetComponent<MeshBase>().JoinTo(GameObject.Find("Triangle").GetComponent<MeshBase>());
    }

    private void AddQuadrangle(Vector3 pos)
    {
        
        Vector2[] verts = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            verts[i] = new Vector2(x, y);
        }

        GameObject quadrangle = QuadrangleMesh.AddRectangleMesh(pos, verts, material);

        quadrangle.GetComponent<QuadrangleMesh>().SetTexture(uvGridTexture);
    }
    private void AddTriangle(Vector3 pos)
    {
        Vector2 p1 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Vector2 p2 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Vector2 p3 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        GameObject triangle = TriangleMesh.AddTriangle(pos, p1, p2, p3, material);

        triangle.GetComponent<MeshRenderer>().material.color = Color.red;

    }
    private void AddCake(Vector3 pos)
    {
        float radius = Random.Range(0.5f, 1.5f);
        int sides = Random.Range(10, 20);
        int sidesToFill = sides - Random.Range(5, 8);

        GameObject cake = CakeMesh.AddCakeMesh(pos, radius, sides, sidesToFill, material);
        
        cake.GetComponent<CakeMesh>().SetTexture(roundTexture);
    }
    private void AddPointedCircle(Vector3 pos)
    {
        float radius = Random.Range(0.2f, 1.2f);
        float x = Random.Range(0.5f, 2f);
        float y = Random.Range(0.5f, 2f);
        int sides = Random.Range(6, 16);

        GameObject pointedCircle = PointedCircleMesh.AddPointedCircleMesh(pos, radius, sides, new Vector2(x, y), material);

        pointedCircle.GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    private void AddRectangle(Vector3 pos)
    {
        float x = Random.Range(0.3f, 2f);
        float y = Random.Range(0.3f, 2f);

        GameObject rectangle = RectangleMesh.AddRectangleMesh(pos, new Vector2(x, y), material);
        rectangle.GetComponent<RectangleMesh>().SetTexture(squareTexture);

        rectangle.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }
    private void AddCircle(Vector3 pos)
    {
        float radius = Random.Range(0.5f, 1.5f);
        int sides = Random.Range(8, 16);

        GameObject circle = CircleMesh.AddCircle(pos, radius, sides, material);

        circle.GetComponent<MeshBase>().SetTexture(roundTexture);
    }
    private void AddEllipse(Vector3 pos)
    {
        float radiusA = Random.Range(0.2f, 0.9f);
        float radiusB = Random.Range(0.2f, 0.9f);
        int sides = Random.Range(8, 16);

        GameObject ellipse = EllipseMesh.AddEllipseMesh(pos, radiusA, radiusB, sides, material);

        ellipse.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
    private void AddRing(Vector3 pos)
    {
        float innerRadius = Random.Range(0.4f, 0.8f);
        float outerRadius = innerRadius + Random.Range(0.5f, 0.8f);
        int sides = Random.Range(10, 18);

        GameObject ring = RingMesh.AddRingMesh(pos, innerRadius, outerRadius, sides, material);
        ring.GetComponent<RingMesh>().SetTexture(otherTexture);

        ring.GetComponent<MeshRenderer>().material.color = Color.yellow;

    }
    private void AddStar(Vector3 pos)
    {
        float rA = Random.Range(0.1f, 0.5f);
        float rB = rA + Random.Range(0.1f, 0.6f);
        int sides = Random.Range(3, 30);

        GameObject star = StarMesh.AddStartMesh(pos, rA, rB, sides, material);

        star.GetComponent<StarMesh>().SetTexture(otherTexture);
    }
    private void AddConvex(Vector3 pos)
    {
        Vector3[] verts = new Vector3[20];
        for (int i = 0; i < verts.Length; i++)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            verts[i] = new Vector3(x, y);
        }

        GameObject convex = ConvexMesh.AddConvexMesh(pos, verts, material);

        convex.GetComponent<ConvexMesh>().SetTexture(uvGridTexture);

    }
    private void AddGear(Vector3 pos)
    {;
        float innerRadius = Random.Range(0.3f, 0.5f);
        float rootRadius = innerRadius + Random.Range(0.1f, 0.5f);
        float outerRadius = rootRadius + Random.Range(0.3f, 0.5f);
        int sides = Random.Range(3, 17);

        GameObject gear = GearMesh.AddGearMesh(pos, innerRadius, rootRadius, outerRadius, sides, material);

        gear.GetComponent<GearMesh>().AddHingeJoint();

        gear.GetComponent<MeshRenderer>().material.color = Color.gray;
    }

}
