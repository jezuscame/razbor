using AutoMapper;
using MediatR;
using Razbor.DAL;
using Services.HashingService;
using Services.TokenService;
using RequestBodies.UserInformation;
using System.Net;
using API.Features.Extensions;

namespace API.Features.Authentication
{
    public class UserUpdateInfoHandler : IRequest<HttpStatusCode>
    {
        public UserInfoUpdateRequest Data { get; init; } = null!;
    }

    // Command Handler
    public class InfoUpdateHandler : IRequestHandler<UserUpdateInfoHandler, HttpStatusCode>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InfoUpdateHandler(
            DatabaseContext databaseContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _databaseContext = databaseContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpStatusCode> Handle(UserUpdateInfoHandler request, CancellationToken cancellationToken)
        {
            string userId = _httpContextAccessor.HttpContext!.GetUserId();
            var userToUpdate = _databaseContext.UserInfoTable.FirstOrDefault(user => user.Username == userId);

            if (userToUpdate == null)
            {
                throw new HttpRequestException("User not found", new(), System.Net.HttpStatusCode.NotFound);
            }

            userToUpdate.FirstName = request.Data.FirstName ?? userToUpdate.FirstName;
            userToUpdate.LastName = request.Data.LastName ?? userToUpdate.LastName;
            userToUpdate.Birthday = request.Data.Birthday ?? userToUpdate.Birthday;
            userToUpdate.PrimaryPicture = request.Data.PrimaryPicture ?? userToUpdate.PrimaryPicture;
            userToUpdate.SelectedSong = request.Data.SelectedSong ?? userToUpdate.SelectedSong;
            userToUpdate.Email = request.Data.Email ?? userToUpdate.Email;
            userToUpdate.Height = request.Data.Height ?? userToUpdate.Height;
            userToUpdate.Weight = request.Data.Weight ?? userToUpdate.Weight;
            userToUpdate.Description = request.Data.Description ?? userToUpdate.Description;

            _databaseContext.UserInfoTable.Update(userToUpdate);
            await _databaseContext.SaveChangesAsync();

            return HttpStatusCode.OK;
        }
    }
}

