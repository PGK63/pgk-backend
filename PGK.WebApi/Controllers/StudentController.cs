using Market.Application.App.User.Student.Commands.UpdateState;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGK.Application.App.User.Student.Commands.Delete;
using PGK.Application.App.User.Student.Commands.Registration;
using PGK.Application.App.User.Student.Commands.UpdateGroup;
using PGK.Application.App.User.Student.Queries.GetStudentUserDetails;
using PGK.Application.App.User.Student.Queries.GetStudentUserList;
using PGK.Domain.User.Student;

namespace PGK.WebApi.Controllers
{
    public class StudentController : Controller
    {
        /// <summary>
        /// Получить список студентов
        /// </summary>
        /// <param name="search">Ключивые слова для поиска</param>
        /// <param name="userRoles">список пользовательских ролей разделенные запятой. Например userRoles=HEADMAN,DEPUTY_HEADMAN</param>
        /// <param name="groupIds">список id групп разделенные запятой. Например groupIds=1,2,3</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="pageSize">Количество результатов для возврата на страницу</param>
        /// <returns>StudentUserListVm object</returns>
        /// <response code="200">Запрос выполнен успешно</response>
        /// <response code="401">Пустой или неправильный токен</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentUserListVm))]
        public async Task<ActionResult> GetAll(
            string? search,
            StudentState? state,
            [FromQuery] List<int> groupIds,
            [FromQuery] List<int> departmenIds,
            [FromQuery] List<int> specialityIds,
            int pageNumber = 1, int pageSize = 20)
        {
            var query = new GetStudentUserListQuery {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                DepartmenIds = departmenIds,
                SpecialityIds = specialityIds,
                GroupIds = groupIds,
                State = state
            };

            var vm = await Mediator.Send(query);

            return Ok(vm);
        }

        /// <summary>
        /// Получить пстудента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор студента</param>
        /// <returns>StudentDto object</returns>
        /// <response code="200">Запрос выполнен успешно</response>
        /// <response code="401">Пустой или неправильный токен</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        public async Task<ActionResult> GetById(int id)
        {
            var query = new GetStudentUserDetailsQuery
            {
                Id = id
            };

            var dto = await Mediator.Send(query);

            return Ok(dto);
        }

        [Authorize(Roles = "TEACHER,EDUCATIONAL_SECTOR,ADMIN")]
        [HttpPatch("{id}/State")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        public async Task<ActionResult> UpdateState(int id, StudentState state)
        {
            var command = new StudentUpdateStateCommand
            {
                StudentId = id,
                State = state,
                UserId = UserId,
                Role = UserRole.Value
            };
        
            var dto = await Mediator.Send(command);
        
            return Ok(dto);
        }

        [Authorize(Roles = "TEACHER,EDUCATIONAL_SECTOR,ADMIN")]
        [HttpPatch("{studentId}/Group/{groupId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        public async Task<ActionResult> StudentUpdateGroup(int studentId, int groupId)
        {
            var command = new StudentUpdateGroupCommand
            {
                StudentId = studentId,
                GroupId = groupId,
                UserId = UserId,
                UserRole = UserRole.Value
            };

            var dto = await Mediator.Send(command);

            return Ok(dto);
        }

        /// <summary>
        /// Зарегистрировать студента
        /// </summary>
        /// <param name="command">RegistrationStudentCommand object</param>
        /// <returns>RegistrationStudentVm object</returns>
        /// <response code="200">Запрос выполнен успешно</response>
        /// <response code="401">Пустой или неправильный токен</response>
        /// <response code="403">Авторизация роль TEACHER,EDUCATIONAL_SECTOR,ADMIN</response>
        [Authorize(Roles = "TEACHER,EDUCATIONAL_SECTOR,ADMIN")]
        [HttpPost("Registration")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegistrationStudentVm))]
        public async Task<ActionResult> Registration(
            RegistrationStudentCommand command)
        {
           var vm = await Mediator.Send(command);

            return Ok(vm);
        }


        /// <summary>
        /// Удалить студента
        /// </summary>
        /// <param name="id">Идентификатор студента</param>
        /// <returns>Returns NoContend</returns>
        /// <response code="200">Запрос выполнен успешно</response>
        /// <response code="401">Пустой или неправильный токен</response>
        /// <response code="403">Авторизация роль EDUCATIONAL_SECTOR,DEPARTMENT_HEAD,ADMIN</response>
        [Authorize(Roles = "EDUCATIONAL_SECTOR,DEPARTMENT_HEAD,ADMIN")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteStudentCommand
            {
                StudentId = id
            };

            await Mediator.Send(command);

            return Ok();
        }
    }
}
