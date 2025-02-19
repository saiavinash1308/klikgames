using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private CanvasGroup cg;
    [SerializeField]
    private TextMeshProUGUI GameModeText;
    // Start is called before the first frame update
    void Start()
    {
        GameController.onGameSet += GameSet;
    }

    private void OnDestroy()
    {
        GameController.onGameSet -= GameSet;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GameSet()
    {
        // Show Game Mode at Start
        if(GameController.instance.isBowler())
        {
            GameModeText.text = "PLAYER2BAT";
        }
        else if(GameController.instance.isBatsman())
        {
            GameModeText.text = "PLAYER1BAT";
        }
        LeanTween.alphaCanvas(cg, 1f, 0.5f);        // smooth transition to Panel
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }
}
