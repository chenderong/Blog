using Dapper;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dncblogs.Core
{
    public class SuggestService : BaseService
    {
        public SuggestService(IOptions<DatabaseSetting> options) : base(options)
        {

        }


        /// <summary>
        /// 添加 意见
        /// </summary>
        /// <param name="suggestDto"></param>
        /// <returns></returns>
        public async Task<bool> AddBlog(SuggestDto suggestDto)
        {
            string sql = "insert into `suggest`(SuggestContact,SuggestContent,CreateDate) VALUES(@SuggestContact,@SuggestContent,@CreateDate)";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { SuggestContact = suggestDto.SuggestContact, SuggestContent = suggestDto.SuggestContent, CreateDate = DateTime.Now }) > 0;
            }
        }

        /// <summary>
        /// 修改 意见
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBlog(SuggestDto suggestDto)
        {
            string sql = "update `suggest` set SuggestContact=@SuggestContact,SuggestContent=@SuggestContent where  SuggestID=@SuggestID";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { SuggestID = suggestDto.SuggestID, SuggestContact = suggestDto.SuggestContact, SuggestContent = suggestDto.SuggestContent }) > 0;
            }
        }


        /// <summary>
        /// 删除意见
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBlog(SuggestDto suggestDto)
        {
            string sql = "update `suggest` set IsDelete=1 where  SuggestID=@SuggestID";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { SuggestID = suggestDto.SuggestID }) > 0;
            }
        }
    }
}
