using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour 
{

#region General Variables

    #region Health/Stamina Variables
    [Header("Health/Stamina")]
	public Slider healthSlider;
	public Slider staminaSlider;
	[Header("DamageHUD")]
	public Image damageImage;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);   
	[HideInInspector] public bool damaged;
    #endregion

    #region Action Text Variables
    [HideInInspector] public bool showInteractiveText = true;
    [Header("Interactable Text-Icon")]
    public Text interactableText;
    public Image interactableIcon;
    public Sprite gamepadActionButton;
    public Sprite keyboardActionButton;
    [HideInInspector] public bool controllerInput;
    #endregion

    #region Display Controls Variables
    [Header("Controls Display")]
    public Image displayControls;
    public Sprite gamepadControls;
    public Sprite keyboardControls;
    #endregion

    #region Debug Info Variables
    [Header("Debug Window")]
    public GameObject debugPanel;
    #endregion

    #region Change Input Text Variables
    [Header("Text with FadeIn/Out")]
    public Text fadeText;
    private float textDuration, fadeDuration, durationTimer, timer;
    private Color startColor, endColor;
    private bool fade;
    #endregion

    #endregion

    private static HUDController _instance;
    public static HUDController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HUDController>();
                //Tell unity not to destroy this object when loading a new scene
                //DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    void Start()
    {
        DisableSprite();
        InitFadeText();
    }

    void Update()
    {
        FadeEffect();
        ChangeInputDisplay();
    }

    //**********************************************************************************//
    // ACTION TEXT																		//
    // show/hide text collected by FindTarget.cs and the sprite button 					//
    //**********************************************************************************//
    public void EnableSprite(string name)
	{
		showInteractiveText = true;
		if(controllerInput)
			interactableIcon.sprite = gamepadActionButton;
		else
			interactableIcon.sprite = keyboardActionButton;					
		interactableIcon.enabled = true;
		interactableText.enabled = true;
		interactableText.text = name;
	}
	
	public void DisableSprite()
	{
		showInteractiveText = false;
		interactableIcon.enabled = false;
		interactableText.enabled = false;
		interactableText.text = "";
	}

    //**********************************************************************************//
    // DISPLAY CONTROLS                                                                 //
    // show the hud with the img controls			 									//
    //**********************************************************************************//
    void ChangeInputDisplay()
	{
		#if MOBILE_INPUT
		displayControls.enabled = false;
		#else
		if(controllerInput)		
			displayControls.sprite = gamepadControls;		
		else		
			displayControls.sprite = keyboardControls;
		#endif
	}

	//**********************************************************************************//
	// SHOW CHANGE INPUT TEXT															//
	// fadeIn text, show text during 'x' time then fadeOut							    //
	//**********************************************************************************//
    void InitFadeText()
    {
        if (fadeText != null)
        {
            startColor = fadeText.color;
            endColor.a = 0f;
            fadeText.color = endColor;
        }
        else
            Debug.Log("Please assign a Text object on the field Fade Text");
    }
	
	void FadeEffect()
	{
		if(fadeText != null)
		{
			if(fade)
			{
				fadeText.color = Color.Lerp(endColor, startColor, timer);
				
				if(timer < 1)			
					timer += Time.deltaTime/fadeDuration;			
				
				if(fadeText.color.a >= 1)
				{			
					fade = false;
					timer = 0f;
				}
			}
			else
			{
				if(fadeText.color.a >= 1)
					durationTimer += Time.deltaTime;
				
				if(durationTimer >= textDuration)
				{
					fadeText.color = Color.Lerp(startColor, endColor, timer);
					if(timer < 1)			
						timer += Time.deltaTime/fadeDuration;				
				}
			}
		}
	}
	
	public void FadeText(string textToFade, float textTime, float fadeTime)
	{
		if(fadeText != null)
		{
			fadeText.text = textToFade; 	
			textDuration = textTime;	
			fadeDuration = fadeTime;
			durationTimer = 0f;
			timer = 0f;	
			fade = true;
		}
		else
			Debug.Log("Please assign a Text object on the field Fade Text");
	}
}
