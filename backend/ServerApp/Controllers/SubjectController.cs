using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;
using ServerApp.Utilities;
using ServerApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.Globalization;
namespace ServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult GetSubjects()
        {
            var subjects = _context.Subjects
                .Select(s => new { s.Id, s.Name })
                .ToList();

            return Ok(subjects);
        }
        [HttpGet("teacher/{id}"), AllowAnonymous]
        public IActionResult GetSubjectsByTeacherId(int id)
        {
            var subjects = _context.TeacherData
                .Include(x => x.Subject)
                .Select(td => new { td.UserId, td.SubjectId, td.Subject.Name})
                .Where(s => s.UserId == id)
                .ToList();
            return Ok(subjects);
        }
    }
}

