using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace TimelyEvaluation.DataBase
{
    public class UserInfo
    {

        #region 数据结构
        DataBaseOperation database;
        private int _userid;
        private int _searchuserid;
        private string _username;
        private string _password;
        private string _email;
        private int _isstudent;
        private string _photourl;
        private string _personalprofile;
        #endregion

        #region 初始化
        public UserInfo()
        {
            database = new DataBaseOperation();
            _userid = 0;
            _searchuserid = 0;
            _username = "";
            _password = "";
            _email = "";
            _isstudent = -1;
            _photourl = "";
            _personalprofile = "";
        }
        /// <summary>
        /// 用户ID（只读）
        /// </summary>
        public int UserID
        {
            get { return _userid; }
        }
        /// <summary>
        /// 用户ID（用于查找个人信息时使用）
        /// </summary>
        public int SearchUserID
        {
            set { _searchuserid = value; }
            get { return _searchuserid; }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            set { _username = value; }
            get { return _username; }
        }
        /// <summary>
        ///密码
        /// </summary>
        public string Password
        {
            set { _password = value; }
            get { return _password; }
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            set { _email = value; }
            get { return _email; }
        }
        /// <summary>
        /// 是否是学生
        /// </summary>
        public int isStudent
        {
            set { _isstudent = value; }
            get { return _isstudent; }
        }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string PhotoUrl
        {
            set { _photourl = value; }
            get { return _photourl; }
        }
        /// <summary>
        /// 个人信息
        /// </summary>
        public string PersonalProfile
        {
            set { _personalprofile = value; }
            get { return _personalprofile; }
        }
        #endregion

        #region 操作
        /// <summary>
        /// 邮箱登录(需填写成员变量Email、Password)，登录成功自动将用户信息填写到成员变量中。
        /// 返回值：-1账号未激活、0用户不存在、1登录成功
        /// </summary>
        public int Login_Email()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@Email",  SqlDbType.VarChar, 50, _email),
                database.CreateInputParameter("@Password",  SqlDbType.VarChar, 50, _password)
            };
            DataSet ds = new DataSet();
            ds = database.QueryWithReturnValue("SELECT * FROM UserAccount WHERE Email=@Email AND Password=lower(right(sys.fn_varbintohexstr(hashbytes('MD5',@Password)),32))", parameters, "LoginUser");
            if (1 != ds.Tables["LoginUser"].Rows.Count)
            {
                return 0;
            }
            if (0 == (int)ds.Tables["LoginUser"].Rows[0]["Active"])
            {
                return -1;
            }
            _userid = (int)ds.Tables["LoginUser"].Rows[0]["UserID"];
            _username = (string)ds.Tables["LoginUser"].Rows[0]["UserName"];
            _isstudent = (int)ds.Tables["LoginUser"].Rows[0]["isStudent"];
            _photourl = (string)ds.Tables["LoginUser"].Rows[0]["PhotoUrl"];
            _personalprofile = (string)ds.Tables["LoginUser"].Rows[0]["PersonalProfile"];
            return 1;
        }

        /// <summary>
        /// 用户注册(只需填写成员变量UserName、Email、Password、isStudent)
        /// 返回值：-3邮件发送失败、-2教师姓名不存在、-1邮箱已被注册、0其他错误、1注册成功
        /// </summary>
        public int Register()
        {
            SqlParameter[] parameters1 = {
                database.CreateInputParameter("@Email", SqlDbType.VarChar,50,_email)
            };
            DataSet ds = new DataSet();
            ds = database.QueryWithReturnValue("SELECT * FROM UserAccount WHERE Email=@Email", parameters1,"SearchEmail");
            if (ds.Tables["SearchEmail"].Rows.Count > 0) 
            {
                return -1;
            }
            if (0 == _isstudent)
            {
                SqlParameter[] parameters2 = {
                    database.CreateInputParameter("@TeachersName", SqlDbType.VarChar,50,"%"+_username+"%")
                };
                DataSet myclass = database.QueryWithReturnValue("SELECT CourseID FROM Course WHERE TeachersName LIKE @TeachersName", parameters2, "SearchName");
                if (0 == myclass.Tables["SearchName"].Rows.Count)
                {
                    return -2;
                }
            }
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random randrom = new Random((int)DateTime.Now.Ticks);
            string verificationcode = "";
            for (int i = 0; i < 6; i++)
            {
                verificationcode += chars[randrom.Next(chars.Length)];
            }
            Guid activecode = Guid.NewGuid();
            SqlParameter[] parameters3 = {
                database.CreateInputParameter("@UserName", SqlDbType.VarChar,50,_username),
                database.CreateInputParameter("@Password", SqlDbType.VarChar,50,_password),
                database.CreateInputParameter("@Email", SqlDbType.VarChar,50,_email),
                database.CreateInputParameter("@isStudent", SqlDbType.Int,-1,_isstudent),
                database.CreateInputParameter("@PhotoUrl", SqlDbType.VarChar,100,""),
                database.CreateInputParameter("@PersonalProfile", SqlDbType.VarChar,1000,""),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,DateTime.Now),
                database.CreateInputParameter("@ActiveCode", SqlDbType.UniqueIdentifier,-1,activecode),
                database.CreateInputParameter("@VerificationCode", SqlDbType.VarChar,8,verificationcode)
            };
            ds = database.QueryWithReturnValue("INSERT INTO UserAccount VALUES (@UserName,lower(right(sys.fn_varbintohexstr(hashbytes('MD5',@Password)),32)),@Email,@isStudent,@PhotoUrl,@PersonalProfile,@CreateDate,0,@ActiveCode,@VerificationCode) SELECT scope_Identity()", parameters3);
            if (0 == ds.Tables[0].Rows.Count)
            {
                return 0;
            }
            string newid = Convert.ToString(ds.Tables[0].Rows[0][0]);
            if(SendMail_Register(_email, activecode.ToString(), newid))
            {
                return 1;
            }
            SqlParameter[] parameters4 = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,Convert.ToInt32(newid))
            };
            database.QueryWithoutReturnValue("DELETE FROM UserAccount WHERE UserID=@UserID", parameters4);
            return -3;
        }

        //发送激活邮件
        private bool SendMail_Register(string email, string activecode, string id)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.From = new MailAddress("notifications@mail.ustckkpk.cn");
            try
            {
                mailMsg.To.Add(email);
            }
            catch(FormatException ex)
            {
                return false;
            }
            mailMsg.Subject = "科科评科激活邮件";

            string url = "http://www.ustckkpk.cn/aspx/Activate.aspx?id=" + id + "&activecode=" + activecode;
            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.Append("您好，欢迎您在科科评科注册帐户！激活帐户需要点击下面的链接：");
            contentBuilder.Append("<br /><a href='" + url + "'>" + url + "</a>");

            mailMsg.Body = contentBuilder.ToString();//拼接字符串
            mailMsg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            //发件方服务器地址
            //client.Host = "smtp.exmail.qq.com";
            client.Host = "smtpdm.aliyun.com";
            client.Port = 80;

            NetworkCredential credetial = new NetworkCredential();
            credetial.UserName = "notifications@mail.ustckkpk.cn";
            //credetial.Password = "EmailKekepingke.2018";
            credetial.Password = "EmailKekepingke2018";
            client.Credentials = credetial;
            try
            {
                client.Send(mailMsg);
            }
            catch (SmtpException ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 用户激活(指定UserID)
        /// 返回值：-2激活码失效（已激活或被封禁）、-1用户不存在、0激活失败、1激活成功
        /// </summary>
        public int Activate(string activecode)
        {
            SqlParameter[] parameters1 = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_searchuserid)
            };
            DataSet ds = new DataSet();
            ds = database.QueryWithReturnValue("SELECT UserName,isStudent,Active,ActiveCode FROM UserAccount WHERE UserID=@UserID", parameters1);
            if (0 == ds.Tables[0].Rows.Count) 
            {
                return -1;
            }
            if (Convert.ToInt32(ds.Tables[0].Rows[0]["Active"]) != 0)
            {
                return -2;
            }
            if (ds.Tables[0].Rows[0]["ActiveCode"].ToString().Equals(activecode))
            {
                SqlParameter[] parameters2 = {
                    database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_searchuserid)
                };
                database.QueryWithoutReturnValue("UPDATE UserAccount SET Active=1 WHERE UserID=@UserID", parameters2);
                if (0 == Convert.ToInt32(ds.Tables[0].Rows[0]["isStudent"]))
                {
                    SqlParameter[] parameters3 = {
                        database.CreateInputParameter("@TeachersName", SqlDbType.VarChar,50,"%"+ds.Tables[0].Rows[0]["UserName"].ToString()+"%")
                    };
                    DataSet myclass = database.QueryWithReturnValue("SELECT CourseID FROM Course WHERE TeachersName LIKE @TeachersName", parameters3, "SearchName");
                    int rows = myclass.Tables["SearchName"].Rows.Count;
                    for (int i = 0; i < rows; i++)
                    {
                        SqlParameter[] parameters4 = {
                            database.CreateInputParameter("@UserID", SqlDbType.Int ,-1,_searchuserid),
                            database.CreateInputParameter("@CourseID", SqlDbType.Int ,-1,Convert.ToInt32(myclass.Tables["SearchName"].Rows[i]["CourseID"]))
                        };
                        database.QueryWithoutReturnValue("INSERT INTO CourseSelection VALUES (@UserID,@CourseID,0)", parameters4);
                    }
                }
                return 1;
            }
            return 0;
        }

        //发送验证码
        private bool SendMail_VerificationCode(string email, string verificationcode)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.From = new MailAddress("notifications@mail.ustckkpk.cn");
            try
            {
                mailMsg.To.Add(email);
            }
            catch (FormatException ex)
            {
                return false;
            }
            mailMsg.Subject = "科科评科验证邮件";

            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.Append("您好，欢迎您使用科科评科！您的验证码是：" + verificationcode);
            mailMsg.Body = contentBuilder.ToString();//拼接字符串

            SmtpClient client = new SmtpClient();
            client.Host = "smtpdm.aliyun.com";
            client.Port = 80;

            NetworkCredential credetial = new NetworkCredential();
            credetial.UserName = "notifications@mail.ustckkpk.cn";
            credetial.Password = "EmailKekepingke2018";
            client.Credentials = credetial;
            try
            {
                client.Send(mailMsg);
            }
            catch (SmtpException ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送验证码（需填写Email字段）
        /// 返回值：-1用户不存在、0发送失败、1验证码发送成功、2账号未激活并已发送激活邮件。
        /// </summary>
        public int SendVerificationCode()
        {
            SqlParameter[] parameters1 = {
                database.CreateInputParameter("@Email", SqlDbType.VarChar,50,_email)
            };
            DataSet ds = database.QueryWithReturnValue("SELECT UserID,Active,ActiveCode FROM UserAccount WHERE Email=@Email", parameters1);
            if (0 == ds.Tables[0].Rows.Count)
            {
                return -1;
            }
            if (0 == Convert.ToInt32(ds.Tables[0].Rows[0]["Active"]))
            {
                string id = ds.Tables[0].Rows[0]["UserID"].ToString();
                string activecode = ds.Tables[0].Rows[0]["ActiveCode"].ToString();
                if(SendMail_Register(_email, activecode, id))
                {
                    return 2;
                }
                return 0;
            }
            //if封禁
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random randrom = new Random((int)DateTime.Now.Ticks);
            string verificationcode = "";
            for (int i = 0; i < 6; i++)
            {
                verificationcode += chars[randrom.Next(chars.Length)];
            }
            SqlParameter[] parameters2 = {
                database.CreateInputParameter("@Email", SqlDbType.VarChar,50,_email),
                database.CreateInputParameter("@VerificationCode", SqlDbType.VarChar,8,verificationcode)
            };
            database.QueryWithoutReturnValue("UPDATE UserAccount SET VerificationCode=@VerificationCode WHERE Email=@Email", parameters2);
            if (SendMail_VerificationCode(_email, verificationcode))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 忘记密码（根据邮箱修改密码,需填写Email、Password字段,函数参数填写验证码）
        /// 返回值：-2验证码错误、-1邮箱不存在或未激活、0修改失败、1修改成功
        /// </summary>
        public int ChangePasswordByEmail(string verificationcode)
        {
            SqlParameter[] parameters1 = {
                database.CreateInputParameter("@Email", SqlDbType.VarChar,50,_email)
            };
            DataSet ds = database.QueryWithReturnValue("SELECT VerificationCode FROM UserAccount WHERE Email=@Email AND Active=1", parameters1);
            if (0 == ds.Tables[0].Rows.Count)
            {
                return -1;
            }
            if (ds.Tables[0].Rows[0]["VerificationCode"].ToString().Equals(verificationcode))
            {
                SqlParameter[] parameters2 = {
                    database.CreateInputParameter("@Email", SqlDbType.VarChar,50,_email),
                    database.CreateInputParameter("@Password", SqlDbType.VarChar,50,_password)
                };
                return (database.QueryWithoutReturnValue("UPDATE UserAccount SET Password=lower(right(sys.fn_varbintohexstr(hashbytes('MD5',@Password)),32)) WHERE Email=@Email", parameters2));
            }
            return -2;
        }

        /// <summary>
        /// 查找用户信息（昵称、邮箱、身份、照片、个人简介）
        /// 返回值：Dataset。表名UserInfo。列名UserName、Email、isStudent、PhotoUrl、PersonalProfile
        /// </summary>
        public DataSet SearchUserInfo()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_searchuserid)
            };
            return (database.QueryWithReturnValue("SELECT UserName,Email,isStudent,PhotoUrl,PersonalProfile FROM UserAccount WHERE UserID=@UserID AND Active=1", parameters, "UserInfo"));
        }

        /// <summary>
        /// 用户信息修改（修改昵称、密码、个人简介）
        /// 返回值：0修改失败、1修改成功
        /// </summary>
        public int ChangeUserInfo()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@UserName", SqlDbType.VarChar,50,_username),
                database.CreateInputParameter("@Password", SqlDbType.VarChar,50,_password),
                database.CreateInputParameter("@PersonalProfile", SqlDbType.VarChar,1000,_personalprofile)
            };
            return (database.QueryWithoutReturnValue("UPDATE UserAccount SET UserName=@UserName,Password=lower(right(sys.fn_varbintohexstr(hashbytes('MD5',@Password)),32)),PersonalProfile=@PersonalProfile WHERE UserID=@UserID", parameters));
        }

        /// <summary>
        /// 获取正在上的所有课程信息
        /// 返回值：Dataset。表名StudyingCourse。列名CourseID、CourseNumber、CourseName、TeachersName、CourseTime
        /// </summary>
        public DataSet GetStudyingCourse()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            return (database.QueryWithReturnValue("SELECT CourseID,CourseNumber,CourseName,TeachersName,CourseTime FROM Course WHERE Active=1 AND CourseID IN (SELECT CourseID FROM CourseSelection WHERE UserID=@UserID AND IdentityInfo=1)", parameters, "StudyingCourse"));
        }

        /// <summary>
        /// 获取上过的所有课程信息
        /// 返回值：Dataset。表名StudiedCourse。列名CourseID、CourseNumber、CourseName、TeachersName、CourseTime
        /// </summary>
        public DataSet GetStudiedCourse()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            return (database.QueryWithReturnValue("SELECT CourseID,CourseNumber,CourseName,TeachersName,CourseTime FROM Course WHERE Active=0 AND CourseID IN (SELECT CourseID FROM CourseSelection WHERE UserID=@UserID AND IdentityInfo=1)", parameters, "StudiedCourse"));
        }

        /// <summary>
        /// 获取正在教的所有课程信息
        /// 返回值：Dataset。表名TeachingCourse。列名CourseID、CourseNumber、CourseName、TeachersName、CourseTime
        /// </summary>
        public DataSet GetTeachingCourse()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            return (database.QueryWithReturnValue("SELECT CourseID,CourseNumber,CourseName,TeachersName,CourseTime FROM Course WHERE Active=1 AND CourseID IN (SELECT CourseID FROM CourseSelection WHERE UserID=@UserID AND (IdentityInfo=0 OR IdentityInfo=2))", parameters, "TeachingCourse"));
        }

        /// <summary>
        /// 获取教过的所有课程信息
        /// 返回值：Dataset。表名TaughtCourse。列名CourseID、CourseNumber、CourseName、TeachersName、CourseTime
        /// </summary>
        public DataSet GetTaughtCourse()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            return (database.QueryWithReturnValue("SELECT CourseID,CourseNumber,CourseName,TeachersName,CourseTime FROM Course WHERE Active=0 AND CourseID IN (SELECT CourseID FROM CourseSelection WHERE UserID=@UserID AND (IdentityInfo=0 OR IdentityInfo=2))", parameters, "TaughtCourse"));
        }

        /// <summary>
        /// 获取用户UserID新消息的数量
        /// 返回值：DataSet。表名NewMessageNum。列名Num。1-6行分别表示 新课程评论数、新课程建议数、新评论回复数(师/助)、新建议回复数(师/助)、新评论回复数(学)、新建议回复数(学)
        /// </summary>
        public DataSet GetNewMessageNum()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            return (database.QueryWithReturnValue("SELECT a.Type,count(b.Type) AS Num FROM UnreadType a LEFT JOIN Unread b ON a.Type=b.Type AND UserID=@UserID GROUP BY a.Type ORDER BY Type ASC", parameters, "NewMessageNum"));
        }

        /// <summary>
        /// 获取用户UserID收到的新课程评论的详细信息,按时间降序排列。
        /// 返回值：DataSet。表名NewComment。列名CommentID,UserID,UserName,PhotoUrl,CourseID,CourseName,Tags,Content,CreateDate,LikeCounts,RealUserID
        /// </summary>
        public DataSet GetNewComment()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            string sql = "SELECT CommentID,a.UserID,UserName,PhotoUrl,a.CourseID,CourseName,a.Tags,MyContent AS Content,a.CreateDate,";
            sql += "(SELECT count(*) FROM Likes WHERE CommentID=a.CommentID) AS LikeCounts,a.RealUserID ";
            sql += "FROM Comment a INNER JOIN UserAccount b ON a.UserID=b.UserID INNER JOIN Course c ON a.CourseID=c.CourseID ";
            sql += "WHERE CommentID IN (SELECT MessageID FROM Unread WHERE UserID=@UserID AND Type=1) ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "NewComment"));
        }

        /// <summary>
        /// 获取用户UserID收到的新课程建议的详细信息,按时间降序排列。
        /// 返回值：DataSet。表名NewAdvice。列名AdviceID,UserID,UserName,PhotoUrl,CourseID,CourseName,Content,CreateDate,RealUserID
        /// </summary>
        public DataSet GetNewAdvice()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            string sql = "SELECT AdviceID,a.UserID,UserName,PhotoUrl,a.CourseID,CourseName,MyContent AS Content,a.CreateDate,a.RealUserID ";
            sql += "FROM Advice a INNER JOIN UserAccount b ON a.UserID=b.UserID INNER JOIN Course c ON a.CourseID=c.CourseID ";
            sql += "WHERE AdviceID IN (SELECT MessageID FROM Unread WHERE UserID=@UserID AND Type=2) ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "NewAdvice"));
        }

        /// <summary>
        /// 获取用户UserID收到的新课程评论回复的详细信息,按时间降序排列。
        /// 返回值：DataSet。表名NewCommentReply。列名ReplyID,UserID,UserName,PhotoUrl,Content,CreateDate,RealUserID,CommentID,CourseID,CourseName,C_UserID,C_UserName,C_PhotoUrl,C_Tags,C_Content,C_CreateDate,C_LikeCounts,C_RealUserID
        /// </summary>
        public DataSet GetNewCommentReply()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            string sql = "SELECT ReplyID,a.UserID,b.UserName,b.PhotoUrl,a.MyContent AS Content,a.CreateDate,a.RealUserID,a.CommentID,c.CourseID,e.CourseName,c.UserID AS C_UserID,d.UserName AS C_UserName,";
            sql += "d.PhotoUrl AS C_PhotoUrl,c.Tags AS C_Tags,c.MyContent AS C_Content,c.CreateDate AS C_CreateDate,";
            sql += "(SELECT count(*) FROM Likes WHERE CommentID=a.CommentID) AS C_LikeCounts,c.RealUserID AS C_RealUserID ";
            sql += "FROM CommentReply a INNER JOIN UserAccount b ON a.UserID=b.UserID INNER JOIN Comment c ON a.CommentID=c.CommentID INNER JOIN UserAccount d ON c.UserID=d.UserID INNER JOIN Course e ON c.CourseID=e.CourseID ";
            sql += "WHERE ReplyID IN (SELECT MessageID FROM Unread WHERE UserID=@UserID AND Type=3) ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "NewCommentReply"));
        }

        /// <summary>
        /// 获取用户UserID收到的新课程建议回复的详细信息,按时间降序排列。
        /// 返回值：DataSet。表名NewAdviceReply。列名ReplyID,UserID,UserName,PhotoUrl,Content,CreateDate,RealUSerID,AdviceID,A_UserID,A_UserName,A_PhotoUrl,A_Content,A_CreateDate,A_RealUserID
        /// </summary>
        public DataSet GetNewAdviceReply()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            string sql = "SELECT ReplyID,a.UserID,b.UserName,b.PhotoUrl,a.MyContent AS Content,a.CreateDate,a.RealUserID,a.AdviceID,c.CourseID,e.CourseName,c.UserID AS A_UserID,d.UserName AS A_UserName,";
            sql += "d.PhotoUrl AS A_PhotoUrl,c.MyContent AS A_Content,c.CreateDate AS A_CreateDate,c.RealUserID AS A_RealUserID ";
            sql += "FROM AdviceReply a INNER JOIN UserAccount b ON a.UserID=b.UserID INNER JOIN Advice c ON a.AdviceID=c.AdviceID INNER JOIN UserAccount d ON c.UserID=d.UserID INNER JOIN Course e ON c.CourseID=e.CourseID ";
            sql += "WHERE ReplyID IN (SELECT MessageID FROM Unread WHERE UserID=@UserID AND Type=4) ORDER BY a.CreateDate DESC";
            return (database.QueryWithReturnValue(sql, parameters, "NewAdviceReply"));
        }
        #endregion
    }
}
