using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	micro main thread dispatcher for Autoya.
*/
namespace AutoyaFramework {
	public class AutoyaMainThreadDispatcher : MonoBehaviour, ICoroutineUpdater {
		private List<IEnumerator> coroutines = new List<IEnumerator>();
		
		public void Commit (IEnumerator iEnum) {
			coroutines.Add(iEnum);
		}

		private void Update () {
			
			if (0 < coroutines.Count) {
				foreach (var coroutine in coroutines) {
					// Debug.Log("commiting:" + coroutine);
					StartCoroutine(coroutine);
				}
				coroutines.Clear();
			}
		}

		/**
			automatically destory this gameObject.
		*/
		private void OnApplicationQuit () {
			Destroy(gameObject);
		}
		
		public void Destroy () {
			GameObject.Destroy(gameObject);
		}
	}

	/**
		Update() runner class for Editor.
	*/
	public class EditorUpdator : ICoroutineUpdater {
		private List<IEnumerator> readyCoroutines = new List<IEnumerator>();

		public EditorUpdator () {
			#if UNITY_EDITOR
			{
				UnityEditor.EditorApplication.update += this.EditorCoroutineUpdate;
			}
			#endif
		}

		public void Commit(IEnumerator iEnum) {
			readyCoroutines.Add(iEnum);
		}

		private List<IEnumerator> runningCoroutines = new List<IEnumerator>();
		private List<IEnumerator> finishedCoroutines = new List<IEnumerator>();

		private void EditorCoroutineUpdate () {
			// run coroutines.
			{
				foreach (var runningCoroutine in runningCoroutines) {
					if (!runningCoroutine.MoveNext()) {
						finishedCoroutines.Add(runningCoroutine);
					}
				}

				foreach (var finishedCoroutine in finishedCoroutines) {
					runningCoroutines.Remove(finishedCoroutine);
				}

				finishedCoroutines.Clear();
			}

			// add new coroutines.
			if (0 < readyCoroutines.Count) {
				foreach (var coroutine in readyCoroutines) {
					// Debug.Log("commiting:" + coroutine);
					runningCoroutines.Add(coroutine);
				}
				readyCoroutines.Clear();
			}
		}

		public void Destroy () {
			// do nothing.
		}
	}
}