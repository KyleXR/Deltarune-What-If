using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueVisualController : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject typingSound;
    public int soundPerCharacter;
    public Animator animator;
    public List<char> pauseCharacters;
    public bool hasPortrait = true;
    public bool hasName = true;
    public float textSpeed;

    private Queue<string> sentences;
    private Queue<Sprite> portraits;
    private bool detectInput = false;
    private string currentSentence;
    private bool typing = false;

    // New variables for shake effect
    [Header("Shake Variables")]
    //private bool isShaking = false;
    public float defaultShakeAmount = 1f;
    public float defaultShakeSpeed = 10f;
    private string shakeStartMarker = "<shake>";
    private string shakeEndMarker = "</shake>";
    private List<int> shakingIndexes = new List<int>();
    private List<Vector3[]> originalVertexPositions = new List<Vector3[]>();

    [Header("Wave Variables")]
    //private bool isWaving = false;
    public float defaultWaveAmplitude = 1f;
    public float defaultWaveSpeed = 10f;
    private string waveStartMarker = "<wave>";
    private string waveEndMarker = "</wave>";
    private List<int> wavingIndexes = new List<int>();

    public event Action<bool> dialogueEnd;
    public event Action<bool> YoinkSpamton;

    [SerializeField] GameObject spamtonNeo;

    private GameObject[] typingSounds;
    private int sentenceID = 0;

    void Start()
    {
        sentences = new Queue<string>();
        portraits = new Queue<Sprite>();
        hasPortrait = portrait != null;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (detectInput)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !typing) DisplayNextSentence();
            if (Input.GetKey(KeyCode.X)) DisplayFullSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        animator.SetBool("IsOpen", true);
        sentences.Clear();
        var player = FindAnyObjectByType<FirstPersonController>();
        if (player != null) { player.enabled = false; }
        if (hasName) nameText.text = dialogue.name;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        foreach (Sprite portrait in dialogue.portraitSprite)
        {
            portraits.Enqueue(portrait);
        }
        typingSounds = dialogue.txtSounds;
        sentenceID = -1;
        if (typingSounds == null || typingSounds.Length < sentences.Count)
        {
            typingSounds = new GameObject[sentences.Count];
            for (int i = 0; i < typingSounds.Length; i++) typingSounds[i] = typingSound;
        }
        DisplayNextSentence();
        detectInput = true;
    }
    public void DisplayNextSentence()
    {
        sentenceID++;
        dialogueText.ClearMesh();

        originalVertexPositions.Clear();
        //isShaking = false;
        //isWaving = false;
        if (sentences.Count == 1)
        {
            if(spamtonNeo.active == true)
            {
                YoinkSpamton.Invoke(true);
            }
        }
        if (sentences.Count <= 0)
        {
            
            Invoke("EndDialogue", 0.1f);
            return;
        }
        string sentence = sentences.Dequeue();
        currentSentence = sentence; // Store the original sentence

        if (portraits.Count > 0)
        {
            Sprite portraitSprite = portraits.Dequeue();
            if (portraitSprite != null) portrait.sprite = portraitSprite;
        }

        StopAllCoroutines();
        FindShakingIndexes(RemoveWaveMarkers(RemoveColorMarkers(currentSentence)));
        FindWavingIndexes(RemoveShakeMarkers(RemoveColorMarkers(currentSentence)));

        StartCoroutine(TypeSentence(RemoveAllMarkers(currentSentence), textSpeed)); // Use the original sentence for shake detection
        if (shakingIndexes.Count > 0) StartCoroutine(ShakeTextCoroutine(defaultShakeAmount, defaultShakeSpeed));
        if (wavingIndexes.Count > 0) StartCoroutine(WaveTextCoroutine(defaultWaveAmplitude, defaultWaveSpeed));
        typing = true;
    }

    private void StoreOriginalVertexPositions()
    {
        // Clear the list of original vertex positions
        //originalVertexPositions.Clear();

        // Force a mesh update to ensure text info is up-to-date
        dialogueText.ForceMeshUpdate();

        // Check if textInfo is null or characterCount is 0
        if (dialogueText.textInfo == null || dialogueText.textInfo.characterCount == 0)
        {
            //Debug.LogWarning("Dialogue text info is null or character count is 0. Cannot store original vertex positions.");
            return;
        }

        // Get the index of the last character
        int lastIndex = dialogueText.text.Length - 1;

        // Check if lastIndex is within valid range
        if (lastIndex < 0 || lastIndex >= dialogueText.textInfo.characterCount)
        {
            //Debug.LogWarning("Invalid lastIndex. Cannot store original vertex positions.");
            return;
        }

        // Get the index of the last quad
        int lastQuadIndex = dialogueText.textInfo.characterInfo[lastIndex].vertexIndex / 4;

        // Check if lastQuadIndex is within valid range
        if (lastQuadIndex < 0 || lastQuadIndex >= dialogueText.textInfo.meshInfo[0].vertices.Length / 4)
        {
            //Debug.LogWarning("Invalid lastQuadIndex. Cannot store original vertex positions.");
            return;
        }

        // Get the positions of the vertices of the last quad
        Vector3[] vertices = dialogueText.textInfo.meshInfo[0].vertices;

        // Store the original positions of the vertices for the last quad
        Vector3[] originalVertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            int vertexIndex = (lastQuadIndex * 4) + i;
            originalVertices[i] = vertices[vertexIndex];
        }

        // Add the original positions to the global list
        originalVertexPositions.Add(originalVertices);
    }

    private void StoreOriginalVertexPositionsAllAtOnce()
    {
        // Clear the list of original vertex positions
        originalVertexPositions.Clear();

        // Force a mesh update to ensure text info is up-to-date
        dialogueText.ForceMeshUpdate();

        // Check if textInfo is null or characterCount is 0
        if (dialogueText.textInfo == null || dialogueText.textInfo.characterCount == 0)
        {
            //Debug.LogWarning("Dialogue text info is null or character count is 0. Cannot store original vertex positions.");
            return;
        }

        // Get the positions of all characters in the text
        Vector3[] vertices = dialogueText.textInfo.meshInfo[0].vertices;
        int characterCount = dialogueText.textInfo.characterCount;
        for (int i = 0; i < characterCount; i++)
        {
            // Get the index of the vertex for the current character
            int vertexIndex = dialogueText.textInfo.characterInfo[i].vertexIndex;

            // Get the positions of the vertices for the current character's quad
            Vector3[] originalVertices = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                originalVertices[j] = vertices[vertexIndex + j];
            }

            // Add the original positions to the global list
            originalVertexPositions.Add(originalVertices);
        }
    }
    IEnumerator TypeSentence(string sentenceWithOutMarkers, float speed)
    {
        dialogueText.text = sentenceWithOutMarkers;
        dialogueText.ForceMeshUpdate();
        StoreOriginalVertexPositionsAllAtOnce();

        sentenceWithOutMarkers = RemoveColorMarkers(sentenceWithOutMarkers);

        // Get the total number of characters
        int totalCharacters = sentenceWithOutMarkers.Length;

        // Set all characters to transparent
        Color32[] originalColors = dialogueText.textInfo.meshInfo[0].colors32;
        Color32[] transparentColors = new Color32[originalColors.Length];
        for (int i = 0; i < transparentColors.Length; i++)
        {
            transparentColors[i] = new Color32(originalColors[i].r, originalColors[i].g, originalColors[i].b, 0);
        }
        dialogueText.textInfo.meshInfo[0].colors32 = transparentColors;
        dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // Split the sentence into characters
        var charArray = sentenceWithOutMarkers.ToCharArray();
        StringBuilder typedSentence = new StringBuilder();

        //var currentSound = Instantiate(typingSound);
        
        var currentSound = Instantiate(typingSounds[sentenceID]);


        typing = true;
        // Iterate over each character
        for (int i = 0; i < totalCharacters; i++)
        {
            typedSentence.Append(charArray[i]);

            // Update colors to gradually reveal characters
            Color32[] colors = dialogueText.textInfo.meshInfo[0].colors32;
            int charIndex = typedSentence.Length - 1; // Index of the last character
            int vertexIndex = dialogueText.textInfo.characterInfo[charIndex].vertexIndex;

            for (int j = 0; j < 4; j++)
            {
                int index = vertexIndex + j;
                if (index < colors.Length) // Ensure we don't go out of bounds
                {
                    colors[index].a = 255; // Set alpha to 255 to reveal character
                }
            }
            dialogueText.textInfo.meshInfo[0].colors32 = colors;
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // Play typing sound every soundPerCharacter and when it is not a pause character
            if (i % soundPerCharacter == 0 && !pauseCharacters.Contains(sentenceWithOutMarkers[i]) && typingSound != null)
            {
                Destroy(currentSound);
                currentSound = Instantiate(typingSounds[sentenceID]);
            }
            //if (pauseCharacters.Contains(sentenceWithOutMarkers[i])) Debug.Log(sentenceWithOutMarkers[i]);
            float tempSpeed = pauseCharacters.Contains(sentenceWithOutMarkers[i]) ? speed * 10 : speed;
            yield return new WaitForSeconds(tempSpeed);
        }
        typing = false;
    }

    private string ExtractMarker(string input, int id)
    {
        string output = "";
        for (int i = id; i < input.Length; i++)
        {
            if (input[i] == '>') return output + ">";
            else output += input[i];
        }
        return output;
    }

    private void FindShakingIndexes(string sentence)
    {
        // Clear the list of shaking indexes before populating it again
        shakingIndexes.Clear();

        int startIndex = 0;
        while (startIndex < sentence.Length)
        {
            // Find the start index of the next shake marker
            int shakeStartIndex = sentence.IndexOf(shakeStartMarker, startIndex);
            if (shakeStartIndex == -1)
                break; // No more shake markers found

            // Find the end index of the shake marker
            int shakeEndIndex = sentence.IndexOf(shakeEndMarker, shakeStartIndex);
            if (shakeEndIndex == -1)
                break; // No end marker found

            // Add the indexes of characters between the markers
            for (int i = shakeStartIndex + shakeStartMarker.Length; i < shakeEndIndex; i++)
            {
                // Ensure the index is within the length of the sentence
                if (i < sentence.Length)
                {
                    // Add only the indexes of characters within the <shake></shake> tags
                    shakingIndexes.Add(i - shakeStartMarker.Length);
                }
            }

            // Move the start index to the end of the current shake marker
            startIndex = shakeEndIndex + shakeEndMarker.Length;
        }
    }

    private IEnumerator ShakeTextCoroutine(float shakeAmount, float shakeSpeed)
    {
        while (true)
        {
            // Get the positions of the vertices of all characters
            Vector3[] vertices = dialogueText.textInfo.meshInfo[0].vertices;

            // Iterate through each index in shakingIndexes and apply shake effect
            foreach (int index in shakingIndexes)
            {
                // Check if the index is within the valid range
                if (index >= 0 && index < dialogueText.textInfo.characterCount)
                {
                    // Calculate the vertex index for the current character
                    int vertexIndex = dialogueText.textInfo.characterInfo[index].vertexIndex;

                    // Calculate the midpoint position of the character using original vertex positions
                    Vector3 originalMidpoint = (originalVertexPositions[index][0] + originalVertexPositions[index][2]) / 2f;

                    // Use Perlin noise to generate shake offsets
                    float noiseX = Mathf.PerlinNoise(Time.time * shakeSpeed + index, 0) * 2 - 1;
                    float noiseY = Mathf.PerlinNoise(0, Time.time * shakeSpeed + index) * 2 - 1;

                    // Apply the shake effect to the character's midpoint position relative to its original position
                    Vector3 shakeOffset = new Vector3(noiseX, noiseY, 0f) * shakeAmount;
                    vertices[vertexIndex] = originalVertexPositions[index][0] + shakeOffset;
                    vertices[vertexIndex + 1] = originalVertexPositions[index][1] + shakeOffset;
                    vertices[vertexIndex + 2] = originalVertexPositions[index][2] + shakeOffset;
                    vertices[vertexIndex + 3] = originalVertexPositions[index][3] + shakeOffset;
                }
                else
                {
                    // Index is out of range, log a warning
                    //Debug.LogWarning("Shaking index out of range: " + index);
                }
            }

            // Update the vertex data to apply the changes
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

            // Wait for the end of the frame before continuing
            yield return null;
        }
    }

    private IEnumerator WaveTextCoroutine(float waveAmplitude, float waveFrequency)
    {
        float timeOffset = 0f;
        while (true)
        {
            // Get the positions of the vertices of all characters
            Vector3[] vertices = dialogueText.textInfo.meshInfo[0].vertices;

            // Increment the time offset each frame
            timeOffset += Time.deltaTime;

            // Loop through all characters in the text
            for (int i = 0; i < dialogueText.textInfo.characterCount; i++)
            {
                if (wavingIndexes.Contains(i))
                {
                    // Calculate the wave offset once per frame
                    float waveOffset = Mathf.Sin((timeOffset * waveFrequency) + i) * waveAmplitude;

                    TMP_CharacterInfo charInfo = dialogueText.textInfo.characterInfo[i];

                    // Check if character is visible and has valid vertex data
                    if (charInfo.isVisible && charInfo.vertexIndex >= 0 && charInfo.vertexIndex + 3 < vertices.Length)
                    {
                        // Apply the wave offset to the y-coordinate of each vertex of the character's quad
                        for (int j = 0; j < 4; j++)
                        {
                            int vertexOffset = charInfo.vertexIndex + j;

                            // Retrieve the original Y position of the vertex from the stored originalVertexPositions
                            Vector3 originalPosition = Vector3.zero;
                            if (i + 3 < originalVertexPositions.Count)
                            {
                                originalPosition = originalVertexPositions[i][j];
                            }

                            // Apply the wave offset to the original Y position
                            vertices[vertexOffset].y = originalPosition.y + waveOffset;
                        }
                    }
                }
            }

            // Update vertex data to apply the changes
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

            // Wait for the end of the frame before continuing
            yield return null;
        }
    }



    private void FindWavingIndexes(string sentence)
    {
        // Clear the list of waving indexes before populating it again
        wavingIndexes.Clear();

        int startIndex = 0;
        while (startIndex < sentence.Length)
        {
            // Find the start index of the next wave marker
            int waveStartIndex = sentence.IndexOf(waveStartMarker, startIndex);
            if (waveStartIndex == -1)
                break; // No more wave markers found

            // Find the end index of the wave marker
            int waveEndIndex = sentence.IndexOf(waveEndMarker, waveStartIndex);
            if (waveEndIndex == -1)
                break; // No end marker found

            // Add the indexes of characters between the markers
            for (int i = waveStartIndex + waveStartMarker.Length; i < waveEndIndex; i++)
            {
                // Ensure the index is within the length of the sentence
                if (i < sentence.Length)
                {
                    // Add only the indexes of characters within the <wave></wave> tags
                    wavingIndexes.Add(i - waveStartMarker.Length);
                }
            }

            // Move the start index to the end of the current wave marker
            startIndex = waveEndIndex + waveEndMarker.Length;
        }
    }

    private string RemoveShakeMarkers(string sentence)
    {
        // Remove shake markers from the sentence
        sentence = sentence.Replace(shakeStartMarker, "");
        sentence = sentence.Replace(shakeEndMarker, "");

        return sentence;
    }
    private string RemoveWaveMarkers(string sentence)
    {
        sentence = sentence.Replace(waveStartMarker, "");
        sentence = sentence.Replace(waveEndMarker, "");
        
        return sentence;
    }
    public static string RemoveColorMarkers(string input)
    {
        // Use regular expression to remove color markers
        return Regex.Replace(input, @"<color=.*?>|<\/color>", "");
    }
    private string RemoveAllMarkers(string sentence)
    {
        sentence = RemoveShakeMarkers(sentence);
        sentence = RemoveWaveMarkers(sentence);
        return sentence;
    }
    public void DisplayFullSentence()
    {
        StopAllCoroutines();
        Color32[] finalColors = dialogueText.textInfo.meshInfo[0].colors32;
        for (int j = 0; j < finalColors.Length; j++)
        {
            finalColors[j].a = 255;
        }
        dialogueText.textInfo.meshInfo[0].colors32 = finalColors;
        dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        if (shakingIndexes.Count > 0) StartCoroutine(ShakeTextCoroutine(defaultShakeAmount, defaultShakeSpeed));
        if (wavingIndexes.Count > 0) StartCoroutine(WaveTextCoroutine(defaultWaveAmplitude, defaultWaveSpeed));
        typing = false;
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        wavingIndexes.Clear();
        shakingIndexes.Clear();
        originalVertexPositions.Clear();
        animator.SetBool("IsOpen", false);
        var player = FindAnyObjectByType<FirstPersonController>();
        if (player != null) { player.enabled = true; }
        if (spamtonNeo.active == true)
        {
            dialogueEnd.Invoke(true);
        }
        detectInput = false;
        gameObject.SetActive(false);

    }
}