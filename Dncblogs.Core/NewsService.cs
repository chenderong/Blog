using AutoMapper;
using Dapper;
using Dncblogs.Domain.Entities;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNetCoreCommon;

namespace Dncblogs.Core
{
    public class NewsService : BaseService
    {
        public NewsService(IOptions<DatabaseSetting> options) : base(options)
        {

        }

        /// <summary>
        /// 分页获取新闻
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="keyWork"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<NewsDto>>> GetListByPage(string keyWork, string orderFiled, int pageSize, int pageIndex)
        {
            DataResultDto<List<NewsDto>> dataResultDto = new DataResultDto<List<NewsDto>>();
            List<NewsDto> list = new List<NewsDto>();
            string where = "where news.IsDelete=0 ";

            if (!string.IsNullOrEmpty(keyWork))
                where += "  and (news.Title like @KeyWork or news.Body like @KeyWork ) ";

            string order = string.Empty;
            if (!string.IsNullOrEmpty(orderFiled))
                order = string.Format("order by news.{0}", orderFiled);

            string sql = string.Format("select news.NewsId,news.Title,news.Body,news.CreateDate,news.VisitCount,news.CommentCount,news.OriginalUrl,news.IsDelete from `news`  " +
                                       " {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

            string sqlCount = string.Format("select count(1) from `news`  ", where);

            IEnumerable<News> blist = null;
            using (var connection = CreateConnection())
            {
                dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { KeyWork = string.Format("%{0}%", keyWork) });
                blist = await connection.QueryAsync<News>(sql, new { KeyWork = string.Format("%{0}%", keyWork), OrderFiled = orderFiled });
            }

            NewsDto newsDto = null;
            foreach (var ib in blist)
            {
                newsDto = Mapper.Map<News, NewsDto>(ib);
                newsDto.Body = string.Empty;//优化
                list.Add(newsDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }


        /// <summary>
        /// 分页获取新闻
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <param name="keyWork"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<NewsDto>>> GetListByPage(DateTime? startTime, DateTime? endTime, string keyWord, string orderFiled, int pageSize, int pageIndex)
        {
            DataResultDto<List<NewsDto>> dataResultDto = new DataResultDto<List<NewsDto>>();
            List<NewsDto> list = new List<NewsDto>();

            string where = "where IsDelete=0 ";
            if (startTime != null && endTime != null)
            {
                where += "  and CreateDate BETWEEN @StartTime and @EndTime ";
                endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            }

            if (!string.IsNullOrEmpty(keyWord))
                where += "  and (Title like @KeyWork or Body like @KeyWork ) ";

            string order = string.Empty;
            if (!string.IsNullOrEmpty(orderFiled))
                order = string.Format("order by {0}", orderFiled);

            string sql = string.Format("select NewsId,Title,Body,KeyWord,Description,CreateDate,VisitCount,CommentCount,OriginalUrl,Sort,IsDelete from `news`  {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

            string sqlCount = string.Format("select count(1) from `news`   {0}  ", where);

            IEnumerable<News> blist = null;
            using (var connection = CreateConnection())
            {
                dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { StartTime = startTime, EndTime = endTime, KeyWork = string.Format("%{0}%", keyWord) });
                blist = await connection.QueryAsync<News>(sql, new { StartTime = startTime, EndTime = endTime, KeyWork = string.Format("%{0}%", keyWord), OrderFiled = orderFiled });
            }

            NewsDto newsDto = null;
            foreach (var ib in blist)
            {
                newsDto = Mapper.Map<News, NewsDto>(ib);
                list.Add(newsDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;

            return dataResultDto;
        }


        /// <summary> 
        /// 获取一条新闻
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<NewsDto> GetOneNewsByNewsIdAsync(int newsId)
        {
            string where = "where IsDelete=0 and NewsId=@NewsId";
            string sql = string.Format("select NewsId,Title,Body,KeyWord,Description,CreateDate,VisitCount,CommentCount,OriginalUrl,Sort,IsDelete from `news`  {0} ", where);
            List<News> clist = null;
            using (var connection = CreateConnection())
            {
                clist = (List<News>)await connection.QueryAsync<News>(sql, new { NewsId = newsId });
            }
            NewsDto newsDto = new NewsDto();
            if (clist.Count() > 0)
                newsDto = Mapper.Map<News, NewsDto>(clist[0]);

            return newsDto;
        }


        /// <summary>
        /// 获取一条新闻  浏览数 +1
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="userIP"></param>
        /// <returns></returns>
        public async Task<NewsDto> GetOneNewsByNewsIdAsync(int newsId, string userIP, bool isAddVistNum)
        {
            var newsDto = await GetOneNewsByNewsIdAsync(newsId);
            if (MemoryCacheTool.GetCacheValue(userIP) == null)//同一个IP一个小时后读取才增加一次
            {
                if (isAddVistNum)
                    await UpdateVistNum(newsId, 1);
                MemoryCacheTool.SetChacheValue(userIP, userIP, TimeSpan.FromHours(1));
            }
            return newsDto;
        }



        /// <summary>
        /// 获取一条新闻  浏览数增加
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<NewsDto> GetOneBlogByBlogIdAsync(int newsId, string userIP)
        {
            var newsDto = await this.GetOneNewsByNewsIdAsync(newsId);

            if (MemoryCacheTool.GetCacheValue(userIP) == null)//同一个IP一个小时后读取才增加一次
            {
                await UpdateVistNum(newsId, 1);
                MemoryCacheTool.SetChacheValue(userIP, userIP, TimeSpan.FromHours(1));
            }
            return newsDto;
        }

        /// <summary>
        /// 更新新闻浏览数
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task UpdateVistNum(int newsId, int count)
        {
            string sql = "update `news` set VisitCount=VisitCount + @Count where NewsId=@NewsId";
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { Count = count, NewsId = newsId });
            }
        }

        /// <summary>
        /// 更新新闻评论数
        /// </summary>
        /// <param name="blogId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task UpdateCommentNum(long newsId, int count)
        {
            string sql = "update `news` set CommentCount=CommentCount + @Count where NewsId=@NewsId";
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { Count = count, NewsId = newsId });
            }
        }

        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="newsDto"></param>
        /// <returns></returns>
        public async Task<bool> AddNews(NewsDto newsDto)
        {
            string sql = "insert into `news`(Title,Body,KeyWord,Description,OriginalUrl,Sort) VALUES(@Title,@Body,@KeyWord,@Description,@OriginalUrl,@Sort)";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { Title = newsDto.Title, Body = newsDto.Body, KeyWord = newsDto.KeyWord, Description = newsDto.Description, OriginalUrl = newsDto.OriginalUrl, Sort = newsDto.Sort }) > 0;
            }
        }

        /// <summary>
        /// 修改新闻
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNews(NewsDto newsDto)
        {
            string sql = "update `news` set Title=@Title,Body=@Body,KeyWord=@KeyWord,Description=@Description,OriginalUrl=@OriginalUrl,Sort=@Sort where  NewsId=@NewsId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { Title = newsDto.Title, Body = newsDto.Body, KeyWord = newsDto.KeyWord, Description = newsDto.Description, OriginalUrl = newsDto.OriginalUrl, Sort = newsDto.Sort, NewsId = newsDto.NewsId }) > 0;
            }
        }


        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> DeleteNews(int newsId)
        {
            string sql = "update `news` set IsDelete=1 where  NewsId=@NewsId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { NewsId = newsId }) > 0;
            }
        }

        /// <summary>
        /// 获取新闻
        /// </summary>
        /// <param name="count"></param>
        /// <param name="orderString"></param>
        /// <returns></returns>
        public async Task<List<NewsDto>> GetTopNewsByCount(int count, string orderString)
        {
            DataResultDto<List<NewsDto>> dataResultDto = new DataResultDto<List<NewsDto>>();
            List<NewsDto> list = new List<NewsDto>();
            string where = "where IsDelete=0 ";
            string order = string.Format("order by {0}", orderString);
            string sql = string.Format(" select NewsId,Title,Body,KeyWord,Description,VisitCount,CommentCount,CreateDate,OriginalUrl,Sort,IsDelete from news {0}  {1}  limit 0,{2}", where, order, count);

            IEnumerable<News> nlist = null;
            using (var connection = CreateConnection())
            {
                nlist = await connection.QueryAsync<News>(sql);
            }
            NewsDto newsDto = null;
            foreach (var ib in nlist)
            {
                newsDto = Mapper.Map<News, NewsDto>(ib);
                newsDto.User = new UserDto();
                newsDto.User.UserId = 1;
                newsDto.User.UserName = "admin";
                newsDto.User.LoginName = "管理员";
                newsDto.User.UserHeadImaUrl = "/upload/user-head/default.png";
                list.Add(newsDto);
            }
            return list;
        }
    }
}
