using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Comment("Handles UI inserts to comment player deaths and the general win situation during the game.")]

    private static readonly string INSERT_TAG = "TextInsert";

    #region Inspector Variables
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

    [FancyHeader("Tween settings")]
    [SerializeField]
    private float tweenTime = 0.2f;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeOutBack;
    #endregion

    #region Internal Variables
    private PlayerManager playerManager;
    private Canvas mainCanvas;
    private Text textInsert;
    private LTDescr textInsertTween;
    #endregion

    private void Start()
    {
        InitializeUI();
        InitializePlayerManager();
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
        mainCanvas = GetComponentInChildren<Canvas>();
        if (mainCanvas == null)
            Debug.LogError("[UIManager]: No canvas found!.", gameObject);

        textInsert = mainCanvas.gameObject.FindComponentInChildWithTag<Text>(INSERT_TAG);
        if (textInsert == null)
            Debug.LogError("[UIManager]: No text insert object found!", gameObject);

        SetTextInsertAlpha(0f);
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
            DoPlayerWinTextInsert();
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
            DoPlayerWinTextInsert();
        }
    }

    private void DoPlayerWinTextInsert()
    {
        if (textInsertTween != null)
            LeanTween.cancel(textInsertTween.uniqueId);

        textInsert.text = insertEnd;
        SetTextInsertAlpha(1f);
        textInsert.rectTransform.localScale = new Vector2(endScale, endScale);

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
}