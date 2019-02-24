using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dncblogs.Domain.Entities;
using System.Linq;
using AutoMapper;

namespace Dncblogs.Core
{
    /// <summary>
    /// 分类
    /// </summary>
    public class CategoryService : BaseService
    {
        public CategoryService(IOptions<DatabaseSetting> options) : base(options)
        {

        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        public async Task<bool> AddCategory(CategoryDto categoryDto)
        {
            string sql = "insert into `category`(CategoryName,ParentId,UserId,Sort) VALUES(@CategoryName,@ParentId,@UserId,@Sort)";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { CategoryName = categoryDto.CategoryName, ParentId = categoryDto.ParentId, UserId = categoryDto.UserId, Sort = categoryDto.Sort }) > 0;
            }
        }


        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        public async Task<List<CategoryDto>> GetAllCategoryByUserId(int userId)
        {
            List<CategoryDto> list = new List<CategoryDto>();
            string sql = "select CategoryId,CategoryName,ParentId,UserId,Sort from `category` where UserId=@userId and IsDelete=0";
            using (var connect = CreateConnection())
            {
                var qList = await connect.QueryAsync<Category>(sql, new { UserId = userId });
                CategoryDto categoryDto = null;
                foreach (var ic in qList)
                {
                    categoryDto = Mapper.Map<Category, CategoryDto>(ic);
                    categoryDto.UserId = userId;
                    list.Add(categoryDto);
                }
            }

            list = list.OrderBy(p => p.ParentId).OrderBy(p => p.Sort).ToList();
            return list;
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        public async Task<CategoryDto> GetOneCategoryByUserId(int categoryId, int userId)
        {
            CategoryDto categoryDto = new CategoryDto();
            string sql = "select CategoryId,CategoryName,ParentId,UserId,Sort from `category` where CategoryId=@CategoryId and UserId=@userId and IsDelete=0";
            using (var connect = CreateConnection())
            {
                var qList = await connect.QueryAsync<Category>(sql, new { CategoryId = categoryId, UserId = userId });
                if (qList.Count() > 0)
                {
                    var cc = qList.ToList();
                    categoryDto = Mapper.Map<Category, CategoryDto>(cc[0]);
                }
            }
            return categoryDto;
        }


        /// <summary>
        /// 获取分类的子类
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<CategoryDto>> GetAllCategoryByUserId(int categoryId, int userId)
        {
            List<CategoryDto> list = new List<CategoryDto>();
            string sql = "select CategoryId,CategoryName,ParentId,UserId,Sort from `category` where  ParentId=@CategoryId and IsDelete=0";
            using (var connect = CreateConnection())
            {
                var qList = await connect.QueryAsync<Category>(sql, new { UserId = userId, CategoryId = categoryId });
                CategoryDto categoryDto = null;
                foreach (var ic in qList)
                {
                    categoryDto = Mapper.Map<Category, CategoryDto>(ic);
                    categoryDto.UserId = userId;
                    list.Add(categoryDto);
                }
            }

            list = list.OrderBy(p => p.ParentId).OrderBy(p => p.Sort).ToList();
            return list;
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCategory(int userId, int categoryId)
        {
            string sql = "update  `category` set IsDelete=1 where UserId=@UserId and CategoryId=@CategoryId ";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { UserId = userId, CategoryId = categoryId }) > 0;
            }
        }

        /// <summary>
        /// 修改分类信息
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateCategory(CategoryDto categoryDto)
        {
            string sql = "update `category` set CategoryName=@CategoryName,ParentId=@ParentId,Sort=@Sort where UserId=@UserId and CategoryId=@CategoryId";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { CategoryName = categoryDto.CategoryName, CategoryId = categoryDto.CategoryId, ParentId = categoryDto.ParentId, Sort = categoryDto.Sort, UserId = categoryDto.UserId }) > 0;
            }
        }

        /// <summary>
        /// 获取分类  树结构
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Node>> GetNodeListByUserIdAsync(int userId)
        {
            List<CategoryDto> list = await GetAllCategoryByUserId(userId);
            List<Node> nodeList = new List<Node>();
            var parentList = list.Where(p => p.ParentId == 0).OrderBy(p => p.Sort);
            foreach (var ip in parentList)
                nodeList.Add(new Node() { id = ip.CategoryId, text = ip.CategoryName, pId = 0, userId = ip.UserId, sort = ip.Sort });
            foreach (var iroot in nodeList)
                BuildTree(iroot, iroot.id, list);

            return nodeList;
        }

        private void BuildTree(Node rootNode, int parentID, List<CategoryDto> dataList)
        {
            List<CategoryDto> queryList = dataList.Where(p => p.ParentId == parentID).OrderBy(p => p.Sort).ToList();
            if (queryList.Count > 0)
            {
                rootNode.nodes = new List<Node>();
                foreach (var ic in queryList)
                {
                    Node n = new Node();
                    n.id = ic.CategoryId;
                    n.text = ic.CategoryName;
                    n.pId = ic.ParentId;
                    n.userId = ic.UserId;
                    n.sort = ic.Sort;
                    rootNode.nodes.Add(n);
                    BuildTree(n, n.id, dataList);
                }
            }
        }
    }
}
