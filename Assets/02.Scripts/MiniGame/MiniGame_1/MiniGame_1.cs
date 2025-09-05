using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Utils.EnumTypes;
using System.Collections;

public class MiniGame_1 : MiniGameController
{
    private AudioSource audioSource;
    private RectTransform clothes;
    public Sprite[] clotheSprites;

    public List<GameObject> spots;
    public GameObject spotPrefab;

    private float randomRangeX = 100.0f;
    private float randomRangeY = 160.0f;

    private int spotCount = 0;
    private const int minSpotCount = 4;
    private const int maxSpotCount = 8;
    private const int reward = 2100;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        clothes = transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RectTransform>();
        spotCount = Random.Range(minSpotCount, maxSpotCount);
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (GameManager.Instance.isGameOver)
        {
            isGameOver = true;
            isGameSuccess = false;
            transform.gameObject.SetActive(false);
            GameManager.Instance.ReputationHandler(-GameManager.Instance.reputeDecr);
            return;
        }

        if (isGameOver)
            return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = ((int)currentTime).ToString();

            if (spots.Count <= 0)
            {
                isGameSuccess = true;
                StartCoroutine(GameEnd());
            }
        }
        else
        {
            isGameSuccess = false;
            StartCoroutine(GameEnd());
        }
    }

    public override void Init()
    {
        base.Init();

        isGameOver = false;
        isGameSuccess = false;
        currentTime = miniGameTime;
        resultPhanel.SetActive(false);

        clothes.GetComponent<Image>().sprite = clotheSprites[Random.Range(0, clotheSprites.Length)];

        for (int i = 0; i < spotCount; i++)
        {
            spots.Add(Instantiate(spotPrefab, clothes.transform));
            spots[i].transform.position = SetRandomPosition();
        }
    }

    public override void MiniGameStart()
    {
        base.MiniGameStart();
    }

    public override void MiniGameOver()
    {
        base.MiniGameOver();

        if (isGameSuccess)
            audioSource.clip = AudioManager.Instance.sfxClips[(int)SFXType.MiniGame_Clear];
        else
            audioSource.clip = AudioManager.Instance.sfxClips[(int)SFXType.MiniGame_Over];

        audioSource.Play();
    }

    public override void MiniGameReward()
    {
        base.MiniGameReward();

        if (isGameSuccess)
        {
            UIManager.Instance.ChangeSpotCompleteText();
            GameManager.Instance.ReputationHandler(GameManager.Instance.reputeAdd);
            GameManager.Instance.MoneyHandler(reward * (int)currentTime);
        }
        else
        {
            GameManager.Instance.ReputationHandler(-GameManager.Instance.reputeDecr);
        }

        for (int i = 0; i < spots.Count; i++)
            Destroy(spots[i]);
        spots.Clear();

        transform.gameObject.SetActive(false);
        MiniGameManager.Instance.OnMiniGameEnd(isGameSuccess);
    }

    public IEnumerator GameEnd()
    {
        MiniGameOver();
        yield return new WaitForSeconds(audioSource.clip.length);
        MiniGameReward();
    }

    // ¾ó·è ·£´ý À§Ä¡ ¼³Á¤
    public Vector2 SetRandomPosition()
    {
        //Vector2 _anchorPosition = clothes.GetComponent<RectTransform>().position;
        //Vector2 _randomOffest = new Vector2(Random.Range(-randomRangeX, randomRangeX), Random.Range(-randomRangeY, randomRangeY));
        //return _anchorPosition + _randomOffest;

        Vector2 _size = clothes.rect.size;
        Vector2 _randomOffest = new Vector2(Random.Range(-_size.x / 2f, _size.x / 2f), Random.Range(-_size.y / 2f, _size.y / 2f));

        Vector3 _woldPos = clothes.TransformPoint(_randomOffest);
        return _woldPos;
    }
}