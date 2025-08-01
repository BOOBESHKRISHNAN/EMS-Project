using EventManagementCL.DTO;
using EventManagementCL.Interface;
using EventManagementCL.Models;
using EventManagementCL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;


namespace EventManagementWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenService _jwtService;
        //private readonly NotificationService _notificationService;

        public UserController(IUserRepository userRepo, JwtTokenService jwtService, NotificationService notificationService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            //_notificationService = notificationService;
        }

        //SuperAdmin Register

        //[Authorize(Roles = "RegisteredUser,SuperAdmin")]
        [HttpPost("register-User")]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict("Email already registered.");

            var passwordHash = HashPassword(request.Password);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                PasswordHash = passwordHash,
                Role = UserRole.RegisteredUser,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddUserAsync(newUser);
            //await _notificationService.SendUserWelcomeEmail(newUser);

            return Ok("User registered successfully.");
        }


        //SuperAdmin creates a Admin (location required)

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register-Admin-by-SuperAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict("Email already registered.");

            var passwordHash = HashPassword(request.Password);

            var admin = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                PasswordHash = passwordHash,
                Role = UserRole.Admin,
                Location = request.Location,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddUserAsync(admin);
            //await _notificationService.SendAdminRegistrationEmail(admin);

            return Ok("Admin registered successfully.");
        }

        //SuperAdmin creates a Organizer

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register-Organizer-by-SuperAdmin")]
        public async Task<IActionResult> RegisterOrganizer(RegisterOrganizerRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict("Email already registered.");

            var passwordHash = HashPassword(request.Password);

            var organizer = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                PasswordHash = passwordHash,
                Role = UserRole.Organizer,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepo.AddUserAsync(organizer);
            //await _notificationService.SendOrganizerWelcomeEmail(organizer);

            return Ok("Organizer registered successfully.");
        }

        // Actor login

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(SHA256.HashData(bytes));
        }

        private static bool VerifyPassword(string rawPassword, string hashedPassword)
        {
            return HashPassword(rawPassword) == hashedPassword;
        }
    }
}



