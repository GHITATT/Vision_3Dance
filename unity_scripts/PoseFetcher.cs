using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class PoseFetcher : MonoBehaviour
{
    public string playerUrl = "http://127.0.0.1:5002/pose";
    public string refUrl = "http://127.0.0.1:5001/pose";

    public List<Vector3> playerJoints = new List<Vector3>();
    public List<Vector3> refJoints = new List<Vector3>();

    void Start()
    {
        StartCoroutine(FetchLoop());
    }

    IEnumerator FetchLoop()
    {
        while (true)
        {
            yield return GetPose(playerUrl, playerJoints);
            yield return GetPose(refUrl, refJoints);
            yield return null;
        }
    }

    IEnumerator GetPose(string url, List<Vector3> target)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            var arr = JSON.Parse(json).AsArray;

            target.Clear();
            foreach (JSONNode node in arr)
            {
                float x = node[0].AsFloat;
                float y = node[1].AsFloat;
                float z = node[2].AsFloat;
                target.Add(new Vector3(x, y, z));
            }
        }
    }
}
