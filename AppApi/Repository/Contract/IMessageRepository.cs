using System;
using AppApi.Models;
using AppApi.Models.Chat;

namespace AppApi.Repository.Contract
{
	public interface IMessageRepository
	{
		public List<Room> GetUserRooms(int userId);
		public List<Message> GetRoomMessages(int roomId, int userId);
		public Response InsertMessage(int roomId, string senderId, string content, string date, string timestamp);
		public Response CreateRoom(int postId, int senderId);
		public int GetUserUnreadMessages(int userId);
	}
}

