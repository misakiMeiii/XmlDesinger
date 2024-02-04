using System;
using System.Collections.Generic;

namespace XML
{
	public class School
	{
		public Dictionary<int, ClassGrade> ClassGradeDic = new Dictionary<int, ClassGrade>();
	}
	public class ClassGrade
	{
		public string ClassName;
		public Teacher HeadTeacher = new Teacher();
		public List<Student> Students = new List<Student>();
	}
	
	public class Student
	{
		public string Name;
		public int Age = 16;
		public string Sex = "男";
	}
	
	public class Teacher
	{
		public string Name;
		public int Age;
		public string Sex;
		public string Subject;
	}
}
