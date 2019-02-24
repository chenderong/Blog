using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Entities
{
    public class Category
    {
        /// <summary>
        /// 主键自增
        /// </summary>
        public int CategoryId { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { set; get; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 父类Id
        /// </summary>
        public int ParentId { get; set; }

        public virtual Category ParentCategory { get; set; }

        /// <summary>
        /// 博客列表
        /// </summary>
        public virtual ICollection<Blog> Blogs
        {
            get { return blogs; }
            set { blogs = value; }
        }

        private ICollection<Blog> blogs = new List<Blog>();


        /// <summary>
        /// 用名
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }




    }
}
