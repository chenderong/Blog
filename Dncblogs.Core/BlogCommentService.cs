using Dapper;
using Dncblogs.Domain.Entities;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using XNetCoreCommon;
using AutoMapper;

namespace Dncblogs.Core
{
    public class BlogCommentService : BaseService
    {
        public BlogCommentService(IOptions<DatabaseSetting> options) : base(options)
        {

        }



        /// <summary>
        /// 获取博客所有评论
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="keyWork"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<BlogCommentDto>>> GetList(long blogId)
        {
            DataResultDto<List<BlogCommentDto>> dataResultDto = new DataResultDto<List<BlogCommentDto>>();
            List<BlogCommentDto> list = new List<BlogCommentDto>();
            string sql = "select BCId,BlogId,ReferenceId,PostId,PostName,Content,PostDate,user.UserName,user.UserId from blogcomment left join user on user.UserId=blogcomment.PostId  where BlogId=@BlogId and blogcomment.IsDelete=0";
            IEnumerable<BlogComment> bClist = null;
            using (var connection = CreateConnection())
            {
                bClist = await connection.QueryAsync<BlogComment, User, BlogComment>(sql, (qBlogComment, quser) =>
                {
                    qBlogComment.User = quser;
                    return qBlogComment;
                }
                , new { BlogId = blogId }
                , splitOn: "UserName");
            }

            BlogCommentDto BlogCommentDto = null;
            foreach (var ibc in bClist)
            {
                BlogCommentDto = Mapper.Map<BlogComment, BlogCommentDto>(ibc); 
                if (ibc.PostId != 0)
                {
                    BlogCommentDto.PostId = ibc.User.UserId;
                    BlogCommentDto.PostName = ibc.User.UserName;
                }
                else
                {
                    BlogCommentDto.PostId = ibc.PostId;
                    BlogCommentDto.PostName = ibc.PostName;
                }

                list.Add(BlogCommentDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }


        /// <summary>
        /// 获取博客所有评论  回复盖楼html
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<DataResultDto<string>> GetAll(long blogId, string userName)
        {
            DataResultDto<string> dataResultDto = new DataResultDto<string>();
            dataResultDto.Code = 1;
            dataResultDto.DataList = string.Empty;
            var blogCommentList = await GetList(blogId);
            if (blogCommentList.Code != 0)
                return dataResultDto;

            dataResultDto.Code = 0;
            var bcAllList = blogCommentList.DataList;//所有评论
            var showList = bcAllList.OrderByDescending(p => p.BCId).ToList();//找所有没有引用的评论
            StringBuilder reference = new StringBuilder();
            StringBuilder resultComment = new StringBuilder();
            foreach (var ibc in showList)
            {
                var referenceList = new List<BlogCommentDto>(); // 创建当前评论所引用的评论列表 
                AddComment(bcAllList, referenceList, ibc);//查询 bci 该评论 给引用的列表、
                referenceList = referenceList.OrderBy(p => p.BCId).ToList();
                foreach (var irbc in referenceList)
                {
                    if (reference.Length == 0)
                    {
                        reference.AppendLine(CreateComment_Detail(irbc, userName, ""));
                    }
                    else
                    {
                        string chreference = reference.ToString();
                        reference.Clear();
                        reference.AppendLine(CreateComment_Detail(irbc, userName, chreference));
                    }
                }
                resultComment.AppendLine("<div class='comment'>");
                resultComment.AppendLine(CreateComment_Detail(ibc, userName, reference.ToString()));
                resultComment.AppendLine("</div>");
                resultComment.AppendLine("<br/>");
                reference.Clear();
            }

            dataResultDto.DataList = resultComment.ToString();
            return dataResultDto;
        }

        // 向quoteList中添加 符合条件的Comment
        private void AddComment(List<BlogCommentDto> list, List<BlogCommentDto> quoteList, BlogCommentDto blogCommentDto)
        {
            BlogCommentDto fbcd = list.FirstOrDefault(p => p.BCId == blogCommentDto.ReferenceId);
            if (fbcd != null)
            {
                quoteList.Add(fbcd);
                AddComment(list, quoteList, fbcd); // 递归调用，只要ReferenceId不为零，就加入到引用评论列表
            }
        }

        //private string CreateComment_Detail(BlogCommentDto blogCommentDto, string userName, string reference)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    stringBuilder.AppendLine("<div class='comment_detail'>");
        //    stringBuilder.AppendLine($"  <div class='comment_title'>{blogCommentDto.PostName}<span class='comment_date'>发表于：{blogCommentDto.PostDate}</span></div>");
        //    if (string.IsNullOrEmpty(reference))
        //    {
        //        stringBuilder.AppendLine($"  <div class='comment_content'>{blogCommentDto.Content}</div>");
        //    }
        //    else
        //    {
        //        stringBuilder.AppendLine($"  <div class='comment_content'>{reference}</div>");
        //        stringBuilder.AppendLine($"  <div class='comment_content'>{blogCommentDto.Content}</div>");
        //    }
        //    //回复界面
        //    stringBuilder.AppendLine(CreateCommentOperator(blogCommentDto, userName));
        //    stringBuilder.AppendLine("</div>");
        //    return stringBuilder.ToString();
        //}

        ///// <summary>
        ///// 创建评论回复界面
        ///// </summary>
        ///// <param name="blogCommentDto"></param>
        ///// <returns></returns>
        //private string CreateCommentOperator(BlogCommentDto blogCommentDto, string userName)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    stringBuilder.AppendLine("<div class='comment_operator'>");
        //    stringBuilder.AppendLine($" <span class='comment_reply' id='comment_reply{blogCommentDto.BCId}' commentid='{blogCommentDto.BCId}'>回复</span>");
        //    stringBuilder.AppendLine("  <div class='replyBox' style='display: none'>");
        //    //stringBuilder.AppendLine($"   <span id = 'repalyId{blogCommentDto.BCId}' class='commentId' style='display: none'>{blogCommentDto.BCId}</span>");
        //    stringBuilder.AppendLine("    <div class='comment_replyer'>");
        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        stringBuilder.AppendLine($"     昵称：<input id = 'repalyTitle{blogCommentDto.BCId}' class='repalyTitle' type='text' value='匿名用户' />");
        //    }
        //    else
        //    {
        //        stringBuilder.AppendLine($"     昵称：<input id = 'repalyTitle{blogCommentDto.BCId}' class='repalyTitle' type='text' readonly='readonly' value='{userName}' />");
        //    }
        //    stringBuilder.AppendLine("    </div>");
        //    stringBuilder.AppendLine("    <div class='comment_replycontent'>");
        //    stringBuilder.AppendLine($"    <textarea id = 'repalyContent{blogCommentDto.BCId}' class='repalyContent' maxlength'800' cols='80' rows='5'></textarea>");
        //    stringBuilder.AppendLine("    </div>");
        //    stringBuilder.AppendLine("    <div class='comment_replybutton'>");
        //    stringBuilder.AppendLine($"    <input id = 'repalyButton{blogCommentDto.BCId}' class='repalyButton' refid='{blogCommentDto.BCId}' type='button' value='发表' /><input id = 'closeButton{blogCommentDto.BCId}' class='closeButton' refid='{blogCommentDto.BCId}' type='button' value='关闭' />");
        //    stringBuilder.AppendLine("    </div>");
        //    stringBuilder.AppendLine(" </div>");
        //    stringBuilder.AppendLine("</div>");

        //    return stringBuilder.ToString();
        //}

        private string CreateComment_Detail(BlogCommentDto blogCommentDto, string userName, string reference)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<div class='comment_detail'>");
            stringBuilder.AppendLine($"  <div class='comment_title'>{blogCommentDto.PostName}<span class='comment_date'>发表于：{blogCommentDto.PostDate}</span><span class='comment_reply'>回复</span></div>");
            //stringBuilder.AppendLine(" <span class='comment_reply'>回复</span>");
            if (string.IsNullOrEmpty(reference))
            {
                stringBuilder.AppendLine($"  <div class='comment_content'>{blogCommentDto.Content}</div>");
            }
            else
            {
                stringBuilder.AppendLine($"  <div class='comment_content'>{reference}</div>");
                stringBuilder.AppendLine($"  <div class='comment_content'>{blogCommentDto.Content}</div>");
            }
            //回复界面
            stringBuilder.AppendLine(CreateCommentOperator(blogCommentDto, userName));
            stringBuilder.AppendLine("</div>");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 创建评论回复界面
        /// </summary>
        /// <param name="blogCommentDto"></param>
        /// <returns></returns>
        private string CreateCommentOperator(BlogCommentDto blogCommentDto, string userName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<div class='comment_operator'>");
            //stringBuilder.AppendLine($" <span class='comment_reply'>回复</span>");
            stringBuilder.AppendLine("  <div class='replyBox' style='display: none'>");
            //stringBuilder.AppendLine($"   <span id = 'repalyId{blogCommentDto.BCId}' class='commentId' style='display: none'>{blogCommentDto.BCId}</span>");
            stringBuilder.AppendLine("    <div class='comment_replyer'>");
            if (string.IsNullOrEmpty(userName))
            {
                stringBuilder.AppendLine("     昵称：<input  class='repalyTitle' type='text' value='匿名用户' />");
            }
            else
            {
                stringBuilder.AppendLine($"     昵称：<input  class='repalyTitle' type='text' readonly='readonly' value='{userName}' />");
            }
            stringBuilder.AppendLine("    </div>");
            stringBuilder.AppendLine("    <div class='comment_replycontent'>");
            stringBuilder.AppendLine("    <textarea  maxlength='800' cols='80' rows='5' class='repalyContent' ></textarea>");
            stringBuilder.AppendLine("    </div>");
            stringBuilder.AppendLine("    <div class='comment_replybutton'>");
            stringBuilder.AppendLine($"    <input  class='repalyButton' refid='{blogCommentDto.BCId}' type='button' value='发表' /><input  class='closeButton' refid='{blogCommentDto.BCId}' type='button' value='关闭' />");
            stringBuilder.AppendLine("    </div>");
            stringBuilder.AppendLine(" </div>");
            stringBuilder.AppendLine("</div>");

            return stringBuilder.ToString();
        }



        /// <summary>
        /// 添加博客评论
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> AddBlogComment(BlogCommentDto blogCommentDto)
        {
            string sql = "insert into `blogcomment`(BlogId,ReferenceId,PostId,PostName,Content,PostDate) VALUES(@BlogId,@ReferenceId,@PostId,@PostName,@Content,@PostDate)";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { BlogId = blogCommentDto.BlogId, ReferenceId = blogCommentDto.ReferenceId, PostId = blogCommentDto.PostId, PostName = blogCommentDto.PostName, Content = blogCommentDto.Content, PostDate = DateTime.Now }) > 0;
            }
        }

