using AutoMapper;
using Dncblogs.Domain.Entities;
using Dncblogs.Domain.EntitiesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XNetCoreCommon;

namespace Dncblogs.Web.AotuMapper
{
    public static class MapperConfig
    {
        private readonly static string DateTimeFormString = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 初始化autoMapper 映射
        /// </summary>
        public static void MapperConfigIntit()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User, UserDto>().ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate.ToString(DateTimeFormString)));

                cfg.CreateMap<Category, CategoryDto>();

                cfg.CreateMap<Blog, BlogDto>().ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate.ToString(DateTimeFormString)))
                .ForMember(dto => dto.BodyAbs, opt => opt.MapFrom(src => HtmlTool.ReplaceHtmlTag(src.Body,200)));
                cfg.CreateMap<BlogComment, BlogCommentDto>().ForMember(dto => dto.PostDate, opt => opt.MapFrom(src => src.PostDate.ToString(DateTimeFormString)));
                
                cfg.CreateMap<News, NewsDto>().ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate.ToString(DateTimeFormString)))
                 .ForMember(dto => dto.BodyAbs, opt => opt.MapFrom(src => HtmlTool.ReplaceHtmlTag(src.Body, 200)));
                cfg.CreateMap<NewsComment, NewsCommentDto>().ForMember(dto => dto.PostDate, opt => opt.MapFrom(src => src.PostDate.ToString(DateTimeFormString)));

                cfg.CreateMap<OpenSource, OpenSourceDto>().ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate.ToString(DateTimeFormString)));

                cfg.CreateMap<Note, NoteDto>().ForMember(dto => dto.CreateDate, opt => opt.MapFrom(src => src.CreateDate.ToString(DateTimeFormString)));

            });

        }
    }
}
