using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCourse
{
    public class Course
    {
        private int course_ID;
        public int Course_ID
        {
            get { return course_ID; }
            set { course_ID = value; }
        }

        private string course_Code;
        public string Course_Code
        {
            get { return course_Code; }
            set { course_Code = value; }
        }

        private string course_Name;
        public string Course_Name
        {
            get { return course_Name; }
            set { course_Name = value; }
        }

        private int course_Period;  //课程课时
        public int Course_Period
        {
            get { return course_Period; }
            set { course_Period = value; }
        }

        private double course_Credit;  //课程学分
        public double Course_Credit
        {
            get { return course_Credit; }
            set { course_Credit = value; }
        }

        private string course_IsDegree;  //是否学位课
        public string Course_IsDegree
        {
            get { return course_IsDegree; }
            set { course_IsDegree = value; }
        }

        private string course_ExamType;  //考核方式
        public string Course_ExamType
        {
            get { return course_ExamType; }
            set { course_ExamType = value; }
        }

        private string course_Teacher;  //主讲教师
        public string Course_Teacher
        {
            get { return course_Teacher; }
            set { course_Teacher = value; }
        }

        private string course_TimeStr;  //上课时间字符串
        public string Course_TimeStr
        {
            get { return course_TimeStr; }
            set { course_TimeStr = value; }
        }

        private int course_Weekday;  //周几上课
        public int Course_Weekday
        {
            get { return course_Weekday; }
            set { course_Weekday = value; }
        }

        private int[] course_DaySpan;  //第几节
        public int[] Course_DaySpan
        {
            get { return course_DaySpan; }
            set { course_DaySpan = value; }
        }

        private string course_Classroom;  //教室
        public string Course_Classroom
        {
            get { return course_Classroom; }
            set { course_Classroom = value; }
        }

        private int[] course_Weeks;  //周次
        public int[] Course_Weeks
        {
            get { return course_Weeks; }
            set { course_Weeks = value; }
        }

    }
}
