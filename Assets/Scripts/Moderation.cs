using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UnityLibrary
{
    public class Moderation : MonoBehaviour
    {
        private string apiKey = "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"; // Your OpenAI API key
        public InputField contentInputField; // Input field for user-generated content
        public Button submitButton;
        public Button returnButton;
        const string url = "https://api.openai.com/v1/moderations";
        public Text sexual;
        public Text hate;
        public Text violence;
        public Text self_harm;
        public Text sexual_minors;
        public Text hate_threatening;
        public Text violence_graphic;
        [SerializeField] private GameObject PlayCanvas;
        [SerializeField] private GameObject ModerationCanvas;



        // Start is called before the first frame update
        void Start()
        {
            // Add an event listener to the submit button
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
            returnButton.onClick.AddListener(Returntogame);

        }
        public void Returntogame()
        {
            PlayCanvas.SetActive(true);
            ModerationCanvas.SetActive(false);
        }
        // Method to handle submit button click event
        void OnSubmitButtonClicked()
        {
            // Get the content to moderate from the input field
            string contentToModerate = contentInputField.text;
            Debug.Log(contentToModerate);

            // Call the moderation API
            ModerateContent(contentToModerate);
        }

        // Coroutine to make content moderation API request

        private void ModerateContent(string content)
        {

            RequestData requestData = new RequestData()
            {
                input = content,
            };

            string jsonData = JsonUtility.ToJson(requestData);

            byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

            UnityWebRequest request = UnityWebRequest.Post(url, jsonData);
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);



            UnityWebRequestAsyncOperation async = request.SendWebRequest();

            async.completed += (op) =>
            {

                // Check for errors
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error: {request.error}");
                }
                else
                {
                    // Parse the response
                    string responseText = request.downloadHandler.text;

                    ContentModerationData moderationData = JsonConvert.DeserializeObject<ContentModerationData>(responseText);

                    // Access the moderation results
                    foreach (var result in moderationData.results)
                    {
                        Debug.Log("Flagged: " + result.flagged);
                        Debug.Log("Categories: Sexual=" + result.categories.sexual +
                            ", Hate=" + result.categories.hate +
                            ", Violence=" + result.categories.violence +
                            ", Self-harm=" + result.categories.self_harm +
                            ", Sexual/Minors=" + result.categories.sexual_minors +
                            ", Hate/Threatening=" + result.categories.hate_threatening +
                            ", Violence/Graphic=" + result.categories.violence_graphic);
                        sexual.text = "Sexual:"+result.categories.sexual.ToString();
                        hate.text = "Hate:"+result.categories.hate.ToString();
                        violence.text = "Violence:"+result.categories.violence.ToString();
                        self_harm.text = "Self Harm:"+result.categories.self_harm.ToString();
                        sexual_minors.text = "Sexual Minors:"+result.categories.sexual_minors.ToString();
                        hate_threatening.text = "Hate Threatening:"+result.categories.hate_threatening.ToString();
                        violence_graphic.text = "Violence Graphic:"+result.categories.violence_graphic.ToString();
                        Debug.Log("Category Scores: Sexual=" + result.category_scores.sexual +
                            ", Hate=" + result.category_scores.hate +
                            ", Violence=" + result.category_scores.violence +
                            ", Self-harm=" + result.category_scores.self_harm +
                            ", Sexual/Minors=" + result.category_scores.sexual_minors +
                            ", Hate/Threatening=" + result.category_scores.hate_threatening +
                            ", Violence/Graphic=" + result.category_scores.violence_graphic);

                        // Update your Unity application based on the moderation result
                    }
                }
            };
        }

        // Class to represent the content moderation response data
        [System.Serializable]
        public class ContentModerationData
        {
            public string id;
            public string model;
            public Result[] results;
        }

        [System.Serializable]
        public class Result
        {
            public bool flagged;
            public Categories categories;
            public CategoryScores category_scores;
        }

        [System.Serializable]
        public class Categories
        {
            public bool sexual;
            public bool hate;
            public bool violence;
            public bool self_harm;
            public bool sexual_minors;
            public bool hate_threatening;
            public bool violence_graphic;
        }

        [System.Serializable]
        public class CategoryScores
        {
            public float sexual;
            public float hate;
            public float violence;
            public float self_harm;
            public float sexual_minors;
            public float hate_threatening;
            public float violence_graphic;
        }
        public class RequestData
        {
            
            public string input;
        }
    }
}
