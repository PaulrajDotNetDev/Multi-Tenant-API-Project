using MultiDB.DTO;
using MultiDB.Models.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDB.Service.Interface
{
  public interface IClientServices
  {
    ResponseToken LoginDetails(string sUserName, string sPassword);
    Response GetAllUsers();
    Response DecriptText(string sText);
    Response CreateClients(ClientsDTO clientsDTO);
    Response CreateUsers(UsersDetailsDTO userDTO);
    Response GetUser();
    Response GetClientDetails();
    Response CreateClientUsers(ClientUserDTO clientUserDTO);
  }
} 
