using CarRental.API.Extensions;
using CarRental.Application.Identity.Commands;
using CarRental.Application.Identity.Requests;
using CarRental.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ResetPasswordRequest = CarRental.Application.Identity.Requests.ResetPasswordRequest;

namespace CarRental.API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/identity")] //custom route naming for clarity

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IdentityController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Register a new user account.
        /// </summary>
        /// <remarks>
        /// Email and password are mandatory; first and last names are optional.
        /// After registration, a confirmation email with a 6-digit code is sent to the user.
        /// Confirmation is required before login is available.
        /// 
        /// Password requirements:
        /// - At least 6 characters long
        /// - Must contain an uppercase letter
        /// - Must contain a lowercase letter
        /// - Must contain a digit
        /// - Must contain a non-alphanumeric character
        /// </remarks>
        /// <param name="request">The registration details including email, password, and optional name fields.</param>
        /// <returns>201 Created on success, or a validation error response.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
        {
            var result = await sender.Send(new RegisterNewUserCommand(request));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Confirm a user account using the emailed 6-digit code.
        /// </summary>
        /// <remarks>
        /// The 6-digit code is sent to the user's email address upon registration.
        /// The code expires after 24 hours. Login is only available after confirmation.
        /// </remarks>
        /// <param name="request">The email address and the 6-digit confirmation code.</param>
        /// <returns>200 OK on success, or an error if the code is invalid or expired.</returns>
        [HttpPost("confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmationAsync([FromBody] ConfirmEmailRequest request)
        {
            var result = await sender.Send(new ConfirmEmailCommand(request));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Resend the account confirmation email.
        /// </summary>
        /// <remarks>
        /// Use this endpoint if the original confirmation email was not received or the code has expired.
        /// A new 6-digit code will be issued, valid for 24 hours.
        /// </remarks>
        /// <param name="request">The email address to resend the confirmation code to.</param>
        /// <returns>200 OK on success, or an error if the account does not exist or is already confirmed.</returns>
        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationAsync([FromBody] ResendConfirmationEmailRequest request)
        {
            var result = await sender.Send(new ResendConfirmationCommand(request));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Request a password reset email.
        /// </summary>
        /// <remarks>
        /// Sends an email containing a password reset link to the specified address.
        /// The link includes the user's email and a reset token as query parameters.
        /// Only valid for confirmed accounts.
        /// 
        /// **TODO:** A front-end page URL must be configured as the redirect target embedded in the email link.
        /// </remarks>
        /// <param name="request">The email address associated with the account.</param>
        /// <returns>200 OK regardless of whether the email exists, to prevent user enumeration.</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            var result = await sender.Send(new ForgotPasswordCommand(request));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Reset a user's password using the token received via email.
        /// </summary>
        /// <remarks>
        /// This endpoint is called after the user clicks the reset link received from the <c>forgot-password</c> endpoint.
        /// The request must include the email and token supplied in that link.
        ///
        /// Password requirements:
        /// - At least 6 characters long
        /// - Must contain an uppercase letter
        /// - Must contain a lowercase letter
        /// - Must contain a digit
        /// - Must contain a non-alphanumeric character
        ///
        /// **TODO:** A front-end page must be set up to capture the token from the link and submit it here.
        /// </remarks>
        /// <param name="request">The email, reset token, and new password.</param>
        /// <returns>200 OK on success, or an error if the token is invalid or expired.</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            var result = await sender.Send(new ResetPasswordCommand(request));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Change the password for the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// Requires the user to be logged in. The current password must be provided alongside the new one.
        ///
        /// Password requirements:
        /// - At least 6 characters long
        /// - Must contain an uppercase letter
        /// - Must contain a lowercase letter
        /// - Must contain a digit
        /// - Must contain a non-alphanumeric character
        /// </remarks>
        /// <param name="request">The current password and the desired new password.</param>
        /// <returns>200 OK on success, or an error if the current password is incorrect.</returns>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            var result = await sender.Send(new ChangePasswordCommand(request));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Authenticate a user from the web application.
        /// </summary>
        /// <remarks>
        /// Returns a JWT access token (valid for 60 minutes) and a refresh token (valid for 7 days).
        /// If 2FA is enabled on the account, the response will indicate that a 2FA code is required
        /// to complete the login — use the <c>2fa/login</c> endpoint to finalize authentication.
        /// </remarks>
        /// <param name="request">The user's email and password.</param>
        /// <returns>200 OK with access and refresh tokens, or a 2FA challenge indicator.</returns>
        [HttpPost("login/web")]
        [AllowAnonymous]
        public async Task<IActionResult> WebLoginAsync([FromBody] LoginUserRequest request)
        {
            var result = await sender.Send(new LoginWebCommand(request, ClientType.Web));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Authenticate a user from the mobile application.
        /// </summary>
        /// <remarks>
        /// Returns a JWT access token (valid for 60 minutes) and a refresh token (valid for 30 days).
        /// </remarks>
        /// <param name="request">The user's email and password.</param>
        /// <returns>200 OK with access and refresh tokens on success.</returns>
        [HttpPost("login/mobile")]
        [AllowAnonymous]
        public async Task<IActionResult> MobileLoginAsync([FromBody] LoginUserRequest request)
        {
            var result = await sender.Send(new LoginMobileCommand(request, ClientType.Mobile));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Obtain a new JWT access token using a valid refresh token.
        /// </summary>
        /// <remarks>
        /// The refresh token must not be expired. Token validity by client:
        /// - **Web:** refresh token valid for 7 days
        /// - **Mobile:** refresh token valid for 30 days
        /// </remarks>
        /// <param name="request">The expired (or near-expiry) access token and a valid refresh token.</param>
        /// <returns>200 OK with a new access token and refresh token on success.</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request)
        {
            var result = await sender.Send(new RefreshTokenCommand(request));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Begin the 2FA setup process for the authenticated user.
        /// </summary>
        /// <remarks>
        /// Sends an email containing a QR code (as a base64 image) that the user can scan
        /// with any TOTP-compatible authenticator app (e.g. Google Authenticator, Authy).
        /// After scanning, the setup must be confirmed via the <c>2fa/confirm</c> endpoint.
        /// </remarks>
        /// <returns>200 OK with the QR code payload, or an error if 2FA is already enabled.</returns>
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> Enable2FAAsync()
        {
            var result = await sender.Send(new Enable2FACommand());
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Confirm and activate 2FA after scanning the QR code.
        /// </summary>
        /// <remarks>
        /// After scanning the QR code provided by <c>2fa/enable</c>, submit a valid TOTP code
        /// from the authenticator app to complete the setup. 2FA will only be active after this step.
        /// One-time recovery codes will be returned in the response — store them securely.
        /// </remarks>
        /// <param name="request">A valid TOTP code from the authenticator app.</param>
        /// <returns>200 OK with recovery codes on success, or an error if the code is invalid.</returns>
        [HttpPost("2fa/confirm")]
        public async Task<IActionResult> Confirm2FAAsync([FromBody] Confirm2FARequest request)
        {
            var result = await sender.Send(new Confirm2FACommand(request.Code));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Disable 2FA for the authenticated user.
        /// </summary>
        /// <remarks>
        /// Requires a valid TOTP code from the authenticator app to confirm the action.
        /// If access to the authenticator app has been lost, use <c>2fa/disable/recovery</c> instead.
        /// </remarks>
        /// <param name="request">A valid TOTP code from the authenticator app.</param>
        /// <returns>200 OK on success, or an error if the code is invalid.</returns>
        [HttpPost("2fa/disable")]
        public async Task<IActionResult> Disable2FAAsync([FromBody] Confirm2FARequest request)
        {
            var result = await sender.Send(new Disable2FACommand(request.Code));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Complete web login for accounts with 2FA enabled.
        /// </summary>
        /// <remarks>
        /// After a successful email/password login that returns a 2FA challenge, submit the
        /// current TOTP code from the authenticator app here to receive the access and refresh tokens.
        /// </remarks>
        /// <param name="request">The pending login session identifier and the TOTP code.</param>
        /// <returns>200 OK with access and refresh tokens on success.</returns>
        [HttpPost("2fa/login")]
        [AllowAnonymous]
        public async Task<IActionResult> WebLogin2FAAsync([FromBody] Login2FARequest request)
        {
            var result = await sender.Send(new LoginWeb2FACommand(request, ClientType.Web));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Check whether 2FA is currently enabled for the authenticated user.
        /// </summary>
        /// <returns>200 OK with a boolean indicating the current 2FA status.</returns>
        [HttpPost("2fa/status")]
        public async Task<IActionResult> Check2FAStatusAsync()
        {
            var result = await sender.Send(new Check2FAStatusCommand());
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Complete web login using a one-time 2FA recovery code.
        /// </summary>
        /// <remarks>
        /// Use this endpoint if access to the authenticator app has been lost.
        /// Recovery codes are single-use and were provided when 2FA was first confirmed.
        /// </remarks>
        /// <param name="request">The user's email and one of their unused recovery codes.</param>
        /// <returns>200 OK with access and refresh tokens on success, or an error if the code is invalid or already used.</returns>
        [HttpPost("2fa/login/recovery")]
        [AllowAnonymous]
        public async Task<IActionResult> WebLogin2FARecoveryAsync([FromBody] Login2FARecoveryRequest request)
        {
            var result = await sender.Send(new LoginWeb2FARecoveryCommand(request, ClientType.Web));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Disable 2FA using a one-time recovery code.
        /// </summary>
        /// <remarks>
        /// Use this endpoint when the authenticator app is unavailable and 2FA needs to be removed.
        /// Recovery codes are single-use and were issued when 2FA was first confirmed.
        /// </remarks>
        /// <param name="request">The user's email and one of their unused recovery codes.</param>
        /// <returns>200 OK on success, or an error if the recovery code is invalid or already used.</returns>
        [HttpPost("2fa/disable/recovery")]
        [AllowAnonymous]
        public async Task<IActionResult> Disable2FAWithRecoveryCodeAsync([FromBody] Disable2FARecoveryRequest request)
        {
            var result = await sender.Send(new Disable2FAWithRecoveryCodeCommand(request.Email, request.RecoveryCode));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Log out the authenticated user from the web application.
        /// </summary>
        /// <remarks>
        /// Invalidates the current web session's refresh token server-side.
        /// The client application should also close any active SignalR hub connections after calling this endpoint
        /// (via <c>HubConnection.StopAsync()</c>).
        /// </remarks>
        /// <returns>200 OK on success.</returns>
        [HttpPost("logout/web")]
        public async Task<IActionResult> WebLogoutAsync()
        {
            var result = await sender.Send(new LogoutCommand(ClientType.Web));
            return result.ToActionResult(this);
        }

        /// <summary>
        /// Log out the authenticated user from the mobile application.
        /// </summary>
        /// <remarks>
        /// Invalidates the current mobile session's refresh token server-side.
        /// </remarks>
        /// <returns>200 OK on success.</returns>
        [HttpPost("logout/mobile")]
        public async Task<IActionResult> MobileLogoutAsync()
        {
            var result = await sender.Send(new LogoutCommand(ClientType.Mobile));
            return result.ToActionResult(this);
        }
    }
}