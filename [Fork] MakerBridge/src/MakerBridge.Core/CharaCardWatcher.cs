﻿using BepInEx;
using System;
using System.IO;
using System.Threading;

namespace KeelPlugins
{
	internal static class CharaCardWatcher
	{
		public static FileSystemWatcher Watch(string filePath, Action<string> fileChangeCompleted)
		{
			var watcher = new FileSystemWatcher
			{
				Path = Path.GetDirectoryName(filePath),
				Filter = $"{Path.GetFileNameWithoutExtension(filePath)}*{Path.GetExtension(filePath)}"
			};

			FileSystemEventHandler handler = (sender, e) => FileChanged(sender, e, fileChangeCompleted);
			watcher.Created += handler;
			watcher.Changed += handler;

			return watcher;
		}

		private static void FileChanged(object sender, FileSystemEventArgs e, Action<string> fileChangeCompleted)
		{
			bool fileIsBusy = true;
			while (fileIsBusy)
			{
				try
				{
					using (var file = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read)) { }
					fileIsBusy = false;
				}
				catch (IOException)
				{
					Thread.Sleep(100);
				}
			}

			ThreadingHelper.Instance.StartSyncInvoke(() =>
			{
				fileChangeCompleted(e.FullPath);
				File.Delete(e.FullPath);
			});
		}
	}
}
