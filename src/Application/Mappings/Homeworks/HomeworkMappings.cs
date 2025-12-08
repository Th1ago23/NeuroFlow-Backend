using Application.DTO.Homework;
using Domain.Entities;

namespace Application.Mappings.Homeworks;

public static class HomeworkMappings
{
    public static HomeworkDto ToDto(this Homework homework)
        => new(
            Id: homework.Id,
            Title: homework.Title,
            Description: homework.Description,
            ExpirationTime: homework.ExpirationTime,
            IsDone: homework.IsDone
        );

    //public static Homework ToEntity(this CreateHomeworkRequest request)
    //    => new(
    //        title: request.Title,
    //        expirationTime: request.ExpirationTime,
    //        description: request.Description
    //    );
}
