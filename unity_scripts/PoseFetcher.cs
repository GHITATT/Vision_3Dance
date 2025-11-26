using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class PoseFetcher : MonoBehaviour
{
    public string url = "http://127.0.0.1:5000/pose";
    public List<Vector3> joints = new List<Vector3>();

    void Start()
    {
        StartCoroutine(FetchLoop());
    }

    IEnumerator FetchLoop()
    {
        while (true)
        {
            UnityWebRequest req = UnityWebRequest.Get(url);
            yield return req.SendWebRequest();

            if (!req.isNetworkError && !req.isHttpError)
            {
                string json = req.downloadHandler.text;
                // Debug.Log(json);  // on sait que ça marche

                var arr = JSON.Parse(json).AsArray;
                joints.Clear();

                foreach (JSONNode node in arr)
                {
                    float x = node[0].AsFloat;
                    float y = node[1].AsFloat;
                    float z = node[2].AsFloat;

                    joints.Add(new Vector3(x, y, z));
                }
            }

            yield return null;
        }
    }
}
