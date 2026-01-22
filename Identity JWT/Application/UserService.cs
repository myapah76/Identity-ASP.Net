using Application.Abstrations;
using Application.AppExceptions;
using Application.Constants;
using Application.Dtos.User.Request;
using Application.Dtos.User.Respone;
using Application.Persistences.Repositories;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //CUSTOMER CREATE
        public async Task<UserRep> CustomerCreateAsync(CustomerCreateUserReq request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ConflictDuplicateException(Message.UserMessage.ExistingUser);

            var user = _mapper.Map<User>(request);
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.RoleId ??= 2; // Set role User
            user.CreatedAt = DateTimeOffset.UtcNow;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            user.Is_blocked ??= false;

            await _userRepository.AddWithRoleAsync(user);

            return _mapper.Map<UserRep>(user);
        }

        //ADMIN CREATE
        public async Task<UserRep> AdminCreateAsync(CreateUserReq request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ConflictDuplicateException(Message.UserMessage.ExistingUser);

            var user = _mapper.Map<User>(request);
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.CreatedAt = DateTimeOffset.UtcNow;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            user.Is_blocked ??= false;
            await _userRepository.AddWithRoleAsync(user);

            return _mapper.Map<UserRep>(user);
        }

        //UPDATE
        public async Task<UserRep?> UpdateAsync(long id, UpdateUserReq request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new ConflictDuplicateException(Message.UserMessage.ExistingUser);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            _mapper.Map(request, user);
            user.UpdatedAt = DateTimeOffset.UtcNow;

            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserRep>(user);
        }

        //DELETE
        public async Task<bool> DeleteAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user.Id);
            return true;
        }

        //GET ALL
        public async Task<IEnumerable<UserRep>> GetAllAsync()
        {
            var users = await _userRepository.GetAllWithRoleAsync();

            return _mapper.Map<IEnumerable<UserRep>>(users);
        }

        //GET BY ID
        public async Task<UserRep?> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            return user == null ? null : _mapper.Map<UserRep>(user);
        }
    }
}