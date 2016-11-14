using System.IO;
using UnityEngine;

/**
	data storage which can be hacked easily.
	do not use severe data. (should use with some kind of encryption.)
*/
namespace AutoyaFramework.Persistence.Files {
    public class FilePersistence {
		private readonly string basePath;

		public FilePersistence (string basePathSource) {
			this.basePath = basePathSource;
		}
		
		/*
			sync series.
		*/
		public bool Update (string domain, string fileName, string data) {
			var domainPath = Path.Combine(basePath, domain);
			if (Directory.Exists(domainPath)) {

				var filePath = Path.Combine(domainPath, fileName);
				using (var sw = new StreamWriter(filePath, true))	{
					sw.Write(data);
				}
				
				return true;
			} else {// no directory = domain exists.
				var created = Directory.CreateDirectory(domainPath);
				
				if (created.Exists) {
					
					#if UNITY_IOS
					{
						UnityEngine.iOS.Device.SetNoBackupFlag(domainPath);
					}
					#endif

					var filePath = Path.Combine(domainPath, fileName);
					using (var sw = new StreamWriter(filePath, true)) {
						sw.Write(data);
					}
					return true;
				} 
			}
			return false;
		}

		public string[] FileNamesInDomain (string domain) {
			var domainPath = Path.Combine(basePath, domain);
			if (Directory.Exists(domainPath)) {
				var filePaths = Directory.GetFiles(domainPath);
				
				if (filePaths.Length == 0) {
					return new string[]{};
				}

				var fileNames = new string[filePaths.Length];
				for (var i = 0; i < filePaths.Length; i++) {
					var fileName = filePaths[i];
					fileNames[i] = Path.GetFileName(fileName);
				}
				return fileNames;
			}
			return new string[]{};
		}

		public string Load (string domain, string fileName) {
			var domainPath = Path.Combine(basePath, domain);
			if (Directory.Exists(domainPath)) {
				var filePath = Path.Combine(domainPath, fileName);
				if (File.Exists(filePath)) {
					using (var sr = new StreamReader(filePath))	{
						return sr.ReadToEnd();
					}
				}
			}
			return string.Empty;
		}

		public bool Delete (string domain, string fileName) {
			var domainPath = Path.Combine(basePath, domain);
			if (Directory.Exists(domainPath)) {
				var filePath = Path.Combine(domainPath, fileName);
				if (File.Exists(filePath)) {
					File.Delete(filePath);
					return true;
				}
			}
			return false;
		}

		public bool DeleteByDomain (string domain) {
			var domainPath = Path.Combine(basePath, domain);
			if (Directory.Exists(domainPath)) {
				Directory.Delete(domainPath, true);
				return true;
			}
			return false;
		}

		// /*
		// 	async series.
		// */
		// public void UpdateAsync (string domain, string fileName, string data, Action<bool> result) {
		// 	var domainPath = Path.Combine(basePath, domain);
		// 	if (Directory.Exists(domainPath)) {

		// 		var filePath = Path.Combine(domainPath, fileName);
		// 		using (var sw = new StreamWriter(filePath, false))	{
		// 			sw.WriteLine(data);
		// 		}

		// 		result(true);
		// 	} else {// no directory = domain exists.
		// 		var created = Directory.CreateDirectory(domainPath);
				
		// 		if (created.Exists) {

		// 			#if UNITY_IOS
		// 			{
        //                 UnityEngine.iOS.Device.SetNoBackupFlag(domainPath);
		// 			}
		// 			#endif

		// 			var filePath = Path.Combine(domainPath, fileName);
		// 			using (var sw = new StreamWriter(filePath, false))	{
		// 				sw.WriteLine(data);
		// 			}
		// 			result(true);
		// 		} 
		// 	}
		// 	result(false);
		// }

		// public void LoadAsync (string domain, string fileName, Action<string> resultData) {
		// 	var domainPath = Path.Combine(basePath, domain);
		// 	if (Directory.Exists(domainPath)) {
		// 		var filePath = Path.Combine(domainPath, fileName);
		// 		if (File.Exists(filePath)) {
		// 			using (var sr = new StreamReader(filePath))	{
		// 				resultData(sr.ReadLine());
		// 			}
		// 		}
		// 	}
		// 	resultData(string.Empty);
		// }

		// public void DeleteAsync (string domain, string fileName, Action<bool> result) {
		// 	var domainPath = Path.Combine(basePath, domain);
		// 	if (Directory.Exists(domainPath)) {
		// 		var filePath = Path.Combine(domainPath, fileName);
		// 		if (File.Exists(filePath)) {
		// 			File.Delete(filePath);
		// 			result(true);
		// 		}
		// 	}
		// 	result(false);
		// }

		// public void DeleteByDomainAsync (string domain, Action<bool> result) {
		// 	var domainPath = Path.Combine(basePath, domain);
		// 	if (Directory.Exists(domainPath)) {
		// 		Directory.Delete(domainPath, true);
		// 		result(true);
		// 	}
		// 	result(false);
		// }
	}
}


	/*
		iOS-Keychain Persistence is under consideration.
	*/