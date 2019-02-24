using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using Dncblogs.Domain.EntitiesDto;
using System.Net.Http.Headers;
using System.IO;

namespace Dncblogs.Web.Controllers
{
    public class FileOperateController : Controller
    {
        private IHostingEnvironment _env;
        private WebStaticConfig _webStaticConfig;

        public FileOperateController(IHostingEnvironment env, IOptions<WebStaticConfig> options)
        {
            this._env = env;
            this._webStaticConfig = options.Value;
        }

        // <summary>
        /// 图片上传
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<JsonResult> ImgUpload()
        {
            try
            {
                var imgFile = Request.Form.Files[0];
                if (imgFile == null || string.IsNullOrEmpty(imgFile.FileName))
                    return Json(new BaseDataResultDto() { Code = 1, Msg = "上传失败" });
                string altname = imgFile.FileName;
                string filename = ContentDispositionHeaderValue.Parse(imgFile.ContentDisposition).FileName.Trim();
                //扩展名，如.jpg
                string extname = filename.Substring(filename.LastIndexOf('.'), filename.Length - filename.LastIndexOf('.'));
                extname = extname.Replace("\"", "");
                if (!extname.ToLower().Contains("jpg") && !extname.ToLower().Contains("png") && !extname.ToLower().Contains("gif"))
                    return Json(new BaseDataResultDto() { Code = 1, Msg = "只允许上传jpg,png,gif格式的图片." });
                //判断文件大小
                long mb = imgFile.Length / 1024 / 1024; // MB
                if (mb > this._webStaticConfig.ImgMaxSize)
                    return Json(new BaseDataResultDto() { Code = 1, Msg = "只允许上传小于 5MB 的图片." });
                string imgNewName = string.Format($"{DateTime.Now.ToString("yyyyMMddHHmmssss")}{extname}");
                //网站静态文件目录  wwwroot
                var rootPath = _env.WebRootPath;
                //完整物理路径
                string pyPath = rootPath + $"{Path.DirectorySeparatorChar}upload{Path.DirectorySeparatorChar}user-head{Path.DirectorySeparatorChar}";
                if (!Directory.Exists(pyPath))
                    Directory.CreateDirectory(pyPath);
                filename = pyPath + imgNewName;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    await imgFile.CopyToAsync(fs);
                    fs.Flush();
                }
                return Json(new BaseDataResultDto() { Code = 0, Msg = $"{Path.DirectorySeparatorChar}upload{Path.DirectorySeparatorChar}user-head{Path.DirectorySeparatorChar}{imgNewName}" });
            }
            catch (Exception ex)
            {
                return Json(new BaseDataResultDto() { Code = 1, Msg = $"上传失败,原因：{ ex }" });
            }
        }
    }
}