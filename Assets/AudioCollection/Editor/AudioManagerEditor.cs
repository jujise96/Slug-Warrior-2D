using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEditor;
using UnityEngine;
using Game.Audio;
using System.Linq;
using System.IO;
using Game.Utilities;

namespace Game.Audio.EditorExtensions
{
    [CustomEditor(typeof(AudioManager))]
    internal class AudioManagerEditor : ExtendedEditor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AudioManager audioManager = (AudioManager)target;

            EditorGUILayout.Space(10);
            DrawLabel("Will population List<BackgroundAudio> with all BackgroundAudioSO in the path above", Color.gray, isItalicized: true);
            if (GUILayout.Button("Set All Background Audio"))
            {
                List<BackgroundAudioSO> foundAssets= new List<BackgroundAudioSO>();

                AssetDatabase.Refresh();
                string[] assetPaths = Directory.GetFiles(audioManager.BackgroundAudioPath, "*.asset");
                foreach (string fileInfo in assetPaths) foundAssets.Add(AssetDatabase.LoadAssetAtPath<BackgroundAudioSO>(fileInfo.FormatAsUnityPath()));

                if (foundAssets != null && foundAssets.Count> 0)
                {
                    foreach (BackgroundAudioSO asset in foundAssets)
                    {
                        audioManager.BackgroundAudio.Add(asset);
                    }
                    UnityEngine.Debug.Log($"Successfully found {foundAssets.Count} assets at path {audioManager.BackgroundAudioPath} and added them all to {typeof(AudioManager)}.BackgroundAudio");
                }
                else UnityEngine.Debug.LogError($"Tried to population BackgroundAudio with {typeof(BackgroundAudioSO)}, but 0 assets were found at path {audioManager.BackgroundAudioPath}!");
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}
