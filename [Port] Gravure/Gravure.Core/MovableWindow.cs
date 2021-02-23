﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gravure
{
	public class MovableWindow : UIBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler //Shamelessly stolen from Joan6694
	{
		private Vector2 _cachedDragPosition;
		private Vector2 _cachedMousePosition;
		private bool _pointerDownCalled = false;
		private BaseCameraControl _cameraControl;
		private BaseCameraControl.NoCtrlFunc _noControlFunctionCached;

		public event Action<PointerEventData> onPointerDown;
		public event Action<PointerEventData> onDrag;
		public event Action<PointerEventData> onPointerUp;

		public RectTransform toDrag;
		public bool preventCameraControl;

		public static void makeWindowMovable(RectTransform bg)
		{
			// RectTransform bg = (RectTransform) GUI.transform.Find("MainPanel");
			MovableWindow mw = bg.gameObject.AddComponent<MovableWindow>();
			mw.toDrag = bg;
			mw.preventCameraControl = true;
		}

		protected override void Awake()
		{
			base.Awake();
			_cameraControl = GameObject.FindObjectOfType<BaseCameraControl>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (preventCameraControl && _cameraControl)
			{
				_noControlFunctionCached = _cameraControl.NoCtrlCondition;
				_cameraControl.NoCtrlCondition = () => true;
			}
			_pointerDownCalled = true;
			_cachedDragPosition = toDrag.position;
			_cachedMousePosition = Input.mousePosition;
			onPointerDown?.Invoke(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_pointerDownCalled == false)
				return;
			toDrag.position = _cachedDragPosition + ((Vector2)Input.mousePosition - _cachedMousePosition);
			onDrag?.Invoke(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_pointerDownCalled == false)
				return;
			if (preventCameraControl && _cameraControl)
				_cameraControl.NoCtrlCondition = _noControlFunctionCached;
			_pointerDownCalled = false;
			onPointerUp?.Invoke(eventData);
		}
	}
}