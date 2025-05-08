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
    public class UsersController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public UsersController(AppDbContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_context.UserData.ToList());
        }


        [HttpPost("login"), AllowAnonymous]
        public IActionResult LoginUser([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            
           

            if (user == null)
            {
                return Unauthorized("Неверный email или пароль.");
            }

            bool isPasswordValid = PasswordHelper.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized("Неверный email или пароль.");
            }

            var jwt = _jwtTokenGenerator.GenerateToken(user.Email, user.Id);
            return Ok(new { token = jwt, message = "Успешный вход", userId = user.Id, name = user.Name, role = user.Role });

        }
        
        public class RegisterRequest
        {
            public string Name { get; set; } 
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public List<int>? SubjectIds { get; set; }

        }

        [HttpPost("register"), AllowAnonymous]
        public IActionResult RegisterUser([FromBody] RegisterRequest registerRequest)
        {
            Debug.WriteLine("Пользователь called with request: {Request}", JsonSerializer.Serialize(registerRequest));
            if (registerRequest == null) return BadRequest("Некорректные данные");

            // Проверка на существование пользователя с таким email
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == registerRequest.Email);
            if (existingUser != null)
            {
                return BadRequest("Пользователь с таким email уже существует.");
            }

            // Создание нового пользователя
            var user = new User
            {
                Name = registerRequest.Name, 
                Email = registerRequest.Email, // Устанавливаем email пользователя
                PasswordHash = PasswordHelper.HashPassword(registerRequest.Password), // Хэшируем пароль
                Role = registerRequest.Role, // Устанавливаем роль пользователя
            };
            
            _context.Users.Add(user);
            _context.SaveChanges();

            // Обработка предметов для преподавателя
            if (registerRequest.Role == "Teacher" && registerRequest.SubjectIds?.Any() == true)
            {
                var existingSubjects = _context.Subjects
                    .Where(s => registerRequest.SubjectIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToList();

                var invalidSubjects = registerRequest.SubjectIds.Except(existingSubjects).ToList();
                if (invalidSubjects.Any())
                {
                    return BadRequest($"Некорректные ID предметов: {string.Join(", ", invalidSubjects)}");
                }

                foreach (var subjectId in registerRequest.SubjectIds)
                {
                    _context.TeacherData.Add(new TeacherData
                    {
                        UserId = user.Id,
                        SubjectId = subjectId
                    });
                }
                _context.SaveChanges();
            }

            return Ok(new { message = "Регистрация прошла успешно" });
        }

        [HttpGet("test/{id}")]
        [AllowRole(Role.Admin)]
        public IActionResult Test(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound("Пользователь не найден.");

            return Ok(user);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] UserRequest userRequest)
        {
            if (userRequest == null) return BadRequest();

            // Создание нового пользователя
            var user = new User
            {
                Name = userRequest.ChildName, // Можно настроить поле Name как, например, имя ребенка
                Email = userRequest.email,
                PasswordHash = "1"
                // Здесь вы можете добавить другие поля, такие как PasswordHash, если нужно
            };
            
            // Добавление пользователя в таблицу Users
            _context.Users.Add(user);
            _context.SaveChanges(); // Сохраняем изменения, чтобы получить Id нового пользователя

            // Создание записей в UserData
            var userData = new UserData
            {
                ChildName = userRequest.ChildName,
                ChildAddress = userRequest.ChildAddress,
                LearningWorkplace = userRequest.LearningWorkplace,
                ChildContact = userRequest.ChildContact,
                MotherName = userRequest.MotherName,
                MotherPassport = userRequest.MotherPassport,
                MotherAddress = userRequest.MotherAddress,
                MotherWorkplace = userRequest.MotherWorkplace,
                MotherContact = userRequest.MotherContact,
                FatherName = userRequest.FatherName,
                FatherPassport = userRequest.FatherPassport,
                FatherAddress = userRequest.FatherAddress,
                FatherWorkplace = userRequest.FatherWorkplace,
                FatherContact = userRequest.FatherContact,
                UserId = user.Id // Указываем внешний ключ для связи с таблицей User
            };
            
            // Добавление данных пользователя в таблицу UserData
            _context.UserData.Add(userData);

            // Создание записи в UserAprrove
            var userApprove = new UserAprrove
            {
                IsApproved = false, // Здесь по умолчанию можно поставить false
                UserId = user.Id // Указываем внешний ключ для связи с таблицей User
            };
            
            // Добавление записи в таблицу UserAprrove
            _context.UserAprrove.Add(userApprove);
            
            // Сохраняем изменения
            _context.SaveChanges();

            return Ok(userRequest); // Возвращаем данные пользователя
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string email)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не загружен.");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Найти пользователя по email и обновить ImageUrl
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.ImageUrl = $"/images/{fileName}";
                _context.SaveChanges();
            }

            return Ok(new { message = "Файл загружен", imageUrl = $"/images/{fileName}" });
        }




        public class ApprovalRequest
        {
            public string AppointmentDate { get; set; }
            public string AppointmentTime { get; set; }
            public string ChildName { get; set; }
            public string Email { get; set; }
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveUser(int id, [FromBody] ApprovalRequest request)
        {
            // Находим запись в таблице UserData
            var userData = await _context.UserData.FindAsync(id);
            if (userData == null)
            {
                return NotFound("Данные пользователя не найдены.");
            }

            // Находим существующего пользователя по Email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound("Пользователь с таким Email не найден.");
            }

            // Генерация нового пароля и его хэширование
            var password = PasswordHelper.GeneratePassword();
            var passwordHash = PasswordHelper.HashPassword(password);

            // Обновляем хэш пароля в базе
            user.PasswordHash = passwordHash;
            await _context.SaveChangesAsync();

            // Проверяем, есть ли запись в UserApprove
            var userApprove = await _context.UserAprrove.FirstOrDefaultAsync(ua => ua.UserId == user.Id);
            if (userApprove == null)
            {
                userApprove = new UserAprrove
                {
                    IsApproved = true,
                    UserId = user.Id
                };
                _context.UserAprrove.Add(userApprove);
            }
            else
            {
                userApprove.IsApproved = true;
            }

            await _context.SaveChangesAsync();

            // Формирование письма
            string subject = "Запись на прослушивание";
            string body = $@"
                Здравствуйте!

                Рады сообщить, что ваш ребенок, {request.ChildName}, записан на прослушивание в музыкальную школу 
                {request.AppointmentDate} в {request.AppointmentTime}.

                Ваш логин: {request.Email}
                Ваш новый пароль: {password}
            ";

            // Отправка письма
            var emailService = new EmailService("smtp.mail.ru", 587, "music_novokubansk@mail.ru", "CLjrv1Ph1U7p3aWdbKza");
            try
            {
                await emailService.SendEmailAsync(request.Email, subject, body);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при отправке письма: {ex.Message}");
            }
        }

        [HttpGet("check")]
        public void Check()
        {

        }


        [HttpGet("{id}")]
        public IActionResult GetUserDetails(int id)
        {
            var userData = (from ud in _context.UserData
                            join u in _context.Users on ud.UserId equals u.Id
                            where ud.Id == id
                            select new 
                            {
                                ud.Id,
                                ud.ChildName,
                                ud.ChildAddress,
                                ud.LearningWorkplace,
                                ud.ChildContact,
                                ud.MotherName,
                                ud.MotherPassport,
                                ud.MotherAddress,
                                ud.MotherWorkplace,
                                ud.MotherContact,
                                ud.FatherName,
                                ud.FatherPassport,
                                ud.FatherAddress,
                                ud.FatherWorkplace,
                                ud.FatherContact,
                                email = u.Email // Добавляем email пользователя
                            }).FirstOrDefault();

            if (userData == null)
            {
                return NotFound("Данные пользователя не найдены.");
            }

            return Ok(userData);
        }

        [HttpGet("students")]
        [AllowRole(Role.Teacher)]
        public IActionResult GetStudents()
        {
            var students = _context.Users.Where(u => u.Role == Role.Student.ToString()).ToList();

            return Ok(students);
        }
    }
}