using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace Framework 
{
	public class AUIManager : MonoBehaviour
	{
		private class QueuedDialog
		{
			public Dialog         prefab;
			public Action<Dialog> onCreated;
		}


		public  static AUIManager instance;
		private static Dictionary<string, Dialog> openedDialogs = new Dictionary<string, Dialog>();
		private static List<QueuedDialog>         queuedTips    = new List<QueuedDialog>();


		public GameObject uiRoot;
		public Camera     uiCamera;
		public int        screenWidth;
		public int        screenHeight;

        

		[HideInInspector]
		public Vector3    fullScreenDimensions;


		private AUIManager() 
		{
		}


		void Awake()
		{
			ADebug.Assert(this.uiRoot   != null);
			ADebug.Assert(this.uiCamera != null);

			instance = this;

			float num = this.screenHeight / this.uiCamera.pixelHeight;
			this.fullScreenDimensions = new Vector3(this.uiCamera.pixelWidth * num, this.uiCamera.pixelHeight * num, 1f);
		}

		public static int GetOpenedDialogCount()
		{
			return openedDialogs.Count;
		}


		public static void SetDialogActive(string name, bool isActive)
		{
			SetDialogActive(openedDialogs[name], isActive);
		}

		public static void SetDialogActive(Dialog dialog, bool isActive)
		{
			ADebug.Assert(dialog != null);
			
			if (dialog.container != null) 
			{
				dialog.container.SetActive(isActive);
			} 
			else 
			{
				dialog.gameObject.SetActive(isActive);
			}
		}


		public static Dialog ShowTip(Dialog prefab)
		{
			ADebug.Assert(prefab.autoCloseTime > 0);
			Dialog dialog = SpawnDialog(prefab);

			dialog.onOpend += (Dialog d) =>
			{
				DOTween.Sequence()
					   .PrependInterval(prefab.autoCloseTime)
					   .AppendCallback
					   (
							() =>
							{
								CloseDialog(d);
							}
					   );
			};

			return dialog;
		}

		private static bool isQueuedTipShowing = false;
		public  static void ShowQueuedTip(Dialog prefab, Action<Dialog> onCreated = null) 
		{
			ADebug.Assert(prefab != null);

			if (isQueuedTipShowing) 
			{
				QueuedDialog qd = new QueuedDialog();
				qd.prefab       = prefab;
				qd.onCreated   += onCreated;
				queuedTips.Add(qd);
			} 
			else 
			{
				isQueuedTipShowing = true;

				Dialog tip   = ShowTip(prefab);
				tip.onOpend += (Dialog d) =>
				{
					if (onCreated != null)
					{
						onCreated(d);
					}
				};
				
				tip.onClosed += (Dialog d) =>
				{				
					isQueuedTipShowing = false;
					if (queuedTips.Count > 0)
					{
						QueuedDialog qd = queuedTips[0];
						queuedTips.RemoveAt(0);
						ShowQueuedTip(qd.prefab, qd.onCreated);						
					} 
				};
			}
		}



		public static Dialog OpenDialog(Dialog prefab, GameObject parent = null)
		{
			ADebug.Assert(prefab.autoCloseTime == 0);
			return SpawnDialog(prefab, parent);
		}


		public static void CloseDialog(string name)
		{
			Dialog dialog = openedDialogs[name];
			ADebug.Assert(dialog != null);
			CloseDialog(dialog);
		}

		public static void CloseAllDialogs()
		{
			Dictionary<string, Dialog>.Enumerator enumerator = openedDialogs.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CloseDialog(enumerator.Current.Value);
			}

			openedDialogs.Clear();
			queuedTips.Clear();
		}

		public static void CloseDialog(Dialog dialog)
		{
			ADebug.Assert(dialog != null);
			ADebug.Assert(openedDialogs.ContainsKey(dialog.dialogName));

			openedDialogs.Remove(dialog.dialogName);

			switch (dialog.closeTransition) 
			{
				case Dialog.TransitionStyle.zoom:
				{
					dialog.gameObject.transform.DOScale(Vector3.zero, dialog.closeDuration)
											   .SetEase(dialog.closeEase)
											   .OnComplete
											   (
											       () => 
											       {
														OnClosedTweenComplete(dialog);
											       }
											   );

				} break;

				case Dialog.TransitionStyle.slide_left:
				{
					dialog.gameObject.transform.DOLocalMoveX(-instance.fullScreenDimensions.x - dialog.width / 2, dialog.closeDuration)
											   .SetEase(dialog.closeEase)
						                       .OnComplete
						                       (
						                       		() =>
						                       		{
														OnClosedTweenComplete(dialog);
						                         	}
						                       );										
				} break;
					
					
				case Dialog.TransitionStyle.slide_right:
				{
					dialog.gameObject.transform.DOLocalMoveX(instance.fullScreenDimensions.x + dialog.width / 2, dialog.closeDuration)
											   .SetEase(dialog.closeEase)
											   .OnComplete
											   (
											   		() =>
											   		{
														OnClosedTweenComplete(dialog);
											   		}
											   );										
				} break;
					
					
				case Dialog.TransitionStyle.slide_top:
				{
					dialog.gameObject.transform.DOLocalMoveY(instance.fullScreenDimensions.y + dialog.height / 2, dialog.closeDuration)
						                       .SetEase(dialog.closeEase)
						                       .OnComplete
						                       (
						                      	  () =>
						                      	  {
													 OnClosedTweenComplete(dialog);
						                      	  }
						                       );										
				} break;
					
				case Dialog.TransitionStyle.slide_bottom:
				{
					dialog.gameObject.transform.DOLocalMoveY(-instance.fullScreenDimensions.y - dialog.height / 2, dialog.closeDuration)
											   .SetEase(dialog.closeEase)
											   .OnComplete
											   (
											   		() =>
											   		{
														OnClosedTweenComplete(dialog);
											   	    }
											   );										
				} break;

			}

			if (dialog.scrim)
			{				
				Image image = dialog.scrim.GetComponentInChildren<Image>();
				DOTween.ToAlpha
				(
					()  => image.color,
					(c) => image.color = c,
					0f, 
					0.618f
				);
			}

		}

		private static void OnClosedTweenComplete(Dialog dialog)
		{
			if (dialog.onClosed != null)
			{
				dialog.onClosed(dialog);
			}
			
			if (dialog.container != null) 
			{
				UnityEngine.Object.Destroy(dialog.container);
			} 
			else 
			{
				UnityEngine.Object.Destroy(dialog.gameObject);
			}

			Resources.UnloadUnusedAssets();
		}

		private static Dialog SpawnDialog(Dialog prefab, GameObject parent = null) 
		{
			ADebug.Assert(prefab != null);
			ADebug.Assert(!openedDialogs.ContainsKey(prefab.dialogName));

			GameObject container;
			if (parent == null)
			{
				container = AddChild(instance.uiRoot.gameObject);
			}
			else 
			{
				container = AddChild(parent);
			}

			GameObject scrim     = AddChild(container); 
			scrim.transform.localPosition = new Vector3(0, 0, 10f);

			GameObject go        = AddChild(container, prefab.gameObject);
			container.name       = go.name + "(DialogContainer)";
			scrim.name           = go.name + "(DialogScrim)";
							    
			Dialog dialog        = go.GetComponent<Dialog>();
			dialog.container     = container;
			dialog.scrim         = scrim;


			GameObject image;
			if (prefab.isModal)
			{
				image = AddImage(scrim, LoadSprite("Scrim"));

				Image imageComponent = image.GetComponent<Image>();
				imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0f);
				DOTween.ToAlpha
				(
					()  => imageComponent.color,
					(c) => imageComponent.color = c,
					1f, 
					0.618f
				);
			} 
			else 
			{
				image = AddImage(scrim, null);
			}


			image.transform.localScale    = instance.fullScreenDimensions;
			Vector3    v3                 = image.transform.localPosition;
			v3.z                          = -9.99f;
			image.transform.localPosition = v3;
			image.name                    = "ScrimSprite";


			if (dialog.isCloseOnFocusOutside)
			{
				scrim.AddComponent<Button>().onClick.AddListener
				(
					() => 
					{
						CloseDialog(dialog);
					}
				);
			}


			if (dialog.isTouchFallThrough)
			{
				CanvasGroup group    = scrim.AddComponent<CanvasGroup>();
				group.interactable   = false;
				group.blocksRaycasts = false;
			}


			switch (dialog.openTransition) 
			{
				case Dialog.TransitionStyle.zoom:
				{
					go.transform.localScale    = Vector3.zero;
					go.transform.localPosition = new Vector3(prefab.openPosOffsetX, prefab.openPosOffsetY, 0f);
					go.transform.DOScale(Vector3.one, prefab.openDuration)
						        .SetEase(prefab.openEase)
							    .OnComplete
							    (
							    	() => 
							    	{
							    		OnOpenedTweenComplete(dialog);
							    	}
							    );
				} break;

				case Dialog.TransitionStyle.slide_left:
				{
					Vector3 pos                = go.transform.localPosition;
					go.transform.localPosition = new Vector3(-instance.fullScreenDimensions.x - dialog.width / 2, pos.y + prefab.openPosOffsetY, pos.z);
					go.transform.DOLocalMoveX(pos.x + prefab.openPosOffsetX, prefab.openDuration)
					            .SetEase(prefab.openEase)
								.OnComplete
								(
									() =>
									{
										OnOpenedTweenComplete(dialog);
									}
								);										
				} break;


				case Dialog.TransitionStyle.slide_right:
				{
					Vector3 pos                = go.transform.localPosition;
					go.transform.localPosition = new Vector3(instance.fullScreenDimensions.x + dialog.width / 2, pos.y + prefab.openPosOffsetY, pos.z);
					go.transform.DOLocalMoveX(pos.x + prefab.openPosOffsetX, prefab.openDuration)
					            .SetEase(prefab.openEase)
								.OnComplete
								(
									() =>
									{
										OnOpenedTweenComplete(dialog);
									}
								);										
				} break;


				case Dialog.TransitionStyle.slide_top:
				{
					Vector3 pos                = go.transform.localPosition;
					go.transform.localPosition = new Vector3(pos.x + prefab.openPosOffsetX, instance.fullScreenDimensions.y + dialog.height / 2, pos.z);
					go.transform.DOLocalMoveY(pos.y + prefab.openPosOffsetY, prefab.openDuration)
						        .SetEase(prefab.openEase)
							    .OnComplete
							    (
							    	() =>
							    	{
							    		OnOpenedTweenComplete(dialog);
							        }
							    );										
				} break;

				case Dialog.TransitionStyle.slide_bottom:
				{
					Vector3 pos                = go.transform.localPosition;
					go.transform.localPosition = new Vector3(pos.x + prefab.openPosOffsetX, -instance.fullScreenDimensions.y - dialog.height / 2, pos.z);
					go.transform.DOLocalMoveY(pos.y + prefab.openPosOffsetY, prefab.openDuration)
						        .SetEase(prefab.openEase)
						        .OnComplete
						        (
						        	() =>
						        	{
						        		OnOpenedTweenComplete(dialog);
						        	}
						        );										
				} break;
			}

			openedDialogs.Add(prefab.dialogName, dialog);
			return dialog;
		}


		private static void OnOpenedTweenComplete(Dialog dialog)
		{
			if (dialog.onOpend != null)
			{
				dialog.onOpend(dialog);
			}

		}

        public static GameObject AddImage(GameObject parent, Sprite sprite)
        {
            GameObject go = AddChild(parent);
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();

            return go;
        }

		public static Image AddImage1(GameObject parent, Sprite sprite)
		{
			GameObject go    = AddChild(parent);
			Image      image = go.AddComponent<Image>();
			image.sprite     = sprite;
			image.SetNativeSize();
            image.rectTransform.localScale = Vector3.one;
            image.rectTransform.localEulerAngles = Vector3.zero;

			return image;
		}


		public static Sprite LoadSprite(string spriteName, string dir = "Framework/Sprites/")
		{
			return Resources.Load<Sprite>(dir + spriteName);
		}


		public static GameObject AddChild(GameObject parent)
		{
			ADebug.Assert(parent != null);

			GameObject go           = new GameObject();
			Transform  transform    = go.transform;
			transform.SetParent(parent.transform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale    = Vector3.one;
			go.layer                = parent.layer;

			return go;
		}

		public static GameObject AddChild(GameObject parent, GameObject prefab)
		{
			GameObject go = UnityEngine.Object.Instantiate(prefab) as GameObject;
			ADebug.Assert(go != null && parent != null);

			Transform transform     = go.transform;
			transform.SetParent(parent.transform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale    = Vector3.one;
			go.layer                = parent.layer;

			return go;
		}

		public static T AddCollider<T>(GameObject go, bool isDynamic) where T : Collider
		{
			T local = go.AddComponent<T>();
			if (isDynamic && (go.GetComponent<Rigidbody>() == null))
			{
				Rigidbody rigidbody = go.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				rigidbody.useGravity  = false;
			}

			return local;
		}

		public static BoxCollider AddFullScreenCollider(Dialog dialog)
		{
			GameObject  scrim    = dialog.scrim;
			BoxCollider collider = scrim.GetComponent<BoxCollider>();

			if (collider == null) 
			{
				collider        = AddCollider<BoxCollider>(scrim, false);
				collider.size   = instance.fullScreenDimensions;
				collider.center = new Vector3(0f, 0f, 0.02f);
			}

			return collider;
		}

        
        public GameObject CreateDooberForResource(string resourceName, Transform beginning, Transform destination, float uiOffset, int index = 0)
        {
            GameObject go   = AUIManager.AddChild(AUIManager.instance.uiCamera.gameObject);
            go.name         = "Doober_" + resourceName;
            Doober doober = go.AddComponent<Doober>();

            if (beginning.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                go.transform.position = beginning.position;
                doober.isDisperseOnMap = false;
            }
            else
            {
                go.transform.position = ACamera.WorldToUI(beginning.position, uiOffset);
                doober.isDisperseOnMap = true;
            }

            doober.resourceType = resourceName;
            doober.destination  = destination;
            doober.index        = index;

            return go;
        }
	}
}