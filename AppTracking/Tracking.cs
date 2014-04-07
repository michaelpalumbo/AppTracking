using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;
using System.IO;

namespace AppTracking
{
	public class Tracking
	{
		private List<string> _logBuffer;
		private const int _logBufferLimit = 50;

		public void Track()
		{
			_logBuffer = new List<string>();

			Timer timer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
			timer.Elapsed += (sender, e) =>
			{
				Log();
			};

			timer.Start();
		}

		private void Log()
		{
			const int nChars = 256;
			int handle = 0;
			StringBuilder Buff = new StringBuilder(nChars);

			handle = GetForegroundWindow();

			if (GetWindowText(handle, Buff, nChars) > 0)
			{
				lock (_logBuffer)
				{
					_logBuffer.Add(string.Format("{0},{1}", DateTime.Now, Buff.ToString()));
				}

				if (_logBuffer.Count >= _logBufferLimit)
				{
					FlushLogBuffer();
				}
			}
		}

		private void FlushLogBuffer()
		{
			lock (_logBuffer)
			{
				using (StreamWriter writer = new StreamWriter(@"C:\AppTracking.log", true))
				{
					foreach (var entry in _logBuffer)
					{
						writer.WriteLine(entry);
					}
				}

				_logBuffer.Clear();
			}
		}

		[DllImport("user32.dll")]
		static extern int GetForegroundWindow();

		[DllImport("user32.dll")]
		static extern int GetWindowText(int hWnd, StringBuilder text, int count);
	}
}