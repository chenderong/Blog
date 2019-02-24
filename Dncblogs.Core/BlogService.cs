using Dncblogs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dncblogs.Domain.Entities;
using Microsoft.Extensions.Options;
using Dncblogs.Domain.EntitiesDto;
using System.Linq;
using XNetCoreCommon;
using System.Threading.Tasks;
using AutoMapper;

namespace Dncblogs.Core
{
    /// <summary>
    /// 博客
    /// </summary>
    public class BlogService : BaseService
    {
        private CategoryService _categoryService;
        public BlogService(CategoryService categoryService, IOptions<DatabaseSetting> options) : base(options)
        {
            this._categoryService = categoryService;
        }

        /// <summary>
        /// 分页获取博客
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="keyWork"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<BlogDto>>> GetListByPage(int userId, int categoryId, string keyWork, string orderFiled, int pageSize, int pageIndex, bool IsEssence = false, bool IsHomePage = true)
        {
            DataResultDto<List<BlogDto>> dataResultDto = new DataResultDto<List<BlogDto>>();
            List<BlogDto> list = new List<BlogDto>();
            string where = "where blog.IsDelete=0 ";
            if (userId != 0)
                where += " and blog.UserId=@UserId ";
            if (categoryId != 0)
                where += " and blog.CategoryID=@CategoryID ";
            if (IsEssence)
                where += " and blog.IsEssence=1 ";

            if (IsHomePage)
                where += " and blog.IsHomePage=1 ";

            if (!string.IsNullOrEmpty(keyWork))
                where += "  and (blog.Title like @KeyWork or Body like @KeyWork or category.CategoryName like @KeyWork) ";

            string order = string.Empty;
            if (!string.IsNullOrEmpty(orderFiled))
                order = string.Format("order by blog.{0}", orderFiled);

            string sql = string.Format("select blog.BlogId,blog.CategoryID,blog.Title,blog.Body,blog.Remark,blog.CreateDate,blog.UpdateDate,blog.Sort,blog.UserId,blog.VisitCount,blog.CommentCount,blog.IsEssence,blog.IsHomePage,blog.IsDelete,category.CategoryID,category.CategoryName,category.CreateDate,user.UserId,user.LoginName,user.UserName,user.UserHeadImaUrl from `blog`  " +
                                       " left join `category`  on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

            string sqlCount = string.Format("select count(1) from `blog`  left join `category` on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  ", where);

            IEnumerable<Blog> blist = null;
            using (var connection = CreateConnection())
            {
                dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { UserId = userId, CategoryID = categoryId, KeyWork = string.Format("%{0}%", keyWork) });
                blist = await connection.QueryAsync<Blog, Category, User, Blog>(sql, (qblog, qcategory, quser) =>
                   {
                       qblog.Category = qcategory;
                       qblog.User = quser;
                       return qblog;
                   }, new { UserId = userId, CategoryID = categoryId, KeyWork = string.Format("%{0}%", keyWork), OrderFiled = orderFiled },
                 splitOn: "BlogId,CategoryId,UserId");
            }
            BlogDto blogDto = null;
            foreach (var ib in blist)
            {
                blogDto = Mapper.Map<Blog, BlogDto>(ib);
                blogDto.Body = string.Empty;//优化，不传到显示页面
                blogDto.CategoryDto = Mapper.Map<Category, CategoryDto>(ib.Category);
                blogDto.User = Mapper.Map<User, UserDto>(ib.User);
                list.Add(blogDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }


        /// <summary>
        /// 分页获取博客
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="keyWork"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<BlogDto>>> GetListByPage(DateTime? startTime, DateTime? endTime, int userId, int categoryId, string keyWord, string orderFiled, int pageSize, int pageIndex)
        {
            DataResultDto<List<BlogDto>> dataResultDto = new DataResultDto<List<BlogDto>>();
            List<BlogDto> list = new List<BlogDto>();
            try
            {
                string where = "where blog.IsDelete=0 ";
                if (userId != 0)
                    where += " and blog.UserId=@UserId ";
                if (categoryId != 0)
                    where += " and blog.CategoryID=@CategoryID ";

                if (startTime != null && endTime != null)
                {
                    where += "  and blog.CreateDate BETWEEN @StartTime and @EndTime ";
                    endTime = endTime.Value.AddDays(1).AddSeconds(-1);
                }

                if (!string.IsNullOrEmpty(keyWord))
                    where += "  and (blog.Title like @KeyWork or Body like @KeyWork or category.CategoryName like @KeyWork) ";

                string order = string.Empty;
                if (!string.IsNullOrEmpty(orderFiled))
                    order = string.Format("order by blog.{0}", orderFiled);

                string sql = string.Format("select blog.BlogId,blog.CategoryID,blog.Title,blog.Body,blog.Remark,blog.CreateDate,blog.UpdateDate,blog.Sort,blog.UserId,blog.VisitCount,blog.CommentCount,blog.IsEssence,blog.IsHomePage,blog.IsDelete,category.CategoryID,category.CategoryName,category.CreateDate,user.UserId,user.LoginName,user.UserName,user.UserHeadImaUrl from `blog`  " +
                                           " left join `category`  on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

                string sqlCount = string.Format("select count(1) from `blog`  left join `category` on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  ", where);

                IEnumerable<Blog> blist = null;
                using (var connection = CreateConnection())
                {
                    dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { StartTime = startTime, EndTime = endTime, UserId = userId, CategoryID = categoryId, KeyWork = string.Format("%{0}%", keyWord) });
                    blist = await connection.QueryAsync<Blog, Category, User, Blog>(sql, (qblog, qcategory, quser) =>
                    {
                        qblog.Category = qcategory;
                        qblog.User = quser;
                        return qblog;
                    }, new { StartTime = startTime, EndTime = endTime, UserId = userId, CategoryID = categoryId, KeyWork = string.Format("%{0}%", keyWord), OrderFiled = orderFiled },
                     splitOn: "BlogId,CategoryId,UserId");
                }

                BlogDto blogDto = null;
                foreach (var ib in blist)
                {
                    blogDto = Mapper.Map<Blog, BlogDto>(ib);
                    blogDto.Body = string.Empty;//优化，不传到显示页面
                    blogDto.CategoryDto = Mapper.Map<Category, CategoryDto>(ib.Category);
                    blogDto.User = Mapper.Map<User, UserDto>(ib.User);
                    list.Add(blogDto);
                }
                dataResultDto.Code = 0;
                dataResultDto.DataList = list;
            }
            catch
            {
                dataResultDto.Code = 1;
                dataResultDto.DataList = null;
            }
            return dataResultDto;
        }


        /// <summary> 
        /// 获取一条博客
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<BlogDto> GetOneBlogByBlogIdAsync(long blogId)
        {
            string where = "where blog.IsDelete=0 and blog.BlogId=@BlogId";
            string sql = string.Format("select blog.BlogId,blog.CategoryID,blog.Title,blog.Body,blog.Remark,blog.KeyWord,blog.Description,blog.CreateDate,blog.UpdateDate,blog.Sort,blog.UserId,blog.VisitCount,blog.CommentCount,blog.IsEssence,blog.IsHomePage,blog.IsDelete,category.CategoryID,category.CategoryName,category.CreateDate,user.UserId,user.LoginName,user.UserName,user.UserHeadImaUrl from `blog`  " +
                                       " left join `category`  on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  ", where);
            List<Blog> blist = null;
            using (var connection = CreateConnection())
            {
                blist = (List<Blog>)await connection.QueryAsync<Blog, Category, User, Blog>(sql, (qblog, qcategory, quser) =>
                {
                    qblog.Category = qcategory;
                    qblog.User = quser;
                    return qblog;
                }, new { BlogId = blogId },
                splitOn: "BlogId,CategoryId,UserId");
            }
            BlogDto blogDto = new BlogDto();
            if (blist.Count() > 0)
            {
                blogDto = Mapper.Map<Blog, BlogDto>(blist[0]);
                blogDto.CategoryDto = Mapper.Map<Category, CategoryDto>(blist[0].Category);
                blogDto.User = Mapper.Map<User, UserDto>(blist[0].User);
            }
            return blogDto;
        }



        /// <summary>
        /// 获取一条博客  浏览数增加
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<BlogDto> GetOneBlogByBlogIdAsync(long blogId, string userIP, bool isAddVistNum = true)
        {
            var blogDto = await this.GetOneBlogByBlogIdAsync(blogId);

            if (MemoryCacheTool.GetCacheValue(userIP) == null)//同一个IP一个小时后读取才增加一次
            {
                if (isAddVistNum)
                    await UpdateVistNum(blogId, 1);
                MemoryCacheTool.SetChacheValue(userIP, userIP, TimeSpan.FromHours(1));
            }
            return blogDto;
        }

        /// <summary>
        /// 更新博客浏览数
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task UpdateVistNum(long blogId, int count)
        {
            string sql = "update `blog` set VisitCount=VisitCount + @Count where BlogId=@BlogId";
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { Count = count, BlogId = blogId });
            }
        }


