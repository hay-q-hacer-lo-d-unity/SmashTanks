using UnityEngine;

using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
public class MergeColliders2D : MonoBehaviour
{
    public Collider2D[] collidersToMerge;

    void Start()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        List<Vector2[]> paths = new List<Vector2[]>();

        foreach (var c in collidersToMerge)
        {
            if (c is BoxCollider2D b)
            {
                Vector2 size = b.size * 0.5f;
                Vector2[] boxPoints = new Vector2[]
                {
                    b.offset + new Vector2(-size.x, -size.y),
                    b.offset + new Vector2(-size.x,  size.y),
                    b.offset + new Vector2( size.x,  size.y),
                    b.offset + new Vector2( size.x, -size.y)
                };
                for (int j = 0; j < boxPoints.Length; j++)
                    boxPoints[j] = b.transform.TransformPoint(boxPoints[j]) - transform.position;
                paths.Add(boxPoints);
            }
            else if (c is CircleCollider2D circ)
            {
                int segments = 12;
                Vector2[] circle = new Vector2[segments];
                for (int i = 0; i < segments; i++)
                {
                    float angle = (i / (float)segments) * Mathf.PI * 2;
                    Vector2 local = circ.offset + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circ.radius;
                    circle[i] = circ.transform.TransformPoint(local) - transform.position;
                }
                paths.Add(circle);
            }
            else if (c is PolygonCollider2D p)
            {
                for (int i = 0; i < p.pathCount; i++)
                {
                    Vector2[] path = p.GetPath(i);
                    for (int j = 0; j < path.Length; j++)
                        path[j] = p.transform.TransformPoint(path[j]) - transform.position;
                    paths.Add(path);
                }
            }
        }

        poly.pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++)
            poly.SetPath(i, paths[i]);
    }
}
