using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class PoseGizmos : MonoBehaviour
{
    public PoseFetcher fetcher;
    public float sphereSize = 0.02f;
    public float refOffsetX = -0.5f;
    public float playerOffsetX = 0.5f;

    Color[] playerPalette =
    {
        new Color(1f, 0.8f, 0.2f),
        new Color(0.2f, 0.8f, 1f),
        new Color(1f, 0.4f, 0.4f),
        new Color(0.4f, 1f, 0.4f)
    };

    Color[] refPalette =
    {
        new Color(1f, 0.2f, 0.2f),
        new Color(0.2f, 1f, 1f),
        new Color(1f, 1f, 0.2f),
        new Color(0.2f, 1f, 0.2f)
    };

    void OnDrawGizmos()
    {
        if (fetcher == null) return;

        DrawPose(fetcher.playerJoints, playerPalette, playerOffsetX);
        DrawPose(fetcher.refJoints, refPalette, refOffsetX);
    }

    void DrawPose(List<Vector3> pts, Color[] palette, float offsetX)
    {
        if (pts == null || pts.Count == 0) return;

        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 p = pts[i];

            Vector3 pos = new Vector3(
                (p.x - 0.5f) + offsetX,
                -(p.y - 0.5f),
                -p.z * 0.1f
            );

            Gizmos.color = GetColorForLandmark(i, palette);
            Gizmos.DrawSphere(transform.position + pos, sphereSize);
        }
    }

    Color GetColorForLandmark(int id, Color[] pal)
    {
        if (id <= 10) return pal[0];
        if (id >= 11 && id <= 16) return pal[2];
        if (id >= 23 && id <= 32) return pal[3];
        return pal[1];
    }
}
