// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Http;
internal readonly struct ConventionBasedAwaitable<TAwaitable, TAwaiter, TResult>
{
    private readonly TAwaitable _customAwaitable;
    private readonly Func<TAwaitable, TAwaiter> _getAwaiterMethod;
    private readonly Func<TAwaiter, bool> _isCompleteMethod;
    private readonly Func<TAwaiter, TResult> _getResultMethod;
    private readonly Action<TAwaiter, Action> _onCompletedMethod;
    private readonly Action<TAwaiter, Action>? _unsafeOnCompleted;

    public ConventionBasedAwaitable(
        TAwaitable customAwaitable,
        Func<TAwaitable, TAwaiter> getAwaiterMethod,
        Func<TAwaiter, bool> isCompleteMethod,
        Func<TAwaiter, TResult> getResultMethod,
        Action<TAwaiter, Action> onCompletedMethod,
        Action<TAwaiter, Action>? unsafeOnCompleted)
    {
        Debug.Assert(typeof(TResult) != typeof(void));

        _customAwaitable = customAwaitable;
        _getAwaiterMethod = getAwaiterMethod;
        _isCompleteMethod = isCompleteMethod;
        _getResultMethod = getResultMethod;
        _onCompletedMethod = onCompletedMethod;
        _unsafeOnCompleted = unsafeOnCompleted;
    }

    public Awaiter<TAwaiter, TResult> GetAwaiter()
    {
        var customAwaiter = _getAwaiterMethod(_customAwaitable);
        return new Awaiter<TAwaiter, TResult>(customAwaiter, _isCompleteMethod, _getResultMethod, _onCompletedMethod, _unsafeOnCompleted);
    }

    public readonly struct Awaiter<TAwaiter, TResult> : ICriticalNotifyCompletion
    {
        private readonly TAwaiter _customAwaiter;
        private readonly Func<TAwaiter, bool> _isCompletedMethod;
        private readonly Func<TAwaiter, TResult> _getResultMethod;
        private readonly Action<TAwaiter, Action> _onCompletedMethod;
        private readonly Action<TAwaiter, Action>? _unsafeOnCompletedMethod;

        public Awaiter(
            TAwaiter customAwaiter,
            Func<TAwaiter, bool> isCompletedMethod,
            Func<TAwaiter, TResult> getResultMethod,
            Action<TAwaiter, Action> onCompletedMethod,
            Action<TAwaiter, Action>? unsafeOnCompletedMethod)
        {
            _customAwaiter = customAwaiter;
            _isCompletedMethod = isCompletedMethod;
            _getResultMethod = getResultMethod;
            _onCompletedMethod = onCompletedMethod;
            _unsafeOnCompletedMethod = unsafeOnCompletedMethod;
        }

        public bool IsCompleted => _isCompletedMethod(_customAwaiter);

        public TResult GetResult() => _getResultMethod(_customAwaiter);

        public void OnCompleted(Action continuation)
        {
            _onCompletedMethod(_customAwaiter, continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            var underlyingMethodToUse = _unsafeOnCompletedMethod ?? _onCompletedMethod;
            underlyingMethodToUse(_customAwaiter, continuation);
        }
    }
}

internal readonly struct ConventionBasedVoidAwaitable<TAwaitable, TAwaiter>
{
    private readonly TAwaitable _customAwaitable;
    private readonly Func<TAwaitable, TAwaiter> _getAwaiterMethod;
    private readonly Func<TAwaiter, bool> _isCompleteMethod;
    private readonly Action<TAwaiter> _getResultMethod;
    private readonly Action<TAwaiter, Action> _onCompletedMethod;
    private readonly Action<TAwaiter, Action>? _unsafeOnCompleted;

    public ConventionBasedVoidAwaitable(
        TAwaitable customAwaitable,
        Func<TAwaitable, TAwaiter> getAwaiterMethod,
        Func<TAwaiter, bool> isCompleteMethod,
        Action<TAwaiter> getResultMethod,
        Action<TAwaiter, Action> onCompletedMethod,
        Action<TAwaiter, Action>? unsafeOnCompleted)
    {
        _customAwaitable = customAwaitable;
        _getAwaiterMethod = getAwaiterMethod;
        _isCompleteMethod = isCompleteMethod;
        _getResultMethod = getResultMethod;
        _onCompletedMethod = onCompletedMethod;
        _unsafeOnCompleted = unsafeOnCompleted;
    }

    public Awaiter<TAwaiter> GetAwaiter()
    {
        var customAwaiter = _getAwaiterMethod(_customAwaitable);
        return new Awaiter<TAwaiter>(customAwaiter, _isCompleteMethod, _getResultMethod, _onCompletedMethod, _unsafeOnCompleted);
    }

    public readonly struct Awaiter<TAwaiter> : ICriticalNotifyCompletion
    {
        private readonly TAwaiter _customAwaiter;
        private readonly Func<TAwaiter, bool> _isCompletedMethod;
        private readonly Action<TAwaiter> _getResultMethod;
        private readonly Action<TAwaiter, Action> _onCompletedMethod;
        private readonly Action<TAwaiter, Action>? _unsafeOnCompletedMethod;

        public Awaiter(
            TAwaiter customAwaiter,
            Func<TAwaiter, bool> isCompletedMethod,
            Action<TAwaiter> getResultMethod,
            Action<TAwaiter, Action> onCompletedMethod,
            Action<TAwaiter, Action>? unsafeOnCompletedMethod)
        {
            _customAwaiter = customAwaiter;
            _isCompletedMethod = isCompletedMethod;
            _getResultMethod = getResultMethod;
            _onCompletedMethod = onCompletedMethod;
            _unsafeOnCompletedMethod = unsafeOnCompletedMethod;
        }

        public bool IsCompleted => _isCompletedMethod(_customAwaiter);

        public void GetResult() => _getResultMethod(_customAwaiter);
        public void OnCompleted(Action continuation)
        {
            _onCompletedMethod(_customAwaiter, continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            var underlyingMethodToUse = _unsafeOnCompletedMethod ?? _onCompletedMethod;
            underlyingMethodToUse(_customAwaiter, continuation);
        }
    }
}
