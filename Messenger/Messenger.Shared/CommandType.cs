using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Shared
{
    public enum CommandType
    {
        Login,
        LoginResponse,
        Logout,
        GetChats,
        ChatsList,
        GetMessages,
        MessagesList,
        SendMessage,
        NewMessage,
        UserStatusChanged,
        CreateChat,
        ChatCreated,
        GetDepartments,
        DepartmentsList,

        GetAvailableUsers,
        AvailableUsersList,
        CreatePrivateChat,
        CreateGroupChat,
        MessagesRead,

        GetDepartmentChat,
        DepartmentChatInfo,
        AddChatParticipant,
        RemoveChatParticipant,

        ChatUpdated,
        ChatInfo,
        GetChatInfo,

        DeleteMessage,
        MessageDeleted,

        EditMessage,
        MessageEdited,

        GetAllUsers,
        AllUsersList,
        AddUser,
        UpdateUser,
        DeleteUser,
        UserUpdated,
        Error,

        ChangePassword,
        PasswordChanged,

        GetChatsForHistory,    // получить список всех чатов для админа
        GetHistoryMessages,    // запрос сообщений с фильтром
        DeleteHistoryMessages, // удалить сообщения за период

        DeleteChat,
        ChatDeleted,
        TypingStatus
    }
}
