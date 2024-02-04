using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XML;

public class XmlDesignerExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var xmlFilePath = Application.dataPath + "/XmlDesigner/Example/XmlData/school.xml";
        var schoolData = SchoolDataManager.ReadFromFile(xmlFilePath);

        foreach (var classDic in schoolData.ClassGradeDic)
        {
            Debug.Log($"班级名：{classDic.Value.ClassName}");
            Debug.Log(
                $"班主任：{classDic.Value.HeadTeacher.Name},年龄：{classDic.Value.HeadTeacher.Age},性别：{classDic.Value.HeadTeacher.Sex},学科：{classDic.Value.HeadTeacher.Subject}");
            foreach (var student in classDic.Value.Students)
            {
                Debug.Log($"学生姓名：{student.Name},年龄：{student.Age},性别：{student.Sex}");
            }
        }

        var classGrade = new ClassGrade();
        classGrade.ClassName = "三班";
        classGrade.HeadTeacher = new Teacher
        {
            Name = "罗老师",
            Age = 35,
            Sex = "男",
            Subject = "化学"
        };
        classGrade.Students.Add(new Student
        {
            Name = "小明",
            Age = 16,
            Sex = "男"
        });

        schoolData.ClassGradeDic.Add(3, classGrade);
        
        SchoolDataManager.ExportXml(schoolData,Application.dataPath + "/XmlDesigner/Example/XmlData/","newschool.xml");
    }
}