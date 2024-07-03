using AutoMapper;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Extensions;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;
public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository,
    IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessage)
    {
        var username = User.GetUserName();

        if (username == createMessage.RecipientUsername.ToLower())
            return BadRequest("You cannot message yourself");

        var sender = await userRepository.GetUserByUsernameAsync(username);

        var recipient = await userRepository.GetUserByUsernameAsync(createMessage.RecipientUsername);

        if (recipient == null || sender == null)
            return BadRequest("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessage.Content,
        };

        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync())
            return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Failed to save message");
    }
}
