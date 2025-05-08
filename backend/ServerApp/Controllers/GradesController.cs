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
    public class GradesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GradesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetGrades()
        {
            var grades = _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Select(g => new
                {
                    g.Student.Name,
                    Subject = g.Subject.Name,
                    g.Date,
                    g.GradeValue
                })
                .ToList();

            return Ok(grades);
        }

        [HttpPost]
        public IActionResult AddGrade([FromBody] GradeDto gradeDto)
        {
            var grade = new Grade
            {
                StudentId = gradeDto.StudentId,
                SubjectId = gradeDto.SubjectId,
                Date = gradeDto.Date,
                GradeValue = gradeDto.Grade
            };

            _context.Grades.Add(grade);
            _context.SaveChanges();

            return Ok(grade);
        }
    }

    public class GradeDto
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public DateTime Date { get; set; }
        public int Grade { get; set; }
    }
}