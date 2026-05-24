using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CharacterDotween : MonoBehaviour
{
    [Header("Пути движения")]
    public Transform[] path1;
    public Transform[] path2;
    public Transform[] path3;

    [Header("Настройки анимации")]
    public float duration = 10f;
    public Ease easeType = Ease.InOutSine;
    public LoopType loopType = LoopType.Restart;

    [Header("UI Кнопки")]
    public Button btnPath1;
    public Button btnPath2;
    public Button btnPath3;

    private Tweener currentTween;

    void Start()
    {
        DOTween.Init();

        // Подключаем кнопки
        if (btnPath1) btnPath1.onClick.AddListener(() => StartPath(1));
        if (btnPath2) btnPath2.onClick.AddListener(() => StartPath(2));
        if (btnPath3) btnPath3.onClick.AddListener(() => StartPath(3));

        // Автоматический старт
        StartPath(1);
    }

    public void StartPath(int pathIndex)
    {
        if (currentTween != null)
            currentTween.Kill();

        Transform[] selectedPath = pathIndex == 1 ? path1 : pathIndex == 2 ? path2 : path3;

        if (selectedPath == null || selectedPath.Length < 2)
        {
            Debug.LogWarning("Путь не заполнен!");
            return;
        }

        Vector3[] waypoints = new Vector3[selectedPath.Length];
        for (int i = 0; i < selectedPath.Length; i++)
        {
            waypoints[i] = selectedPath[i].position;
        }

        // Основное движение по пути
        currentTween = transform.DOPath(waypoints, duration, PathType.CatmullRom, PathMode.Full3D)
            .SetEase(easeType)
            .SetLoops(-1, loopType)
            .SetOptions(true);

        // Дополнительные красивые эффекты
        transform.DOScale(1.25f, 0.8f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        transform.DORotate(new Vector3(0, 360, 0), 12f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}