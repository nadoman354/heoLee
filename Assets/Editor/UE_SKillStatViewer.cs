// WeaponDataEditor.cs (Assets/Editor 폴더 안)
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_SkillMetaData), true)]
public class UE_SKillStatViewer : Editor
{
    SerializedProperty relicProp;

    void OnEnable()
    {
        relicProp = serializedObject.FindProperty("stat");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // WeaponData 기본 필드
        DrawPropertiesExcluding(serializedObject, "m_Script", "stat");

        // 참조 SO 선택 필드
        EditorGUILayout.PropertyField(relicProp);

        // 참조된 RelicData의 특정 속성만 인라인으로 그리기
        var stat = relicProp.objectReferenceValue as SO_StatData;
        if (stat != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Relic (Preview)", EditorStyles.boldLabel);

            var so = new SerializedObject(stat);
            so.Update();

            // 원하는 필드만 선택적으로 표시
            EditorGUILayout.PropertyField(so.FindProperty("statList"));
            // icon 같은 건 미리보기만 하고 수정 막고 싶다면:
            //using (new EditorGUI.DisabledScope(true))
            //    EditorGUILayout.PropertyField(so.FindProperty("icon"));

            if (so.ApplyModifiedProperties())
                EditorUtility.SetDirty(stat); // 값 변경 저장 안전장치
        }

        serializedObject.ApplyModifiedProperties();
    }
}
