using System;
using AppApi.Models;
using AppApi.Models.Chat;
using AppApi.Repository;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppApi.Controllers
{
    [Authorize]
	[Route("api/[controller]")]
    [ApiController]
	public class MessageController : ControllerBase
	{
		private IMessageRepository _messageRepository;

		public MessageController(IMessageRepository messageRepository)
		{
			_messageRepository = messageRepository;
		}

        [HttpPost("createroom")]
        public ActionResult<Response> CreateChatRoom([FromBody] RoomRequest room)
        {
            Response response = new Response();
            try
            {
                Response resp = _messageRepository.CreateRoom(room.receiverId, room.senderId);
                if (resp.Status == RequestStatus.Success)
                {
                    return Ok(resp);
                }
                else
                {
                    response.Data = JsonConvert.SerializeObject(false);
                    response.Message = resp.Message;
                    response.Status = RequestStatus.Error;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [HttpGet("getrooms")]
        public ActionResult<Response> GetRooms(int userId)
        {
            Response response = new Response();
            try
            {
                List<Room> resp = _messageRepository.GetUserRooms(userId);
                response.Data = JsonConvert.SerializeObject(resp);
                response.Status = RequestStatus.Success;
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [HttpGet("getmessages")]
        public ActionResult<Response> GetUser(int roomId, int userId)
        {
            Response response = new Response();
            try
            {
                List<Message> resp = _messageRepository.GetRoomMessages(roomId, userId);
                response.Data = JsonConvert.SerializeObject(resp);
                response.Status = RequestStatus.Success;
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

        [HttpPost("insertmessage")]
        public ActionResult<Response> InsertMessage([FromBody] Message message)
        {
            Response response = new Response();
            try
            {
                Response resp = _messageRepository.InsertMessage(message.roomId, message.senderId, message.content, message.date, message.timestamp);
                if (resp.Status == RequestStatus.Success)
                {
                    return Ok(resp);
                }
                else
                {
                    response.Data = JsonConvert.SerializeObject(false);
                    response.Message = resp.Message;
                    response.Status = RequestStatus.Error;
                    return BadRequest(response);
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }

        }

        [HttpGet("getunreadmessages")]
        public ActionResult<Response> GetUserUnereadMessages(int userId)
        {
            Response response = new Response();
            try
            {
                int resp = _messageRepository.GetUserUnreadMessages(userId);
                response.Data = JsonConvert.SerializeObject(resp);
                response.Status = RequestStatus.Success;
                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = RequestStatus.Error;
                return BadRequest(response);
            }
        }

    }
}

