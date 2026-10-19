using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.WebEncoders;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.AuthData.DTO.Others;
using Students_Info_Api.AuthData.DTO.UserRequestDtoFolder;
using Students_Info_Api.Repository.Interfaces;
using Students_Info_Api.Service.Interfaces;
using Students_Info_Api.Service.UserServiceFolder.UserResultClass;


namespace Students_Info_Api.Service.UserService
{
    public class UserService:IUserService
    {
        private readonly IUserRepo userRepo;
        private readonly IPasswordHasher<User_DM> passwordHasher;
        private readonly IJwtTokenService jwtTokenService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IEmailService emailService;
        private readonly ILogger<UserService> logger;

        public UserService(IUserRepo userRepo,IPasswordHasher<User_DM> passwordHasher,IJwtTokenService jwtTokenService,IHttpContextAccessor httpContextAccessor,IEmailService emailService,ILogger<UserService> logger)
        {
            this.userRepo = userRepo;
            this.passwordHasher = passwordHasher;
            this.jwtTokenService = jwtTokenService;
            this.httpContextAccessor = httpContextAccessor;
            this.emailService = emailService;
            this.logger = logger;
        }

        public UserResultClass<T> Helper<T>(T? data, bool resBool, string? msg,string token,ErrorType errorType)
        {
                if (!resBool) 
                {
                UserResultClass<T> resultFailure = new UserResultClass<T>
                {
                    Success = false,
                    Data = default,
                    Token = null,
                    Message = msg,
                    errorType = errorType
                };
                return resultFailure;

                }

                UserResultClass<T> resultSuccess = new UserResultClass<T>
                {
                    Success = true,
                    Data = data,
                    Token = token,
                    Message = msg,
                    errorType = errorType

                };
                return resultSuccess;            
        }

