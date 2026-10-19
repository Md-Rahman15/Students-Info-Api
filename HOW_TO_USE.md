# Students Information API — Usage Guide

ASP.NET Core Web API for authentication, student profiles, and courses.

## Deployed API

**Base URL**

```text
https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net
```

Replace `{id}`, `{getid}`, or `{deleteid}` in a route with the relevant student or course GUID. The deployed URLs below use the base URL above.

## Authentication

Protected endpoints require a JWT bearer token. After logging in, copy the token returned by the API and send it with protected requests using this header:

```http
Authorization: Bearer <your-token>
```

In Swagger UI, use **Authorize** and enter the token in the format requested by the UI. Do not share JWTs, passwords, SMTP credentials, or signing keys.

> **Access note:** Routes marked **Student-facing** are the student routes identified in the supplied endpoint list. Publicly reachable URLs can still enforce authorization or service-level checks. The supplied AuthController has no controller-level `[Authorize]` attributes, but a service may perform additional checks.

## Deployed endpoints

### Authentication

These routes are marked **Student-facing** in the supplied endpoint list.

#### Register

- **Method and path:** `POST /api/Auth/Register`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Register>
- **Body:** `User_Register_Request_DTO` as JSON.

```json
{
  "userName": "Ayaan",
  "email": "ayaan@example.com",
  "password": "Strong@123"
}
```

#### Login

- **Method and path:** `POST /api/Auth/Login`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Login>
- **Body:** `User_Login_Request_DTO` as JSON.
- **Successful response:** Includes a JWT token.

```json
{
  "email": "ayaan@example.com",
  "password": "Strong@123"
}
```

#### Change password

- **Method and path:** `PATCH /api/Auth/Password%20Change`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Password%20Change>
- **Body:** `Change_Password_Dto` as JSON.

```json
{
  "currentPassword": "OldPass@123",
  "newPassword": "NewPass@123",
  "confirmNewPassword": "NewPass@123"
}
```

#### Request a password reset

- **Method and path:** `POST /api/Auth/Forgot%20Password`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Forgot%20Password>
- **Body:** `RequestPasswordResetDto` as JSON.

```json
{
  "email": "ayaan@example.com"
}
```

The response should be generic so it does not reveal whether an account exists.

#### Reset password using an email token

- **Method and path:** `POST /api/Auth/Email%20Based%20Password%20Reset`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Email%20Based%20Password%20Reset>
- **Body:** `EmailBasedResetPasswordDto` as JSON.

```json
{
  "token": "TOKEN_FROM_RESET_EMAIL",
  "password": "NewPass@123",
  "confirmNewPassword": "NewPass@123"
}
```

#### Request an account email change

- **Method and path:** `POST /api/Auth/UserAccountEmailChange`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/UserAccountEmailChange>
- **Body:** JSON matching `NewEmailInfo`.

#### Reset an email using an email token

- **Method and path:** `POST /api/Auth/EmailBasedEmailReset`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/EmailBasedEmailReset>
- **Body:** A JSON string containing the token, for example:

```json
"token-value"
```

#### Verify an email token

- **Method and path:** `POST /api/Auth/EmailTokenVerify`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/EmailTokenVerify>
- **Body:** A JSON string containing the token, for example:

```json
"token-value"
```

### Courses

#### List courses

- **Method and path:** `GET /api/Courses`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses>
- **Access:** Student or Admin JWT. **Student-facing.**

#### Get a course by ID

- **Method and path:** `GET /api/Courses/{getid}`
- **Full URL pattern:** `https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/{getid}`
- **Access:** Student or Admin JWT.

#### Create a course

- **Method and path:** `POST /api/Courses`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses>
- **Access:** Admin JWT.
- **Body:** `Request_Course_Dto` as JSON.

```json
{
  "name": "MCA",
  "description": "Master of Computer Applications course",
  "duration": "2 years"
}
```

#### Update a course

- **Method and path:** `PATCH /api/Courses/{id}`
- **Full URL pattern:** `https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/{id}`
- **Access:** Admin JWT.
- **Body:** `Request_Course_Dto` as JSON.

#### Delete a course

- **Method and path:** `DELETE /api/Courses/{id}`
- **Full URL pattern:** `https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/{id}`
- **Access:** Admin JWT.

### Students

#### Create a student

