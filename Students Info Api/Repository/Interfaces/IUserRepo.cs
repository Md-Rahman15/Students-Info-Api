using Students_Info_Api.AuthData.DM;
using Students_Info_Api.AuthData.DTO;

namespace Students_Info_Api.Repository.Interfaces
{
    public interface IUserRepo
    {
        public Task<User_DM> GetUserEmailRepo(string email);
        public Task<User_DM> UserRegisterRepo(User_DM user);
        public Task<User_DM> GetUserById(Guid id);
        public Task<User_DM> UpdateUserNewHashedPassword(string newHashPassowrd, Guid id);
        public Task<TokenDataGeneration_DM> SavedHashedTokenDBRepo(TokenDataGeneration_DM data);
        public Task<TokenDataGeneration_DM> ReturnHashedTokenRepo(string HashedTokenString);
        public Task<bool> TokenUsageToTrue(string HashedTokenString);
        public Task<bool> UserEmailAddressChange(Guid userId,string emailaddress);
        public Task<bool> UpdateEmailVerifcationCheck(Guid userId);

    }
}