        public async Task<UserResultClass<User_DM>> LoginUserService(User_DM user, User_Login_Request_DTO logindto)
        {
            //calling login repo method
            var logincheck = await userRepo.GetUserEmailRepo(user.Email);

            //if email not matched then deny access
            if (logincheck == null)
            {
                return Helper(logincheck, false,"Invalid id or password", null,ErrorType.Unauthorized);
            }

            //checking password
            var verifyhashpassword = passwordHasher.VerifyHashedPassword(user, logincheck.PasswordHash, logindto.Password);

            //if password is incorrent return deny access
            if (verifyhashpassword == PasswordVerificationResult.Failed) 
            {
                return Helper(logincheck, false, "Invalid id or password",null,ErrorType.Unauthorized);
            }                     
            //valdation of email verifed or not
            if (logincheck.IsEmailVerified == false)
            {
                return Helper(logincheck, false, "Email is not verified please verify email address and try again", null,ErrorType.Unauthorized);
            }            

            var token = jwtTokenService.GenerateJwtToken(logincheck);

            if (token == null) 
            {
                return Helper(logincheck, false, "Token cannot be generated please try again later", null, ErrorType.Unauthorized);
            }


            //if both validated then return success with data 
            return Helper(logincheck, true, null,token,ErrorType.None);
        
        }
        public async Task<UserResultClass<User_DM>> UserRegisterService(User_DM user , User_Register_Request_DTO userdto)
        {
            //email check 
             var emailValidation = await userRepo.GetUserEmailRepo(user.Email);

            if (emailValidation != null)
            {
                if (emailValidation.IsEmailVerified == true)
                {
                    return Helper(emailValidation, false, "Email Already Exists", null,ErrorType.Conflict);
                }
                else 
                {   
                    //if email is not verified and password matched then resend token
                    var resendVerificationToken = await ResendVerificationTokenUserRegister(emailValidation, userdto);

                    //return the failure if any
                    if (!resendVerificationToken.Success) 
                    {
                        return Helper<User_DM>(null, false, resendVerificationToken.Message, null,ErrorType.ValidationError);
                    }

                    //success
                    return Helper<User_DM>(resendVerificationToken.Data, true, resendVerificationToken.Message,null,ErrorType.None);


                }
               
            }

            //we call method that create a new user who's does not exits in database a new entry
            var createNewUser = await CreateNewUserRegister(user, userdto);

            //return failure if any
            if (!createNewUser.Success) 
            {
                return Helper<User_DM>(null,false,createNewUser.Message,null,createNewUser.errorType);
            }
            //success
            return Helper<User_DM>(createNewUser.Data,true, createNewUser.Message,null,ErrorType.None);

        }
        public async Task<UserResultClass<string>> UserEmailVerifactionService(string token) 
        {
            if (token == null) 
            {
                return Helper<string>(null, false, "Invalid token", null,ErrorType.Unauthorized);
            }

            //this method verifies if token is valid or not
            var tokenVerify = await TokenVerifier(token);

            //if not valid then return the token error message
            if (!tokenVerify.Success) 
            {
                return Helper<string>(null, false, tokenVerify.Message, null,ErrorType.ValidationError);
            }

            //we verified token and get user and set email verified = true

            var getUser = await userRepo.GetUserById(tokenVerify.Data.UserId);

            if (getUser == null) 
            {
                return Helper<string>(null, false,"User not found", null,ErrorType.Unauthorized);
            }

            //call method that mark the email verification = true
            var emailmarked = await userRepo.UpdateEmailVerifcationCheck(getUser.UserId);

            if (!emailmarked)
            {
                return Helper<string>(null, false, "User not found", null, ErrorType.Unauthorized);
            }

            //mark the tokenusage = true means token is not reusable
           var tokenUsageTrue = await userRepo.TokenUsageToTrue(tokenVerify.Data.TokenHash);

            if (!tokenUsageTrue) 
            {
                return Helper<string>(null, false, "Token Not Marked", null, ErrorType.InternalError);
            }

            return Helper<string>(null,true, "User has been verified now can login into your account", null, ErrorType.None);


        }
        public async Task<UserResultClass<User_DM>> PasswordChangeService(Change_Password_Dto change_Password)
        {
            //calling method
            var GetGuid = GetUserGuidFromJwtClaim();

            //failure check
            if (!GetGuid.Success)
            {
                return Helper<User_DM>(null, false, GetGuid.Message, null,GetGuid.errorType);
            }

            var userInfo = await userRepo.GetUserById(GetGuid.Data);

            if (userInfo == null)
            {
                return Helper<User_DM>(default, false, "User Not Found", null, ErrorType.Unauthorized);
            }

            //verify the current password against stored password hash

         
            
            if ((passwordHasher.VerifyHashedPassword(userInfo, userInfo.PasswordHash,change_Password.CurrentPassword) == PasswordVerificationResult.Failed)) 
            {
                return Helper<User_DM>(default, false, "Wrong Password", null, ErrorType.Unauthorized);
            }

            //User current password and hash Password matched

            if (change_Password.NewPassword != change_Password.ConfirmNewPassword) 
            {
                return Helper<User_DM>(default, false, "Passwords do not match", null, ErrorType.ValidationError);
            }

            //user both password are valid

            //now we will hash new password
            var hashedNewPassowrd = passwordHasher.HashPassword(userInfo, change_Password.ConfirmNewPassword);

            //pass new hash password to repo

            await userRepo.UpdateUserNewHashedPassword(hashedNewPassowrd, userInfo.UserId);
       
            return Helper<User_DM>(null, true, "New password changes saved", null, ErrorType.None);
        
        }
        public async Task<UserResultClass<string>> ForgotPasswordService(string email)
        {
            if (email == null) 
            {   
                return Helper<string>(null, false, "Invalid Email", null, ErrorType.Unauthorized);
            }

            var user =  await userRepo.GetUserEmailRepo(email);           

            if (user == null)
            {
                return Helper<string>(null, true, "If an account exists for this email, a password reset link has been sent", null, ErrorType.None);
            }

            //this method creates and returns a hash token and raw token 
            var tokenCreation = TokenCreator(user.UserId, email);

            if (!tokenCreation.Success) 
            {
                return Helper<string>(null, false, tokenCreation.Message, null,tokenCreation.errorType);
            }

            var message = "If an account exists for this email, a password reset link has been sent.";

            //saves hashed token in database and hashed token comes from tokencreatoremethod
            var savedToken = await userRepo.SavedHashedTokenDBRepo(tokenCreation.Data);

            if (savedToken == null)
            {
             
                return Helper<string>(null, false, "Could Not Saved taken in db", null, ErrorType.InternalError);
            }

            //we send email to target email address by using user (to know user name and etc data),rawtoken of tokencreator method , target email address
            var emailsendresult = await emailService.SendTokenLink(user,tokenCreation.Token, user.Email);

            if (emailsendresult == false)
            {
                return Helper<string>(null, false, "STMP COULD NOT WORK", null, ErrorType.InternalError);
            }

            return Helper<string>(null, true, message, null, ErrorType.None);

        }
        public async Task<UserResultClass<User_DM>> EmailBasedPasswordResetService(EmailBasedResetPasswordDto input)
        {
            //if token is null or invalid
            if (input.Token == null) 
            {
                return Helper<User_DM>(default, false, "Invalid Token", null, ErrorType.Unauthorized);
            }

            //this method verifies if token is valid or not
            var tokenVerification = await TokenVerifier(input.Token);

            //if not valid then return the token error message
            if (tokenVerification.Success == false)
            {
                return Helper<User_DM>(null, false, tokenVerification.Message, null, ErrorType.ValidationError);
            }

            //get user from repo method
            var getUser = await userRepo.GetUserById(tokenVerification.Data.UserId);

            //check if user exits
            if (getUser == null) 
            {
                return Helper<User_DM>(default, false, "User Does Not Exists", null, ErrorType.Unauthorized);
            }

            //if yes then check both password are same or not
            if (input.Password != input.ConfirmNewPassword) 
            {
                return Helper<User_DM>(default, false, "Passwords not match", null, ErrorType.ValidationError);
            }

            //if matched then hased the password
            var updatedHashedPassword = passwordHasher.HashPassword(getUser, input.ConfirmNewPassword);

            //update the user password in the repo
            var UpdateUserPasswordRepo = await userRepo.UpdateUserNewHashedPassword(updatedHashedPassword,getUser.UserId);
            
            //null check
            if (UpdateUserPasswordRepo == null) 
            {
                return Helper<User_DM>(default, false, "Password cannot be changed now try again later", null, ErrorType.InternalError);
            }

            //token should now marked as used so isused = true in repo method of passwordresettoken_dm
            var TokenUsage = await userRepo.TokenUsageToTrue(tokenVerification.Data.TokenHash);

            if (!TokenUsage) 
            {
                return Helper<User_DM>(default, true, "Password has been updated.But there is a internal token error", null, ErrorType.InternalError);
            }

            return Helper<User_DM>(default, true, "Password has successfully updated", null, ErrorType.None);
        }
        public async Task<UserResultClass<string>> UserAccountEmailChangeService(NewEmailInfo input)
        {
            //calling method
            var GetGuid = GetUserGuidFromJwtClaim();

            //failure check
            if (!GetGuid.Success) 
            {
                return Helper<string>(null, false, GetGuid.Message, null, ErrorType.InternalError);
            }

            //calling getuserbyid method and passing output of guid conversion
            var user = await userRepo.GetUserById(GetGuid.Data);

            if (user == null) 
            {
                return Helper<string>(null, false, "User Not Found", null, ErrorType.Unauthorized);
            }

            //now lets verify the entered password 
            var passwordVerification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, input.Password);

