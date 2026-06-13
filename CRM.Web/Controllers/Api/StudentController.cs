using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Enums;

namespace CRM.Web.Controllers.Api;
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // POST: api/Student/convert/{leadId}
        [HttpPost("convert/{leadId:guid}")]
        public async Task<ActionResult<StudentDto>> ConvertLead(Guid leadId, [FromBody] StudentConversionDto conversionDto)
        {
            var student = await _studentService.ConvertLeadAsync(leadId, conversionDto);
            var resultDto = await _studentService.GetStudentAsync(student.Id);
            return Ok(resultDto);
        }

        // GET: api/Student/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StudentDto>> GetStudent(Guid id)
        {
            var dto = await _studentService.GetStudentAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        // PUT: api/Student/status/{id}
        [HttpPut("status/{id:guid}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StudentStatusUpdateDto statusDto)
        {
            var actorId = User?.FindFirst("sub")?.Value;
            Guid actorGuid = Guid.Empty;
            if (!Guid.TryParse(actorId, out actorGuid))
                actorGuid = Guid.Empty;
            await _studentService.UpdateStatusAsync(id, statusDto, actorGuid);
            return NoContent();
        }

        // GET: api/Student/inactive
        [HttpGet("inactive")]
        public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> GetInactiveStudents()
        {
            var students = await _studentService.GetInactiveStudentsAsync();
            return Ok(students);
        }

        // GET: api/Student/inactive/export
        [HttpGet("inactive/export")]
        public async Task<IActionResult> ExportInactive()
        {
            var bytes = await _studentService.ExportInactiveAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InactiveStudents.xlsx");
        }
    }
}
