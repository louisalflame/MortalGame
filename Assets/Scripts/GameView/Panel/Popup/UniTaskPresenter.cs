using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Optional;
using UniRx;
using UnityEngine;

public interface IUniTaskPresenter<T>
{
    public abstract record Event();
    public record None() : Event;
    public record Halt() : Event;

    UniTask<Option<T>> Run(
        IDisposable disposables,
        Func<bool> conditionFunc,
        CancellationToken cancellationToken,
        Func<Event, UniTask<Event>> eventTaskHandler,
        UniTask<Event> firstTask = default);
    void TryEnqueueNextEvent(Event task);
    void SetResult(T result);
}

public class UniTaskPresenter<T> : IUniTaskPresenter<T>
{
    private Option<UniTask<IUniTaskPresenter<T>.Event>> _currentTask = Option.None<UniTask<IUniTaskPresenter<T>.Event>>();
    private Func<IUniTaskPresenter<T>.Event, UniTask<IUniTaskPresenter<T>.Event>> _eventTaskHandler;
    private Option<T> _result = Option.None<T>();

    public UniTaskPresenter()
    {
    }

    public async UniTask<Option<T>> Run(
        IDisposable disposables,
        Func<bool> conditionFunc,
        CancellationToken cancellationToken,
        Func<IUniTaskPresenter<T>.Event, UniTask<IUniTaskPresenter<T>.Event>> eventTaskHandler,
        UniTask<IUniTaskPresenter<T>.Event> firstTask = default)
    {
        _eventTaskHandler = eventTaskHandler;

        using (disposables)
        {
            _currentTask = firstTask.SomeNotNull();

            while (conditionFunc() && !cancellationToken.IsCancellationRequested)
            {
                if (_TryPopOutNextTask(out var task))
                {
                    var evt = await task;
                    if (evt is IUniTaskPresenter<T>.Halt)
                    {
                        break;
                    }
                }
                else
                {
                    await UniTask.NextFrame();
                }
            }
        }

        return _result;
    }

    private bool _TryPopOutNextTask(out UniTask<IUniTaskPresenter<T>.Event> task)
    {
        if (_currentTask.HasValue)
        {
            task = _currentTask.ValueOr(UniTask.FromResult<IUniTaskPresenter<T>.Event>(new IUniTaskPresenter<T>.None()));
            _currentTask = Option.None<UniTask<IUniTaskPresenter<T>.Event>>();
            return true;
        }

        task = UniTask.FromResult<IUniTaskPresenter<T>.Event>(new IUniTaskPresenter<T>.None());
        return false;
    }

    public void TryEnqueueNextEvent(IUniTaskPresenter<T>.Event nextEvent)
    {
        if (!_currentTask.HasValue)
        {
            var task = _eventTaskHandler(nextEvent);
            _currentTask = Option.Some(task);
        }
    }
    public void SetResult(T result)
    {
        _result = Option.Some(result);
    }
}