        /// <summary>
        /// 获取用户博客的 最新的 几条博客
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<BlogCommentDto>>> GetBlogCommentByUserId(int topCount)
        {
            DataResultDto<List<BlogCommentDto>> dataResultDto = new DataResultDto<List<BlogCommentDto>>();
            string sql = $"select  BCId,BlogId,ReferenceId,PostId,PostName,Content,PostDate from blogcomment where IsDelete=0 order by PostDate desc  limit 0,{topCount}";
            IEnumerable<BlogComment> bClist = null;
            using (var connection = CreateConnection())
            {
                bClist = await connection.QueryAsync<BlogComment>(sql);
            }
            List<BlogCommentDto> list = new List<BlogCommentDto>();
            BlogCommentDto BlogCommentDto = null;
            foreach (var ibc in bClist)
            {
                BlogCommentDto = Mapper.Map<BlogComment, BlogCommentDto>(ibc);;
                list.Add(BlogCommentDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;

            return dataResultDto;
        }

        /// <summary>
        /// 获取所有的评论数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> StatisticsCommentCount()
        {
            int count = 0;
            string sql = "select count(1) from `blogcomment` where  IsDelete=0 ";
            using (var connect = CreateConnection())
            {
                count = await connect.QueryFirstAsync<int>(sql);
            }
            return count;
        }
    }
}
