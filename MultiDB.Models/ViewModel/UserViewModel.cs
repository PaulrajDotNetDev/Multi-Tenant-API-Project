using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDB.Models.ViewModel
{
  public class UserViewModel
  {
  }
  public class User
  {
    public int Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ConnectionString { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
  }
  public class user_details
  {
    public int id { get; set; }
    // public string preferences { get; set; }
    public DateTime created_at { get; set; }
    public bool is_active { get; set; }
    public bool is_deleted { get; set; }
    public string client_id { get; set; }
    public string full_name { get; set; }
    public string phone_number { get; set; }
    public string address { get; set; }
    public string user_name { get; set; } 
    public string password { get; set; }
  }
  public class users
  {    
    public string role { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string emailaddress { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

  }
}
