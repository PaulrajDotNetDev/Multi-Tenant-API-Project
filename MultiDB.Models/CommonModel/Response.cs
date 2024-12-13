using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDB.Models.CommonModel
{
  public class ResponseToken
  {
    public string message { get; set; }
    public string StatusCode { get; set; }
    public object data { get; set; }
    public string token { get; set; }
  }
  public class Response
  {
    public string message{ get; set; }
    public string StatusCode{ get; set; }
    public  string data { get; set; }
    
  }
}
