using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Optional;
using UniRx;
using UnityEngine;

public interface IUniTaskPresenter<T>
{
    UniTask<Option<T>> Run(
        IDisposable disposables,
        Func<bool> conditionFunc,
        CancellationToken cancellationToken,
        UniTask firstTask = default);
    void TryEnqueueTask(UniTask task);
    void SetResult(T result);
}

public class UniTaskPresenter<T> : IUniTaskPresenter<T>
{
    private Option<UniTask> _currentTask = Option.None<UniTask>();
    private Option<T> _result = Option.None<T>();

    public UniTaskPresenter()
    {
    }

    public async UniTask<Option<T>> Run(
        IDisposable disposables,
        Func<bool> conditionFunc,
        CancellationToken cancellationToken,
        UniTask firstTask = default)
    {
        using (disposables)
        {
            _currentTask = firstTask.SomeNotNull();

            while (conditionFunc() && !cancellationToken.IsCancellationRequested)
            {
                if (_TryPopOutNextTask(out var task))
                {
                    await task;
                }
                else
                {
                    await UniTask.NextFrame();
                }
            }
        }

        return _result;
    }

    private bool _TryPopOutNextTask(out UniTask task)
    {
        if (_currentTask.HasValue)
        {
            task = _currentTask.ValueOr(UniTask.CompletedTask);
            _currentTask = Option.None<UniTask>();
            return true;
        }

        task = UniTask.CompletedTask;
        return false;
    }

    public void TryEnqueueTask(UniTask task)
    {
        if (!_currentTask.HasValue)
        {
            _currentTask = Option.Some(task);
        }
    }
    public void SetResult(T result)
    {
        _result = Option.Some(result);
    }
}
