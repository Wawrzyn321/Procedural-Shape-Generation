using UnityEngine;
using System.Collections;
using PSG;

/// <summary>
/// Demo scene to show PSG's line with some circles.
/// </summary>
public class LineMeshDemo : MonoBehaviour {

    public Material material;
    public Texture otherTexture;
    public int ballsToSpawn = 10;
    public bool doubleCollider = true;

    void Start ()
    {
        AddLineMesh();
        StartCoroutine(AddBalls());
    }

    private void AddLineMesh()
    {
        GameObject line = new GameObject();

        LineMesh lineMesh = line.AddComponent<LineMesh>();
        Vector2[] v = new Vector2[7];
        for (int i = 0; i < v.Length; i++)
        {
            float x = -7.5f + i * 2.5f;
            float y = (i - 3)*(i - 3)/3f + Random.Range(-1f, 1f);
            v[i] = new Vector2(x, y);
        }
        lineMesh.Build(v, 0.2f, doubleCollider, material);

        lineMesh.C_MR.material.color = Color.green;
        line.AddComponent<Rigidbody2D>().useAutoMass = true;
    }

    private IEnumerator AddBalls()
    {
        yield return new WaitForSeconds(2);
        WaitForSeconds delay = new WaitForSeconds(0.75f);
        for (int i = 0; i < ballsToSpawn; i++)
        {
            AddBall();
            yield return delay;
        }
    }

    private void AddBall()
    {
        Vector3 position = transform.position + new Vector3(Random.Range(-8f, 8f), 0);
        CircleMesh ball = CircleMesh.AddCircle(position, 0.5f, 32, true, material);
        ball.SetTexture(otherTexture);
    }
    
}