            if (passwordVerification == PasswordVerificationResult.Failed)
            {
                return Helper<string>(null, false, "Incorrect password", null, ErrorType.Unauthorized);
            }

            //check both email address should be matched
            if (input.EmailAddress != input.ConfirmEmailAddress) 
            {
                return Helper<string>(null, false, "Both email address does not match", null, ErrorType.ValidationError);
            }

            //check the new email does not exist in database 
            var emailcheck = await userRepo.GetUserEmailRepo(input.ConfirmEmailAddress); 

            if (emailcheck != null) 
            {
                return Helper<string>(null, false, "Email address Already Exits. Try different email address", null, ErrorType.None);
            }

            //this method creates and returns a hash token and raw token 
            var tokenCreation = TokenCreator(user.UserId, input.ConfirmEmailAddress);

            if (!tokenCreation.Success) 
            {
                return Helper<string>(null, false,tokenCreation.Message, null, ErrorType.InternalError);
            }

            string message = "If email address exists we will send a link.you can change email address from that link";

            //save the hashedtokeninfo into database and hashed token comes from tokencreatoremethod 
            var tokenDBCall = await userRepo.SavedHashedTokenDBRepo(tokenCreation.Data);

            if (tokenDBCall == null) 
            {
               return Helper<string>(null, false, "Internal error try again later.", null, ErrorType.InternalError);
            }

            //we send email to target email address by using user (to know user name and etc data),rawtoken of tokencreator method , target email address
            var sendTokenToOldEmailAddress =  await emailService.SendTokenLink(user,tokenCreation.Token, input.ConfirmEmailAddress);

