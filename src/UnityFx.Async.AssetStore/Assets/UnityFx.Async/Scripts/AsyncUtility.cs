﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
#if UNITY_5_4_OR_NEWER || UNITY_2017 || UNITY_2018
using UnityEngine.Networking;
#elif UNITY_5_2_OR_NEWER
using UnityEngine.Experimental.Networking;
#endif

namespace UnityFx.Async
{
	/// <summary>
	/// Utility classes.
	/// </summary>
	public static class AsyncUtility
	{
		#region data

		private static GameObject _go;

		#endregion

		#region interface

		/// <summary>
		/// Name of a <see cref="GameObject"/> used by the library tools.
		/// </summary>
		public const string RootGoName = "UnityFx.Async";

		/// <summary>
		/// Returns a <see cref="GameObject"/> used by the library tools.
		/// </summary>
		public static GameObject GetRootGo()
		{
			if (ReferenceEquals(_go, null))
			{
				_go = new GameObject(RootGoName);
				GameObject.DontDestroyOnLoad(_go);
			}

			return _go;
		}

		/// <summary>
		/// Initializes the utilities. If skipped the utilities are lazily initialized.
		/// </summary>
		public static void Initialize()
		{
			GetRootBehaviour();
		}

		/// <summary>
		/// Returns an instance of an <see cref="IAsyncUpdateSource"/> for Update.
		/// </summary>
		public static IAsyncUpdateSource GetUpdateSource()
		{
			return GetRootBehaviour().UpdateSource;
		}

		/// <summary>
		/// Returns an instance of an <see cref="IAsyncUpdateSource"/> for LateUpdate.
		/// </summary>
		public static IAsyncUpdateSource GetLateUpdateSource()
		{
			return GetRootBehaviour().LateUpdateSource;
		}

		/// <summary>
		/// Returns an instance of an <see cref="IAsyncUpdateSource"/> for FixedUpdate.
		/// </summary>
		public static IAsyncUpdateSource GetFixedUpdateSource()
		{
			return GetRootBehaviour().FixedUpdateSource;
		}

		/// <summary>
		/// Returns an instance of an <see cref="IAsyncUpdateSource"/> for end of frame.
		/// </summary>
		public static IAsyncUpdateSource GetEndOfFrameUpdateSource()
		{
			return GetRootBehaviour().EofUpdateSource;
		}

		/// <summary>
		/// Creates an operation that completes after a time delay.
		/// </summary>
		/// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned operation, or -1 to wait indefinitely.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="millisecondsDelay"/> is less than -1.</exception>
		/// <returns>An operation that represents the time delay.</returns>
		public static IAsyncOperation Delay(int millisecondsDelay)
		{
			return AsyncResult.Delay(millisecondsDelay, GetRootBehaviour().UpdateSource);
		}

		/// <summary>
		/// Creates an operation that completes after a time delay.
		/// </summary>
		/// <param name="secondsDelay">The number of seconds to wait before completing the returned operation, or -1 to wait indefinitely.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="secondsDelay"/> is less than -1.</exception>
		/// <returns>An operation that represents the time delay.</returns>
		public static IAsyncOperation Delay(float secondsDelay)
		{
			return AsyncResult.Delay(secondsDelay, GetRootBehaviour().UpdateSource);
		}

		/// <summary>
		/// Creates an operation that completes after a time delay.
		/// </summary>
		/// <param name="delay">The time span to wait before completing the returned operation, or <c>TimeSpan.FromMilliseconds(-1)</c> to wait indefinitely.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="delay"/> represents a negative time interval other than <c>TimeSpan.FromMillseconds(-1)</c>.</exception>
		/// <returns>An operation that represents the time delay.</returns>
		public static IAsyncOperation Delay(TimeSpan delay)
		{
			return AsyncResult.Delay(delay, GetRootBehaviour().UpdateSource);
		}

		/// <summary>
		/// Starts a coroutine.
		/// </summary>
		/// <param name="enumerator">The coroutine to run.</param>
		public static Coroutine StartCoroutine(IEnumerator enumerator)
		{
			if (enumerator == null)
			{
				throw new ArgumentNullException("enumerator");
			}

			return GetRootBehaviour().StartCoroutine(enumerator);
		}

		/// <summary>
		/// Stops the specified coroutine.
		/// </summary>
		/// <param name="coroutine">The coroutine to run.</param>
		public static void StopCoroutine(Coroutine coroutine)
		{
			if (coroutine != null)
			{
				var runner = TryGetRootBehaviour();

				if (runner)
				{
					runner.StopCoroutine(coroutine);
				}
			}
		}

		/// <summary>
		/// Stops the specified coroutine.
		/// </summary>
		/// <param name="enumerator">The coroutine to run.</param>
		public static void StopCoroutine(IEnumerator enumerator)
		{
			if (enumerator != null)
			{
				var runner = TryGetRootBehaviour();

				if (runner)
				{
					runner.StopCoroutine(enumerator);
				}
			}
		}

