using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using TMPro; // Adjust this based on your render pipeline

public class MainMenuScript : MonoBehaviour
{
    public bool isSameScene;
    public string firstScene;
    public GameObject title;
    public GameObject background; // Reference to the background GameObject with Image component
    public float parallaxEffect = 0.2f; // Parallax effect multiplier
    public TextMeshProUGUI tipText;
    public AudioSource audioSource;
    public AudioClip selectSound;
    public Button startButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button backButtonoptions;
    public Button backButtoncredits;

    public ParticleSystem backgroundParticles; // Particle system for background effects
    public Volume globalVolume; // Reference to the global Volume component
    public float normalBloomIntensity = 0.2f; // Normal bloom intensity
    public float hoverBloomIntensity = 1.5f; // Bloom intensity on button hover
    private bool start;
    private Vector3 backgroundStartPos;
    private float backgroundWidth;
    private Vector3 originalTitlePos;
    private float wiggleStrength = 17f; // Strength of the title wiggle animation
    private float wiggleSpeed = 10f; // Speed of the title wiggle animation

    private string[] tips = {
        "Tip 1: Explore every corner.",
        "Tip 2: Use your items wisely.",
        "Tip 3: Learn enemy patterns.",
        "Tip 4: Don't rush, plan your moves."
    };
    public Camera mainCamera; // Referência à câmera principal


    void Start()
{
    start = false;
    DisplayRandomTip();
    PlayBackgroundMusic();
    SetupButtonAnimations(startButton);
    SetupButtonAnimations(optionsButton);
    SetupButtonAnimations(creditsButton);
    SetupButtonAnimations(backButtonoptions);
    SetupButtonAnimations(backButtoncredits); // Adicionar
    startButton.onClick.AddListener(OnStartButtonClick);
    optionsButton.onClick.AddListener(OnOptionsButtonClick);
    creditsButton.onClick.AddListener(OnCreditsButtonClick);
    backButtonoptions.onClick.AddListener(OnBackToMainMenuClick); // Listener para voltar ao menu principal
    backButtoncredits.onClick.AddListener(OnBackToMainMenuClick);

    if (background != null)
    {
        backgroundStartPos = background.transform.position;
        backgroundWidth = background.GetComponent<RectTransform>().rect.width;
    }

    if (title != null)
    {
        originalTitlePos = title.transform.position;
        AnimateTitle(); // Chama a animação do título
    }

    // Inicializa a referência da câmera principal
    mainCamera = Camera.main;
}

    void Update()
    {
        UpdateBackgroundParallax();
        UpdateTitleWiggle();
    }

