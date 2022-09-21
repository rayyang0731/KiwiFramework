using System;

using UnityEngine.Events;

namespace KiwiFramework.Runtime
{
	public sealed class Timer : IDisposable
	{
		[Serializable]
		public class TimerEvent : UnityEvent<Timer>
		{
		}

		/// <summary>
		/// 传入的参数
		/// </summary>
		private object[] _args;

		/// <summary>
		/// 是否已经释放过资源
		/// </summary>
		private bool _disposed;

		/// <summary>
		/// 计时开始时回调
		/// </summary>
		private readonly TimerEvent _onStart = new();

		/// <summary>
		/// 计时到设定时间回调
		/// </summary>
		private readonly TimerEvent _onCallback = new();

		/// <summary>
		/// 计时中回调
		/// </summary>
		private readonly TimerEvent _onUpdate = new();

		/// <summary>
		/// 计时器暂停回调
		/// </summary>
		private readonly TimerEvent _onPause = new();

		/// <summary>
		/// 计时器恢复回调
		/// </summary>
		private readonly TimerEvent _onResume = new();

		/// <summary>
		/// 计时结束回调
		/// </summary>
		private readonly TimerEvent _onStop = new();

		/// <summary>
		/// 计时取消回调
		/// </summary>
		private readonly TimerEvent _onCancel = new();

		/// <summary>
		/// 计时器唯一标识符
		/// </summary>
		public int Guid { get; private set; }

		/// <summary>
		/// 总持续时间
		/// </summary>
		public float Duration { get; private set; }

		/// <summary>
		/// 计时器是否循环
		/// </summary>
		public bool Loop { get; private set; }

		/// <summary>
		/// 计时器是否暂停
		/// </summary>
		public bool IsPause { get; private set; }

		/// <summary>
		/// 计时器是否停止
		/// </summary>
		public bool IsStop => !IsPause && RemainTime <= 0;

		/// <summary>
		/// 计时器是否忽略时间缩放
		/// </summary>
		public bool IgnoreTimeScale { get; private set; }

		/// <summary>
		/// 回调频率
		/// </summary>
		public float CallbackFrequency { get; private set; }

		/// <summary>
		/// 已用时间
		/// </summary>
		public float ElapsedTime => Duration - RemainTime;

		/// <summary>
		/// 剩余时间
		/// </summary>
		public float RemainTime { get; private set; }

		/// <summary>
		/// 计时器已经完成的百分比
		/// </summary>
		public float Ratio => 1 - RemainTime / Duration;

		/// <summary>
		/// 下次回调时间
		/// </summary>
		public float NextCallbackTime { get; private set; }

		/// <summary>
		/// 开始次数
		/// </summary>
		/// <returns></returns>
		public int StartCount { get; private set; }

		/// <summary>
		/// 结束次数
		/// </summary>
		/// <returns></returns>
		public int FinishCount { get; private set; }

		/// <summary>
		/// 下一个 GUID 
		/// </summary>
		private static int _nextGUID;

		/// <summary>
		/// 计时器容器
		/// </summary>
		public static readonly TimerContainer Container = new();

		/// <summary>
		/// 创建计时器
		/// </summary>
		/// <param name="duration">持续时间</param>
		/// <param name="callback">到时回调</param>
		/// <param name="callFrequency">回调频率</param>
		/// <param name="loop">是否循环</param>
		/// <param name="ignoreTimeScale">是否忽略 TimeScale</param>
		/// <returns>创建好的计时器对象</returns>
		public static Timer Create(float duration, UnityAction<Timer> callback, float callFrequency = 0f, bool loop = false,
			bool ignoreTimeScale = false)
		{
			if (callback == null) throw new Exception("创建计时器失败,回调方法不能为Null");

			var timer = Container.Pop();

			timer.Guid = _nextGUID++;
			timer.Duration = duration;
			timer._onCallback.AddListener(callback);
			timer.CallbackFrequency = callFrequency;
			timer.Loop = loop;
			timer.IgnoreTimeScale = ignoreTimeScale;

			timer.ResetRunVariable();

			return timer;
		}

		/// <summary>
		/// 启动计时器
		/// </summary>
		/// <param name="duration">持续时间</param>
		/// <param name="onCallback">计时到设定时间回调</param>
		/// <param name="callFrequency">回调频率</param>
		/// <param name="loop">是否循环</param>
		/// <param name="ignoreTimeScale">是否忽略 TimeScale</param>
		public static Timer Startup(float duration, UnityAction<Timer> onCallback, float callFrequency = 0f,
			bool loop = false, bool ignoreTimeScale = false)
		{
			return _startup(duration, onCallback, callFrequency, loop, ignoreTimeScale);
		}

