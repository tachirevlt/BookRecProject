namespace Application.Services
{
    using Core.Models;
    using System.Collections.Generic;

    public interface IBadgeService
    {
        void AssignBadges(List<BookDTO> books);
    }
}