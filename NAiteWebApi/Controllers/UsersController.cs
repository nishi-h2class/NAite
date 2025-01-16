using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using Newtonsoft.Json;

namespace NAiteWebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public UsersController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetUsers([FromQuery] UserParams param)
        {
            Logs.Logger.Debug("ユーザ一覧の取得");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            var users = _repository.User.GetUsers(param);

            var metadata = new
            {
                users.TotalCount,
                users.PageSize,
                users.CurrentPage,
                users.TotalPages,
                users.HasNext,
                users.HasPrevious,
            };
            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var usersResult = _mapper.Map<IEnumerable<UserRes>>(users);

            return Ok(usersResult);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(string id)
        {
            Logs.Logger.Debug("ユーザ情報の取得 Id:" + id);

            var user = _repository.User.GetUser(id);

            if (user is null)
            {
                string msg = $"ユーザID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            var usersResult = _mapper.Map<UserRes>(user);

            return Ok(usersResult);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserParams param)
        {
            Logs.Logger.Debug("ユーザ情報の新規登録");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            if (param is null)
            {
                Logs.Logger.Error("User object sent from client is null.");
                return BadRequest("User object is null");
            }

            if (!ModelState.IsValid)
            {
                string msg = "対象のユーザ情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            if (_repository.User.CheckDuplicateLoginId("", param.LoginId))
            {
                string msg = $"社員番号:{param.LoginId}は登録済みです";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var userEntity = _mapper.Map<User>(param);

            _repository.User.CreateUser(userEntity);
            _repository.Save();

            var createdUser = _mapper.Map<UserRes>(userEntity);

            return CreatedAtAction("GetUser", new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] UpdateUserParams param)
        {
            Logs.Logger.Debug("ユーザ情報の更新");
            Logs.Logger.Debug(JsonConvert.SerializeObject(param));

            if (param is null)
            {
                Logs.Logger.Error("User object sent from client is null.");
                return BadRequest("User object is null");
            }

            if (!ModelState.IsValid)
            {
                string msg = "対象のユーザ情報が編集中です";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            var userEntity = _repository.User.GetUser(id);
            if (userEntity is null)
            {
                string msg = $"ユーザID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }

            if (_repository.User.CheckDuplicateLoginId(id, param.LoginId))
            {
                string msg = $"社員番号:{param.LoginId}は登録済みです";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            _mapper.Map(param, userEntity);
            _repository.User.UpdateUser(userEntity);
            _repository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            Logs.Logger.Debug("ユーザ情報の削除 Id:" + id);

            var user = _repository.User.GetUser(id);
            if (user is null)
            {
                string msg = $"ユーザID:{id}は存在しません";
                Logs.Logger.Error(msg);
                return NotFound(msg);
            }
            if (user.Authority == "admin")
            {
                string msg = $"管理者は削除できません";
                Logs.Logger.Error(msg);
                return BadRequest(msg);
            }

            _repository.User.DeleteUser(user);
            _repository.Save();

            return NoContent();
        }
    }
}