		/// <summary>
		/// 启动计时器
		/// </summary>
		/// <param name="duration">持续时间</param>
		/// <param name="onCallback">计时到设定时间回调</param>
		/// <param name="guid">计时器唯一标识符</param>
		/// <param name="callFrequency">回调频率</param>
		/// <param name="loop">是否循环</param>
		/// <param name="ignoreTimeScale">是否忽略 TimeScale</param>
		public static Timer Startup(float duration, UnityAction<Timer> onCallback, out int guid, float callFrequency = 0f,
			bool loop = false, bool ignoreTimeScale = false)
		{
			var timer = Startup(duration, onCallback, callFrequency, loop, ignoreTimeScale);
			if (timer != null)
				guid = timer.Guid;
			else
				throw new Exception("启动计时器失败,要启动的计时器为 Null");
			return timer;
		}

		/// <summary>
		/// (私有方法)启动计时器
		/// </summary>
		/// <param name="duration">持续时间</param>
		/// <param name="onCallback">计时到设定时间回调</param>
		/// <param name="callFrequency">回调频率</param>
		/// <param name="loop">是否循环</param>
		/// <param name="ignoreTimeScale">是否忽略 TimeScale</param>
		private static Timer _startup(float duration, UnityAction<Timer> onCallback, float callFrequency = 0f,
			bool loop = false, bool ignoreTimeScale = false)
		{
			if (onCallback == null) throw new Exception("启动计时器失败,回调方法不能为Null");

			var timer = Create(duration, onCallback, callFrequency, loop, ignoreTimeScale);
			timer.Startup();
			return timer;
		}

		/// <summary>
		/// 初始运行变量
		/// </summary>
		private void ResetRunVariable()
		{
			IsPause = true;
			RemainTime = 0f;
			NextCallbackTime = 0f;
		}

		/// <summary>
		/// 初始运行变量
		/// </summary>
		private void InitTick()
		{
			IsPause = false;
			RemainTime = Duration;
			StartCount++;

			if (CallbackFrequency > 0)
				NextCallbackTime = RemainTime - CallbackFrequency;
		}

		/// <summary>
		/// 重置并回收计时器
		/// </summary>
		private void ResetAndRecycle()
		{
			ResetRunVariable();
			ClearAllEvent();

			Container.RemoveTimer(this);
		}

		/// <summary>
		/// 清理全部事件
		/// </summary>
		private void ClearAllEvent()
		{
			_onStart.RemoveAllListeners();
			_onCallback.RemoveAllListeners();
			_onUpdate.RemoveAllListeners();
			_onPause.RemoveAllListeners();
			_onResume.RemoveAllListeners();
			_onStop.RemoveAllListeners();
			_onCancel.RemoveAllListeners();
		}

		private void Close()
		{
			if (_disposed) return;
			Guid = 0;
			Duration = 0;
			Loop = false;
			CallbackFrequency = 0;
			IgnoreTimeScale = false;
			IsPause = false;
			RemainTime = 0;
			NextCallbackTime = 0;
			StartCount = 0;
			FinishCount = 0;
			_args = null;
			ClearAllEvent();

			_disposed = true;
		}

		/// <summary>
		/// 启动计时器
		/// </summary>
		public void Startup()
		{
			//表示未在计时
			if (RemainTime <= 0)
				Restart();
		}

