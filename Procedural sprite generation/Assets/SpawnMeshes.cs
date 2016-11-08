using UnityEngine;

public class SpawnMeshes : MonoBehaviour
{

    public Material material;
    public Texture roundTexture;
    public Texture otherTexture;
    public Texture squareTexture;
    public Texture uvGridTexture;

    void Start()
    {
        AddQuadrangle(new Vector3(3, 0,0));
        AddCircle(new Vector3(-3, 0, 0));
        AddTriangle(new Vector3(0, 3, 0));
        AddCake(new Vector3(0, -3, 0));
        AddRing(new Vector3(-3,-3,0));
        AddPointedCircle(new Vector3(-3,3,0));
        AddRectangle(new Vector3(3,3,0));
        AddEllipse(new Vector3(3,-3,0));
        AddStar(new Vector3(1,1));
        AddGear(new Vector3(-1,1));
        AddConvex(Vector3.zero);
    }

    private void AddQuadrangle(Vector3 pos)
    {
        GameObject quadrangle = new GameObject();
        quadrangle.transform.position = pos;

        Vector2[] p = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            p[i] = new Vector2(x, y);
        }

        quadrangle.AddComponent<QuadrangleMesh>().Build(p, material);
        quadrangle.GetComponent<QuadrangleMesh>().SetTexture(uvGridTexture);

        quadrangle.AddComponent<Rigidbody2D>();
    }
    private void AddTriangle(Vector3 pos)
    {
        GameObject triangle = new GameObject();
        triangle.transform.position = pos;

        Vector2 p1 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Vector2 p2 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Vector2 p3 = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        triangle.AddComponent<TriangleMesh>().Build(p1, p2, p3, material, Space.World);

        triangle.AddComponent<Rigidbody2D>();

        triangle.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    private void AddCake(Vector3 pos)
    {
        GameObject cake = new GameObject();
        cake.transform.position = pos;

        float radius = Random.Range(0.5f, 1.5f);
        int sides = Random.Range(10, 20);
        int sidesToFill = sides - Random.Range(5, 8);

        cake.AddComponent<CakeMesh>().Build(radius, sides, sidesToFill, material);
        cake.GetComponent<CakeMesh>().SetTexture(roundTexture);

        cake.AddComponent<Rigidbody2D>();
    }
    private void AddPointedCircle(Vector3 pos)
    {
        GameObject pointedCircle = new GameObject();
        pointedCircle.transform.position = pos;

        float radius = Random.Range(0.2f, 1.2f);
        float x = Random.Range(0.5f, 2f);
        float y = Random.Range(0.5f, 2f);
        int sides = Random.Range(6, 16);

        pointedCircle.AddComponent<PointedCircleMesh>().Build(radius, sides, new Vector2(x, y), material);

        pointedCircle.AddComponent<Rigidbody2D>();

        pointedCircle.GetComponent<MeshRenderer>().material.color = Color.blue;
    }
    private void AddRectangle(Vector3 pos)
    {
        GameObject rectangle = new GameObject();
        rectangle.transform.position = pos;

        float x = Random.Range(0.3f, 2f);
        float y = Random.Range(0.3f, 2f);

        rectangle.AddComponent<RectangleMesh>().Build(new Vector2(x, y), material);
        rectangle.GetComponent<RectangleMesh>().SetTexture(squareTexture);

        rectangle.AddComponent<Rigidbody2D>();

        rectangle.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }
    private void AddCircle(Vector3 pos)
    {
        GameObject circle = new GameObject();
        circle.transform.position = pos;

        float radius = Random.Range(0.5f, 1.5f);
        int sides = Random.Range(8, 16);

        circle.AddComponent<CircleMesh>().Build(radius, sides, material);
        circle.GetComponent<MeshBase>().SetTexture(roundTexture);

        circle.AddComponent<Rigidbody2D>();
    }
    private void AddEllipse(Vector3 pos)
    {
        GameObject ellipse = new GameObject();
        ellipse.transform.position = pos;

        float radiusA = Random.Range(0.2f, 0.9f);
        float radiusB = Random.Range(0.2f, 0.9f);
        int sides = Random.Range(6, 16);

        ellipse.AddComponent<EllipseMesh>().Build(radiusA, radiusB, sides, material);

        ellipse.AddComponent<Rigidbody2D>();

        ellipse.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
    private void AddRing(Vector3 pos)
    {
        GameObject ring = new GameObject();
        ring.transform.position = pos;

        float innerRadius = Random.Range(0f, 0.8f);
        float outerRadius = innerRadius+Random.Range(0, 0.8f);

        ring.AddComponent<RingMesh>().Build(innerRadius, outerRadius, Random.Range(3, 15), material);
        ring.GetComponent<RingMesh>().SetTexture(otherTexture);

        ring.AddComponent<Rigidbody2D>();

        ring.GetComponent<MeshRenderer>().material.color = Color.yellow;

    }
    private void AddStar(Vector3 pos)
    {
        GameObject star = new GameObject();
        star.transform.position = pos;

        float rA = Random.Range(0.1f, 0.5f);
        float rB = rA + Random.Range(0.1f, 0.6f);
        int sides = Random.Range(3, 30);

        star.AddComponent<StarMesh>().Build(rA, rB, sides, material);
        star.GetComponent<StarMesh>().SetTexture(otherTexture);

        star.AddComponent<Rigidbody2D>();
        
    }
    private void AddConvex(Vector3 pos)
    {
        GameObject convex = new GameObject();
        convex.transform.position = pos;

        Vector3[] verts = new Vector3[20];
        for (int i = 0; i < verts.Length; i++)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            verts[i] = new Vector3(x, y);
        }
        convex.AddComponent<ConvexMesh>().Build(verts, material);
        convex.GetComponent<ConvexMesh>().SetTexture(uvGridTexture);

        convex.AddComponent<Rigidbody2D>();

    }
    private void AddGear(Vector3 pos)
    {
        GameObject gearObject = new GameObject();
        gearObject.transform.position = pos;

        GearMesh gear = gearObject.AddComponent<GearMesh>();
        float lowRadius = Random.Range(0f, 0.5f);
        float midRadius = lowRadius + Random.Range(0f, 0.5f);
        float highRadius = midRadius + Random.Range(0f, 0.5f);
        int sides = Random.Range(3, 17);

        gear.Build(lowRadius, midRadius, highRadius, sides, material);
        gear.AddHingeJoint();

        gearObject.GetComponent<MeshRenderer>().material.color = Color.gray;
    }

}
