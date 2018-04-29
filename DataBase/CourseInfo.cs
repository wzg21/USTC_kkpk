using System;
using System.Data;
using System.Data.SqlClient;

namespace TimelyEvaluation.DataBase
{
    public class CourseInfo
    {

        #region 数据结构
        DataBaseOperation database;
        private int _userid;
        private int _courseid;
        private string _coursename;
        private string _introduction;
        private Byte[] _tags;
        private int _active;
        #endregion

        #region 初始化
        public CourseInfo()
        {
            database = new DataBaseOperation();
            _userid = 0;
            _courseid = 0;
            _coursename = "";
            _introduction = "";
            _tags = new Byte[20];
            _active = 0;
        }
        /// <summary>
        /// 登录用户ID
        /// </summary>
        public int UserID
        {
            set { _userid = value; }
            get { return _userid; }
        }
        /// <summary>
        /// 课程ID
        /// </summary>
        public int CourseID
        {
            set { _courseid = value; }
            get { return _courseid; }
        }
        /// <summary>
        ///课程名称
        /// </summary>
        public string CourseName
        {
            set { _coursename = value; }
            get { return _coursename; }
        }
        /// <summary>
        /// 课程介绍
        /// </summary>
        public string Introduction
        {
            set { _introduction = value; }
            get { return _introduction; }
        }
        /// <summary>
        /// 课程标签
        /// </summary>
        public Byte[] Tags
        {
            set { _tags = value; }
            get { return _tags; }
        }
        /// <summary>
        /// 是否开课
        /// </summary>
        public int Active
        {
            set { _active = value; }
            get { return _active; }
        }
        #endregion

        #region 操作
        /// <summary>
        /// 按条件获取当前学期所有课程信息
        /// 返回值：Dataset。表名AllCourse。列名CourseID、CourseNumber、CourseName、TeachersName、CourseTime、isChosen(0未选、1已选)
        /// </summary>
        public DataSet GetAllCourse(string _coursenumber, string _coursename, string _teachersname)
        {
            int count = 1;
            if (_coursenumber != null)
            {
                count++;
            }
            if (_coursename != null)
            {
                count++;
            }
            if (_teachersname != null)
            {
                count++;
            }
            SqlParameter[] parameters = new SqlParameter[count];
            parameters[0] = database.CreateInputParameter("@UserID", SqlDbType.Int, -1, _userid);
            string sql = "SELECT CourseID,CourseNumber,CourseName,TeachersName,CourseTime,";
            sql += "(SELECT count(*) FROM CourseSelection WHERE UserID=@UserID AND CourseID=a.CourseID AND IdentityInfo=1) AS isChosen FROM Course a WHERE Active=1";
            count = 1;
            if (_coursenumber != null)
            {
                parameters[count] = database.CreateInputParameter("@CourseNumber", SqlDbType.VarChar, 50, _coursenumber);
                sql += " AND CourseNumber=@CourseNumber";
                count++;
            }
            if (_coursename != null)
            {
                _coursename = "%" + _coursename + "%";
                parameters[count] = database.CreateInputParameter("@CourseName", SqlDbType.VarChar, 50, _coursename);
                sql += " AND CourseName LIKE @CourseName";
                count++;
            }
            if (_teachersname != null)
            {
                _teachersname = "%" + _teachersname + "%";
                parameters[count] = database.CreateInputParameter("@TeachersName", SqlDbType.VarChar, 200, _teachersname);
                sql += " AND TeachersName LIKE @TeachersName";
                count++;
            }
            return (database.QueryWithReturnValue(sql, parameters, "AllCourse"));
        }

        /// <summary>
        /// 判断用户UserID是否是课程CourseID的老师/助教
        /// 返回值：0不是、1是
        /// </summary>
        public int IsCourseTeacher()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            DataSet ds = database.QueryWithReturnValue("SELECT IdentityInfo FROM CourseSelection WHERE UserID=@UserID AND CourseID=@CourseID", parameters);
            if (0 == ds.Tables[0].Rows.Count)
            {
                return 0;
            }
            else
            {
                int identity = Convert.ToInt32(ds.Tables[0].Rows[0]["IdentityInfo"]);
                if (0 == identity || 2 == identity) 
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 学生UserID选择课程CourseID
        /// 返回值：0选择失败、1选择成功
        /// </summary>
        public int SelectCourse()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            return (database.QueryWithoutReturnValue("INSERT INTO CourseSelection VALUES (@UserID,@CourseID,1)", parameters));
        }

