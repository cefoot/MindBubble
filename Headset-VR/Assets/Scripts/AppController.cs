using System.Collections.Generic;
using System.Net.Http;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using UnityEngine.Events;

public class AppController : MonoBehaviour
{

    public static AppController Instance { get; private set; }
    public BubbleContent BubblePrefab;
    public UnityEvent StartedLoadingBubbles;
    public UnityEvent FinishedLoadingBubbles;
    private Coroutine _loadRoutine;
    private const string OLD_KEYWORDS = "OLD_KEYWORDS";

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitToPlaceKeywords());
    }

    private IEnumerator WaitToPlaceKeywords()
    {
        yield return new WaitUntil(() => Buddy.Instance);
        PlaceOldKeywords(Buddy.Instance.transform.position);
    }

    public void PlaceOldKeywords(Vector3 worldPos)
    {
        if (!PlayerPrefs.HasKey(OLD_KEYWORDS)) return;
        var keywordsJson = PlayerPrefs.GetString(OLD_KEYWORDS);
        var keywords = JsonConvert.DeserializeObject<string[]>(keywordsJson);
        var bubbles = new List<BubbleContent>();
        bubbles.AddRange(CreateBubbles(
            keywords.ToDictionary(k => k, k => new List<string>()), //text to place in bubbles
            worldPos, //where place bubbles
            worldPos - Camera.main.transform.position, //which direction should bubbles face
            .3f, //whats the radius of the desired circle
            t =>
            { //action when bubble pops
                LoadBubbles(t);
                bubbles.ForEach(b => Destroy(b.gameObject));
            }));
    }

    public void LoadBubbles(string hashTag)
    {
        if (_loadRoutine == null)
            _loadRoutine = StartCoroutine(LoadBubblesAsync(hashTag));
    }

    public IEnumerator LoadBubblesAsync(string hashTag)
    {
        StartedLoadingBubbles?.Invoke();
        try
        {
            Array.ForEach(GetComponentsInChildren<BubbleContent>(), b => b.Pop(false));
            var bubbles = GetComponentsInChildren<BubbleContent>();
            foreach (var item in bubbles)
            {
                item.Pop(false);
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0f, 0.2f));
            }
            UpdateHashTag(hashTag);
            using (var client = new HttpClient())
            {
                //get my location
                //quest does not have gps so use geo ip
                var ipRequest = client.GetAsync("https://api64.ipify.org/?format=txt");
                yield return new WaitUntil(() => ipRequest.Status >= System.Threading.Tasks.TaskStatus.RanToCompletion);
                if (ipRequest.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    Debug.LogError($"Was not able to complete Request Status:{ipRequest.Status}");
                    yield break;
                }
                var ipResult = ipRequest.Result.Content.ReadAsStringAsync();
                yield return new WaitUntil(() => ipResult.Status >= System.Threading.Tasks.TaskStatus.RanToCompletion);
                var content = ipResult.Result;
                Debug.Log($"my public ip is {content}");



                var geoIpRequest = client.GetAsync($"http://ip-api.com/json/{content}");
                yield return new WaitUntil(() => geoIpRequest.Status >= System.Threading.Tasks.TaskStatus.RanToCompletion);
                if (geoIpRequest.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    Debug.LogError($"Was not able to complete Request Status:{geoIpRequest.Status}");
                    yield break;
                }
                var geoIpResult = geoIpRequest.Result.Content.ReadAsStringAsync();
                yield return new WaitUntil(() => geoIpResult.Status >= System.Threading.Tasks.TaskStatus.RanToCompletion);
                var geoData = JsonConvert.DeserializeObject<GeoLocation>(geoIpResult.Result);
                if (geoData == null || geoData.Latitude == 0)
                {
                    geoData = new GeoLocation { Latitude = 0f, Longitude = 0f };
                }


                var uri = $"https://realityhack25-minbubble.azurewebsites.net/api/mindBubble/{hashTag}/{geoData.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}/{geoData.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture)}?";
                Debug.Log($"Sending Request: {uri}");
                var request = client.GetAsync($"{uri}code=6yo_NxBLKz-gg56I5UuEhBdecycemSTn4Wblo-YZUhmmAzFuAfsZrw%3D%3D");
                yield return new WaitUntil(() => request.Status >= System.Threading.Tasks.TaskStatus.RanToCompletion);
                if (request.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    Debug.LogError($"Was not able to complete Request Status:{request.Status}");
                    yield break;
                }
                Debug.Log($"Received Status: {request.Status}");
                var gptResult = request.Result.Content.ReadAsStringAsync();
                yield return new WaitUntil(() => gptResult.Status >= System.Threading.Tasks.TaskStatus.RanToCompletion);
                content = gptResult.Result;
                Debug.Log($"Received Answer: {content}");
                CreateBubbles(content, Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.forward);
            }
        }
        finally
        {
            _loadRoutine = null;
            FinishedLoadingBubbles?.Invoke();
        }
    }

    private void UpdateHashTag(string hashTag)
    {
        List<string> hashTags = new List<string>();
        if (PlayerPrefs.HasKey(OLD_KEYWORDS))
        {
            var keywordsJson = PlayerPrefs.GetString(OLD_KEYWORDS);
            hashTags.AddRange(JsonConvert.DeserializeObject<string[]>(keywordsJson));
        }
        if (hashTags.Any(s => s.Equals(hashTag, StringComparison.InvariantCultureIgnoreCase))) return;
        hashTags.Add(hashTag);
        if (hashTags.Count > 5)
        {
            hashTags.RemoveAt(0);
        }
        PlayerPrefs.SetString(OLD_KEYWORDS, JsonConvert.SerializeObject(hashTags.ToArray()));
        PlayerPrefs.Save();
    }

    public void CreateBubbles(string jsonContentThemes, Vector3 aroundWorldPos, Vector3 lookDir)
    {
        // Deserialize the content into a ThemesContainer
        var container = JsonConvert.DeserializeObject<ThemesContainer>(jsonContentThemes);
        var topics = container.Themes;
        CreateBubbles(topics, aroundWorldPos, lookDir, .5f);
    }

    public List<BubbleContent> CreateBubbles(Dictionary<string, List<string>> topics, Vector3 aroundWorldPos, Vector3 lookDir, float radius, Action<string> popAction = null)
    {
        var bubbles = new List<BubbleContent>();
        // Normalize the look direction
        lookDir.Normalize();

        // Get a vector perpendicular to the look direction (arbitrary plane)
        Vector3 up = Vector3.up;
        if (Vector3.Dot(lookDir, up) > 0.99f) // Handle edge case: lookDir is nearly parallel to up
        {
            up = Vector3.right;
        }
        Vector3 right = Vector3.Cross(up, lookDir).normalized;
        up = Vector3.Cross(lookDir, right).normalized;
        float angleStep = 360.0f / topics.Count; // Angle between each bubble
        int idx = 0;

        foreach (var item in topics)
        {
            // Calculate the angle for this bubble
            float angle = idx * angleStep * Mathf.Deg2Rad;

            // Position the bubble on the plane
            Vector3 offset = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up) * radius;
            Vector3 bubblePosition = aroundWorldPos + offset;

            // Instantiate the bubble
            var bubble = GameObject.Instantiate(BubblePrefab, transform);
            bubbles.Add(bubble);
            if (popAction != null)
                bubble.PopCallback = popAction;
            bubble.Posts = item.Value.ToArray();
            bubble.Text = item.Key;

            // Set the bubble's position and rotation
            bubble.transform.position = bubblePosition;
            bubble.transform.rotation = Quaternion.LookRotation(lookDir, up);

            idx++;
        }
        return bubbles;
    }

}
