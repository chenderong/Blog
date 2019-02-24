using AutoMapper;
using Dapper;
using Dncblogs.Domain.Entities;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dncblogs.Core
{
    public class OpenSourceService : BaseService
    {
        public OpenSourceService(IOptions<DatabaseSetting> options) : base(options)
        { }


        /// <summary>
        /// 添加开源项目
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> AddOpensource(OpenSourceDto openSourceDto)
        {
            string sql = "insert into `opensource`(OpenSourceType,OpenSourceTitle,OpenSourceDescribe,OpenSourceContent,HeatType,Sort,CreateDate) VALUES(@OpenSourceType,@OpenSourceTitle,@OpenSourceDescribe,@OpenSourceContent,@HeatType,@Sort,@CreateDate)";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { OpenSourceType = openSourceDto.OpenSourceType, OpenSourceTitle = openSourceDto.OpenSourceTitle, OpenSourceDescribe = openSourceDto.OpenSourceDescribe, OpenSourceContent = openSourceDto.OpenSourceContent, HeatType = openSourceDto.HeatType, Sort = openSourceDto.Sort, CreateDate = DateTime.Now }) > 0;
            }
        }

        /// <summary>
        /// 修改开源项目
        /// </summary>
        /// <param name="blogDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOpensource(OpenSourceDto openSourceDto)
        {
            string sql = "update `opensource` set OpenSourceType=@OpenSourceType,OpenSourceTitle=@OpenSourceTitle,OpenSourceDescribe=@OpenSourceDescribe,OpenSourceContent=@OpenSourceContent,HeatType=@HeatType,Sort=@Sort where OpenSourceID=@OpenSourceID";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { OpenSourceType = openSourceDto.OpenSourceType, OpenSourceTitle = openSourceDto.OpenSourceTitle, OpenSourceDescribe = openSourceDto.OpenSourceDescribe, OpenSourceContent = openSourceDto.OpenSourceContent, HeatType = openSourceDto.HeatType, Sort = openSourceDto.Sort, OpenSourceID = openSourceDto.OpenSourceID }) > 0;
            }
        }

        /// <summary>
        /// 删除开源项目
        /// </summary>
        /// <param name="openSourceID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteOpensource(int openSourceID)
        {
            string sql = "update  `opensource` set IsDelete=1  where OpenSourceID=@OpenSourceID";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { OpenSourceID = openSourceID }) > 0;
            }
        }

        /// <summary>
        /// 获取一条开源项目
        /// </summary>
        /// <param name="opensourceId"></param>
        /// <returns></returns>
        public async Task<OpenSourceDto> GetOneOpensourceIdAsync(int opensourceId)
        {
            OpenSourceDto openSourceDto = new OpenSourceDto();
            string sql = "select OpenSourceID,OpenSourceType,OpenSourceTitle,OpenSourceDescribe,OpenSourceContent from `opensource` where OpenSourceID=@OpenSourceID and IsDelete=0  ";
            using (var connection = CreateConnection())
            {
                OpenSource openSource = await connection.QueryFirstAsync<OpenSource>(sql, new { OpenSourceID = opensourceId });
                openSourceDto= Mapper.Map<OpenSource, OpenSourceDto>(openSource);
            }
            return openSourceDto;
        }


        /// <summary>
        /// 分页获取开源项目
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="keyWord"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<OpenSourceDto>>> GetListByPage(DateTime? startTime, DateTime? endTime, string keyWord, string orderFiled, int pageSize, int pageIndex)
        {
            DataResultDto<List<OpenSourceDto>> dataResultDto = new DataResultDto<List<OpenSourceDto>>();
            List<OpenSourceDto> list = new List<OpenSourceDto>();
            string where = "where IsDelete=0 ";


            if (startTime != null && endTime != null)
                where += "  and CreateDate BETWEEN @StartTime and @EndTime ";

            if (!string.IsNullOrEmpty(keyWord))
                where += "  and (OpenSourceTitle like @KeyWork or OpenSourceContent like @KeyWork or OpenSourceDescribe like @KeyWork) ";

            string order = string.Empty;
            if (!string.IsNullOrEmpty(orderFiled))
                order = string.Format("order by {0}", orderFiled);

            string sql = string.Format("select OpenSourceID,OpenSourceTitle,OpenSourceType,OpenSourceDescribe,OpenSourceContent,CreateDate from `opensource` {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

            string sqlCount = string.Format("select count(1) from `opensource`  {0}  ", where);

            IEnumerable<OpenSource> blist = null;
            using (var connection = CreateConnection())
            {
                dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { StartTime = startTime, EndTime = endTime, KeyWork = string.Format("%{0}%", keyWord) });

                blist = await connection.QueryAsync<OpenSource>(sql, new { StartTime = startTime, EndTime = endTime, KeyWork = string.Format("%{0}%", keyWord), OrderFiled = orderFiled });
            }

            OpenSourceDto openSourceDto = null;
            foreach (var io in blist)
            {
                openSourceDto= Mapper.Map<OpenSource, OpenSourceDto>(io);
                list.Add(openSourceDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }

        /// <summary>
        /// 分页获取开源项目
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyWork"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<OpenSourceDto>>> GetListByPage(string keyWork, string orderFiled, int pageSize, int pageIndex)
        {
            DataResultDto<List<OpenSourceDto>> dataResultDto = new DataResultDto<List<OpenSourceDto>>();
            List<OpenSourceDto> list = new List<OpenSourceDto>();
            string where = "where IsDelete=0 ";

            if (!string.IsNullOrEmpty(keyWork))
                where += "  and (OpenSourceTitle like @KeyWork or OpenSourceContent like @KeyWork or OpenSourceDescribe like @KeyWork) ";

            string order = string.Empty;
            if (!string.IsNullOrEmpty(orderFiled))
                order = string.Format("order by {0}", orderFiled);

            string sql = string.Format("select OpenSourceID,OpenSourceTitle,OpenSourceType,OpenSourceDescribe,OpenSourceContent,CreateDate from `opensource` {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

            string sqlCount = string.Format("select count(1) from `opensource`  {0}  ", where);

            IEnumerable<OpenSource> blist = null;
            using (var connection = CreateConnection())
            {
                dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { KeyWork = string.Format("%{0}%", keyWork) });
                blist = await connection.QueryAsync<OpenSource>(sql, new { KeyWork = string.Format("%{0}%", keyWork), OrderFiled = orderFiled });
            }
            OpenSourceDto openSourceDto = null;
            foreach (var io in blist)
            {
                openSourceDto = Mapper.Map<OpenSource, OpenSourceDto>(io);
                list.Add(openSourceDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }



        public async Task<DataResultDto<List<OpenSourceDto>>> GetList(int HeatType, int count)
        {
            DataResultDto<List<OpenSourceDto>> dataResultDto = new DataResultDto<List<OpenSourceDto>>();
            List<OpenSourceDto> list = new List<OpenSourceDto>();
            string where = $"where IsDelete=0 and HeatType={HeatType}";
            string sql = string.Format("select OpenSourceID,OpenSourceTitle,OpenSourceType,OpenSourceDescribe,OpenSourceContent,HeatType,Sort,CreateDate from `opensource` {0} order by Sort,CreateDate desc limit 0,{1}", where, count);
            IEnumerable<OpenSource> blist = null;
            using (var connection = CreateConnection())
            {
                blist = await connection.QueryAsync<OpenSource>(sql);
            }
            OpenSourceDto openSourceDto = null;
            foreach (var io in blist)
            {
                openSourceDto = Mapper.Map<OpenSource, OpenSourceDto>(io);
                openSourceDto.OpenSourceContent = string.Empty;//优化，不传到显示页面
                list.Add(openSourceDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }


        /// <summary>
        /// 获取开源项目数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> StatisticsOpenSourceCount()
        {
            int count = 0;
            string sql = "select count(1) from `opensource` where  IsDelete=0 ";
            using (var connect = CreateConnection())
            {
                count = await connect.QueryFirstAsync<int>(sql);
            }
            return count;
        }
    }
}