    void DisplayRandomTip()
    {
        if (tipText != null && tips.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, tips.Length);
            tipText.text = tips[randomIndex];
        }
    }

    void PlayBackgroundMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void OnStartButtonClick()
    {
        if (!start)
        {
            audioSource.PlayOneShot(selectSound);
            start = true;

            if (isSameScene)
            {
                if (GameManager.GetInstance().state == GameState.PreStart)
                {
                    GameManager.GetInstance().ChangeState(GameState.Starting);
                    gameObject.SetActive(false); // Hide the main menu UI
                }
            }
            else
            {
                SceneManager.LoadScene(firstScene);
            }
        }
    }

    void OnOptionsButtonClick()
    {
        // Move a câmera para cima (implemente de acordo com sua lógica)
        MoveCamera(Vector3.up * 600f); // Exemplo: move 5 unidades para cima
    }

    void OnCreditsButtonClick()
    {
        // Move a câmera para baixo (implemente de acordo com sua lógica)
        MoveCamera(Vector3.down * 600f); // Exemplo: move 5 unidades para baixo
    }

    void MoveCamera(Vector3 direction)
    {
        if (mainCamera != null)
        {
            Vector3 targetPosition = mainCamera.transform.position + direction;
            LeanTween.move(mainCamera.gameObject, targetPosition, 0.5f).setEaseInOutSine();
        }
    }
    void SetupButtonAnimations(Button button)
    {
        // Configura a cor de destaque do botão
        ColorBlock colorBlock = button.colors;
        colorBlock.highlightedColor = Color.yellow;
        button.colors = colorBlock;

        // Adiciona um componente EventTrigger se ainda não foi adicionado
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

        // Evento de entrada do ponteiro
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { OnPointerEnter(button); });
        trigger.triggers.Add(entryEnter);

        // Evento de saída do ponteiro
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { OnPointerExit(button); });
        trigger.triggers.Add(entryExit);

        // Evento de clique
        button.onClick.AddListener(() =>
        {
            LeanTween.moveLocalX(button.gameObject, button.transform.localPosition.x + 10f, 0.2f).setEaseInOutSine();
            
            // Verifica qual botão foi clicado
            if (button == startButton)
            {
                OnStartButtonClick();
            }
            else if (button == optionsButton)
            {
                OnOptionsButtonClick();
            }
            else if (button == creditsButton)
            {
                OnCreditsButtonClick();
            }
            else if (button == backButtonoptions) // Adicione a verificação para o botão de voltar
            {
                OnBackToMainMenuClick();
            }
            else if (button == backButtoncredits)
            {
                OnBackToMainMenuClick();
            }
        });
    }

    void OnPointerEnter(Button button)
    {
        LeanTween.scale(button.gameObject, new Vector3(1.1f, 1.1f, 1), 0.2f).setEaseInOutSine();

        // Increase bloom intensity
        SetBloomIntensity(hoverBloomIntensity);
    }

    void OnPointerExit(Button button)
    {
        LeanTween.scale(button.gameObject, new Vector3(1f, 1f, 1), 0.2f).setEaseInOutSine();

        // Reset bloom intensity
        SetBloomIntensity(normalBloomIntensity);
    }

    void SetBloomIntensity(float intensity)
    {
        if (globalVolume != null)
        {
            Bloom bloom;
            if (globalVolume.profile.TryGet(out bloom))
            {
                bloom.intensity.Override(intensity);
            }
        }
    }

    void UpdateBackgroundParallax()
    {
        if (background != null)
        {
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            // Calculate target positions for horizontal and vertical parallax
            float targetPosX = backgroundStartPos.x + (mouseX / Screen.width - 0.5f) * 2f * parallaxEffect * backgroundWidth;
            float targetPosY = backgroundStartPos.y + (mouseY / Screen.height - 0.5f) * 2f * parallaxEffect * background.GetComponent<RectTransform>().rect.height;

            // Smoothly interpolate towards the target positions
            float smoothTime = 0.005f; // Adjust this value to control the smoothness (lower value for quicker response)
            float smoothPosX = Mathf.Lerp(background.transform.position.x, targetPosX, smoothTime);
            float smoothPosY = Mathf.Lerp(background.transform.position.y, targetPosY, smoothTime);

            background.transform.position = new Vector3(smoothPosX, smoothPosY, backgroundStartPos.z);

            // Update particle system rotation based on mouse position
            if (backgroundParticles != null)
            {
                var main = backgroundParticles.main;
                main.startRotationZ = Mathf.Lerp(-0.2f, 0.2f, mouseX / Screen.width);
            }

            // Clamp background position to prevent it from going too far
            float clampedX = Mathf.Clamp(background.transform.position.x, backgroundStartPos.x - backgroundWidth, backgroundStartPos.x + backgroundWidth);
            float clampedY = Mathf.Clamp(background.transform.position.y, backgroundStartPos.y - background.GetComponent<RectTransform>().rect.height, backgroundStartPos.y + background.GetComponent<RectTransform>().rect.height);
            background.transform.position = new Vector3(clampedX, clampedY, backgroundStartPos.z);

            // Update background start position if it goes beyond bounds
            if (background.transform.position.x > backgroundStartPos.x + backgroundWidth)
            {
                backgroundStartPos.x += backgroundWidth;
            }
            else if (background.transform.position.x < backgroundStartPos.x - backgroundWidth)
            {
                backgroundStartPos.x -= backgroundWidth;
            }
        }
    }


    void AnimateTitle()
    {
        if (title != null)
        {
            Vector3 targetPosition = originalTitlePos + Vector3.up * wiggleStrength;

            LeanTween.move(title, targetPosition, wiggleSpeed)
                .setEaseShake()
                .setLoopPingPong();
        }
    }
    void UpdateTitleWiggle()
    {
        // You can add additional title animations or effects here if needed
    }
    void OnBackToMainMenuClick()
    {
        if (mainCamera != null)
        {
            // Mova a câmera de volta para a posição inicial (menu principal)
            LeanTween.move(mainCamera.gameObject, Vector3.zero, 0.5f).setEaseInOutSine();
        }
    }
}