		/// <summary>
		/// Stops all coroutines.
		/// </summary>
		public static void StopAllCoroutines()
		{
			var runner = TryGetRootBehaviour();

			if (runner)
			{
				runner.StopAllCoroutines();
			}
		}

		/// <summary>
		/// Register a completion callback for the specified <see cref="AsyncOperation"/> instance.
		/// </summary>
		/// <param name="op">The request to register completion callback for.</param>
		/// <param name="completionCallback">A delegate to be called when the <paramref name="op"/> has completed.</param>
		public static void AddCompletionCallback(AsyncOperation op, Action completionCallback)
		{
			if (op == null)
			{
				throw new ArgumentNullException("op");
			}

			if (completionCallback == null)
			{
				throw new ArgumentNullException("completionCallback");
			}

			GetRootBehaviour().AddCompletionCallback(op, completionCallback);
		}

#if UNITY_5_2_OR_NEWER || UNITY_5_3_OR_NEWER || UNITY_2017 || UNITY_2018

		/// <summary>
		/// Register a completion callback for the specified <see cref="UnityWebRequest"/> instance.
		/// </summary>
		/// <param name="request">The request to register completion callback for.</param>
		/// <param name="completionCallback">A delegate to be called when the <paramref name="request"/> has completed.</param>
		public static void AddCompletionCallback(UnityWebRequest request, Action completionCallback)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (completionCallback == null)
			{
				throw new ArgumentNullException("completionCallback");
			}

			GetRootBehaviour().AddCompletionCallback(request, completionCallback);
		}

#endif

		/// <summary>
		/// Register a completion callback for the specified <see cref="WWW"/> instance.
		/// </summary>
		/// <param name="request">The request to register completion callback for.</param>
		/// <param name="completionCallback">A delegate to be called when the <paramref name="request"/> has completed.</param>
		internal static void AddCompletionCallback(WWW request, Action completionCallback)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (completionCallback == null)
			{
				throw new ArgumentNullException("completionCallback");
			}

