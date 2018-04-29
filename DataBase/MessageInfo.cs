using System;
using System.Data;
using System.Data.SqlClient;

namespace TimelyEvaluation.DataBase
{
    public class MessageInfo
    {

        #region 数据结构
        DataBaseOperation database;
        private int _messageid;
        private int _userid;
        private int _courseid;
        private int _targetuserid;
        private int _targetreplyid;
        private Byte[] _tags;
        private string _content;
        private int _type;
        private int _anonymous;
        private int[] _shieldid;
        private int[] _unshieldid;
        #endregion

        #region 初始化
        public MessageInfo()
        {
            database = new DataBaseOperation();
            _messageid = 0;
            _userid = 0;
            _courseid = 0;
            _targetuserid = 0;
            _targetreplyid = 0;
            _tags = new Byte[20];
            _content = "";
            _type = -1;
            _anonymous = 1;
            _shieldid = null;
            _unshieldid = null;
        }
        /// <summary>
        /// 评论/建议/回复的ID
        /// </summary>
        public int MessageID
        {
            set { _messageid = value; }
            get { return _messageid; }
        }
        /// <summary>
        /// 评论者ID
        /// </summary>
        public int UserID
        {
            set { _userid = value; }
            get { return _userid; }
        }
        /// <summary>
        /// 被评论的课程ID
        /// </summary>
        public int CourseID
        {
            set { _courseid = value; }
            get { return _courseid; }
        }
        /// <summary>
        /// 被回复的人的ID
        /// </summary>
        public int TargetUserID
        {
            set { _targetuserid = value; }
            get { return _targetuserid; }
        }
        /// <summary>
        /// 被回复的回复的ID，若直接回复评论，则为0
        /// </summary>
        public int TargetReplyID
        {
            set { _targetreplyid = value; }
            get { return _targetreplyid; }
        }
        /// <summary>
        /// 评论标签
        /// </summary>
        public Byte[] Tags
        {
            set { _tags = value; }
            get { return _tags; }
        }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content
        {
            set { _content = value; }
            get { return _content; }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type
        {
            set { _type = value; }
            get { return _type; }
        }
        /// <summary>
        /// 1匿名，0不匿名
        /// </summary>
        public int Anonymous
        {
            set { _anonymous = value; }
            get { return _anonymous; }
        }
        /// <summary>
        /// 被屏蔽的人的ID数组（以0结尾）
        /// </summary>
        public int[] ShieldID
        {
            set { _shieldid = value; }
            get { return _shieldid; }
        }
        /// <summary>
        /// 未被屏蔽的人的ID数组（以0结尾）
        /// </summary>
        public int[] UnShieldID
        {
            set { _unshieldid = value; }
            get { return _unshieldid; }
        }
        #endregion

        #region 操作
        /// <summary>
        /// 获取评论MessageID的所有回复
        /// 返回值：Dataset。表名Reply。列名CommentID,ReplyID,UserID,UserName,PhotoUrl,TargetUserID,TargetUserName,Content,CreateDate,RealUserID
        /// </summary>
        public DataSet GetCommentReply()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid)
            };
            string sql = "SELECT CommentID,ReplyID,b.UserID,b.UserName,b.PhotoUrl,TargetUserID,c.UserName AS TargetUserName,MyContent AS Content,a.CreateDate,a.RealUserID ";
            sql += "FROM CommentReply a INNER JOIN UserAccount b ON a.UserID=b.UserID INNER JOIN UserAccount c ON a.TargetUserID=c.UserID ";
            sql += "WHERE CommentID=@CommentID ORDER BY a.CreateDate ASC";
            return (database.QueryWithReturnValue(sql, parameters, "Reply"));
        }

        /// <summary>
        /// 获取建议MessageID的所有回复
        /// 返回值：Dataset。表名Reply。列名AdviceID,ReplyID,UserID,UserName,PhotoUrl,TargetUserID,TargetUserName,Content,CreateDate,RealUserID
        /// </summary>
        public DataSet GetAdviceReply()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,_messageid)
            };
            string sql = "SELECT AdviceID,ReplyID,b.UserID,b.UserName,b.PhotoUrl,TargetUserID,c.UserName AS TargetUserName,MyContent AS Content,a.CreateDate,a.RealUserID ";
            sql += "FROM AdviceReply a INNER JOIN UserAccount b ON a.UserID=b.UserID INNER JOIN UserAccount c ON a.TargetUserID=c.UserID ";
            sql += "WHERE AdviceID=@AdviceID ORDER BY a.CreateDate ASC";
            return (database.QueryWithReturnValue(sql, parameters, "Reply"));
        }

        /// <summary>
        /// 添加一条评论。发表评论的用户为UserID、评论的课程为CourseID、评论设置的标签为Tags、评论的内容为Content、匿名选项Anonymous。
        /// 返回值：int。-1评论已存在、0添加失败、1添加成功
        /// </summary>
        public int AddComment()
        {
            int realuserid = _userid;
            DataSet ds = new DataSet();
            SqlParameter[] parameters1 = {
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1,_courseid),
                database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid)
            };
            //检查是否已经评论
            ds = database.QueryWithReturnValue("SELECT CourseID FROM Comment WHERE CourseID=@CourseID AND RealUserID=@RealUserID", parameters1);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return -1;
            }
            DateTime now = DateTime.Now;
            if (1 == _anonymous)
            {
                Random rd = new Random();
                _userid = rd.Next(1, 11);
            }
            SqlParameter[] parameters2 = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1,_courseid),
                database.CreateInputParameter("@Tags", SqlDbType.Binary,20,_tags),
                database.CreateInputParameter("@MyContent", SqlDbType.VarChar,1000,_content),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now),
                database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid)
            };
            //添加评论
            ds = database.QueryWithReturnValue("INSERT INTO Comment VALUES (@UserID,@CourseID,@Tags,@MyContent,@CreateDate,@RealUserID) SELECT scope_Identity()", parameters2);
            int newid = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            if(1 == _anonymous)
            {
                SqlParameter[] parameters3 = {
                    database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,newid),
                    database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid),
                    database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
                };
                //添加匿名信息
                database.QueryWithoutReturnValue("INSERT INTO CommentAnonymity VALUES (@CommentID,@RealUserID,@UserID)", parameters3);
            }
            SqlParameter[] parameters4 = {
                database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,newid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1,_courseid),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now)
            };
            //添加老师和助教的未读消息
            return (database.QueryWithoutReturnValue("INSERT INTO Unread SELECT UserID,@CommentID,1,@CreateDate FROM CourseSelection WHERE CourseID=@CourseID AND (IdentityInfo=0 OR IdentityInfo=2)", parameters4));
        }

        /// <summary>
        /// 修改一条评论。修改用户UserID对课程CourseID的评论，新评论标签为Tags、新评论内容为Content。
        /// 返回值：int。0修改失败、1修改成功
        /// </summary>
        public int ChangeComment()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1,_courseid),
                database.CreateInputParameter("@Tags", SqlDbType.Binary,20,_tags),
                database.CreateInputParameter("@MyContent", SqlDbType.VarChar,1000,_content)
            };
            //修改是否给出提醒？
            return (database.QueryWithoutReturnValue("UPDATE Comment SET Tags=@Tags,MyContent=@MyContent WHERE RealUserID=@UserID AND CourseID=@CourseID", parameters));
        }

        /// <summary>
        /// 删除ID为MessageID的一条评论与所有对评论的回复
        /// 返回值：int。0删除失败、1删除成功
        /// </summary>
        public int DeleteComment()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid)
            };
            //删除评论的未读提醒(注意最后的空格)
            string sql = "DELETE FROM Unread WHERE Type=1 AND MessageID=@CommentID ";
            //删除对评论回复的未读提醒
            sql += "DELETE FROM Unread WHERE Type=3 AND MessageID IN (SELECT ReplyID FROM CommentReply WHERE CommentID=@CommentID) ";
            //删除匿名信息
            sql += "DELETE FROM CommentAnonymity WHERE CommentID=@CommentID ";
            //删除评论
            sql += "DELETE FROM Comment WHERE CommentID=@CommentID ";
            //删除对评论的回复
            sql += "DELETE FROM CommentReply WHERE CommentID=@CommentID ";
            //删除赞
            sql += "DELETE FROM Likes WHERE CommentID=@CommentID";
            return (database.QueryWithoutReturnValue(sql, parameters));
        }

        /// <summary>
        /// 添加一条建议。发表建议的用户为UserID、建议的课程为CourseID、建议的内容为Content、屏蔽用户ShieldID、未屏蔽用户UnShieldID、匿名选项Anonymous。
        /// 返回值：int。0添加失败、1添加成功
        /// </summary>
        public int AddAdvice()
        {
            DateTime now = DateTime.Now;
            int realuserid = _userid;
            if (1 == _anonymous)
            {
                Random rd = new Random();
                _userid = rd.Next(1, 11);
            }
            SqlParameter[] parameters1 = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@CourseID", SqlDbType.Int,-1,_courseid),
                database.CreateInputParameter("@MyContent", SqlDbType.VarChar,1000,_content),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now),
                database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid)
            };
            //添加建议
            DataSet ds = new DataSet();
            ds = database.QueryWithReturnValue("INSERT INTO Advice VALUES (@UserID,@CourseID,@MyContent,@CreateDate,@RealUserID) SELECT scope_Identity()", parameters1);
            int newid = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            SqlParameter[] parameters2 = {
                database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,newid),
                database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid),
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            if (1 == _anonymous)
            {
                //添加匿名信息
                database.QueryWithoutReturnValue("INSERT INTO AdviceAnonymity VALUES (@AdviceID,@RealUserID,@UserID)", parameters2);
            }
            foreach (int sid in _shieldid)
            {
                if (sid != 0)
                {
                    SqlParameter[] parameters3 = {
                        database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,newid),
                        database.CreateInputParameter("@UserID", SqlDbType.Int,-1,sid)
                    };
                    //添加屏蔽信息
                    database.QueryWithoutReturnValue("INSERT INTO AdviceShielded VALUES (@AdviceID,@UserID)", parameters3);
                }
            }
            foreach (int usid in _unshieldid)
            {
                if (usid != 0)
                {
                    SqlParameter[] parameters4 = {
                        database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,newid),
                        database.CreateInputParameter("@UserID", SqlDbType.Int,-1,usid),
                        database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now)
                    };
                    //添加未读消息
                    database.QueryWithoutReturnValue("INSERT INTO Unread VALUES (@UserID,@AdviceID,2,@CreateDate)", parameters4);
                }
            }
            return 1;
        }

        /// <summary>
        /// 删除ID为MessageID的一条建议与所有对建议的回复
        /// 返回值：int。0删除失败、1删除成功
        /// </summary>
        public int DeleteAdvice()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,_messageid)
            };
            //删除建议的未读提醒(注意最后的空格)
            string sql = "DELETE FROM Unread WHERE Type=2 AND MessageID=@AdviceID ";
            //删除对建议回复的未读提醒
            sql += "DELETE FROM Unread WHERE Type=4 AND MessageID IN (SELECT ReplyID FROM AdviceReply WHERE AdviceID=@AdviceID) ";
            //删除匿名信息
            //sql += "DELETE FROM AdvideAnonymity WHERE AdviceID=@AdviceID ";
            //删除建议
            sql += "DELETE FROM Advice WHERE AdviceID=@AdviceID ";
            //删除对建议的回复
            sql += "DELETE FROM AdviceReply WHERE AdviceID=@AdviceID ";
            //删除建议的屏蔽信息
            sql += "DELETE FROM AdviceShielded WHERE AdviceID=@AdviceID";
            return (database.QueryWithoutReturnValue(sql, parameters));
        }

        /// <summary>
        /// 添加一条评论的回复。发表回复的用户为UserID、被回复的评论ID为MessageID、被回复的用户为TargetUserID、被回复的回复ID为TargetReplyID、回复的内容为Content、匿名选项Anonymous。
        /// 返回值：int。0添加失败、1添加成功
        /// </summary>
        public int AddCommentReply()
        {
            DateTime now = DateTime.Now;
            int realuserid = _userid;
            if (1 == _anonymous)
            {
                SqlParameter[] parameters1 = {
                    database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid),
                    database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid)
                };
                DataSet ds1 = database.QueryWithReturnValue("SELECT AnonymousID FROM CommentAnonymity WHERE CommentID=@CommentID AND UserID=@RealUserID", parameters1);
                if (ds1.Tables[0].Rows.Count == 0)
                {
                    Random rd = new Random();
                    _userid = rd.Next(1, 11);
                    SqlParameter[] parameters2 = {
                        database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid),
                        database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid),
                        database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
                    };
                    //添加匿名信息
                    database.QueryWithoutReturnValue("INSERT INTO CommentAnonymity VALUES (@CommentID,@RealUserID,@UserID)", parameters2);
                }
                else
                {
                    _userid = Convert.ToInt32(ds1.Tables[0].Rows[0][0]);
                }
            }
            SqlParameter[] parameters3 = {
                database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid),
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@TargetUserID", SqlDbType.Int,-1,_targetuserid),
                database.CreateInputParameter("@MyContent", SqlDbType.VarChar,1000,_content),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now),
                database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid)
            };
            //添加回复
            DataSet ds2 = new DataSet();
            ds2 = database.QueryWithReturnValue("INSERT INTO CommentReply VALUES (@CommentID,@UserID,@TargetUserID,@MyContent,@CreateDate,@RealUserID) SELECT scope_Identity()", parameters3);
            int newid = Convert.ToInt32(ds2.Tables[0].Rows[0][0]);
            if (_targetuserid <= 10 && _targetuserid >= 1)
            {
                //目标为匿名用户，将未读提醒转换为真用户ID
                if (0 == _targetreplyid)
                {
                    SqlParameter[] parameters4 = {
                        database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid)
                    };
                    DataSet ds4 = database.QueryWithReturnValue("SELECT RealUserID FROM Comment WHERE CommentID=@CommentID", parameters4);
                    //TODO:0行数据报错
                    _targetuserid = Convert.ToInt32(ds4.Tables[0].Rows[0][0]);
                }
                else
                {
                    SqlParameter[] parameters5 = {
                        database.CreateInputParameter("@ReplyID", SqlDbType.Int,-1,_targetreplyid)
                    };
                    DataSet ds5 = database.QueryWithReturnValue("SELECT RealUserID FROM CommentReply WHERE ReplyID=@ReplyID", parameters5);
                    //TODO:0行数据报错
                    _targetuserid = Convert.ToInt32(ds5.Tables[0].Rows[0][0]);
                }
            }
            SqlParameter[] parameters6 = {
                database.CreateInputParameter("@ReplyID", SqlDbType.Int,-1,newid),
                database.CreateInputParameter("@TargetUserID", SqlDbType.Int,-1,_targetuserid),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now)
            };
            //添加被回复者的未读消息
            return (database.QueryWithoutReturnValue("INSERT INTO Unread VALUES (@TargetUserID,@ReplyID,3,@CreateDate)", parameters6));
        }

        /// <summary>
        /// 删除ID为MessageID的一条评论的回复
        /// 返回值：int。0删除失败、1删除成功
        /// </summary>
        public int DeleteCommentReply()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@ReplyID", SqlDbType.Int,-1,_messageid)
            };
            //删除回复的未读提醒(注意最后的空格)
            string sql = "DELETE FROM Unread WHERE MessageID=@ReplyID AND Type=3 ";
            //删除回复
            sql += "DELETE FROM CommentReply WHERE ReplyID=@ReplyID";
            return (database.QueryWithoutReturnValue(sql, parameters));
        }

        /// <summary>
        /// 添加一条建议的回复。发表回复的用户为UserID、被回复的建议ID为MessageID、被回复的用户为TargetUserID、回复的内容为Content、匿名选项Anonymous。
        /// 返回值：int。-1不可以匿名回复、0添加失败、1添加成功
        /// </summary>
        public int AddAdviceReply()
        {
            //这里认为只有学生能匿名,故一条建议里只有学生匿名,且必须一开始就匿名
            DateTime now = DateTime.Now;
            int realuserid = _userid;
            if (1 == _anonymous)
            {
                SqlParameter[] parameters1 = {
                    database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,_messageid)
                };
                DataSet ds1 = database.QueryWithReturnValue("SELECT UserID FROM Advice WHERE AdviceID=@AdviceID", parameters1);
                if (ds1.Tables[0].Rows.Count == 0)
                {
                    //TODO:建议不存在而报错
                    return 0;
                }
                else
                {
                    _userid = Convert.ToInt32(ds1.Tables[0].Rows[0][0]);
                    if (_userid > 10)
                    {
                        return -1;
                    }
                }
            }
            SqlParameter[] parameters2 = {
                database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,_messageid),
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@TargetUserID", SqlDbType.Int,-1,_targetuserid),
                database.CreateInputParameter("@MyContent", SqlDbType.VarChar,1000,_content),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now),
                database.CreateInputParameter("@RealUserID", SqlDbType.Int,-1,realuserid)
            };
            //添加回复
            DataSet ds = new DataSet();
            ds = database.QueryWithReturnValue("INSERT INTO AdviceReply VALUES (@AdviceID,@UserID,@TargetUserID,@MyContent,@CreateDate,@RealUserID) SELECT scope_Identity()", parameters2);
            int newid = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            if (_targetuserid <= 10 && _targetuserid >= 1)
            {
                //目标为匿名用户，将未读提醒转换为真用户ID
                SqlParameter[] parameters3 = {
                    database.CreateInputParameter("@AdviceID", SqlDbType.Int,-1,_messageid)
                };
                DataSet ds3 = database.QueryWithReturnValue("SELECT RealUserID FROM Advice WHERE AdviceID=@AdviceID", parameters3);
                //TODO:0行数据报错
                _targetuserid = Convert.ToInt32(ds3.Tables[0].Rows[0][0]);
            }
            SqlParameter[] parameters4 = {
                database.CreateInputParameter("@ReplyID", SqlDbType.Int,-1,newid),
                database.CreateInputParameter("@TargetUserID", SqlDbType.Int,-1,_targetuserid),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,now)
            };
            //添加被回复者的未读消息
            return (database.QueryWithoutReturnValue("INSERT INTO Unread VALUES (@TargetUserID,@ReplyID,4,@CreateDate)", parameters4));
        }

        /// <summary>
        /// 删除ID为MessageID的一条建议的回复
        /// 返回值：int。0删除失败、1删除成功
        /// </summary>
        public int DeleteAdviceReply()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@ReplyID", SqlDbType.Int,-1,_messageid)
            };
            //删除回复的未读提醒(注意最后的空格)
            string sql = "DELETE FROM Unread WHERE MessageID=@ReplyID AND Type=4 ";
            //删除回复
            sql += "DELETE FROM AdviceReply WHERE ReplyID=@ReplyID";
            return (database.QueryWithoutReturnValue(sql, parameters));
        }

        /// <summary>
        /// 用户UserID给评论MessageID添加一个赞。
        /// 返回值：int。0添加失败、1添加成功
        /// </summary>
        public int AddLike()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid)
            };
            return (database.QueryWithoutReturnValue("INSERT INTO Likes VALUES (@CommentID,@UserID)", parameters));
        }

        /// <summary>
        /// 用户UserID取消给评论MessageID的赞。
        /// 返回值：int。0删除失败、1删除成功
        /// </summary>
        public int DeleteLike()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@CommentID", SqlDbType.Int,-1,_messageid)
            };
            return (database.QueryWithoutReturnValue("DELETE FROM Likes WHERE UserID=@UserID AND CommentID=@CommentID", parameters));
        }

        /// <summary>
        /// 用户UserID的消息MessageID（消息类型Type）标记为已读。
        /// 返回值：0标记失败、1标记成功
        /// </summary>
        public int Read()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@MessageID", SqlDbType.Int,-1,_messageid),
                database.CreateInputParameter("@Type", SqlDbType.Int,-1,_type)
            };
            return (database.QueryWithoutReturnValue("DELETE FROM Unread WHERE UserID=@UserID AND MessageID=@MessageID AND Type=@Type", parameters));
        }

        /// <summary>
        /// 用户UserID的所有类型为Type的消息标记为已读。
        /// 返回值：0标记失败、1标记成功
        /// </summary>
        public int ReadType()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@Type", SqlDbType.Int,-1,_type)
            };
            return (database.QueryWithoutReturnValue("DELETE FROM Unread WHERE UserID=@UserID AND Type=@Type", parameters));
        }

        /// <summary>
        /// 用户UserID的所有消息标记为已读。
        /// 返回值：0标记失败、1标记成功
        /// </summary>
        public int ReadAll()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid)
            };
            return (database.QueryWithoutReturnValue("DELETE FROM Unread WHERE UserID=@UserID", parameters));
        }

        /// <summary>
        /// 用户UserID将消息MessageID（消息类型Type）标记为未读。
        /// 返回值：0标记失败、1标记成功
        /// </summary>
        public int UnRead()
        {
            SqlParameter[] parameters = {
                database.CreateInputParameter("@UserID", SqlDbType.Int,-1,_userid),
                database.CreateInputParameter("@MessageID", SqlDbType.Int,-1,_messageid),
                database.CreateInputParameter("@Type", SqlDbType.Int,-1,_type),
                database.CreateInputParameter("@CreateDate", SqlDbType.DateTime,-1,DateTime.Now)
            };
            //此时CreateDate不再是消息创建的时间，而是标记的时间
            return (database.QueryWithoutReturnValue("INSERT INTO Unread VALUES (@UserID,@MessageID,@Type,@CreateDate)", parameters));
        }
        #endregion
    }
}
