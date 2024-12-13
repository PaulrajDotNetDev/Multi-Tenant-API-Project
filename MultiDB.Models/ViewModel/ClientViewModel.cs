using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDB.Models.ViewModel
{
  public class ClientViewModel
  {
  }
  public class clients
  {
    public int id { get; set; } 
    public string client_name { get; set; }
    public string client_email { get; set; }
     
    public string connection_string { get; set; }
    public DateTime created_at { get; set; }
    public bool is_active { get; set; }
    public bool is_deleted { get; set; }

  }
  public class UserDetail
  {
    public int Id { get; set; }
    public string ClientId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Preferences { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
  }
}
