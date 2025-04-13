using API.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

public record PagedResponse<T>
(
    List<T> Data,
    int CurrentPage,
    int PageSize,
    int TotalItems,
    int TotalPages
);

