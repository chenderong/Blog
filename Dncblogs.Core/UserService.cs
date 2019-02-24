using AutoMapper;
using Dapper;
using Dncblogs.Domain.Entities;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XNetCoreCommon;

namespace Dncblogs.Core
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserService : BaseService
    {
        private WebStaticConfig _webStaticConfig;
        public UserService(IOptions<DatabaseSetting> options, IOptions<WebStaticConfig> optionw) : base(options)
        {
            this._webStaticConfig = optionw.Value;
        }

        /// <summary>
        /// 获取一个用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetOneUserByUserIdAsync(int userId)
        {
            UserDto userDto = new UserDto();
            string sql = "select u.UserId,u.UserName,u.UserHeadImaUrl,u.BlogTitle,u.BlogName,u.BlogDesc,u.BlogNotice,u.CreateDate,c.CategoryId,c.ParentId,c.CategoryName,c.Sort from `user` u left join `category` c on u.UserId=c.UserId where u.UserId =@UserId ";
            string totlsql = "select CategoryID,count(1) BlogCount from `blog` where UserId=@UserId and IsDelete=0 GROUP BY CategoryID";
            User u = null;
            List<Category> clist = new List<Category>();
            IEnumerable<CategoryDto> caList;
            using (var connection = CreateConnection())
            {
                var user = await connection.QueryAsync<User, Category, User>(sql, (quser, qcategory) =>
                {
                    if (qcategory != null)
                        clist.Add(qcategory);
                    return quser;
                }, new { UserId = userId },
                   splitOn: "UserId,CategoryId");
                u = user.FirstOrDefault();
                caList = await connection.QueryAsync<CategoryDto>(totlsql, new { UserId = userId });
            }
            if (u != null)
            {
                userDto = Mapper.Map<User, UserDto>(u);
                List<CategoryDto> allCategories = new List<CategoryDto>();
                //一级分类   
                var pclist = clist.Where(p => p.ParentId == 0).OrderBy(p => p.Sort);
                CategoryDto cgDto;
                foreach (var ic in pclist)
                {
                    cgDto = Mapper.Map<Category, CategoryDto>(ic);
                    var countCg = caList.FirstOrDefault(p => p.CategoryId == ic.CategoryId);
                    if (countCg != null)
                        cgDto.BlogCount = countCg.BlogCount;

                    allCategories.Add(cgDto);
                    //查询是否有子类  只考虑2级
                    var cclist = clist.Where(p => p.ParentId == cgDto.CategoryId).OrderBy(p => p.Sort).ToList();
                    if (cclist != null && cclist.Count > 0)
                    {
                        CategoryDto ccgDto;
                        foreach (var icc in cclist)
                        {
                            ccgDto = Mapper.Map<Category, CategoryDto>(icc);
                            countCg = caList.FirstOrDefault(p => p.CategoryId == icc.CategoryId);
                            if (countCg != null)
                                ccgDto.BlogCount = countCg.BlogCount;

                            cgDto.ChildCategoryDtoList.Add(ccgDto);
                        }

                        cgDto.BlogCount += cgDto.ChildCategoryDtoList.Sum(p => p.BlogCount);
                    }
                }
                userDto.Categorys = allCategories;
            }
            return userDto;
        }

        /// <summary>
        /// 获取一个用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetOneUserByCategoryIDAsync(int categoryID)
        {
            UserDto userDto = new UserDto();
            int userID = 0;
            string sql = "select u.UserId from `user` u left join `category` c on u.UserId=c.UserId where c.CategoryId =@CategoryId ";
            using (var connection = CreateConnection())
            {
                userID = await connection.QueryFirstOrDefaultAsync<int>(sql, new { CategoryId = categoryID });
            }
            if (userID != 0)
                userDto = await GetOneUserByUserIdAsync(userID);

            return userDto;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserByUserIdAsync(int userId)
        {
            string sql = " update `user` set IsDelete=1 where UserId=@UserId ";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { UserId = userId }) > 0;
            }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task<bool> AddUserAsync(UserDto userDto)
        {
            string pwd = $"{userDto.Password}{this._webStaticConfig.MD5Code}";
            string spwd = SecurityTool.MD5Hash(pwd);
            userDto.Password = spwd;
            userDto.UserHeadImaUrl = "/upload/user-head/default.jpg";
            string sql = "insert into `user`(LoginName,UserName,`Password`) VALUES(@LoginName,@UserName,@Password) ";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { LoginName = userDto.LoginName, UserName = userDto.UserName, Password = userDto.Password }) > 0;
            }
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task<bool> CheckLoginNameAsync(string loginName)
        {

            string sql = "select count(1)  from  `user` where LoginName=@LoginName and IsDelete=0";
            using (var connect = CreateConnection())
            {
                return await connect.QueryFirstAsync<int>(sql, new { LoginName = loginName }) > 0;
            }
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserByLoginNameAsync(string loginName)
        {
            UserDto userDto = new UserDto();
            string sql = "select  u.UserId,u.UserName,u.UserHeadImaUrl,u.BlogTitle,u.BlogName,u.BlogDesc,u.BlogNotice from  `user` u where LoginName=@LoginName and IsDelete=0";
            using (var connect = CreateConnection())
            {
                var user = await connect.QueryFirstAsync<User>(sql, new { LoginName = loginName });
                if (user != null)
                    userDto = Mapper.Map<User, UserDto>(user);
            }

            return userDto;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            string sql = "update `user` set UserName=@UserName,UserHeadImaUrl=@UserHeadImaUrl,BlogTitle=@BlogTitle,BlogName=@BlogName,BlogDesc=@BlogDesc,BlogNotice=@BlogNotice  where  UserId=@UserId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new
                {
                    UserName = userDto.UserName,
                    UserHeadImaUrl = userDto.UserHeadImaUrl,
                    BlogTitle = userDto.BlogTitle,
                    BlogName = userDto.BlogName,
                    BlogDesc = userDto.BlogDesc,
                    BlogNotice = userDto.BlogNotice,
                    UserId = userDto.UserId
                }) > 0;
            }
        }


        public async Task<DataResultDto<UserDto>> CheckUser(UserDto userDto)
        {
            DataResultDto<UserDto> dataResultDto = new DataResultDto<UserDto>();
            User user = null;
            string sql = "select UserId,LoginName,UserName,Password,Status,IsAdmin from `user` where LoginName=@LoginName and IsDelete=0 ";
            using (var connect = CreateConnection())
            {
                user = await connect.QueryFirstOrDefaultAsync<User>(sql, new { LoginName = userDto.LoginName });
            }
            if (user == null)
            {
                dataResultDto.Code = 1;
                dataResultDto.Msg = "登陆失败，用户名不正确！";
            }
            else
            {
                if (user.Status == 0)
                {
                    string pwd = $"{userDto.Password}{this._webStaticConfig.MD5Code}";
                    string spwd = SecurityTool.MD5Hash(pwd);
                    if (spwd.ToUpper() == user.Password.ToUpper())
                    {
                        UserDto quserDto = new UserDto();
                        quserDto= Mapper.Map<User, UserDto>(user);
                        dataResultDto.Code = 0;
                        dataResultDto.Msg = "登陆成功!";
                        dataResultDto.DataList = quserDto;
                    }
                    else
                    {
                        dataResultDto.Code = 1;
                        dataResultDto.Msg = "登陆失败，用户名或者密码不对!";
                    }
                }
                else if (user.Status == 1)
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "登陆失败，用户正在审核中！";
                }
                else if (user.Status == 2)
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "登陆失败，用户已冻结,请联系管理员！";
                }
                else if (user.Status == 3)
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "登陆失败，用户已禁用,请联系管理员！";
                }
                else
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "登陆失败，用户名或者密码错误！";
                }
            }

            return dataResultDto;
        }

        /// <summary>
        /// 统计用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> StatisticsUserCount()
        {

            int count = 0;
            string sql = "select count(1) from `user` where  IsDelete=0 ";
            using (var connect = CreateConnection())
            {
                count = await connect.QueryFirstAsync<int>(sql);
            }
            return count;

        }

    }
}
