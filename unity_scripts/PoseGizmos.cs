using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class PoseGizmos : MonoBehaviour
{
    public PoseFetcher pose;
    public float sphereSize = 0.02f;

    // Couleurs par zone anatomique
    Color headColor = new Color(1f, 0.8f, 0.2f);
    Color torsoColor = new Color(0.2f, 0.8f, 1f);
    Color armColor = new Color(1f, 0.4f, 0.4f);
    Color legColor = new Color(0.4f, 1f, 0.4f);

    void OnDrawGizmos()
    {
        if (pose == null || pose.joints == null) return;

        List<Vector3> pts = pose.joints;

        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 p = pts[i];

            // MediaPipe renvoie des coordonnées normalisées (0–1)
            // On convertit en repère Unity simple devant la caméra
            Vector3 pos = new Vector3(
                (p.x - 0.5f),     // centrer
                -(p.y - 0.5f),    // inverser Y
                -p.z * 0.1f       // échelle Z
            );

            Gizmos.color = GetColorForLandmark(i);

            Gizmos.DrawSphere(transform.position + pos, sphereSize);
        }
    }

    // Attribution de couleurs par groupe
    Color GetColorForLandmark(int id)
    {
        // Référence MediaPipe Pose (33 landmarks)
        // 0 = nose
        // 1–7 = upper body / eyes / ears / shoulders
        // 11–16 = arms
        // 23–32 = legs

        if (id == 0 || id <= 10)         // tête + torse haut
            return headColor;
        if (id >= 11 && id <= 16)        // bras
            return armColor;
        if (id >= 23 && id <= 32)        // jambes
            return legColor;

        return torsoColor;                // poitrine / hanches
    }
}
