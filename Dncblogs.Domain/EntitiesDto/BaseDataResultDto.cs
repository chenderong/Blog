using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Dncblogs.Domain.EntitiesDto
{
    public class BaseDataResultDto
    {
        /// <summary>
        /// 0 成功 1 失败
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; } = 1;

        [JsonProperty(PropertyName = "msg")]
        public string Msg { get; set; } = string.Empty;


    }

    public class DataResultDto<T> : BaseDataResultDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; } = 0;

        /// <summary>
        /// 数量
        /// </summary>
        public int total { get { return Count; } }
        /// <summary>
        /// 数据列表
        /// </summary>
        [JsonProperty(PropertyName = "rows")]
        public T DataList { get; set; }

    }
}
