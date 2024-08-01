using NailStore.Application.ModelsDTO;
using NailStore.Data.Models;

namespace NailStore.Application.Mapping;

public static class Mapper
{
    public static UserDTO MapTo(this UserEntity entity) => new()
    {
        userId = entity.Id,
    };
}