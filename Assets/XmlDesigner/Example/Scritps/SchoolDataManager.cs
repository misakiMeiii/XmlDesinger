using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

namespace XML
{
	public class SchoolDataManager
	{
		public static School ReadFromFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				var doc = new XmlDocument();
				doc.Load(filePath);
				var rootNode = doc.SelectSingleNode("School");
				if (rootNode == null)
				{
					Debug.LogError("找不到根节点:School");
					return null;
				}
				return GetSchoolData(rootNode);
			}
			Debug.LogError("找不到文件,路径:" + filePath);
			return null;
		}
		
		public static School ReadFromString(string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				var doc = new XmlDocument();
				doc.LoadXml(content);
				var rootNode = doc.SelectSingleNode("School");
				if (rootNode == null)
				{
					Debug.LogError("找不到根节点:School");
					return null;
				}
				return GetSchoolData(rootNode);
			}
			Debug.LogError("读取的内容为空,请检查！");
			return null;
		}
		
		public static void ExportXml(School school, string exportFolder, string xmlName)
		{
			if (school == null)
			{
				Debug.LogError("导出失败,school为空,请检查！");
				return;
			}
			var doc = new XmlDocument();
			doc.CreateXmlDeclaration("1.0", "utf - 8", null);
			doc.AppendChild(GetNodeFromSchool(doc, school));
			doc.Save(exportFolder + (xmlName.EndsWith(".xml") ? xmlName : xmlName + ".xml"));
		}
		private static School GetSchoolData(XmlNode node)
		{
			var school = new School();
			foreach (XmlNode childNode in node)
			{
				switch (childNode.Name)
				{
					case "ClassGradeDic":
						var classGradeDicKey = Convert.ToInt32(childNode.SelectSingleNode("ClassGradeDicKey").InnerText);
						var classGradeDicValue = GetClassGradeData(childNode.SelectSingleNode("ClassGradeDicValue"));
						if (school.ClassGradeDic.ContainsKey(classGradeDicKey))
						{
							Debug.LogError("key重复无法插入!key:" + classGradeDicKey);
						}
						else
						{
							school.ClassGradeDic.Add(classGradeDicKey, classGradeDicValue);
						}
						break;
				}
			}
			return school;
		}
		private static ClassGrade GetClassGradeData(XmlNode node)
		{
			var classGrade = new ClassGrade();
			XmlAttribute attr;
			attr = node.Attributes["ClassName"];
			if (attr != null)
			{
				classGrade.ClassName = attr.Value;
			}
			foreach (XmlNode childNode in node)
			{
				switch (childNode.Name)
				{
					case "HeadTeacher":
						classGrade.HeadTeacher = GetTeacherData(childNode);
						break;
					case "Students":
						classGrade.Students.Add(GetStudentData(childNode));
						break;
				}
			}
			return classGrade;
		}
		private static Student GetStudentData(XmlNode node)
		{
			var student = new Student();
			XmlAttribute attr;
			attr = node.Attributes["Name"];
			if (attr != null)
			{
				student.Name = attr.Value;
			}
			attr = node.Attributes["Age"];
			if (attr != null)
			{
				if(!int.TryParse(attr.Value, out student.Age))
				{
					Debug.LogError($"无法将 Age 转换为 int: {attr.Value}. 出错节点: {node.Name}, 完整 XML: {node.OuterXml}");
				}
			}
			attr = node.Attributes["Sex"];
			if (attr != null)
			{
				student.Sex = attr.Value;
			}
			return student;
		}
		private static Teacher GetTeacherData(XmlNode node)
		{
			var teacher = new Teacher();
			XmlAttribute attr;
			attr = node.Attributes["Name"];
			if (attr != null)
			{
				teacher.Name = attr.Value;
			}
			attr = node.Attributes["Age"];
			if (attr != null)
			{
				if(!int.TryParse(attr.Value, out teacher.Age))
				{
					Debug.LogError($"无法将 Age 转换为 int: {attr.Value}. 出错节点: {node.Name}, 完整 XML: {node.OuterXml}");
				}
			}
			attr = node.Attributes["Sex"];
			if (attr != null)
			{
				teacher.Sex = attr.Value;
			}
			attr = node.Attributes["Subject"];
			if (attr != null)
			{
				teacher.Subject = attr.Value;
			}
			return teacher;
		}
		private static XmlNode GetNodeFromSchool(XmlDocument doc, School school)
		{
			var schoolNode = doc.CreateElement("School");
			
			foreach(var kvp in school.ClassGradeDic)
			{
				var classGradeDicKeyNode = doc.CreateElement("ClassGradeDicKey");
				classGradeDicKeyNode.InnerText = kvp.Key.ToString();
				var classGradeDicValueNode = doc.CreateElement("ClassGradeDicValue");
				classGradeDicValueNode.AppendChild(GetNodeFromClassGrade(doc, kvp.Value));
				schoolNode.AppendChild(classGradeDicKeyNode);
				schoolNode.AppendChild(classGradeDicValueNode);
			}
			
			return schoolNode;
		}
		private static XmlNode GetNodeFromClassGrade(XmlDocument doc, ClassGrade classGrade)
		{
			var classGradeNode = doc.CreateElement("ClassGrade");
			
			var classNameAb = doc.CreateAttribute("ClassName");
			classNameAb.InnerText = classGrade.ClassName;
			classGradeNode.SetAttributeNode(classNameAb);
			
			classGradeNode.AppendChild(GetNodeFromTeacher(doc, classGrade.HeadTeacher));
			
			foreach (var studentsValue in classGrade.Students)
			{
				classGradeNode.AppendChild(GetNodeFromStudent(doc, studentsValue));
			}
			
			return classGradeNode;
		}
		private static XmlNode GetNodeFromStudent(XmlDocument doc, Student student)
		{
			var studentNode = doc.CreateElement("Student");
			
			var nameAb = doc.CreateAttribute("Name");
			nameAb.InnerText = student.Name;
			studentNode.SetAttributeNode(nameAb);
			
			var ageAb = doc.CreateAttribute("Age");
			ageAb.InnerText = student.Age.ToString();
			studentNode.SetAttributeNode(ageAb);
			
			var sexAb = doc.CreateAttribute("Sex");
			sexAb.InnerText = student.Sex;
			studentNode.SetAttributeNode(sexAb);
			
			return studentNode;
		}
		private static XmlNode GetNodeFromTeacher(XmlDocument doc, Teacher teacher)
		{
			var teacherNode = doc.CreateElement("Teacher");
			
			var nameAb = doc.CreateAttribute("Name");
			nameAb.InnerText = teacher.Name;
			teacherNode.SetAttributeNode(nameAb);
			
			var ageAb = doc.CreateAttribute("Age");
			ageAb.InnerText = teacher.Age.ToString();
			teacherNode.SetAttributeNode(ageAb);
			
			var sexAb = doc.CreateAttribute("Sex");
			sexAb.InnerText = teacher.Sex;
			teacherNode.SetAttributeNode(sexAb);
			
			var subjectAb = doc.CreateAttribute("Subject");
			subjectAb.InnerText = teacher.Subject;
			teacherNode.SetAttributeNode(subjectAb);
			
			return teacherNode;
		}
	}
}
