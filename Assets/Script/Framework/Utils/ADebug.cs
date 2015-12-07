namespace Framework
{
	public static class ADebug
	{
		public static void Log(string format, params object[] args)
		{
			UnityEngine.Debug.Log(string.Format(format, args));
		}

		public static void LogError(string format, params object[] args)
		{
			UnityEngine.Debug.LogError(string.Format(format, args));
		}

		public static void Assert(bool condition) 
		{
			UnityEngine.Debug.Assert(condition);
		}

	}
}