		/// <summary>
		/// 无论是否正在计时，马上重新开始
		/// </summary>
		public void Restart()
		{
			InitTick();

			Container.AddTimer(this);

			_onStart?.Invoke(this);
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public void Pause()
		{
			if (IsPause) return;

			IsPause = true;

			_onPause?.Invoke(this);
		}

		/// <summary>
		/// 继续
		/// </summary>
		public void Resume()
		{
			if (!IsPause) return;

			IsPause = false;

			_onResume?.Invoke(this);
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void Stop()
		{
			_onStop?.Invoke(this);

			ResetAndRecycle();
		}

		/// <summary>
		/// 取消
		/// </summary>
		public void Cancel()
		{
			_onCancel?.Invoke(this);

			ResetAndRecycle();
		}

		public void Tick(float deltaTime)
		{
			if (IsPause) return;

			RemainTime -= deltaTime;
			if (RemainTime <= NextCallbackTime)
			{
				_onCallback.Invoke(this);
				if (RemainTime <= 0f)
				{
					if (Loop)
						InitTick();
					else
						Stop();

					FinishCount++;
					return;
				}

				if (CallbackFrequency > 0)
				{
					NextCallbackTime -= CallbackFrequency;
					if (NextCallbackTime < 0)
						NextCallbackTime = 0f;
				}
				else
				{
					NextCallbackTime = 0f;
				}
			}
			else
			{
				_onUpdate?.Invoke(this);
			}
		}

		/// <summary>
		/// 添加计时开始回调
		/// </summary>
		public Timer AddStartCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onStart?.AddListener(callback);

			return this;
		}

		/// <summary>
		/// 移除计时开始回调
		/// </summary>
		public Timer RemoveStartCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onStart?.RemoveListener(callback);

			return this;
		}

		/// <summary>
		/// 添加计时到设定时间回调
		/// </summary>
		public Timer AddCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onCallback?.AddListener(callback);

			return this;
		}

		/// <summary>
		/// 移除计时到设定时间回调
		/// </summary>
		public Timer RemoveCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onCallback?.RemoveListener(callback);

			return this;
		}

		/// <summary>
		/// 添加计时中回调
		/// </summary>
		public Timer AddUpdateCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onUpdate?.AddListener(callback);

			return this;
		}

		/// <summary>
		/// 移除计时中回调
		/// </summary>
		public Timer RemoveUpdateCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onUpdate?.RemoveListener(callback);

			return this;
		}

		/// <summary>
		/// 添加暂停回调
		/// </summary>
		public Timer AddPauseCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onPause?.AddListener(callback);

			return this;
		}

		/// <summary>
		/// 移除暂停回调
		/// </summary>
		public Timer RemovePauseCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onPause?.RemoveListener(callback);

			return this;
		}

		/// <summary>
		/// 添加恢复计时回调
		/// </summary>
		public Timer AddResumeCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onResume?.AddListener(callback);

			return this;
		}

		/// <summary>
		/// 移除恢复计时回调
		/// </summary>
		public Timer RemoveResumeCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onResume?.RemoveListener(callback);

			return this;
		}

		/// <summary>
		/// 添加结束回调
		/// </summary>
		public Timer AddStopCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onStop?.AddListener(callback);

			return this;
		}

		/// <summary>
		/// 移除计时中回调
		/// </summary>
		public Timer RemoveStopCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onStop?.RemoveListener(callback);

			return this;
		}

		/// <summary>
		/// 添加取消回调
		/// </summary>
		public Timer AddCancelCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onCancel?.AddListener(callback);

			return this;
		}

		/// <summary>
		/// 移除取消回调
		/// </summary>
		public Timer RemoveCancelCallback(UnityAction<Timer> callback)
		{
			if (callback != null)
				_onCancel?.RemoveListener(callback);

			return this;
		}

		/// <summary>
		/// 设置持续时间
		/// </summary>
		public Timer SetDuration(float duration)
		{
			if (duration > 0) Duration = duration;

			return this;
		}

		/// <summary>
		/// 设置是否循环
		/// </summary>
		public Timer SetLoop(bool loop)
		{
			Loop = loop;
			return this;
		}

		/// <summary>
		/// 设置回调频率
		/// </summary>
		public Timer SetCallbackFrequency(float callbackFrequency)
		{
			CallbackFrequency = callbackFrequency > 0 ? callbackFrequency : 0;
			return this;
		}

		/// <summary>
		/// 设置参数对象，会覆盖原有对象
		/// </summary>
		public Timer SetParams(params object[] objs)
		{
			_args = objs;
			return this;
		}

		/// <summary>
		/// 获取参数对象
		/// </summary>
		/// <typeparam name="T">参数类型</typeparam>
		/// <param name="index">参数索引</param>
		public T GetParam<T>(int index)
		{
			if (_args != null && index < _args.Length)
				return (T)_args[index];
			return default(T);
		}

		/// <summary>
		/// 获取参数对象,默认获取第0个参数
		/// </summary>
		/// <typeparam name="T">参数类型</typeparam>
		public T GetParam<T>()
		{
			return GetParam<T>(0);
		}

		/// <summary>
		/// 清理所有正在使用的资源
		/// </summary>
		public void Dispose()
		{
			Close();
		}
	}
}
