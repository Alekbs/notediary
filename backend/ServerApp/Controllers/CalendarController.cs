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
        
    [Route("api/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CalendarController(AppDbContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
           
            
        }
        private const string CalendarId = "80b3b3ca7557025930ae3107d3e4d84fb5508994358e1a2fd496a862a0564ca7@group.calendar.google.com";
        private const string KeyFilePath = "noteddiary-09068dc471f1.json";

        public class LessonRequest
        {
            public string StartTime { get; set; } // ISO 8601 string, e.g. "2025-04-26T16:00"
            public int SubjectId { get; set; }
            public string Teacher { get; set; } // Имя преподавателя
            public string Student { get; set; } // Имя ученика
        }

        [HttpPost("add-lesson")]
        public async Task<IActionResult> AddLesson([FromBody] LessonRequest request)
        {
            try
            {
                // Логируем данные для отладки
                Console.WriteLine($"Summary: {_context.Subjects.Where(s => s.Id ==request.SubjectId)}");
                Console.WriteLine($"Teacher: {request.Teacher}");
                Console.WriteLine($"Student: {request.Student}");
                
                var credential = await GetCredentialFromJsonKey();
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ASP.NET Google Calendar Integration",
                });

                // Парсим время начала и конца занятия
                var moscowTime = DateTime.Parse(request.StartTime, null, DateTimeStyles.AssumeLocal);
                var eventStart = moscowTime;
                var eventEnd = moscowTime.AddMinutes(40);

                var newEvent = new Event
                {
                    Summary = _context.Subjects.Where(s => s.Id ==request.SubjectId).ToString(),
                    Description = $"teacher: {request.Teacher}, student: {request.Student}",
                    Start = new EventDateTime
                    {
                        DateTime = eventStart,
                        TimeZone = "Europe/Moscow"
                    },
                    End = new EventDateTime
                    {
                        DateTime = eventEnd,
                        TimeZone = "Europe/Moscow"
                    },
                    Reminders = new Event.RemindersData()
                    {
                        UseDefault = true
                    }
                };

                // Добавляем событие в календарь
                var insertRequest = service.Events.Insert(newEvent, CalendarId);
                var createdEvent = await insertRequest.ExecuteAsync();

                // Возвращаем успешный ответ с ссылкой на событие
                return Ok(new { link = createdEvent.HtmlLink });
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибки и возвращаем ответ с ошибкой
                return BadRequest(new { error = ex.Message });
            }
        }


        static async Task<GoogleCredential> GetCredentialFromJsonKey()
        {
            using var stream = new FileStream(KeyFilePath, FileMode.Open, FileAccess.Read);
            var credential = await GoogleCredential.FromStreamAsync(stream, CancellationToken.None);
            return credential.CreateScoped(new[]
            {
                CalendarService.Scope.Calendar,
                CalendarService.Scope.CalendarEvents
            });
        }
        [HttpGet("get-lessons")]
        public async Task<IActionResult> GetLessons([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                var credential = await GetCredentialFromJsonKey();
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ASP.NET Google Calendar Integration",
                });

                var request = service.Events.List(CalendarId);
                request.TimeMin = DateTime.Parse(startDate).ToUniversalTime();
                request.TimeMax = DateTime.Parse(endDate).ToUniversalTime();
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                var events = await request.ExecuteAsync();

                var lessons = events.Items.Select(e => new
                {
                    Summary = e.Summary,
                    StartTime = e.Start.DateTime,
                    EndTime = e.End.DateTime,
                    Description = e.Description
                }).ToList();

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}