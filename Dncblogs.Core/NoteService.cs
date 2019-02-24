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
    public class NoteService : BaseService
    {
        public NoteService(IOptions<DatabaseSetting> options) : base(options)
        { }



        /// <summary>
        /// 添加随笔
        /// </summary>
        /// <param name="noteDto"></param>
        /// <returns></returns>
        public async Task<bool> AddNote(NoteDto noteDto)
        {
            string sql = "insert into `note`(Title,Content,UserId) VALUES(@Title,@Content,@UserId)";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { Title = noteDto.Title, Content = noteDto.Content, UserId = noteDto.UserId }) > 0;
            }
        }

        /// <summary>
        /// 修改随笔
        /// </summary>
        /// <param name="noteDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNote(NoteDto noteDto)
        {
            string sql = "update `note` set Title=@Title,Content=@Content where NoteID=@NoteID";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { Title = noteDto.Title, Content = noteDto.Content, NoteID = noteDto.NoteID }) > 0;
            }
        }

        /// <summary>
        /// 删除随笔
        /// </summary>
        /// <param name="noteID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteNote(int noteID)
        {
            string sql = "update  `note` set IsDelete=1  where NoteID=@NoteID";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { NoteID = noteID }) > 0;
            }
        }

        /// <summary>
        /// 获取一条随笔
        /// </summary>
        /// <param name="opensourceId"></param>
        /// <returns></returns>
        public async Task<NoteDto> GetOneNoteAsync(int noteID)
        {
            NoteDto noteDto = new NoteDto();
            string sql = "select NoteID,Title,Content,UserId,CreateDate from `note` where NoteID=@NoteID and IsDelete=0  ";
            using (var connection = CreateConnection())
            {
                Note note = await connection.QueryFirstAsync<Note>(sql, new { NoteID = noteID });
                noteDto = Mapper.Map<Note, NoteDto>(note);
            }

            return noteDto;
        }


        /// <summary>
        /// 分页获取随笔
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="keyWord"></param>
        /// <param name="orderFiled"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<DataResultDto<List<NoteDto>>> GetListByPage(DateTime? startTime, DateTime? endTime, string keyWord, string orderFiled, int pageSize, int pageIndex)
        {
            DataResultDto<List<NoteDto>> dataResultDto = new DataResultDto<List<NoteDto>>();
            List<NoteDto> list = new List<NoteDto>();
            string where = "where IsDelete=0 ";


            if (startTime != null && endTime != null)
                where += "  and CreateDate BETWEEN @StartTime and @EndTime ";

            if (!string.IsNullOrEmpty(keyWord))
                where += "  and (Title like @KeyWork or Content like @KeyWork) ";

            string order = string.Empty;
            if (!string.IsNullOrEmpty(orderFiled))
                order = string.Format("order by {0}", orderFiled);

            string sql = string.Format("select NoteID,Title,Content,UserId,CreateDate from `note` {0}  {1} limit {2},{3}", where, order, (pageIndex - 1) * pageSize, pageSize);

            string sqlCount = string.Format("select count(1) from `note`  {0}  ", where);

            IEnumerable<Note> blist = null;
            using (var connection = CreateConnection())
            {
                dataResultDto.Count = await connection.QueryFirstAsync<int>(sqlCount, new { StartTime = startTime, EndTime = endTime, KeyWork = string.Format("%{0}%", keyWord) });

                blist = await connection.QueryAsync<Note>(sql, new { StartTime = startTime, EndTime = endTime, KeyWork = string.Format("%{0}%", keyWord), OrderFiled = orderFiled });
            }

            NoteDto noteDto = null;
            foreach (var io in blist)
            {
                noteDto = Mapper.Map<Note, NoteDto>(io); 
                list.Add(noteDto);
            }
            dataResultDto.Code = 0;
            dataResultDto.DataList = list;
            return dataResultDto;
        }

    }
}