        /// <summary>
        /// 更新博客评论数
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task UpdateCommentNum(long blogId, int count)
        {
            string sql = "update `blog` set CommentCount=CommentCount + @Count where BlogId=@BlogId";
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { Count = count, BlogId = blogId });
            }
        }


        /// <summary>
        /// 添加博客
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> AddBlog(BlogDto blogDto)
        {
            string sql = "insert into `blog`(CategoryID,Title,Body,KeyWord,Description,Sort,UserId) VALUES(@CategoryID,@Title,@Body,@KeyWord,@Description,@Sort,@UserId)";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { CategoryID = blogDto.CategoryId, Title = blogDto.Title, Body = blogDto.Body, KeyWord = blogDto.KeyWord, Description = blogDto.Description, Sort = blogDto.Sort, UserId = blogDto.UserId }) > 0;
            }
        }

        /// <summary>
        /// 修改博客
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBlog(BlogDto blogDto)
        {
            string sql = "update `blog` set CategoryID=@CategoryID,Title=@Title,Body=@Body,KeyWord=@KeyWord,Description=@Description,Remark=@Remark,Sort=@Sort where  BlogId=@BlogId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { CategoryID = blogDto.CategoryId, Title = blogDto.Title, Body = blogDto.Body, KeyWord = blogDto.KeyWord, Description = blogDto.Description, Remark = blogDto.Remark, Sort = blogDto.Sort, BlogId = blogDto.BlogId }) > 0;
            }
        }


        /// <summary>
        /// 删除博客
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBlog(int blogId)
        {
            string sql = "update `blog` set IsDelete=1 where  BlogId=@BlogId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { BlogId = blogId }) > 0;
            }
        }


        /// <summary>
        /// 设置博客是否是精华
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="isEssence"></param>
        /// <returns></returns>
        public async Task<bool> EssenceBlog(int blogId, bool isEssence)
        {
            string sql = "update `blog` set IsEssence=@IsEssence where  BlogId=@BlogId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { BlogId = blogId, IsEssence = !isEssence }) > 0;
            }
        }


        /// <summary>
        /// 设置博客是否在主页显示
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="IsHomePage"></param>
        /// <returns></returns>
        public async Task<bool> SetIsHomePage(int blogId, bool IsHomePage)
        {
            string sql = "update `blog` set IsHomePage=@IsHomePage where  BlogId=@BlogId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { BlogId = blogId, IsHomePage = !IsHomePage }) > 0;
            }
        }

        /// <summary>
        /// 设置博客是否在主页显示
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="IsHomePage"></param>
        /// <returns></returns>
        public async Task<bool> SetIsHomePage(int userId, int blogId, bool IsHomePage)
        {
            string sql = "update `blog` set IsHomePage=@IsHomePage where  BlogId=@BlogId and UserId=@UserId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { UserId = userId, BlogId = blogId, IsHomePage = !IsHomePage }) > 0;
            }
        }

        /// <summary>
        /// 获取博客数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> StatisticsBlogCount()
        {
            int count = 0;
            string sql = "select count(1) from `blog` where  IsDelete=0 ";
            using (var connect = CreateConnection())
            {
                count = await connect.QueryFirstAsync<int>(sql);
            }
            return count;
        }



        /// <summary>
        /// 获取博客
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <param name="orderString"></param>
        /// <returns></returns>
        public async Task<List<BlogDto>> GetTopBlogByCount(int userId, int count, string orderString)
        {
            DataResultDto<List<BlogDto>> dataResultDto = new DataResultDto<List<BlogDto>>();
            List<BlogDto> list = new List<BlogDto>();
            string where = "where blog.IsDelete=0 ";
            if (userId != 0)
                where += " and blog.UserId=@UserId ";

            string order = string.Format("order by blog.{0}", orderString);

            string sql = string.Format("select  blog.BlogId,blog.CategoryID,blog.Title,blog.Body,blog.Remark,blog.CreateDate,blog.UpdateDate,blog.Sort,blog.UserId,blog.VisitCount,blog.CommentCount,blog.IsEssence,blog.IsDelete,category.CategoryID,category.CategoryName,category.CreateDate,user.UserId,user.LoginName,user.UserName,user.UserHeadImaUrl from `blog`  " +
                                       " left join `category`  on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  {1}  limit 0,{2}", where, order, count);

            IEnumerable<Blog> blist = null;
            using (var connection = CreateConnection())
            {
                blist = await connection.QueryAsync<Blog, Category, User, Blog>(sql, (qblog, qcategory, quser) =>
                {
                    qblog.Category = qcategory;
                    qblog.User = quser;
                    return qblog;
                }, new { UserId = userId, OrderFiled = order },
                 splitOn: "BlogId,CategoryId,UserId");
            }

            BlogDto blogDto = null;
            foreach (var ib in blist)
            {
                blogDto = Mapper.Map<Blog, BlogDto>(ib);
                blogDto.CategoryDto = Mapper.Map<Category, CategoryDto>(ib.Category);
                blogDto.User = Mapper.Map<User, UserDto>(ib.User);
                list.Add(blogDto);
            }
            return list;
        }



        /// <summary>
        /// 分页获取博客 包括子类   二级
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="keyWork"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<BlogDto>>> GetListByPageInChildCate(int userId, int categoryId, string keyWork, string orderFiled, int pageSize, int pageIndex, bool IsEssence = false)
        {

            var cateList = await this._categoryService.GetAllCategoryByUserId(categoryId, userId);
            List<int> listCate = new List<int>();
            listCate.Add(categoryId);
            foreach (var ic in cateList)
                listCate.Add(ic.CategoryId);


            DataResultDto<List<BlogDto>> dataResultDto = new DataResultDto<List<BlogDto>>();
            List<BlogDto> list = new List<BlogDto>();
            string where = "where blog.IsDelete=0 ";
            if (userId != 0)
                where += " and blog.UserId=@UserId ";
            if (categoryId != 0)
            {
                if (listCate.Count == 1)
                    where += $" and blog.CategoryID={categoryId}";
                else
                    where += $" and blog.CategoryID in({string.Join(',', listCate)})";
            }
            if (IsEssence)
                where += " and blog.IsEssence=1 ";

            if (!string.IsNullOrEmpty(keyWork))
                where += "  and (blog.Title like @KeyWork or Body like @KeyWork or category.CategoryName like @KeyWork) ";

            string order = string.Empty;
            if (!string.IsNullOrEmpty(orderFiled))
                order = string.Format("order by blog.{0}", orderFiled);

            string sql = string.Format("select blog.BlogId,blog.CategoryID,blog.Title,blog.Body,blog.Remark,blog.CreateDate,blog.UpdateDate,blog.Sort,blog.UserId,blog.VisitCount,blog.CommentCount,blog.IsEssence,blog.IsDelete,category.CategoryID,category.CategoryName,category.CreateDate,user.UserId,user.LoginName,user.UserName,user.UserHeadImaUrl from `blog`  " +
                                       " left join `category`  on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

            string sqlCount = string.Format("select count(1) from `blog`  left join `category` on blog.CategoryID=category.CategoryID left join `user` on user.UserId=blog.UserId {0}  ", where);

            IEnumerable<Blog> blist = null;
            using (var connection = CreateConnection())
            {
                dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { UserId = userId, CategoryID = categoryId, KeyWork = string.Format("%{0}%", keyWork) });
                blist = await connection.QueryAsync<Blog, Category, User, Blog>(sql, (qblog, qcategory, quser) =>
                {
                    qblog.Category = qcategory;
                    qblog.User = quser;
                    return qblog;
                }, new { UserId = userId, KeyWork = string.Format("%{0}%", keyWork), OrderFiled = orderFiled },
                 splitOn: "BlogId,CategoryId,UserId");
            }
            BlogDto blogDto = null;
            foreach (var ib in blist)
            {
                blogDto = Mapper.Map<Blog, BlogDto>(ib);
                blogDto.Body = string.Empty;
                blogDto.CategoryDto = Mapper.Map<Category, CategoryDto>(ib.Category);
                blogDto.User = Mapper.Map<User, UserDto>(ib.User);
                list.Add(blogDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }

    }
}
