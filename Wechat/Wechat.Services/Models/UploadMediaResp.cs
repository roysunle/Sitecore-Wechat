using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.Services.Models
{
    public class UploadMediaResp
    {
        public string type { get; set; }
        public string media_id { get; set; }
        public string created_at { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
        
            
    }
}
