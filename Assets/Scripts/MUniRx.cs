using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UniRx;
using System.Timers;

public class MUniRx : Tetris
{

    IEnumerator DropGravity()
    {
    //    Debug.Log("drop grav start");
        TryDropDown();
        yield return new WaitForSeconds(1);
      //  Debug.Log("drop grav end");
    }

    // two coroutines
    IEnumerator AsyncA()
    {
       // Debug.Log("a start");
        yield return new WaitForSeconds(3);
      //  Debug.Log("a end");
    }

    IEnumerator AsyncB()
    {
      //  Debug.Log("b start");
        yield return new WaitForEndOfFrame();
      //  Debug.Log("b end");
    }

    void Start()
    {
        // after completed AsyncA, run AsyncB as continuous routine.
        // UniRx expands SelectMany(IEnumerator) as SelectMany(IEnumerator.ToObservable())
        var cancel = Observable.FromCoroutine(AsyncA)
            .SelectMany(AsyncB)
            .Subscribe();

        // If you want to stop Coroutine(as cancel), call subscription.Dispose()
        // cancel.Dispose();



        var gravity = Observable.FromCoroutine(DropGravity)
            .SelectMany(AsyncB)
            .Subscribe();








    }

    public static IObservable<T> Empty<T>()
    {
        return Observable.Create<T>(o =>
        {
            o.OnCompleted();
            return Disposable.Empty;
        });
    }
    public static IObservable<T> Return<T>(T value)
    {
        return Observable.Create<T>(o =>
        {
            o.OnNext(value);
            o.OnCompleted();
            return Disposable.Empty;
        });
    }
    public static IObservable<T> Never<T>()
    {
        return Observable.Create<T>(o =>
        {
            return Disposable.Empty;
        });
    }
    public static IObservable<T> Throws<T>(Exception exception)
    {
        return Observable.Create<T>(o =>
        {
            o.OnError(exception);
            return Disposable.Empty;
        });
    }


    //Example code only
    public void NonBlocking_event_driven()
    {
        var ob = Observable.Create<string>(
        observer =>
        {
            var timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += (s, e) => observer.OnNext("tick");
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
            return Disposable.Empty;
        });
        var subscription = ob.Subscribe(Console.WriteLine);
        Console.ReadLine();
        subscription.Dispose();
    }
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        Console.WriteLine(e.SignalTime);
    }
}
