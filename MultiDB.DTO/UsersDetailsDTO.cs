using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDB.DTO
{
  public class UsersDetailsDTO
  {
    public string user_name { get; set; }
    public string password { get; set; }
    public string client_id { get; set; }
    public string full_name { get; set; }
    public string phone_number { get; set; }
    public string address { get; set; }
  }
  public class ClientUserDTO
  {  
    public string role { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string emailaddress { get; set; }
  }
}
