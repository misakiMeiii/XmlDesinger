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
		
		public static School GetSchoolData(XmlNode node)
		{
			var school = new School();
			foreach (XmlNode childNode in node)
			{
				switch (childNode.Name)
				{
					case "ClassDic":
						var classDicKey = Convert.ToInt32(childNode.SelectSingleNode("ClassDicKey").InnerText);
						var classDicValue = GetClassGradeData(childNode.SelectSingleNode("ClassDicValue"));
						if (school.ClassDic.ContainsKey(classDicKey))
						{
							Debug.LogError("key重复无法插入!key:" + classDicKey);
						}
						else
						{
							school.ClassDic.Add(classDicKey, classDicValue);
						}
						break;
				}
			}
			return school;
		}
		public static ClassGrade GetClassGradeData(XmlNode node)
		{
			var classGrade = new ClassGrade();
			foreach (XmlNode childNode in node)
			{
				switch (childNode.Name)
				{
					case "Students":
						classGrade.Students.Add(GetStudentData(childNode));
						break;
				}
			}
			return classGrade;
		}
		public static Student GetStudentData(XmlNode node)
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
				student.Age = Convert.ToInt32(attr.Value);;
			}
			attr = node.Attributes["Sex"];
			if (attr != null)
			{
				student.Sex = attr.Value;
			}
			return student;
		}
	}
}
