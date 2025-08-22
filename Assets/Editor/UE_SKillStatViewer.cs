// WeaponDataEditor.cs (Assets/Editor ���� ��)
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

        // WeaponData �⺻ �ʵ�
        DrawPropertiesExcluding(serializedObject, "m_Script", "stat");

        // ���� SO ���� �ʵ�
        EditorGUILayout.PropertyField(relicProp);

        // ������ RelicData�� Ư�� �Ӽ��� �ζ������� �׸���
        var stat = relicProp.objectReferenceValue as SO_StatData;
        if (stat != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Relic (Preview)", EditorStyles.boldLabel);

            var so = new SerializedObject(stat);
            so.Update();

            // ���ϴ� �ʵ常 ���������� ǥ��
            EditorGUILayout.PropertyField(so.FindProperty("statList"));
            // icon ���� �� �̸����⸸ �ϰ� ���� ���� �ʹٸ�:
            //using (new EditorGUI.DisabledScope(true))
            //    EditorGUILayout.PropertyField(so.FindProperty("icon"));

            if (so.ApplyModifiedProperties())
                EditorUtility.SetDirty(stat); // �� ���� ���� ������ġ
        }

        serializedObject.ApplyModifiedProperties();
    }
}