            if (!sendTokenToOldEmailAddress) 
            {
                return Helper<string>(null, false, "Cannot send reset link try again some time later.", null, ErrorType.InternalError);
            }

            return Helper<string>(null, true, message, null, ErrorType.None);
        }
        public async Task<UserResultClass<string>> UpdateUserNewEmailAddressService(string token)
        {
            //this method verifies if token is valid or not
            var TokenVerification = await TokenVerifier(token);

            //if not valid then return the token error message
            if (TokenVerification.Success == false) 
            {
                return Helper<string>(null, false, TokenVerification.Message, null, ErrorType.Unauthorized);
            }

            //lets get user from tokendatadm through user id and assign new email
            var getuser = await userRepo.GetUserById(TokenVerification.Data.UserId);

            if (getuser == null) 
            {
                return Helper<string>(null, false, "User not found", null, ErrorType.Unauthorized);
            }

            //change email address of new user to new email

            var emailChange = await userRepo.UserEmailAddressChange(getuser.UserId, TokenVerification.Data.Email);

            if (!emailChange) 
            {
                return Helper<string>(null, false, "Internal Database error please try again sometime later.", null, ErrorType.InternalError);
            }

            //lets mark the token as used
            var tokenUsageToTrue = await userRepo.TokenUsageToTrue(TokenVerification.Data.TokenHash);

            if (!tokenUsageToTrue) 
            {
                return Helper<string>(null, false, "Token Not Marked", null, ErrorType.InternalError);
            }

            return Helper<string>(null, true, "Email address been successfully changed",null, ErrorType.None);

        }
        public UserResultClass<TokenDataGeneration_DM> TokenCreator(Guid userId,string emailaddress) 
        {
            if (emailaddress == null)
            {
                return Helper<TokenDataGeneration_DM>(null, false, "Please enter valid email address",null, ErrorType.Conflict);
            }

            //create empty bytes
            byte[] rawBytes = new byte[32];

            //random fill the empty bytes and we cannot asssing any var becoz its immutalbe obj
            RandomNumberGenerator.Fill(rawBytes);

            //convert the randomgenerate info of number into a string token (we will pass raw to token user)
            string RawToken = WebEncoders.Base64UrlEncode(rawBytes);

            //convert raw token to uft8
            var uftBytes = Encoding.UTF8.GetBytes(RawToken);

            //convert uft data to hashed token 
            var hashedToken = SHA256.HashData(uftBytes);

            //convert hasedtoken into hex string
            string stringHahsedToken = Convert.ToHexString(hashedToken);

            //lets create a new tokendatageneartion_dm obj it have all token info

            var currentTime = DateTime.UtcNow;

            TokenDataGeneration_DM emailResetTokenData = new TokenDataGeneration_DM()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Email = emailaddress,
                IsUsed = false,
                CreatedAt = currentTime,
                ExpiresAt = currentTime.AddHours(1),
                TokenHash = stringHahsedToken

            };

