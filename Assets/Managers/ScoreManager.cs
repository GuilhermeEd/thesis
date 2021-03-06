﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreType {
  Brick,
  Level,
  Time
}

public class ScoreItem {
  public float Amount { get; set; }
  public float Score { get; set; }

  public ScoreItem(float amount, float score) => (Amount, Score) = (amount, score);

  public static ScoreItem operator + (ScoreItem a, ScoreItem b) {
    ScoreItem scoreItem = new ScoreItem(a.Amount, a.Score);
    scoreItem.Amount += b.Amount;
    scoreItem.Score += b.Score;
    return scoreItem;
  }
}

public class ScoreManager : Singleton<ScoreManager>
{
  public float TotalScore {
    get {
      return PlayerPrefsManager.GetTotalScore();
    }
  }

  public float Score {
    get {
      float sum = 0;
      foreach (KeyValuePair<ScoreType, ScoreItem> scoreType in scoresByType) {
        sum += scoreType.Value.Score;
      }
      return sum;
    }
  }
  public Dictionary<ScoreType, ScoreItem> scoresByType;
  [SerializeField] Slider scoreSlider;
  [SerializeField] float scorePerSecond = 0.75f;

  float currentLevelMaxScore = 0f;

  void Start()
  {
    GameManager.Instance.OnGameStart.AddListener(HandleGameStart);
    currentLevelMaxScore = GetLevelMaxScore(1);
    InitializeScoreByType();
  }

  void Update()
  {
    ScoreItem scoreItem = new ScoreItem(Time.deltaTime, scorePerSecond * Time.deltaTime);
    if (GameManager.isGameRunning) { AddScore(ScoreType.Time, scoreItem); }
  }

  void HandleGameStart()
  {
    scoreSlider.value = 0f;
    scoreSlider.maxValue = GetLevelMaxScore(1);
    InitializeScoreByType();
  }

  public void AddGameScoreToTotalScore()
  {
    PlayerPrefsManager.SetTotalScore(TotalScore + Score);
  }

  public void AddToTotalScore()
  {
    PlayerPrefsManager.AddToTotalScore(Score);
  }

  public void AddScore(ScoreType type, ScoreItem item)
  {
    scoresByType[type] += item;

    float newScore = Score;
    scoreSlider.value = newScore;

    int currentLevel = LevelManager.level;
    if (newScore >= currentLevelMaxScore)
    {
      scoreSlider.minValue = currentLevelMaxScore;
      currentLevelMaxScore = GetLevelMaxScore(currentLevel + 1);
      scoreSlider.maxValue = currentLevelMaxScore;
      LevelManager.Instance.LevelUp();
    }
  }

  float GetLevelMaxScore(int? levelArg)
  {
    int level = levelArg ?? LevelManager.level;
    if (level <= 0) return 0;
    return GetLevelMaxScore(level - 1) + GetScoreProgressionCurveY(level);
  }

  void InitializeScoreByType()
  {
    scoresByType = new Dictionary<ScoreType, ScoreItem>() {
      { ScoreType.Brick, new ScoreItem(0, 0f) },
      { ScoreType.Level, new ScoreItem(0, 0f) },
      { ScoreType.Time, new ScoreItem(0, 0f) }
    };
  }

  // Curve Preview https://www.desmos.com/calculator/kokoyb75ge
  public static float GetScoreProgressionCurveY(float x, float min = 10f, float max = 50f, float intensity = 3f, float offset = 100f)
  {
    float rangeFactor = max - min;
    float numerator = rangeFactor * x;
    float growthFactor = intensity * x / rangeFactor;
    float denominator = x + Mathf.Pow(rangeFactor + offset, 1 - growthFactor);

    return min + (numerator / denominator);
  }
}