using System;
using AppApi.Models;
using AppApi.Models.Chat;

namespace AppApi.Repository.Contract
{
	public interface IMessageRepository
	{
		public List<Room> GetUserRooms(string username);
		public List<Message> GetRoomMessages(int roomId, string username);
		public Response InsertMessage(int roomId, string senderId, string content, string date, string timestamp);
		public Response CreateRoom(int postId, int senderId);
	}
}

