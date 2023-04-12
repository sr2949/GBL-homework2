using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace OpenAI
{
    public class DallE : MonoBehaviour
    {
       
        [SerializeField] private Text ChemicalText;
        [SerializeField] private Text procedureText;
        //[SerializeField] private Text EquationText;
        [SerializeField] private Text listText;
        [SerializeField] private Text correctText;
        [SerializeField] private Button button;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button listnextButton;
        [SerializeField] private Button procedurenextButton;
        [SerializeField] private Button nextchemicalButton;
        [SerializeField] private Button checkButton;
        [SerializeField] private Image image;
        [SerializeField] private GameObject loadingLabel;
        [SerializeField] private GameObject procedureCanvas;
        [SerializeField] private GameObject listCanvas;
        [SerializeField] private GameObject mainCanvas;
        [SerializeField] private GameObject PlayCanvas;
        [SerializeField] private GameObject selectCanvas;
        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private GameObject equationCanvas;
        [SerializeField] private GameObject ModerationCanvas;

        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private List<ChatMessage> messages1 = new List<ChatMessage>();
        private List<ChatMessage> messages2 = new List<ChatMessage>();
        private List<ChatMessage> messages3 = new List<ChatMessage>();
        private string prompt = "For a Quiz Randomly select one simple chemical compound which can be solved by students. Don't repeat the compound as it is quiz.Write only name of the compound.";
        private string chemicalname;
        public Dropdown dropdownOption1;
        public Dropdown dropdownOption2;
        public Dropdown dropdownOption3;
        public Dropdown dropdownOption4;
        private string correctAnswer;
        private int optionsCount;


        private void Start()
        {
            PlayCanvas.SetActive(true);
            button.onClick.AddListener(SendReply);
            nextButton.onClick.AddListener(SendList);
            listnextButton.onClick.AddListener(SendProcedure);
            procedurenextButton.onClick.AddListener(SendEquation);
            nextchemicalButton.onClick.AddListener(startGame);
            checkButton.onClick.AddListener(OnSubmitButtonClicked);
        }
        public void startGame()
        {
            PlayCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            listCanvas.SetActive(false);
            procedureCanvas.SetActive(false);
            equationCanvas.SetActive(false);
            selectCanvas.SetActive(true);
            ModerationCanvas.SetActive(false);
        }
        public void switchtoModeration()
        {
            PlayCanvas.SetActive(false);
            mainCanvas.SetActive(false);
            listCanvas.SetActive(false);
            procedureCanvas.SetActive(false);
            equationCanvas.SetActive(false);
            selectCanvas.SetActive(false);
            loadingCanvas.SetActive(false);
            ModerationCanvas.SetActive(true);
        }

        private async void SendImageRequest(string message)
        {
            image.sprite = null;
            button.enabled = false;
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
            loadingLabel.SetActive(false);
            loadingCanvas.SetActive(false);
            mainCanvas.SetActive(true);
        }
            private async void SendReply()
            {
             selectCanvas.SetActive(false);
             loadingCanvas.SetActive(true);
            var newMessage = new ChatMessage()
                {
                    Role = "user",
                    Content = prompt
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
            listCanvas.SetActive(false);
            loadingCanvas.SetActive(true);
            string procedureprompt = "Write procedure to prepare" + chemicalname + "in 50 words as bullet points";
            var newMessage1 = new ChatMessage()
            {
                Role = "user",
                Content = procedureprompt
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
                procedureCanvas.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }
        private async void SendList()
        {
            procedureCanvas.SetActive(false);
            loadingCanvas.SetActive(true);
            string listprompt = "Write list of chemical items needed to prepare" + chemicalname + "in bullet points";
            var newMessage2 = new ChatMessage()
            {
                Role = "user",
                Content = listprompt
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
                listCanvas.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }
                private async void SendEquation()
                {
                    procedureCanvas.SetActive(false);
                    listCanvas.SetActive(false);
                    loadingCanvas.SetActive(true);
                    string equationprompt = "For Chemical Equation Quiz write full stoichiometric equation of" + chemicalname+ "Keep reactants on LHS and Product on RHS.write only stoichiometric equation without text.";
                    var newMessage3 = new ChatMessage()
                    {
                        Role = "user",
                        Content = equationprompt
                    };


                    if (messages3.Count == 0) newMessage3.Content = equationprompt;
                    messages3.Add(newMessage3);


                    // Complete the instruction
                    var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
                    {
                        Model = "gpt-3.5-turbo-0301",
                        Messages = messages3
                    });

                    if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
                    {

                        var message = completionResponse.Choices[0].Message;
                        message.Content = message.Content.Trim();
                            //EquationText.text = message.Content;
                            correctAnswer = message.Content;
                            loadingCanvas.SetActive(false);
                            mainCanvas.SetActive(false);
                            equationCanvas.SetActive(true);
                            List<string> options = ParseEquationToOptions(correctAnswer);
                            dropdownOption1.ClearOptions();
                            dropdownOption1.AddOptions(options);
                            dropdownOption2.ClearOptions();
                            dropdownOption2.AddOptions(options);
                            dropdownOption3.ClearOptions();
                            dropdownOption3.AddOptions(options);
                            optionsCount = options.Count;
                            if (options.Count == 4)
                            {
                                dropdownOption4.ClearOptions();
                                dropdownOption4.AddOptions(options);
                            }
                            else
                            {
                                dropdownOption4.ClearOptions();
                                dropdownOption4.interactable = false;
                                dropdownOption4.gameObject.SetActive(false);
                            }

            }
                    else
                    {
                        Debug.LogWarning("No text was generated from this prompt.");
                    }
                }
        public void OnSubmitButtonClicked()
        {
            // Get the player's selected answers from the dropdowns
            string answer1; string answer2;string answer3; string answer4;
            if (optionsCount == 4)
            {
                answer1 = dropdownOption1.options[dropdownOption1.value].text + "+" +
                                dropdownOption2.options[dropdownOption2.value].text  + "->" + " " +
                                dropdownOption3.options[dropdownOption3.value].text + "+" +
                                dropdownOption4.options[dropdownOption4.value].text;
                answer2 = dropdownOption1.options[dropdownOption1.value].text + "+" +
                                dropdownOption2.options[dropdownOption2.value].text + " " + "→" + " " +
                                dropdownOption3.options[dropdownOption3.value].text + "+" +
                                dropdownOption4.options[dropdownOption4.value].text;
                answer4 = dropdownOption3.options[dropdownOption3.value].text + "+" +
                                dropdownOption4.options[dropdownOption4.value].text + " " + "→" + " " +
                                dropdownOption1.options[dropdownOption1.value].text + "+" +
                                dropdownOption2.options[dropdownOption2.value].text;
                answer3 = dropdownOption3.options[dropdownOption4.value].text + "+" +
                                dropdownOption4.options[dropdownOption4.value].text + "->" + " " +
                                dropdownOption1.options[dropdownOption1.value].text + "+" +
                                dropdownOption1.options[dropdownOption1.value].text;
            }
            else
            {
                answer1 = dropdownOption1.options[dropdownOption1.value].text + "+" +
                                dropdownOption2.options[dropdownOption2.value].text + " " + "->" + " " +
                                dropdownOption3.options[dropdownOption3.value].text;
                answer2 = dropdownOption1.options[dropdownOption1.value].text + "+" +
                                dropdownOption2.options[dropdownOption2.value].text + " " + "→" + " " +
                                dropdownOption3.options[dropdownOption3.value].text;
                answer3 = dropdownOption3.options[dropdownOption3.value].text + " " + "→" + " " +
                            dropdownOption1.options[dropdownOption1.value].text + "+" +
                            dropdownOption2.options[dropdownOption2.value].text;
                answer4 = dropdownOption3.options[dropdownOption3.value].text + " " + "->" + " " +
                            dropdownOption1.options[dropdownOption1.value].text + "+" +
                            dropdownOption2.options[dropdownOption2.value].text;
            }


            // Compare the player's answer with the correct answer
            if (answer2 == correctAnswer || answer1 == correctAnswer || answer3 == correctAnswer || answer4 == correctAnswer)
            {
                Debug.Log(answer1+"Correct!");
                correctText.text = "Correct!!!";
            }
            else
            {
                Debug.Log(answer2+answer1+"Incorrect. The correct answer is: " + correctAnswer);
                correctText.text = "InCorrect Tryagain!!!";
            }
        }
        List<string> ParseEquationToOptions(string equation)
        {
            // Split the equation into individual elements and combine them into a list
            string[] equationParts = equation.Split(new string[] { "->", "→" }, StringSplitOptions.None);
            string reactants = equationParts[0].Trim();
            string products = equationParts[1].Trim();

            // Split the reactants and products into individual elements
            string[] reactantElements = reactants.Split('+');
            string[] productElements = products.Split('+');

            // Combine the elements into a single list of options
            List<string> options = new List<string>();
            options.AddRange(reactantElements);
            options.AddRange(productElements);
            return options;
        }

    }
            
}


