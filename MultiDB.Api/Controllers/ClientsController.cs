using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiDB.DTO;
using MultiDB.Service.Interface;
using Newtonsoft.Json;

namespace MultiDB.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        public readonly IClientServices _clientServices;
        private readonly EncryptionHelper _encryptionHelper;
        public ClientsController(IClientServices clientServices)
        {
            _clientServices = clientServices;
            _encryptionHelper = new EncryptionHelper();
        }
        [Route("LoginDetails")]
        [HttpGet]
        public IActionResult LoginDetails([FromQuery] string sUserName, [FromQuery] string sPassword)
        {
            try
            {
                var result = _clientServices.LoginDetails(sUserName, sPassword);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // [Authorize]
        [Route("GetAllUsers")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var clientId = User?.FindFirst("client_id")?.Value;
                var result = _clientServices.GetAllUsers();
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [Route("Encrypt")]
        [HttpGet]
        public IActionResult Encrypt()
        {
            try
            {
                // var result = _encryptionHelper.Encrypt("test");
                return Ok("test");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Route("Decrypt")]
        [HttpGet]
        public IActionResult Decrypt([FromQuery] string sText)
        {
            try
            {
                var result = _encryptionHelper.Decrypt(sText);
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Authorize]
        [Route("GetUsers")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var clientId = User?.FindFirst("client_id")?.Value;
                var result = _clientServices.GetUser();
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Route("GetClientDetails")]
        [HttpGet]
        public IActionResult GetClientDetails()
        {
            try
            {
                var result = _clientServices.GetClientDetails();
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Route("CreateClients")]
        [HttpPost]
        public IActionResult CreateClients([FromBody] ClientsDTO clientsDTO)
        {
            try
            {
                var result = _clientServices.CreateClients(clientsDTO);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("CreateUsers")]
        [HttpPost]
        public IActionResult CreateUsers([FromBody] UsersDetailsDTO userDTO)
        {
            try
            {
                var result = _clientServices.CreateUsers(userDTO);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [Route("CreateClientUsers")]
        [HttpPost]
        public IActionResult CreateClientUsers([FromBody] ClientUserDTO clientUserDTO)
        {
            try
            {
                var result = _clientServices.CreateClientUsers(clientUserDTO);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
