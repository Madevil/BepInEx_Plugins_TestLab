﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Studio;
using UnityEngine;

namespace KeelPlugins
{
	internal class StudioHandler : HandlerCore
	{
		private void Start()
		{
			watcher = CharaCardWatcher.Watch(MakerBridge.MakerCardPath, LoadChara);
			watcher.EnableRaisingEvents = true;
		}

		private void Update()
		{
			if (MakerBridge.SendChara.Value.IsDown())
				SaveCharacter(MakerBridge.OtherCardPath);
		}

		private void SaveCharacter(string path)
		{
			var characters = GetSelectedCharacters();
			if (characters.Count > 0)
			{
				var empty = new Texture2D(1, 1, TextureFormat.ARGB32, false);
				empty.SetPixel(0, 0, Color.black);
				empty.Apply();

				var charFile = characters[0].charInfo.chaFile;
				charFile.pngData = empty.EncodeToPNG();

				using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
					charFile.SaveCharaFile(fileStream, true);
			}
			else
			{
				MakerBridge.Log("Select a character to send to maker");
			}
		}

		private void LoadChara(string path)
		{
			var characters = GetSelectedCharacters();
			if (characters.Count > 0)
			{
				MakerBridge.Log("Character received");

				foreach (var chara in characters)
					chara.ChangeChara(path);

				UpdateStateInfo();
			}
			else
			{
				MakerBridge.Log("Select a character before replacing it");
			}
		}

		private List<OCIChar> GetSelectedCharacters()
		{
			return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
		}

		private void UpdateStateInfo()
		{
			var mpCharCtrl = FindObjectOfType<MPCharCtrl>();
			if (mpCharCtrl)
			{
				int select = Traverse.Create(mpCharCtrl).Field("select").GetValue<int>();
				if (select == 0) mpCharCtrl.OnClickRoot(0);
			}
		}
	}
}