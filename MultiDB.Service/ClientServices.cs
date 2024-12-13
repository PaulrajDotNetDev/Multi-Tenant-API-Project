using Microsoft.Extensions.Configuration;
using MultiDB.DTO;
using MultiDB.Models.CommonModel;
using MultiDB.Models.ViewModel;
using MultiDB.Service.Helpers;
using MultiDB.Service.Interface;
using MultiDB.Services;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MultiDB.Service
{
  public class ClientServices : IClientServices
  {
    private readonly Response _res;
    private readonly IEntityService<clients> _client;
    private readonly IEntityService<user_details> _users;
    private readonly IEntityService<users> _user;
    private readonly IConfiguration _configuration;
    private readonly EncryptionHelper _encryptionHelper;
    public ClientServices(Response res, IEntityService<clients> client, IConfiguration configuration, IEntityService<user_details> users,
      IEntityService<users> user)
    {
      _res = res;
      _client = client;
      _configuration = configuration;
      _users = users;
      _user = user;
      // _encryptionHelper = new EncryptionHelper(_configuration);
      _encryptionHelper = new EncryptionHelper();
    }
    public ResponseToken LoginDetails(string sUserName, string sPassword)
    {
      try
      {
        var result = _users.GetModel()
                            .FirstOrDefault(x => x.user_name.ToLower() == sUserName.ToLower() &&
                                                 x.password.ToLower() == sPassword.ToLower());
        if (result != null)
        {
          return new ResponseToken
          {
            token = Helpers.JwtTokenService.GenerateToken(result.client_id, _configuration).ToString(),
            message = "Success",
            StatusCode = "200",
            data = null
          };
        }
        else
        {
          return new ResponseToken
          {
            token = null,
            message = "Invalid username or password",
            StatusCode = "401",
            data = null
          };
        }
      }
      catch (Exception ex)
      {
        throw new Exception("An error occurred while fetching client details.", ex);
      }
    }

    public Response GetAllUsers()
    {
      try
      {
        var result = _users.GetModel().ToList();
        if (result != null)
        {
          _res.message = "Success";
          _res.StatusCode = "200";
          _res.data = JsonConvert.SerializeObject(result);
        }
        else
        {
          _res.message = "No Record Found";
          _res.StatusCode = "404";
          _res.data = null;
        }

        return _res;
      }
      catch (Exception ex)
      {

        throw;
      }
    }

    public Response DecriptText(string sText)
    {
      try
      {
        if (!string.IsNullOrEmpty(sText))
        {
          _res.data = _encryptionHelper.Decrypt(sText);
          _res.StatusCode = "200";
          _res.message = "Success";
        }
        else
        {
          _res.data = null;
          _res.StatusCode = "200";
          _res.message = "Success";
        }
      }
      catch (Exception ex)
      {

        _res.data = null;
        _res.StatusCode = "500";
        _res.message = "Internal Server Error";
      }
      return _res;
    }

    public Response CreateClients(ClientsDTO clientsDTO)
    {
      try
      {

        var newClient = new clients
        {
          client_email = clientsDTO.client_email,
          connection_string = $"Host={_configuration["DBSettings:Host"]};Port={_configuration["DBSettings:Port"]};Database={clientsDTO.client_name};Username={_configuration["DBSettings:Username"]};Password={_configuration["DBSettings:Password"]}",
          client_name = clientsDTO.client_name
        };        
        _client.ExecuteQuery(@"
	CREATE TABLE IF NOT EXISTS clients (
    id SERIAL PRIMARY KEY,            
    client_name VARCHAR(100) NOT NULL,       
    client_email VARCHAR(255) NOT NULL,            
    connection_string TEXT NOT NULL,         
    created_at TIMESTAMP DEFAULT NOW(),      
    is_active BOOLEAN DEFAULT TRUE,          
    is_deleted BOOLEAN DEFAULT FALSE         
);");
        if (!CheckExistingClient(clientsDTO.client_name))
        {

          _client.Insert(newClient);
          _client.ExecuteQuery($"CREATE DATABASE {clientsDTO.client_name};");

          _res.data = null;
          _res.StatusCode = "200";
          _res.message = "Success";
        }
        else
        {

          _res.data = null;
          _res.StatusCode = "204";
          _res.message = "Client already exists";
        }
      }
      catch (Exception ex)
      {
        _res.data = null;
        _res.StatusCode = "500";
        _res.message = "Internal Server Error";
      }

      return _res;
    }
    public Response CreateUsers(UsersDetailsDTO userDTO)
    {
      try
      {

        var newUser = new user_details
        {
          client_id = userDTO.client_id,
          user_name = userDTO.user_name,
          full_name = userDTO.full_name,
          password = userDTO.password,
          address = userDTO.address,
          phone_number = userDTO.phone_number

        };
        _client.ExecuteQuery(@"CREATE TABLE IF NOT EXISTS public.user_details
(
    id serial NOT NULL  ,
    client_id character varying(50) COLLATE pg_catalog.""default"" NOT NULL,
    full_name character varying(150) COLLATE pg_catalog.""default"" NOT NULL,
    phone_number character varying(15) COLLATE pg_catalog.""default"",
    address text COLLATE pg_catalog.""default"",
    preferences jsonb,
    created_at timestamp without time zone DEFAULT now(),
    is_active boolean DEFAULT true,
    is_deleted boolean DEFAULT false,
    user_name text COLLATE pg_catalog.""default"",
    password text COLLATE pg_catalog.""default"",
    CONSTRAINT user_details_pkey PRIMARY KEY (id)
)");
        _users.Insert(newUser);
        _res.data = null;
        _res.StatusCode = "200";
        _res.message = "Success";

      }
      catch (Exception ex)
      {
        _res.data = null;
        _res.StatusCode = "500";
        _res.message = "Internal Server Error";
      }

      return _res;
    }
    public Response GetUser()
    {
      try
      {

        var result = _user.GetModel().ToList();
        if (result != null && result.Count > 0)
        {
          _res.data = _encryptionHelper.Encrypt(JsonConvert.SerializeObject(result));
          _res.StatusCode = "200";
          _res.message = "Success";
        }
        else
        {

          _res.data = null;
          _res.StatusCode = "404";
          _res.message = "No Record Found";
        }



      }
      catch (Exception ex)
      {
        _res.data = null;
        _res.StatusCode = "500";
        _res.message = "Internal Server Error";
      }

      return _res;
    }
    public Response GetClientDetails()
    {
      try
      {

        var result = _client.GetModel().ToList();
        if (result != null && result.Count > 0)
        {
          _res.data = _encryptionHelper.Encrypt(JsonConvert.SerializeObject(result));
          _res.StatusCode = "200";
          _res.message = "Success";
        }
        else
        {

          _res.data = null;
          _res.StatusCode = "404";
          _res.message = "No Record Found";
        }



      }
      catch (Exception ex)
      {
        _res.data = null;
        _res.StatusCode = "500";
        _res.message = "Internal Server Error";
      }

      return _res;
    }
    public Response CreateClientUsers(ClientUserDTO clientUserDTO)
    {
      try
      {
        var newUser = new users
        {
          role = clientUserDTO.role,
          firstname = clientUserDTO.firstname,
          lastname = clientUserDTO.lastname,
          emailaddress = clientUserDTO.emailaddress

        };
        _client.ExecuteQuery(@"CREATE TABLE if not exists users (
            id SERIAL PRIMARY KEY,
            role VARCHAR(255) NOT NULL,
            firstname VARCHAR(255) NOT NULL,
            lastname VARCHAR(255) NOT NULL,
            emailaddress VARCHAR(255) NOT NULL UNIQUE,
            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ,
            isactive BOOLEAN DEFAULT TRUE
        );");
        _user.Insert(newUser);
        _res.data = null;
        _res.StatusCode = "200";
        _res.message = "Success";


      }
      catch (Exception ex)
      {
        _res.data = null;
        _res.StatusCode = "500";
        _res.message = "Internal Server Error";
      }

      return _res;
    }
    public bool CheckExistingClient(string sClientName)
    {
      try
      {
        using (var conn = new NpgsqlConnection(_configuration["ConnectionStrings:DB"]))
        {
          conn.Open();

          // Use parameterized queries to prevent SQL injection
          string query = "SELECT client_name FROM clients WHERE client_name = @ClientName";

          using (var cmd = new NpgsqlCommand(query, conn))
          {
            // Add parameter to the query
            cmd.Parameters.AddWithValue("@ClientName", sClientName);

            using (var reader = cmd.ExecuteReader())
            {
              if (reader.Read())
              {
                return true;
              }
              else
              {
                return false;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw;
      }
    }

  }

}
