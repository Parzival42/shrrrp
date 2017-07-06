using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Comment("Handles UI inserts to comment player deaths and the general win situation during the game.")]

    private static readonly string INSERT_TAG = "TextInsert";

    #region Inspector Variables
    [FancyHeader("Canvas", "Different canvas")]
    [SerializeField]
    private Canvas mainCanvas;

    [SerializeField]
    private Canvas playerLifeCanvas;

    [FancyHeader("Settings", "UI Manager Settings")]
    [SerializeField]
    private bool showIngameUI = true;

    [FancyHeader("Text inserts", "Specific inserts during gameplay")]
    [SerializeField]
    private string insertStart = "Cut!";

    [SerializeField]
    private float startFadeOut = 2f;

    [Space(5)]
    [SerializeField]
    private string insertEnd = "Cut into pieces!";

    [SerializeField]
    private float endScale = 20f;

    [SerializeField]
    private float endTweenTime = 0.2f;

    [SerializeField]
    private float endTweenDelay = 5f;

    [FancyHeader("Tween settings")]
    [SerializeField]
    private float tweenTime = 0.2f;

    [SerializeField]
    private float playerLifeFadeIn = 0.3f;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeOutBack;
    #endregion

    #region Internal Variables
    private PlayerManager playerManager;
    private Text textInsert;
    private LTDescr textInsertTween;

    private Text[] playerLifeTexts;
    private StartupEffects startupEffects;
    #endregion

    private void Start()
    {
        InitializeUI();
        InitializePlayerManager();
        InitializePlayerCanvasElements();

        playerManager.OnAllPlayersFound += FadeInPlayerLives;
        playerManager.OnPlayerDied += HandlePlayerDeath;
        playerManager.OnPlayerRespawned += HandlePlayerRespawn;
        playerManager.OnOnePlayerLeft += HandlePlayerDeath;
    }

    private void InitializePlayerCanvasElements()
    {
        playerLifeTexts = playerLifeCanvas.transform.GetComponentsInChildren<Text>();

        // Set alpha of each text to 0
        foreach (Text playerLife in playerLifeTexts)
            SetTextAlpha(playerLife, 0f);
    }

    private void InitializePlayerManager()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager == null)
            Debug.LogError("[UIManager]: No PlayerManager found!", gameObject);
        else
        {
            playerManager.OnAllPlayersDied += HandleAllPlayerDead;
            playerManager.OnOnePlayerLeft += HandleOnePlayerLeft;
            playerManager.OnPlayerDied += HandlePlayerDied;
            playerManager.OnAllPlayersFound += HandleAllPlayersFound;
        }
    }

    private void InitializeUI()
    {
        if(playerLifeCanvas == null)
            Debug.LogError("[UIManager]: No player life canvas found!.", gameObject);

        if (mainCanvas == null)
            Debug.LogError("[UIManager]: No main canvas found!.", gameObject);

        textInsert = mainCanvas.gameObject.FindComponentInChildWithTag<Text>(INSERT_TAG);
        if (textInsert == null)
            Debug.LogError("[UIManager]: No text insert object found!", gameObject);

        SetTextInsertAlpha(0f);
    }

    private void FadeInPlayerLives(Player[] players)
    {
        foreach (Player player in players)
        {
            int playerType = (int) player.PlayerType;
            SetPlayerLifeText(playerLifeTexts[playerType], player);

            LeanTween.value(0f, 1f, playerLifeFadeIn).setEase(LeanTweenType.easeInExpo)
                .setOnUpdate((float value) => {
                    SetTextAlpha(playerLifeTexts[playerType], value);
                });
        }
    }

    private void HandlePlayerDeath(Player player)
    {
        int playerType = (int)player.PlayerType;
        TweenPlayerLifeChange(playerLifeTexts[playerType]);
        LeanTween.value(1f, 0f, playerLifeFadeIn).setEase(LeanTweenType.easeInExpo)
                .setOnUpdate((float value) => {
                    SetTextAlpha(playerLifeTexts[playerType], value);
                });
    }

    private void HandlePlayerRespawn(Player player)
    {
        int playerType = (int) player.PlayerType;
        playerLifeTexts[playerType].text = player.PlayerLives.ToString();
        TweenPlayerLifeChange(playerLifeTexts[playerType]);
    }

    /// <summary>
    /// Handles start text animation.
    /// </summary>
    private void HandleAllPlayersFound(Player[] players)
    {
        if (showIngameUI)
        {
            textInsert.text = insertStart;
            textInsertTween = LeanTween.value(textInsert.gameObject, 0f, 1f, tweenTime)
                .setOnUpdate((float value) => {
                    SetTextInsertAlpha(value);
                })
                .setEase(easeType)
                .setOnComplete(() => {
                    textInsertTween = LeanTween.value(textInsert.gameObject, 1f, 0f, tweenTime)
                        .setOnUpdate((float value) => {
                            SetTextInsertAlpha(value);
                        })
                        .setDelay(startFadeOut);
                });
        }
    }

    private void HandleOnePlayerLeft(Player winner)
    {
        if (showIngameUI)
        {
            LeanTween.delayedCall(endTweenDelay, DoPlayerWinTextInsert);
        }
    }

    private void HandlePlayerDied(Player deadPlayer)
    {
        if (showIngameUI)
        {
        }
    }

    private void HandleAllPlayerDead()
    {
        if (showIngameUI)
        {
            LeanTween.delayedCall(endTweenDelay, DoPlayerWinTextInsert);
        }
    }

    private void DoPlayerWinTextInsert()
    {
        if (textInsertTween != null)
            LeanTween.cancel(textInsertTween.uniqueId);

        textInsert.text = insertEnd;
        SetTextInsertAlpha(1f);
        textInsert.rectTransform.localScale = new Vector2(endScale, endScale);
        textInsert.fontSize = 120;
        textInsert.color = Color.black;
        textInsert.rectTransform.anchoredPosition = new Vector2(-359f, textInsert.rectTransform.anchoredPosition.y);
        textInsertTween = LeanTween.scale(textInsert.rectTransform, Vector2.one, endTweenTime).setEase(LeanTweenType.easeOutBack);
    }

    private void SetTextInsertColor(Color c)
    {
        textInsert.color = c;
    }

    private void SetTextInsertAlpha(float alpha)
    {
        textInsert.color = new Color(textInsert.color.r, textInsert.color.g, textInsert.color.b, alpha);
    }

    private void SetTextAlpha(Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    private void SetPlayerLifeText(Text text, Player player)
    {
        text.text = player.PlayerLives.ToString();
    }

    private void TweenPlayerLifeChange(Text text)
    {
        Vector3 destinationSize = text.rectTransform.localScale * 1.2f;
        LeanTween.value(text.gameObject, text.rectTransform.localScale, destinationSize, playerLifeFadeIn)
            .setOnUpdate((Vector3 value) => {
                text.rectTransform.localScale = value;
            })
            .setEase(LeanTweenType.easeInOutQuad)
            .setLoopClamp()
            .setLoopPingPong(1);
    }
}