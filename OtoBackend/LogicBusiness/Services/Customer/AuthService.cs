using CoreEntities.Models.DTOs;
using CoreEntities.Models;
using LogicBusiness.Interfaces.Customer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using LogicBusiness.Interfaces.Repositories;

namespace LogicBusiness.Services.Customer
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.UserExistsAsync(request.Username, request.Email))
            {
                return new AuthResponse { Success = false, Message = "Username hoặc Email đã tồn tại!" };
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                Role = "Customer", // Mặc định
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            await _userRepository.AddUserAsync(user);

            return new AuthResponse { Success = true, Message = "Đăng ký thành công!" };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse { Success = false, Message = "Sai tài khoản hoặc mật khẩu!" };
            }

            if (user.Status != "Active" || user.IsDeleted)
            {
                return new AuthResponse { Success = false, Message = "Tài khoản đã bị khóa hoặc xóa!" };
            }

            var token = GenerateJwtToken(user);
            return new AuthResponse { Success = true, Message = "Đăng nhập thành công!", Token = token };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            // 1. Khởi tạo danh sách các thẻ cơ bản
            var claims = new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // 2. 👇 BÍ KÍP Ở ĐÂY: Nếu là Nhân viên/Quản lý có ShowroomId, nhét luôn vào Token!
            if (user.ShowroomId.HasValue)
            {
                claims.Add(new Claim("ShowroomId", user.ShowroomId.Value.ToString()));
            }

            // 3. Tiến hành đúc Token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(jwtSettings["ExpireDays"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
