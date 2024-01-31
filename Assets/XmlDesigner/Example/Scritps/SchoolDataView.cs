using System;
using System.Collections.Generic;

namespace XML
{
	public class School
	{
		public Dictionary<int, ClassGrade> ClassDic = new Dictionary<int, ClassGrade>();
	}
	public class ClassGrade
	{
		public List<Student> Students = new List<Student>();
	}
	
	public class Student
	{
		public string Name;
		public int Age = 16;
		public string Sex = "男";
	}
}
