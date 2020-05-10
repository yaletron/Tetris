using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public Text TextBlock;
    private ReactiveProperty<int> _counter = new ReactiveProperty<int>();

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .ThrottleFirst(TimeSpan.FromMilliseconds(500))
            .Subscribe(OnClick);

        _counter.SubscribeToText(TextBlock, i => $"Shots fired: {i}");
    }

    private void OnClick(Unit x)
    {
        _counter.Value++;
    }
}