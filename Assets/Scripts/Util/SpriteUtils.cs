#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Util
{
    public class SpriteUtils : MonoBehaviour
    {
        [MenuItem("SpriteUtils/Пiгнали")]
        public static void CompressSprites()
        {
            ForAllImagesInFolder(Application.dataPath, PNG2ETC);
            Debug.Log("PNG2ETC done.");
            
            
        }

        [MenuItem("SpriteUtils/MaxSizeCut")]
        public static void MaxSizeCut()
        {
            ForAllImagesInFolder(Application.dataPath, CutImageMaxSize);
            Debug.Log("Max size cut done.");
        }

        private static void ForAllImagesInFolder(string folderPath,Action<string> imageOperation)
        {
            if (!Directory.Exists(folderPath)) return;
            var pngFiles = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);
            foreach (var filePath in pngFiles) imageOperation?.Invoke(filePath);
        }

        private static void CutImageMaxSize(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileData = File.ReadAllBytes(filePath);
            
            var originalTexture = new Texture2D(2, 2);
            originalTexture.LoadImage(fileData);

            if (TryCutMaxSize(filePath,originalTexture, out var resized))
            {
                var resizedData = resized.EncodeToPNG();
                File.WriteAllBytes(filePath, resizedData);
                
                Debug.Log($"{fileName} {originalTexture.width} x {originalTexture.height} -> " +
                          $"{resized.width} x {resized.height}");
            }
            else
            {
                //Debug.Log($"{fileName} is OK");
            }
        }

        private static void PNG2ETC(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileData = File.ReadAllBytes(filePath);
            
            var original = new Texture2D(2, 2);
            original.LoadImage(fileData);

            if (IsEtc(original.width) && IsEtc(original.height))
            {
                //Debug.Log($"{fileName} is OK");
            }
            else
            {
                var rt = RenderTexture.GetTemporary(
                    GetEtc(original.width), 
                    GetEtc(original.height),
                    0, 
                    RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
            
                Graphics.Blit(original, rt);
                
                var resized = new Texture2D(GetEtc(original.width), GetEtc(original.height),original.format,false);
                
                resized.ReadPixels(new Rect(0, 0, GetEtc(original.width), GetEtc(original.height)), 0, 0);
                resized.Apply();
                
                Graphics.CopyTexture(rt, resized);
                
                RenderTexture.ReleaseTemporary(rt);
                
                var resizedData = resized.EncodeToPNG();
                File.WriteAllBytes(filePath, resizedData);
                
                Debug.Log($"{fileName} {original.width} x {original.height} -> " +
                          $"{resized.width} x {resized.height}");
            }
        }
        
        private static bool TryCutMaxSize(string path, Texture2D original,out Texture2D resized)
        {
            var maxSize = GetMaxSizeSetting(path);
            resized = null;
            
            if (maxSize == 0) return false;

            if (original.height <= maxSize && original.width <= maxSize) return false;

            var ratio = original.height / (float)original.width;
            
            int newHeight;
            int newWidth;
            
            if (original.height >= original.width)
            {
                newHeight = maxSize;
                newWidth = GetEtc((int)(maxSize / ratio));
            }
            else
            {
                newHeight = GetEtc((int)(maxSize * ratio));
                newWidth = maxSize;
            }

            var newPpuRatio = original.width / (float)newWidth;

            ApplyNewPPU(path, newPpuRatio);
            
            var rt = RenderTexture.GetTemporary(newWidth, newHeight, 
                0,RenderTextureFormat.Default,
                RenderTextureReadWrite.sRGB);
            
            Graphics.Blit(original, rt);
            
            resized = new Texture2D(newWidth, newHeight, original.format, false);
            
            resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            resized.Apply();
            
            Graphics.CopyTexture(rt, resized);
            
            RenderTexture.ReleaseTemporary(rt);
            
            return true;
        }
        
        private static int GetMaxSizeSetting(string filePath)
        {
            var relativePath = "Assets/"+Path.GetRelativePath(Application.dataPath, filePath);
            var assetImporter = AssetImporter.GetAtPath(relativePath);
            try
            {
                var importer = (TextureImporter)assetImporter;
                return importer != null ? importer.maxTextureSize : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static void ApplyNewPPU(string filePath, float ppuRatio)
        {
            EditorApplication.delayCall += () =>
            {
                var relativePath = "Assets/"+Path.GetRelativePath(Application.dataPath, filePath);
                var assetImporter = AssetImporter.GetAtPath(relativePath);
                try
                {
                    var importer = (TextureImporter)assetImporter;
                    Undo.RecordObject(importer, "Custom Texture Import Settings");
                    var textureSettings = new TextureImporterSettings();
                    importer.ReadTextureSettings(textureSettings);
                    textureSettings.spritePixelsPerUnit /= ppuRatio;
                    importer.SetTextureSettings(textureSettings);
                
                    EditorUtility.SetDirty(importer);

                    AssetDatabase.WriteImportSettingsIfDirty(relativePath);
                    AssetDatabase.SaveAssets();
                }
                catch (Exception)
                {
                    // ignored
                }
            };
            
        }
        
        private static int GetEtc(int number) => number - number % 4;
        private static bool IsEtc(int number) => number % 4 == 0;
    }
}

#endif
