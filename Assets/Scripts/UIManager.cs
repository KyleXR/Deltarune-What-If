using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Spell UI Variables
    [Header("Spell Variables")]
    [SerializeField] TargetingLogic spell;
    [SerializeField] TMP_Text spellText;

    [Header("Spamton Health Variables")]
    [SerializeField] Health spamHealth;
    [SerializeField] Slider spamHealthBar;

    // Health UI Variables
    [Header("Player Health Variables")]
    [SerializeField] Health health;
    [SerializeField] Slider healthBar;
    [SerializeField] Image[] healthNumbers;
    [SerializeField] Sprite[] healthSprites;

    public int healthTens;
    public int healthOnes;

    // TP UI Variables
    [Header("TP Meter Variables")]
    [SerializeField] TensionPoints tension;
    [SerializeField] Slider TP_Bar;
    [SerializeField] Image SliderImage;
    [SerializeField] Image[] TPNumbers;
    [SerializeField] Sprite[] TPNumSprites;
    [SerializeField] Image percentSign;
    [SerializeField] Image maxSign;
    [SerializeField] Color[] tpBarColors;

    public int TPTens;
    public int TPOnes;

    // This is a Temp variable until TP is incorperated into the player scripts
    public float tpPercent = 0f;

    void Start()
    {
        if (tension == null) tension = FindFirstObjectByType<TensionPoints>();
        healthBar.value = health.currentHealth / health.maxHealth;
        spamHealthBar.value = spamHealth.currentHealth / spamHealth.maxHealth;
        SetHealthUI();
        SetSpamNEOUI();
        SetCurrentSpell();
        SetTPUI();
    }

    // Update is called once per frame
    void Update()
    {
        SetHealthUI();
        SetSpamNEOUI(); 
        SetCurrentSpell();
        SetTPUI();
    }

    public void SetSpamNEOUI()
    {
        spamHealth.currentHealth = Mathf.Clamp(spamHealth.currentHealth, 0, spamHealth.maxHealth);

        spamHealthBar.value = spamHealth.currentHealth / spamHealth.maxHealth;
    }

    public void SetHealthUI()
    {
        health.currentHealth = Mathf.Clamp(health.currentHealth, 0, health.maxHealth);

        healthTens = (int)(health.currentHealth / 10);
        healthOnes = (int)(health.currentHealth - (healthTens * 10));

        healthNumbers[0].color = healthTens <= 0 ? Color.black : Color.white;

        healthNumbers[0].sprite = healthSprites[healthTens];
        healthNumbers[1].sprite = healthSprites[healthOnes];

        healthBar.value = health.currentHealth / health.maxHealth;

    }

    public void SetTPUI()
    {
        tpPercent = tension.tensionPoints;
        tpPercent = Mathf.Clamp(tpPercent, 0, 100);


        TPTens = (int)(tpPercent / 10);
        TPOnes = (int)(tpPercent - (TPTens * 10));


        if (tpPercent >= 100)
        {
            SliderImage.color = tpBarColors[1];
            SetAlpha(0f, 1f);
        }
        else
        {
            SliderImage.color = tpBarColors[0];
            SetAlpha(1f, 0f);
        }

        if(tpPercent < 100)
        {
            TPNumbers[0].sprite = (tpPercent < 10 ? TPNumSprites[TPOnes] : TPNumSprites[TPTens]);
            TPNumbers[1].sprite = TPNumSprites[TPOnes];
        }

        

        TP_Bar.value = tpPercent / 100;

    }

    void SetAlpha(float tpAlpha, float maxAlpha)
    {
        for (int i = 0; i < 2; i++)
        {
            if(tpPercent >= 10)
            {
                Color tempColor = TPNumbers[i].color;
                tempColor.a = tpAlpha;
                TPNumbers[i].color = tempColor;
            }
            else
            {
                Color temp = TPNumbers[1].color;
                temp.a = 0f;
                TPNumbers[1].color = temp;
            }
        }

        // Set alpha for percentSign
        Color percentColor = percentSign.color;
        percentColor.a = tpAlpha;
        percentSign.color = percentColor;

        // Set alpha for maxSign
        Color maxColor = maxSign.color;
        maxColor.a = maxAlpha;
        maxSign.color = maxColor;
    }

    public void SetCurrentSpell()
    {
        spellText.text = spell.selectedSpell.ToString();
    }
}
