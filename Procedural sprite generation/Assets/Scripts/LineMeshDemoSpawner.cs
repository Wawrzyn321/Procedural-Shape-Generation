using UnityEngine;
using System.Collections;

public class LineMeshDemoSpawner : MonoBehaviour {

    public Material material;
    public Texture otherTexture;
    public int ballsToSpawn = 10;
    public bool doubleCollider = true;

    void Start ()
    {
        SpawnLineMesh();
        StartCoroutine("SpawnBalls");
    }

    private void SpawnLineMesh()
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

        line.GetComponent<MeshRenderer>().material.color = Color.green;
        line.AddComponent<Rigidbody2D>().useAutoMass = true;
    }

    private IEnumerator SpawnBalls()
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < ballsToSpawn; i++)
        {
            SpawnBall();
            yield return new WaitForSeconds(0.75f);
        }
    }

    private void SpawnBall()
    {
        GameObject ball = new GameObject();
        ball.transform.position = transform.position + new Vector3(Random.Range(-8f, 8f), 0);

        CircleMesh circleMesh = ball.AddComponent<CircleMesh>();
        circleMesh.Build(0.5f, 32, material);
        circleMesh.SetTexture(otherTexture);

        ball.AddComponent<Rigidbody2D>();
    }
    
}
