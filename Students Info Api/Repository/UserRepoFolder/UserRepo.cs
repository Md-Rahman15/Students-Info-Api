using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Student_Info_Api.Data;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.AuthData.DTO;
using Students_Info_Api.Repository.Interfaces;

namespace Students_Info_Api.Repository.UserRepoFolder
{
    public class UserRepo:IUserRepo
    {
        private readonly ApplicationDbContext applicationDbContext;

        public UserRepo(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<User_DM> GetUserEmailRepo(string email)
        {
           var search = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

           if (search == null) 
           {
                return null;
           }

            return search;

        }      
        public async Task<User_DM> UserRegisterRepo(User_DM user)
        {
          
           var result =  await applicationDbContext.Users.AddAsync(user);

           await applicationDbContext.SaveChangesAsync();

           return user;

        }
        public async Task<User_DM> GetUserById(Guid id) 
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.UserId == id);

            if (user == null) 
            {
                return null;
            }
        
            return user;        
        
        }
        public async Task<User_DM> UpdateUserNewHashedPassword(string newHashPassowrd, Guid id) 
        {
            
            var VerifedUser = await GetUserById(id);

            if (VerifedUser == null)
            {
                return null;
            }
            
            VerifedUser.PasswordHash = newHashPassowrd;

            await applicationDbContext.SaveChangesAsync();

            return VerifedUser;
        
        }
        public async Task<TokenDataGeneration_DM> SavedHashedTokenDBRepo(TokenDataGeneration_DM data)
        {
            if (data == null) 
            {
                return null;
            }

            await applicationDbContext.PasswordResetToken.AddAsync(data);

            await applicationDbContext.SaveChangesAsync();

            return data;

        }
        public async Task<TokenDataGeneration_DM> ReturnHashedTokenRepo(string HashedTokenString) 
        {
            var search = await applicationDbContext.PasswordResetToken.FirstOrDefaultAsync(x => x.TokenHash == HashedTokenString);

            if (search == null) 
            {
                return null;
            }

            return search;

        }
        public async Task<bool> TokenUsageToTrue(string HashedTokenString)
        {

            //gets token verification
            var search = await applicationDbContext.PasswordResetToken.FirstOrDefaultAsync(x => x.TokenHash == HashedTokenString);

            if (search == null)
            {
                return false;
            }

            if (search.IsUsed == true) 
            {
                return false;
            }
        
            search.IsUsed = true;

            await applicationDbContext.SaveChangesAsync();

            return true;
                    
        }
        public async Task<bool> UserEmailAddressChange(Guid UserId,string emailaddress)
        {        
            var search = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.UserId == UserId);

            if (search == null) 
            {
                return false;
            }

            search.Email = emailaddress;

            await applicationDbContext.SaveChangesAsync();
     
            return true;                    
        
        }
        public async Task<bool> UpdateEmailVerifcationCheck(Guid userId)
        {
            var search = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            if (search == null) 
            {
                return false;
            }
        
            search.IsEmailVerified = true;

            await applicationDbContext.SaveChangesAsync();

            return true;        
        }
    }
}