			GetRootBehaviour().AddCompletionCallback(request, completionCallback);
		}

		#endregion

		#region implementation

		private sealed class InvokeResult : AsyncResult
		{
			private readonly SendOrPostCallback _callback;

			public InvokeResult(SendOrPostCallback d, object asyncState)
				: base(null, asyncState)
			{
				_callback = d;
			}

			public void Invoke()
			{
				_callback.Invoke(AsyncState);
			}

			public void SetCompleted()
			{
				TrySetCompleted();
			}

			public void SetException(Exception e)
			{
				TrySetException(e);
			}
		}

		private sealed class MainThreadSynchronizationContext : SynchronizationContext
		{
			private readonly AsyncRootBehaviour _scheduler;

			public MainThreadSynchronizationContext(AsyncRootBehaviour scheduler)
			{
				_scheduler = scheduler;
			}

			public override SynchronizationContext CreateCopy()
			{
				return new MainThreadSynchronizationContext(_scheduler);
			}

			public override void Send(SendOrPostCallback d, object state)
			{
				if (d == null)
				{
					throw new ArgumentNullException("d");
				}

				_scheduler.Send(d, state);
			}

			public override void Post(SendOrPostCallback d, object state)
			{
				if (d == null)
				{
					throw new ArgumentNullException("d");
				}

				_scheduler.Post(d, state);
			}
		}

		private sealed class AsyncRootBehaviour : MonoBehaviour
		{
			#region data

			private Dictionary<object, Action> _ops;
			private List<object> _opsToRemove;

			private AsyncUpdateSource _updateSource;
			private AsyncUpdateSource _lateUpdateSource;
			private AsyncUpdateSource _fixedUpdateSource;
			private AsyncUpdateSource _eofUpdateSource;
			private WaitForEndOfFrame _eof;

			private SynchronizationContext _context;
			private int _mainThreadId;
			private Queue<InvokeResult> _actionQueue;

			#endregion

			#region interface

			public IAsyncUpdateSource UpdateSource
			{
				get
				{
					if (_updateSource == null)
					{
						_updateSource = new AsyncUpdateSource();
					}

					return _updateSource;
				}
			}

			public IAsyncUpdateSource LateUpdateSource
			{
				get
				{
					if (_lateUpdateSource == null)
					{
						_lateUpdateSource = new AsyncUpdateSource();
					}

					return _lateUpdateSource;
				}
			}

			public IAsyncUpdateSource FixedUpdateSource
			{
				get
				{
					if (_fixedUpdateSource == null)
					{
						_fixedUpdateSource = new AsyncUpdateSource();
					}

					return _fixedUpdateSource;
				}
			}

			public IAsyncUpdateSource EofUpdateSource
			{
				get
				{
					if (_eofUpdateSource == null)
					{
						_eofUpdateSource = new AsyncUpdateSource();
						_eof = new WaitForEndOfFrame();
						StartCoroutine(EofEnumerator());
					}

					return _eofUpdateSource;
				}
			}

			public void AddCompletionCallback(object op, Action cb)
			{
				if (_ops == null)
				{
					_ops = new Dictionary<object, Action>();
					_opsToRemove = new List<object>();
				}

				_ops.Add(op, cb);
			}

			public void Send(SendOrPostCallback d, object state)
			{
				if (!this)
				{
					throw new ObjectDisposedException(GetType().Name);
				}

				if (_mainThreadId == Thread.CurrentThread.ManagedThreadId)
				{
					d.Invoke(state);
				}
				else
				{
					using (var asyncResult = new InvokeResult(d, state))
					{
						lock (_actionQueue)
						{
							_actionQueue.Enqueue(asyncResult);
						}

						asyncResult.Wait();
					}
				}
			}

			public void Post(SendOrPostCallback d, object state)
			{
				if (!this)
				{
					throw new ObjectDisposedException(GetType().Name);
				}

				var asyncResult = new InvokeResult(d, state);

				lock (_actionQueue)
				{
					_actionQueue.Enqueue(asyncResult);
				}
			}

			#endregion

			#region MonoBehavoiur

			private void Awake()
			{
				var currentContext = SynchronizationContext.Current;

				if (currentContext == null)
				{
					var context = new MainThreadSynchronizationContext(this);
					SynchronizationContext.SetSynchronizationContext(context);
					_context = context;
				}

				_mainThreadId = Thread.CurrentThread.ManagedThreadId;
				_actionQueue = new Queue<InvokeResult>();
			}

			private void Update()
			{
				if (_ops != null && _ops.Count > 0)
				{
					foreach (var item in _ops)
					{
						if (item.Key is AsyncOperation)
						{
							var asyncOp = item.Key as AsyncOperation;

							if (asyncOp.isDone)
							{
								item.Value();
								_opsToRemove.Add(asyncOp);
							}
						}
#if UNITY_5_2_OR_NEWER || UNITY_5_3_OR_NEWER || UNITY_2017 || UNITY_2018
						else if (item.Key is UnityWebRequest)
						{
							var asyncOp = item.Key as UnityWebRequest;

							if (asyncOp.isDone)
							{
								item.Value();
								_opsToRemove.Add(asyncOp);
							}
						}
#endif
						else if (item.Key is WWW)
						{
							var asyncOp = item.Key as WWW;

							if (asyncOp.isDone)
							{
								item.Value();
								_opsToRemove.Add(asyncOp);
							}
						}
					}

					foreach (var item in _opsToRemove)
					{
						_ops.Remove(item);
					}

					_opsToRemove.Clear();
				}

				if (_updateSource != null)
				{
					_updateSource.OnNext(Time.deltaTime);
				}

				if (_actionQueue.Count > 0)
				{
					lock (_actionQueue)
					{
						while (_actionQueue.Count > 0)
						{
							var asyncResult = _actionQueue.Dequeue();

							try
							{
								asyncResult.Invoke();
								asyncResult.SetCompleted();
							}
							catch (Exception e)
							{
								asyncResult.SetException(e);
								Debug.LogException(e);
							}
						}
					}
				}
			}

			private void LateUpdate()
			{
				if (_lateUpdateSource != null)
				{
					_lateUpdateSource.OnNext(Time.deltaTime);
				}
			}

			private void FixedUpdate()
			{
				if (_fixedUpdateSource != null)
				{
					_fixedUpdateSource.OnNext(Time.fixedDeltaTime);
				}
			}

			private void OnDestroy()
			{
				if (_updateSource != null)
				{
					_updateSource.Dispose();
					_updateSource = null;
				}

				if (_lateUpdateSource != null)
				{
					_lateUpdateSource.Dispose();
					_lateUpdateSource = null;
				}

				if (_fixedUpdateSource != null)
				{
					_fixedUpdateSource.Dispose();
					_fixedUpdateSource = null;
				}

				if (_eofUpdateSource != null)
				{
					_eofUpdateSource.Dispose();
					_eofUpdateSource = null;
				}

				if (_context != null && _context == SynchronizationContext.Current)
				{
					SynchronizationContext.SetSynchronizationContext(null);
				}

				lock (_actionQueue)
				{
					_actionQueue.Clear();
				}

				_context = null;
			}

			#endregion

			#region implementation

			private IEnumerator EofEnumerator()
			{
				yield return _eof;

				if (_eofUpdateSource != null)
				{
					_eofUpdateSource.OnNext(Time.deltaTime);
				}
			}

			#endregion
		}

		private static AsyncRootBehaviour TryGetRootBehaviour()
		{
			var go = GetRootGo();

			if (go)
			{
				var runner = go.GetComponent<AsyncRootBehaviour>();

				if (runner)
				{
					return runner;
				}
			}

			return null;
		}

		private static AsyncRootBehaviour GetRootBehaviour()
		{
			var go = GetRootGo();

			if (go)
			{
				var runner = go.GetComponent<AsyncRootBehaviour>();

				if (!runner)
				{
					runner = go.AddComponent<AsyncRootBehaviour>();
				}

				return runner;
			}
			else
			{
				throw new ObjectDisposedException(RootGoName);
			}
		}

		#endregion
	}
}
