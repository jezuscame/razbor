using AutoMapper;
using Database.Tables;
using Responses.Chat;
using System.Collections.Generic;

namespace Responses.AutoMapperProfiles.ChatProfiles
{
    public class ChatResponseProfile : Profile
    {
        public ChatResponseProfile()
        {
            CreateMap<ChatTable[], List<OneMessage>>()
                .ConvertUsing(src => MapChatTableToMessage(src));
        }

        private List<OneMessage> MapChatTableToMessage(ChatTable[]? chatTables)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ChatTable, OneMessage>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message)));
            var mapper = config.CreateMapper();

            var messages = mapper.Map<List<OneMessage>>(chatTables);

            return messages;
        }
    }
}