            return Helper<TokenDataGeneration_DM>(emailResetTokenData, true, "Token Class Created", RawToken, ErrorType.None);
       
        
        }
        public async Task<UserResultClass<TokenDataGeneration_DM>> TokenVerifier(string rawToken) 
        {

            if (rawToken == null || rawToken == " " || rawToken == "")
            {
                return Helper<TokenDataGeneration_DM>(default, false, "Incorrect token", null, ErrorType.Unauthorized);
            }

            //convert the raw token to a string hashtoken

            //convert it info uft8
            var uftBytes = Encoding.UTF8.GetBytes(rawToken);

            //convert uft8 to sha256 hashdata
            var hashedToken = SHA256.HashData(uftBytes);

            //convert to hex string
            string stringHashedToken = Convert.ToHexString(hashedToken);

            //call repo method of check if user input token matches to store hasedtoken 
            var TokenVerification = await userRepo.ReturnHashedTokenRepo(stringHashedToken);

            if (TokenVerification == null)
            {
                return Helper<TokenDataGeneration_DM>(default, false, "Incorrect token", null, ErrorType.Unauthorized);
            }

            //lets check the token validity
            if (TokenVerification.IsUsed == true)
            {
                return Helper<TokenDataGeneration_DM>(default, false, "This token has been already used", null, ErrorType.Unauthorized);
            }

            if (TokenVerification.ExpiresAt <= DateTime.UtcNow)
            {
                return Helper<TokenDataGeneration_DM>(default, false, "Token expired", null, ErrorType.None);
            }

            return Helper<TokenDataGeneration_DM>(TokenVerification, true, "Token Verified", null, ErrorType.None);

        }

        public UserResultClass<Guid> GetUserGuidFromJwtClaim() 
        {
            if (httpContextAccessor.HttpContext == null) 
            {
                return Helper<Guid>(default, false, "HttpContext Missing", null, ErrorType.ValidationError);
            }

            //getting user claim info
            var userIdentification = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdentification == null)
            {
                return Helper<Guid>(default, false, "User Not Found. Please login to continue", null, ErrorType.Unauthorized);
            }

            //using the claiminfo and converting it info to user guid userid

            var guidConversion = Guid.TryParse(userIdentification.Value, out Guid userId);

            //if guid conversion fails send error
            if (!guidConversion)
            {
                return Helper<Guid>(default, false, "Guid conversion fails", null, ErrorType.InternalError);
            }

            return Helper<Guid>(userId, true, "Guid conversion success", null, ErrorType.None);

        }

        public async Task<UserResultClass<User_DM>> ResendVerificationTokenUserRegister(User_DM emailValidation, User_Register_Request_DTO userdto) 
        {
            //before resending new token verify password is also correct or not
            var passwordVerify = passwordHasher.VerifyHashedPassword(emailValidation, emailValidation.PasswordHash, userdto.Password);

            //if password does not match the return the error 
            if (passwordVerify == PasswordVerificationResult.Failed)
            {
                return Helper(emailValidation, false, "Incorrect id or password", null, ErrorType.Unauthorized);
            }

            //this method creates and returns a hash token and raw token 
            var tokenResent = TokenCreator(emailValidation.UserId, emailValidation.Email);

            if (!tokenResent.Success)
            {
                return Helper<User_DM>(null, false, tokenResent.Message, null, ErrorType.InternalError);
            }


            //lets save the token after creation
            var reSavedHashedToken = await userRepo.SavedHashedTokenDBRepo(tokenResent.Data);

            if (reSavedHashedToken == null)
            {
                return Helper<User_DM>(null, false, "Could Not Saved taken in db", null, ErrorType.None);
            }



            //resends the email to email addresss once again                    
            var emailResendToUser = await emailService.SendTokenLink(emailValidation, tokenResent.Token, emailValidation.Email);

            if (emailResendToUser == false)
            {
                return Helper<User_DM>(default, false, "Email could not be sent and please try again later", null, ErrorType.InternalError);
            }


            return Helper(emailValidation, true, "A new token has been send to you are email address please verify", null, ErrorType.None);

        }

        public async Task<UserResultClass<User_DM>> CreateNewUserRegister(User_DM user, User_Register_Request_DTO userdto) 
        {

            //hashing password into hashpassword
            user.PasswordHash = passwordHasher.HashPassword(user, userdto.Password);


            //assiging the role of student as genric role
            user.Roles = "Student";

            //mark the isemailverified as false 
            user.IsEmailVerified = false;

            //this method creats user into db            
            var userRegistrationRepoCall = await userRepo.UserRegisterRepo(user);

            //this method creates and returns a hash token and raw token 
            var tokenCreation = TokenCreator(user.UserId, user.Email);

            if (!tokenCreation.Success)
            {
                return Helper<User_DM>(null, false, tokenCreation.Message, null, ErrorType.InternalError);
            }

            //lets save the token after creation
            var savedHashedToken = await userRepo.SavedHashedTokenDBRepo(tokenCreation.Data);

            if (savedHashedToken == null)
            {
                return Helper<User_DM>(null, false, "Could Not Saved taken in db", null, ErrorType.InternalError);
            }

            //we send email to target email address by using user (to know user name and etc data),rawtoken of tokencreator method , target email address
            var emailSendToUser = await emailService.SendTokenLink(userRegistrationRepoCall, tokenCreation.Token, userRegistrationRepoCall.Email);

            if (emailSendToUser == false)
            {
                return Helper<User_DM>(null, false, "Email could not be sent and link has been send your email address please verify it", null,ErrorType.InternalError);
            }

            return Helper(user, true, "User Created Successfully and token has sent to email address please verify before login", null, ErrorType.None);


        }




    }
}
