using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Models;

namespace Api.Controllers
{
    [Route("api/tracking")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrackingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> TrackEvent([FromBody] AddTrackingEventRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var user_id))
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            var command = new AddTrackingEventCommand 
            { 
                user_id = user_id, 
                event_type = request.event_type, 
                book_id = request.book_id 
            };
            
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { message = "Tracking event đã được ghi nhận." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không mong muốn.", error = ex.Message });
            }
        }
    }
}
