using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OpenAI
{
    public class DallE : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Text ChemicalText;
        [SerializeField] private Text procedureText;
        [SerializeField] private Text listText;
        [SerializeField] private Button button;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button listnextButton;
        [SerializeField] private Image image;
        [SerializeField] private GameObject loadingLabel;
        [SerializeField] private GameObject procedureCanvas;
        [SerializeField] private GameObject listCanvas;
        [SerializeField] private GameObject mainCanvas;

        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private List<ChatMessage> messages1 = new List<ChatMessage>();
        private List<ChatMessage> messages2 = new List<ChatMessage>();
        private string prompt = "Randomly select one simple chemical compound which can be solved by students. Don't repeat the compound.";
        private string chemicalname;


        private void Start()
        {
            button.onClick.AddListener(SendReply);
            nextButton.onClick.AddListener(SendProcedure);
            listnextButton.onClick.AddListener(SendList);
        }

        private async void SendImageRequest(string message)
        {
            image.sprite = null;
            button.enabled = false;
            inputField.enabled = false;
            loadingLabel.SetActive(true);

            var response = await openai.CreateImage(new CreateImageRequest
            {
                Prompt = message,
                Size = ImageSize.Size256
            });

            if (response.Data != null && response.Data.Count > 0)
            {
                using(var request = new UnityWebRequest(response.Data[0].Url))
                {
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Access-Control-Allow-Origin", "*");
                    request.SendWebRequest();

                    while (!request.isDone) await Task.Yield();

                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(request.downloadHandler.data);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero, 1f);
                    image.sprite = sprite;
                }
            }
            else
            {
                Debug.LogWarning("No image was created from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
            loadingLabel.SetActive(false);
        }
            private async void SendReply()
            {
                var newMessage = new ChatMessage()
                {
                    Role = "user",
                    Content = inputField.text
                };


                if (messages.Count == 0) newMessage.Content = prompt;
                    messages.Add(newMessage);


                // Complete the instruction
                var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
                    {
                        Model = "gpt-3.5-turbo-0301",
                        Messages = messages
                    });
                    
                if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
                    {

                        var message = completionResponse.Choices[0].Message;
                        message.Content = message.Content.Trim();
                        ChemicalText.text = message.Content;
                        chemicalname = message.Content;
                        SendImageRequest(message.Content);
                    }
                    else
                    {
                        Debug.LogWarning("No text was generated from this prompt.");
                    }
            }
        private async void SendProcedure()
        {
            procedureCanvas.SetActive(true);
            string procedureprompt = "Write procedure to prepare" + chemicalname + "in 50 words as bullet points";
            var newMessage1 = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };


            if (messages1.Count == 0) newMessage1.Content = procedureprompt;
            messages1.Add(newMessage1);


            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0301",
                Messages = messages1
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {

                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                procedureText.text = message.Content;
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }
                private async void SendList()
                {
                    procedureCanvas.SetActive(false);
                    listCanvas.SetActive(true);
                    string listprompt = "Write list of chemical items needed to prepare" + chemicalname + "in bullet points";
                    var newMessage2 = new ChatMessage()
                    {
                        Role = "user",
                        Content = inputField.text
                    };


                    if (messages2.Count == 0) newMessage2.Content = listprompt;
                    messages2.Add(newMessage2);


                    // Complete the instruction
                    var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
                    {
                        Model = "gpt-3.5-turbo-0301",
                        Messages = messages2
                    });

                    if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
                    {

                        var message = completionResponse.Choices[0].Message;
                        message.Content = message.Content.Trim();
                        listText.text = message.Content;
                    }
                    else
                    {
                        Debug.LogWarning("No text was generated from this prompt.");
                    }
                }
        

    }
            
}


