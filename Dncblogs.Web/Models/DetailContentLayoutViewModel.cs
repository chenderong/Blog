using Dncblogs.Domain.EntitiesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dncblogs.Web.Models
{
    public class DetailContentLayoutViewModel
    {
        public UserDto UserDto { get; set; }

        public DetailContentLayoutViewModel()
        {
            UserDto = new UserDto();
        }
    }
}