- **Method and path:** `POST /api/Student`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student>
- **Access:** Student or Admin JWT. **Student-facing.**
- **Content type:** `multipart/form-data`.
- **Body:** Fields matching `RequestStudentDto`. Upload `Image` as a file.

Do not send student creation as JSON. In Postman, choose **form-data** and set `Image` to **File**. Let Postman generate the `Content-Type` boundary automatically.

| Field | Required | Rules or example |
|---|---|---|
| `Image` | Yes | Select an image file. |
| `FirstName` | Yes | `Ayaan` |
| `LastName` | Yes | `Khan` |
| `Email` | Yes | Valid email, maximum 100 characters; e.g. `ayaan@example.com`. |
| `PhoneNumber` | Yes | 10-digit Indian mobile number beginning with 6–9; e.g. `9876543210`. |
| `DateofBirth` | Yes | `2005-04-12` (`YYYY-MM-DD`); minimum age 16. |
| `Gender` | Yes | `Male`, `Female`, or `Others`. |
| `EnrollmentDate` | Yes | `2026-08-01` (`YYYY-MM-DD`); must not be before date of birth. |
| `CourseName` | Yes | Must match an existing course, e.g. `MCA`. |

Example values: `Ayaan`, `Khan`, `ayaan@example.com`, `9876543210`, `2005-04-12`, `Male`, `2026-08-01`, and `MCA`. Select an actual image file for `Image` and use a course that already exists.

Student email addresses and phone numbers must be unique.

#### Get the current student's profile

- **Method and path:** `GET /api/Student/me`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/me>
- **Access:** Student JWT. **Student-facing.**

#### Update the current student's profile

- **Method and path:** `PATCH /api/Student/UpdateMyProfile`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/UpdateMyProfile>
- **Access:** Student JWT. **Student-facing.**
- **Content type:** `multipart/form-data`.
- **Body:** Fields matching `UpdateRequestStudentDto`. The `Image` field is optional; leave it empty if the image is not changing.

#### List students

- **Method and path:** `GET /api/Student`
- **Full URL:** <https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student>
- **Access:** Admin JWT.
- **Optional query parameters:** `search`, `SortBy`, `SortAscending`, `Gender`, `PageNumber`, `PageSize`, `byFirstName`.

#### Get a student by ID

- **Method and path:** `GET /api/Student/{getid}`
- **Full URL pattern:** `https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/{getid}`
- **Access:** Admin JWT.

#### Update a student by ID

- **Method and path:** `PATCH /api/Student/{id}`
- **Full URL pattern:** `https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/{id}`
- **Access:** Admin JWT.
- **Content type:** `multipart/form-data`.
- **Body:** Fields matching `UpdateRequestStudentDto`. Send `Image` as a file if required.

#### Delete a student

- **Method and path:** `DELETE /api/Student/{deleteid}`
- **Full URL pattern:** `https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/{deleteid}`
- **Access:** Admin JWT.

## Validation rules

### Authentication

- `UserName` is required.
- Email is required and must be valid.
- Passwords must be at least 8 characters and include an uppercase letter, a lowercase letter, and a special character.
- For password changes and resets, the confirmation field must match the new password. Ensure this is enforced by service validation.
- `RequestPasswordResetDto.Email` allows up to 320 characters.

### Students

- All student creation fields are required, including `Image`; profile updates may omit `Image`.
- Email must be valid and at most 100 characters.
- Phone number must match `^[6-9]\d{9}$`.
- Gender must be `Male`, `Female`, or `Others` (case-insensitive).
- The student must be at least 16 years old.
- Enrollment date must not be before date of birth.
- Course name must match an existing course.

### Courses

| Field | Rule | Example |
|---|---|---|
| `Name` | 3–20 characters | `MCA` |
| `Description` | 10–250 characters | `Master of Computer Applications course` |
| `Duration` | 3–25 characters | `2 years` |

## Request formatting notes

- Use JSON for authentication and course requests unless the endpoint description says otherwise.
- Use `multipart/form-data` for student create and update requests.
- For multipart requests, let Postman set the `Content-Type` boundary automatically.
- Replace all GUID placeholders with actual IDs from your API responses.
- The supplied DTO validation message says “Enrollment date must before 2025,” while the described implementation checks the enrollment date against the date of birth. Consider correcting that message in the API.
- Never commit real passwords, JWT signing keys, or SMTP credentials to source control.
