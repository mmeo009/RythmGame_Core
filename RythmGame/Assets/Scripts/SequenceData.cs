using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;                  // JSON 라이브러리를 가져옴

[CreateAssetMenu(fileName = "NewSequence", menuName = "Sequencer/Secuence")]        // 생성 파일 메뉴에 추가 시켜준다.
public class SequenceData : ScriptableObject
{
    public int bpm;                                                 // 음악의 BPM
    public int numberOfTracks;                                      // 노트 트랙 수
    public AudioClip audioClip;                                     // 오디오 클립
    public List<List<int>> trackNotes = new List<List<int>>();      // 2차원 데이터 정보                  
    public TextAsset trackJsonFile;                                 // .Json 파일 텍스트 에셋

    public void SaveToJson()
    {
        if(trackJsonFile == null)
        {
            Debug.LogError("Track JSON 파일이 없습니다.");
            return;
        }

        var data = JsonConvert.SerializeObject(new
        {
            bpm,
            numberOfTracks,
            audioClipPath = AssetDatabase.GetAssetPath(audioClip),
            trackNotes
        },Formatting.Indented);         // JSON 변환

        System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(trackJsonFile), data);       // 파일에 JSON을 쓴다.
        AssetDatabase.Refresh();                                                            // 완료 후 리프레시
    }

    public void LoadFromJson()
    {
        if(trackJsonFile == null)
        {
            Debug.LogError("Track JSON 파일이 없습니다.");
            return;
        }

        var data = JsonConvert.DeserializeAnonymousType(trackJsonFile.text, new
        {
            bpm = 0,
            numberOfTracks =0,
            AudioClipPath = "",
            trackNotes = new List<List<int>>()
        });                                     // JSON은 복호화하여 받아온다. data에 저장

        bpm = data.bpm;
        numberOfTracks = data.numberOfTracks;
        audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(data.AudioClipPath);
        trackNotes = data.trackNotes;
        
    }
}

#if UNITY_EDITOR            // 전처리기 : 해당 상황에만 (UnityEditor에서만)
[CustomEditor(typeof(SequenceData))]            // SequenceData의 에디터를 수정 하겠다.
public class SequenceDataEditor : Editor
{
    public override void OnInspectorGUI()       // 기존 OnInspectorGUI함수를 재정의
    {
        var sequenceData = (SequenceData)target;// SequenceData의 Editor를 타겟으로 수정한다.

        DrawDefaultInspector();                 // 인스펙터 표시

        if(sequenceData != null)
        {
            EditorGUILayout.LabelField("Track Notes", EditorStyles.boldLabel);
            for(int i = 0; i < sequenceData.trackNotes.Count; i ++)
            {
                EditorGUILayout.LabelField($"Track {i + 1} : [{string.Join(",", sequenceData.trackNotes[i])}]");
            }
        }

        if (GUILayout.Button("Load from JSON")) sequenceData.LoadFromJson();
        if (GUILayout.Button("Save to JSON")) sequenceData.SaveToJson();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(sequenceData);
        }

    }
}

#endif