        /// <summary>
        /// 学生/老师UserID删除课程CourseID
        /// 返回值：0删除失败、1删除成功
        /// </summary>
        public int DropCourse()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            return (database.QueryWithoutReturnValue("DELETE FROM CourseSelection WHERE UserID=@UserID AND CourseID=@CourseID", parameters));
        }

        /// <summary>
        /// 学生UserID成为课程CourseID的助教
        /// 返回值：0失败、1成功
        /// </summary>
        public int SelectCourseAssistant()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            return (database.QueryWithoutReturnValue("INSERT INTO CourseSelection VALUES (@UserID,@CourseID,2)", parameters));
        }

        /// <summary>
        /// 取消学生UserID在课程CourseID的助教身份
        /// 返回值：0删除失败、1删除成功
        /// </summary>
        public int DropCourseAssistant()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            return (database.QueryWithoutReturnValue("DELETE FROM CourseSelection WHERE UserID=@UserID AND CourseID=@CourseID AND IdentityInfo=2", parameters));
        }

        /// <summary>
        /// 根据CourseID获取课程信息填充到成员变量中
        /// 返回值：0获取失败、1获取成功
        /// </summary>
        public int GetCourseInfo()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            DataSet ds = new DataSet();
            ds = database.QueryWithReturnValue("SELECT * FROM Course WHERE CourseID=@CourseID", parameters, "CourseInfo");
            if (1 != ds.Tables["CourseInfo"].Rows.Count)
            {
                return 0;
            }
            _coursename = (string)ds.Tables["CourseInfo"].Rows[0]["CourseName"];
            _introduction = (string)ds.Tables["CourseInfo"].Rows[0]["Introduction"];
            _tags = (Byte[])ds.Tables["CourseInfo"].Rows[0]["Tags"];
            _active = (int)ds.Tables["CourseInfo"].Rows[0]["Active"];
            return 1;
        }

        /// <summary>
        /// 添加课程（提供课程名CourseName和简介Introduction）
        /// 返回值：0添加失败、1添加成功
        /// </summary>
        //public int AddCourseInfo()
        //{
        //    Byte[] emptytag=new Byte[20];
        //    for(int i = 0; i < 20; i++)
        //    {
        //        emptytag[i] = 0;
        //    }
        //    SqlParameter[] parameters = {
        //        database.CreateInputParameter("@CourseName",  SqlDbType.VarChar, 50, _coursename),
        //        database.CreateInputParameter("@Introduction",  SqlDbType.VarChar, 200, _introduction),
        //        database.CreateInputParameter("@Tags",  SqlDbType.Binary, 20, emptytag)
        //    };
        //    //已开课的课程
        //    return (database.QueryWithoutReturnValue("INSERT INTO Course VALUES (@CourseName,@Introduction,@Tags,1)", parameters));
        //}

        /// <summary>
        /// 用成员变量中的值修改课程CourseID的课程名CourseName和课程简介Introduction
        /// 返回值：0修改失败、1修改成功
        /// </summary>
        public int ChangeCourseInfo()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid),
                database.CreateInputParameter("@CourseName",  SqlDbType.VarChar, 50, _coursename),
                database.CreateInputParameter("@Introduction",  SqlDbType.VarChar, 200, _introduction)
            };
            return (database.QueryWithoutReturnValue("UPDATE Course SET CourseName=@CourseName,Introduction=@Introduction WHERE CourseID=@CourseID", parameters));
        }

        /// <summary>
        /// 根据CourseID获取(已注册的)老师和助教的信息
        /// 返回值：Dataset。表名TeacherName、AssistantName。列名UserID,UserName
        /// </summary>
        public DataSet GetCourseTeacher()
        {
            SqlParameter[] parameters1 = {
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            SqlParameter[] parameters2 = {
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            string sql1 = "SELECT a.UserID,b.UserName FROM CourseSelection a INNER JOIN UserAccount b ON a.UserID=b.UserID WHERE CourseID=@CourseID AND IdentityInfo=0";
            string sql2 = "SELECT a.UserID,b.UserName FROM CourseSelection a INNER JOIN UserAccount b ON a.UserID=b.UserID WHERE CourseID=@CourseID AND IdentityInfo=2";
            return (database.QueryWithReturnValue(sql1, sql2, parameters1, parameters2, "TeacherName", "AssistantName"));
        }

        /// <summary>
        /// 根据CourseID获取课程评论 与 评论赞的数量
        /// 返回值：Dataset。表名CourseComment。列名CommentID,UserID,UserName,PhotoUrl,Tags,Content,CreateDate,LikeCounts,RealUserID
        /// </summary>
        public DataSet GetCourseComment()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            string sql = "SELECT CommentID,a.UserID,UserName,PhotoUrl,Tags,MyContent AS Content,a.CreateDate,(SELECT count(*) FROM Likes WHERE CommentID=a.CommentID) AS LikeCounts,RealUserID ";
            sql += "FROM Comment a INNER JOIN UserAccount b ON a.UserID=b.UserID WHERE CourseID=@CourseID ";
            sql += "ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "CourseComment"));
        }

        /// <summary>
        /// 根据UserID、CourseID获取特定用户的课程评论 与 评论赞的数量
        /// 返回值：Dataset。表名CourseComment。列名CommentID,UserID,UserName,PhotoUrl,Tags,Content,CreateDate,LikeCounts,RealUserID
        /// </summary>
        public DataSet GetMyCourseComment()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            string sql = "SELECT CommentID,a.UserID,UserName,PhotoUrl,Tags,MyContent AS Content,a.CreateDate,(SELECT count(*) FROM Likes WHERE CommentID=a.CommentID) AS LikeCounts,RealUserID ";
            sql += "FROM Comment a INNER JOIN UserAccount b ON a.UserID=b.UserID WHERE RealUserID=@UserID AND CourseID=@CourseID";
            return (database.QueryWithReturnValue(sql, parameters, "CourseComment"));
        }

        /// <summary>
        /// 根据UserID、CourseID获取其他用户的课程评论 与 评论赞的数量
        /// 返回值：Dataset。表名CourseComment。列名CommentID,UserID,UserName,PhotoUrl,Tags,Content,CreateDate,LikeCounts,RealUserID
        /// </summary>
        public DataSet GetOthersCourseComment()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            string sql = "SELECT CommentID,a.UserID,UserName,PhotoUrl,Tags,MyContent AS Content,a.CreateDate,(SELECT count(*) FROM Likes WHERE CommentID=a.CommentID) AS LikeCounts,RealUserID ";
            sql += "FROM Comment a INNER JOIN UserAccount b ON a.UserID=b.UserID WHERE CourseID=@CourseID AND a.RealUserID!=@UserID ";
            sql += "ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "CourseComment"));
        }

        /// <summary>
        /// 根据UserID、CourseID获取自己发表的课程建议(学生使用),按时间降序排列
        /// 返回值：Dataset。表名CourseAdvice。列名AdviceID,UserID,UserName,PhotoUrl,Content,CreateDate,RealUserID
        /// </summary>
        public DataSet GetMyCourseAdvice()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            string sql = "SELECT AdviceID,a.UserID,UserName,PhotoUrl,MyContent AS Content,a.CreateDate,a.RealUserID ";
            sql += "FROM Advice a INNER JOIN UserAccount b ON a.UserID=b.UserID ";
            sql += "WHERE a.RealUserID=@UserID AND CourseID=@CourseID ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "CourseAdvice"));
        }

        /// <summary>
        /// 根据UserID、CourseID获取所有未屏蔽自己的课程建议(老师与助教使用),按时间降序排列
        /// 返回值：Dataset。表名CourseAdvice。列名AdviceID,UserID,UserName,PhotoUrl,Content,CreateDate,RealUserID
        /// </summary>
        public DataSet GetCourseAdvice()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1, _userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1, _courseid)
            };
            string sql = "SELECT a.AdviceID,a.UserID,UserName,PhotoUrl,MyContent AS Content,a.CreateDate,a.RealUserID ";
            sql += "FROM Advice a INNER JOIN UserAccount b ON a.UserID=b.UserID LEFT JOIN AdviceShielded c ON a.AdviceID=c.AdviceID AND c.UserID=@UserID ";
            sql += "WHERE CourseID=@CourseID AND c.UserID is null ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "CourseAdvice"));
        }
        #endregion
    }
}
